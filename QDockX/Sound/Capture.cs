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
    public static class Capture
    {
        private static WasapiCapture capture = null;

        public static void Init()
        {
            Start();
        }

        private static void Start()
        {
            using (capture)
            {
                capture?.StopRecording();
                try
                {                   
                    //capture = new() { WaveFormat = new(22050, 16, 1) };
                    //capture.DataAvailable += Capture_DataAvailable;
                    //capture.StartRecording();                    
                }
                catch(Exception ex) { DebugLog.Exception(ex); }
            }
        }

        private static void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            MessageHub.Send("AudioOut", (e.Buffer, e.BytesRecorded));
        }

    }
}
