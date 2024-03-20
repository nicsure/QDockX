using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Sound
{
    public static class Capture
    {
        private static ICapture capture = null;
        public static void Init()
        {
            capture = new AudioIn();
            capture.Init();
        }

        public static void Gain(double gain)
        {
            capture?.Gain(gain);
        }

    }
}
