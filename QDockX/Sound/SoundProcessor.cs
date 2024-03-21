using Microsoft.Maui.ApplicationModel;
using QDockX.Context;
using QDockX.Debug;
using QDockX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Sound
{
    public static class SoundProcessor
    {
        private static float last = 0;
        public static byte[] AmplifyPCM16(byte[] b, int len, double gain, bool tx = false)
        {
            bool show = tx && (Status.TX || Data.Instance.AdjustingMic.Value);
            short max = 0;
            for (int i = 0; i < len; i += 2)
            {
                short s = (short)(BitConverter.ToInt16(b, i) * gain);
                if (show && s > max) max = s;
                BitConverter.GetBytes(s).CopyTo(b, i);
            }
            if (show)
            {
                MessageHub.Send("LcdSignal", (int)((max + last) / 5040f));
                last = max;
            }
            return b;
        }

        public static byte[] HalfRateAndAmplifyPCM16(byte[] b, int len, double boost)
        {            
            bool show = Status.TX || Data.Instance.AdjustingMic.Value;
            short max = 0;
            for (int i = 0, j = 0; i < len; i += 4, j += 2)
            {
                short s = (short)(BitConverter.ToInt16(b, i) * boost);
                if (show && s > max) max = s;
                BitConverter.GetBytes(s).CopyTo(b, j);
            }
            if (show)
            {
                MessageHub.Send("LcdSignal", (int)((max + last) / 5040f));
                last = max;
            }
            return b;
        }
    }
}
