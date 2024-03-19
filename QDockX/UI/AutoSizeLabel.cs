using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    public class AutoSizeLabel : Label
    {
        public AutoSizeLabel() : base()
        {
            SizeChanged += AutoSizeLabel_SizeChanged;
            MeasureInvalidated += AutoSizeLabel_SizeChanged;            
        }

        private static void AutoSizeLabel_SizeChanged(object sender, EventArgs e)
        {
            if(sender is AutoSizeLabel label)
            {
                label.MeasureInvalidated -= AutoSizeLabel_SizeChanged;
                label.FontSize = 0.1;
                Size requiredSize;
                do
                {
                    label.FontSize += 1;
                    requiredSize = label.Measure(double.PositiveInfinity, double.PositiveInfinity).Request;
                }
                while (label.FontSize < 1000 && requiredSize.Width < label.Width && requiredSize.Height < label.Height);
                label.FontSize -= 1;
                label.MeasureInvalidated += AutoSizeLabel_SizeChanged;
            }
        }
    }
}
