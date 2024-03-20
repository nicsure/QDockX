using NAudio.CoreAudioApi;
using NAudio.Wave;
using Plugin.AudioRecorder;
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
        private static IPlayback player = null;
        public static void Init()
        {
            player = new AudioOut();
            player.Init();

            
        }

        public static void Gain(double gain)
        {
            player?.Gain(gain);
        }
    }


}
