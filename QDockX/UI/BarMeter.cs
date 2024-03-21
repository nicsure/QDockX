using QDockX.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    public class BarMeter : Grid
    {
        private readonly BoxView[] boxes = new BoxView[13];

        public BarMeter() : base()
        {
            Padding = Margin = new(0);
            ColumnDefinition cdef = new() { Width = new(1, GridUnitType.Star) };
            for (int i = 0; i < 13; i++)
            {
                ColumnDefinitions.Add(cdef);
            }
            for (int i = 0; i < 13; i++)
            {
                Grid grid = new() { Margin = Margin, Padding = Margin };                
                double h = (i+1) / 13.0;
                grid.RowDefinitions.Add(new() { Height = new(1 - h, GridUnitType.Star) });
                grid.RowDefinitions.Add(new() { Height = new(h, GridUnitType.Star) });
                boxes[i] = new() { Margin = new(0, 0, 1, 0), Color = null };
                grid.SetRow(boxes[i], 1);
                grid.Add(boxes[i]);
                this.SetColumn(grid, i);
                Add(grid);
            }
            
        }

        public void SetLevel(int val)
        {
            Dispatcher.Dispatch(() =>
            {
                for (int i = 0; i < 13; i++)
                {
                    boxes[i].Color = val > i ? Data.Instance.LCDForeground.Value : null;
                }
            });
        }
    }
}
