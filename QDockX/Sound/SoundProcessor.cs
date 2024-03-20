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

        public static byte[] HalfRateAndAmplifyPCM16(byte[] b, int len, double boost)
        {
            for (int i = 0, j = 0; i < len; i += 4, j += 2)
            {
                BitConverter.GetBytes((short)(BitConverter.ToInt16(b, i) * boost)).CopyTo(b, j);
            }
            return b;
        }
    }
}
