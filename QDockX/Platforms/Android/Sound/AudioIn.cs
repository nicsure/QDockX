using Android.Media;
using QDockX.Context;
using QDockX.Debug;
using QDockX.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Sound
{
    public class AudioIn : ICapture
    {
        private double boost = 1.0;
        private int bufferSize;
        private AudioRecord capture = null;

        public void Gain(double gain)
        {
            if (gain < 0.5)
                boost = gain * 2;
            else
                boost = ((gain - 0.5) * 18) + 1;
        }

        public void Init()
        {
            int minBufferSize = AudioRecord.GetMinBufferSize(44100, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit);
            int reqBufferSize = (int)(88200.0 / Data.Instance.Latency.Value);
            bufferSize = Math.Min(44100, Math.Max(minBufferSize, reqBufferSize));
            if((bufferSize & 3) != 0)
            {
                bufferSize &= 0x7ffffffc;
                bufferSize += 4;
            }
            Task.Run(Capture);            
        }

        private void Capture()
        {
            while (true)
            {
                Thread.Sleep(1000);
                try { capture = new(AudioSource.Mic, 44100, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit, bufferSize); }
                catch (Exception ex) { DebugLog.Exception(ex); }
                using (capture)
                {
                    if (capture != null && capture.State == State.Initialized)
                    {
                        try { capture.StartRecording(); } catch (Exception ex) { DebugLog.Exception(ex); continue; }
                        while(true)
                        {
                            byte[] b = new byte[bufferSize];
                            int br = capture.Read(b, 0, b.Length);
                            if (br <= 0) break;
                            SoundProcessor.HalfRateAndAmplifyPCM16(b, br, boost);
                            MessageHub.Send(Msg._audioout, (b, br >> 1));
                        }
                    }
                    try { capture?.Stop(); } catch (Exception ex) { DebugLog.Exception(ex); continue; }
                }
            }
        }
    }
}
