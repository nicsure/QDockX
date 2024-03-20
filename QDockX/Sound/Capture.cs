using QDockX.Context;
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
            capture.Gain(Data.Instance.Boost.Value);
            Data.Instance.Boost.PropertyChanged += Boost_PropertyChanged;
            capture.Init();
        }

        private static void Boost_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            capture.Gain(Data.Instance.Boost.Value);
        }

        public static void Gain(double gain)
        {
            capture?.Gain(gain);
        }

    }
}
