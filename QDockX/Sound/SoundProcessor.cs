using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Sound
{
    public static class SoundProcessor
    {
        public static byte[] AmplifyPCM16(byte[] b, int len, double gain)
        {
            for (int i = 0; i < len; i += 2)
            {
                BitConverter.GetBytes((short)(BitConverter.ToInt16(b, i) * gain)).CopyTo(b, i);
            }
            return b;
        }

        public static byte[] HalfRatePCM16(byte[] b, int len, double boost)
        {
            for (int i = 0, j = 0; i < len; i += 4, j += 2)
            {
                short s = (short)((b[i+1] << 8) | b[i]);
                s = (short)(s * boost);
                b[j+1] = (byte)(s>>8);
                b[j] = (byte)(s & 0xff);


                //b[j] = b[i];
                //b[j+1] = b[i+1];
            }
            return b;
        }
    }
}
