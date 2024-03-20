using NAudio.CoreAudioApi;
using NAudio.Wave;
using QDockX.Context;
using QDockX.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Sound
{
    public class AudioIn : ICapture
    {
        private WasapiCapture capture = null;
        private double boost = 1.0;

        public void Init()
        {
            Start();
        }

        private void Start()
        {
            using (capture)
            {
                capture?.StopRecording();
                try
                {                   
                    capture = new() { WaveFormat = new(22050, 16, 1) };
                    capture.DataAvailable += Capture_DataAvailable;
                    capture.StartRecording();                    
                }
                catch(Exception ex) { DebugLog.Exception(ex); }
            }
        }

        private static byte[] AmplifyPCM16(byte[] b, int len, double gain)
        {
            for (int i = 0; i < len; i += 2)
            {
                BitConverter.GetBytes((short)(BitConverter.ToInt16(b, i) * gain)).CopyTo(b, i);
            }
            return b;
        }

        private void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            MessageHub.Send("AudioOut", (AmplifyPCM16(e.Buffer, e.BytesRecorded, boost), e.BytesRecorded));
        }

        public void Gain(double gain)
        {
            if (gain < 0.5)
                boost = gain * 2;
            else
                boost = ((gain - 0.5) * 18) + 1;
        }
    }
}
