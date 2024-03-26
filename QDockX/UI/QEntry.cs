using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    internal class QEntry : Entry
    {
        public QEntry() : base()
        {
            SizeChanged += (s, e) =>
            {
                double h = Height / 2;
            };
        }


    }
}
