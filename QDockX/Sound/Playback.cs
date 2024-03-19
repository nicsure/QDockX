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
    public static class Playback
    {
        private static AudioPlayer test;

        private static WasapiOut play = null;
        private static readonly BufferedWaveProvider provider = new(new(22050, 16, 1));
        private static double latencySecs = 0.2;

        public static void Init()
        {
            MessageHub.Message += MessageHub_Message;
            Data.Instance.Latency.PropertyChanged += Latency_PropertyChanged;
            Start();
        }

        private static void Latency_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Start();
        }

        private static void Start()
        {
            using (play)
            {
                latencySecs = Data.Instance.Latency.Value / 500.0;
                play?.Stop();
                try
                {
                    //play = new(AudioClientShareMode.Shared, Data.Instance.Latency.Value);
                    //play.Init(provider);
                    //play.Play();
                }
                catch (Exception ex)
                {
                    DebugLog.Exception(ex);
                }
            }
        }

        private static void MessageHub_Message(object sender, MessageEventArgs e)
        {
            if(e.Message.Equals("AudioIn"))
            {
                var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                if(provider.BufferedDuration.TotalSeconds > latencySecs)
                    provider.ClearBuffer();
                else
                    provider.AddSamples(buffer, 0, length);
            }
        }
    }
}
