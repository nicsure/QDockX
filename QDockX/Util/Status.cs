using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Util
{
    public static class Status
    {
        private static bool rx = false, tx = false, sq = true;

        public static bool RX
        {
            get => rx;
            set { rx = value; tx = sq = !value; }
        }
        public static bool TX
        {
            get => tx;
            set { tx = value; rx = sq = !value; }
        }
        public static bool SQ
        {
            get => sq;
            set { sq = value; rx = tx = !value; }
        }

    }
}
