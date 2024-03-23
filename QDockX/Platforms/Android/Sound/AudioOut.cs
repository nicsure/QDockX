using Android.Media;
using QDockX.Context;
using QDockX.Debug;
using QDockX.Sound;
using QDockX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Sound
{
    public class AudioOut : IPlayback
    {
        private AudioTrack track = null;
        private int bufferSize, bufferFrames;
        private float volume = 0.5f;

        public void Gain(double gain)
        {
            volume = (float)gain;
            track?.SetVolume(volume);
        }

        public void Init()
        {
            int minBufferSize = AudioTrack.GetMinBufferSize(22050, ChannelOut.Mono, Android.Media.Encoding.Pcm16bit);
            int reqBufferSize = (int)(44100.0 / Data.Instance.Latency.Value);
            bufferSize = Math.Min(44100, Math.Max(minBufferSize, reqBufferSize));
            AudioAttributes.Builder aabuilder = new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Unknown)
                .SetUsage(AudioUsageKind.Unknown);
            AudioFormat.Builder afbuilder = new AudioFormat.Builder()
                .SetEncoding(Android.Media.Encoding.Pcm16bit)
                .SetSampleRate(22050)
                .SetChannelMask(ChannelOut.Mono);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
#pragma warning disable CA1416
                AudioTrack.Builder atbuilder = new AudioTrack.Builder()
                    .SetPerformanceMode(AudioTrackPerformanceMode.LowLatency)
#pragma warning restore CA1416
                    .SetAudioFormat(afbuilder.Build())
                    .SetAudioAttributes(aabuilder.Build())
                    .SetTransferMode(AudioTrackMode.Stream)
                    .SetBufferSizeInBytes(bufferSize)
                    .SetSessionId(AudioManager.AudioSessionIdGenerate);
                track = atbuilder.Build();
            }
            else
            {
                track = new(aabuilder.Build(), afbuilder.Build(), bufferSize, AudioTrackMode.Stream, AudioManager.AudioSessionIdGenerate);
            }
            bufferFrames = track.BufferSizeInFrames;
            sbuf = new short[bufferFrames];
            track.SetVolume(volume);
            track.Play();
            MessageHub.Message += MessageHub_Message;
        }

        private static short[] sbuf;
        private static int scnt = 0;
        private void MessageHub_Message(object sender, MessageEventArgs e)
        {
            if (e.Message == Msg._audioin)
            {
                var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                for (int i = 0; i < length; i += 2)
                {
                    sbuf[scnt++] = BitConverter.ToInt16(buffer, i);
                    if (scnt >= bufferFrames)
                    {
                        scnt = 0;
                        track.Write(sbuf, 0, bufferFrames);
                    }
                }
            }
        }


    }


}