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
        private int bufferSize, bufferFrames, lag;
        private Task lastTask = Task.Run(() => { });
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
            track.SetVolume(volume);
            track.Play();
            MessageHub.Message += MessageHub_Message;
        }

        private void MessageHub_Message(object sender, MessageEventArgs e)
        {
            switch (e.Message)
            {
                case "AudioIn":
                    bool idle = lastTask.IsCompleted;
                    lag += idle ? -1 : 1;
                    if (lag < 0) lag = 0;
                    else if (lag > 3) break;
                    if (!idle) lastTask.Wait();
                    var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                    short[] s = new short[length >> 1];
                    unsafe
                    {
                        fixed (short* sptr = s)
                        {
                            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, (IntPtr)sptr, length);
                        }
                    }
                    using (lastTask)
                    {
                        lastTask = Task.Run(() =>
                        {
                            for (int i = 0; i < s.Length; i += bufferFrames)
                            {
                                int sl = s.Length - i;
                                track.Write(s, i, sl > bufferFrames ? bufferFrames : sl);
                            }
                        });
                    }
                    break;
            }
        }
    }


}