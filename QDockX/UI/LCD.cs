using QDockX.Context;
using QDockX.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    public class LCD : Grid
    {
        private readonly Grid singleLines;
        private readonly Grid evenDoubleLines;
        private readonly Grid oddDoubleLines;
        private readonly List<Grid>[] lines = new List<Grid>[8];
        private static readonly Thickness ZeroThickness = new(0);
        private static readonly Thickness BigThickness = new (0, 5, 0, 5);

        public LCD() : base()
        {
            RowDefinition one = new(GridLength.Star);
            RowDefinition two = new(new(2, GridUnitType.Star));
            singleLines = new Grid()
            {
                RowDefinitions = new(new[] { one, one, one, one, one, one, one, one })
            };
            evenDoubleLines = new Grid()
            {
                RowDefinitions = new(new[] { one, one, one, one })
            };
            oddDoubleLines = new Grid()
            {
                RowDefinitions = new(new[] { one, two, two, two, one })
            };
            Add(singleLines);
            Add(evenDoubleLines);
            Add(oddDoubleLines);
            for (int i = 0; i < 8; i++)
                lines[i] = new();
            MessageHub.Message += MessageHub_Message;
        }

        private async Task DelayedRemoveLine(Grid grid, int line)
        {
            await (Watchdog.Watch = Task.Delay(100));
            RemoveLine(grid, line);
        }

        private void RemoveLine(Grid grid, int line) 
        {
            singleLines.Remove(grid);
            evenDoubleLines.Remove(grid);
            oddDoubleLines.Remove(grid);
            lines[line].Remove(grid);
        }

        private void MessageHub_Message(object sender, MessageEventArgs e)
        {
            switch(e.Message)
            {
                case "LcdClear":
                    var (start, end) = ((int start, int end))e.Parameter;
                    Dispatcher.Dispatch(() =>
                    {
                        for (int i = start; i <= end; i++)
                        {
                            foreach(var grid in lines[i])
                            {
                                grid.InputTransparent = true;
                                Watchdog.Watch = DelayedRemoveLine(grid, i);
                            }
                        }
                    });
                    break;
                case "LcdText":
                    Dispatcher.Dispatch(() =>
                    {
                        var (x, line, big, text, bold) = ((double x, int line, bool big, string text, bool bold))e.Parameter;
                        bool even = (line & 1) == 0;
                        Grid parent = !big ? singleLines : even ? evenDoubleLines : oddDoubleLines;
                        int row = !big ? line : even ? line / 2 : (line / 2) + 1;
                        ColumnDefinition left = new(new(x, GridUnitType.Star));
                        ColumnDefinition middle = new(new(128 - x, GridUnitType.Star));
                        ColumnDefinition right = new(new(0, GridUnitType.Star));
                        Grid grid = new()
                        {
                            Padding = ZeroThickness,
                            ColumnDefinitions = new(new[] { left, middle, right })
                        };
                        AutoSizeLabel label = new()
                        {
                            VerticalTextAlignment = TextAlignment.End,
                            HorizontalTextAlignment = line == 0 ? TextAlignment.Center : TextAlignment.Start,
                            Margin = big ? BigThickness : ZeroThickness,
                            Padding = ZeroThickness,
                            TextColor = Data.Instance.LCDForeground.Value,
                            Text = text,
                            FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None
                        };
                        if (line == 0)
                        {
                            label.FontFamily = MonospaceFont.Name;
                            label.ScaleY = 10;
                        }
                        grid.Add(label);
                        grid.SetColumn(label, 1);
                        parent.SetRow(grid, row);
                        parent.Add(grid);
                        foreach (var v in lines[line].ToArray())
                        {
                            if(v.InputTransparent)
                            {
                                RemoveLine(v, line);
                            }
                        }
                        lines[line].Add(grid);
                    });
                    break;
            }
        }
    }
}
