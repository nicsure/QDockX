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
    public class AudioOut : IPlayback
    {

        private WasapiOut play = null;
        private readonly BufferedWaveProvider provider = new(new(22050, 16, 1));
        private double latencySecs = 0.2;
        private float volume = 0.5f;

        public void Init()
        {
            MessageHub.Message += MessageHub_Message;
            Data.Instance.Latency.PropertyChanged += Latency_PropertyChanged;
            Start();
        }

        private void Latency_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Start();
        }

        private void Start()
        {
            using (play)
            {
                latencySecs = Data.Instance.Latency.Value / 500.0;
                play?.Stop();
                try
                {
                    play = new(AudioClientShareMode.Shared, Data.Instance.Latency.Value);
                    play.Init(provider);
                    play.Volume = volume;
                    play.Play();
                }
                catch (Exception ex)
                {
                    DebugLog.Exception(ex);
                }
            }
        }

        private void MessageHub_Message(object sender, MessageEventArgs e)
        {
            if (e.Message.Equals("AudioIn"))
            {
                var (buffer, length) = ((byte[] buffer, int length))e.Parameter;
                if (provider.BufferedDuration.TotalSeconds > latencySecs)
                    provider.ClearBuffer();
                else
                    provider.AddSamples(buffer, 0, length);
            }
        }

        public void Gain(double gain)
        {
            volume = (float)gain;
            if (play is WasapiOut wout)
                wout.Volume = volume;
        }
    }
}
