using QDockX.Context;
using QDockX.Debug;
using QDockX.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{

    public class Screen : Frame
    {

        private readonly Grid grid;
        private readonly Dictionary<int, Label> texts = new();
        private readonly List<Label>[] lines = new List<Label>[8];
        private readonly BarMeter signal;

        public Screen() : base()
        {
            for (int i = 0; i < 8; i++)
                lines[i] = new List<Label>();
            Padding = new(0);
            grid = new Grid()
            {
                Padding = Padding,
                WidthRequest = 256,
                HeightRequest = 256,
            };
            Content = grid;
            signal = new()
            {
                TranslationX = 55,
                TranslationY = 5,
                HeightRequest = 26,
                WidthRequest = 143
            };
            grid.Add(signal);
            SizeChanged += Screen_SizeChanged;
            Padding = new(0);
            MessageHub.Message += MessageHub_Message;
        }

        private async Task DelayedRemoveLine(int line)
        {
            var array = lines[line].ToArray();
            await (Watchdog.Watch = Task.Delay(150));
            foreach(Label label in array)
            {
                grid.Remove(label);
                if(texts.ContainsValue(label))
                    texts.Remove(label.ZIndex);
                lines[line].Remove(label);
            }
        }

        private void MessageHub_Message(object sender, MessageEventArgs e)
        {
            switch (e.Message)
            {
                case "LcdClear":
                    Shared.Dispatch(() =>
                    {
                        var (start, end) = ((int start, int end))e.Parameter;
                        for (int i = start; i <= end; i++)
                            Watchdog.Watch = DelayedRemoveLine(i);
                    });
                    break;
                case "LcdText":
                    Shared.Dispatch(() =>
                    {
                        var (x, line, height, text, bold) = ((int x, int line, double height, string text, bool bold))e.Parameter;
                        x *= 2;
                        int y = line * 32;
                        int c = (y << 8) | x;
                        double w, h;
                        if (height <= 0.5)
                        {
                            w = Data.Instance.SmallWidth.Value / 10.0;
                            h = Data.Instance.SmallHeight.Value / 1.0;
                        }
                        else if (height <= 1)
                        {
                            w = Data.Instance.SmallWidth.Value / 5.0;
                            h = Data.Instance.SmallHeight.Value / 1.0;
                        }
                        else if (height <= 1.5)
                        {
                            w = Data.Instance.MediumWidth.Value / 5.0;
                            h = Data.Instance.MediumHeight.Value / 1.0;
                        }
                        else
                        {
                            w = Data.Instance.LargeWidth.Value / 5.0;
                            h = Data.Instance.LargeHeight.Value / 1.0;
                        }
                        Label label = new()
                        {
                            Padding = Padding,
                            TranslationX = x - ( h / 6),
                            TranslationY = y + h,
                            Text = text,                            
                            FontFamily = MonospaceFont.Name,
                            TextColor = Data.Instance.LCDForeground.Value,
                            FontSize = w,
                            FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None,
                            ZIndex = c
                        };
                        grid.Add(label);
                        if (texts.Remove(c, out Label del))
                            grid.Remove(del);
                        texts[c] = label;
                        lines[line].Add(label);
                    });
                    break;
                case "LcdSignal":
                    if (e.Parameter is int i)
                        signal.SetLevel(i);
                    break;
            }
        }

        private void Screen_SizeChanged(object sender, EventArgs e)
        {
            grid.ScaleX = Width / 256.0;
            grid.ScaleY = Height / 256.0;            
        }




    }
}
