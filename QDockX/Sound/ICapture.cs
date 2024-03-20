using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Sound
{
    public interface ICapture
    {
        void Init();
        void Gain(double gain);
    }
}
