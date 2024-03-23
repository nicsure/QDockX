
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
        private double bSize, bFact, xFact, yFact, fWidth, fSmallWidth, fHeight;

        public Screen() : base()
        {            
            for (int i = 0; i < 8; i++)
                lines[i] = new List<Label>();
            Padding = Margin = new(0);
            grid = new Grid()
            {
                Padding = Padding,
            };
            signal = new();
            CalcScreenFactors(338);
            grid.Add(signal);
            Content = grid;
            SizeChanged += Screen_SizeChanged;
            MessageHub.Message += MessageHub_Message;
        }

        private void CalcScreenFactors(double screenSize)
        {
            bSize = screenSize;
            grid.WidthRequest = bSize;
            grid.HeightRequest = bSize;
            bFact = bSize / 256.0;
            xFact = 2 * bFact;
            yFact = 32 * bFact;
            signal.TranslationX = 55 * bFact;
            signal.TranslationY = 5 * bFact;
            signal.HeightRequest = 26 * bFact;
            signal.WidthRequest = 143 * bFact;
            fSmallWidth = 10 / bFact;
            fWidth = 5 / bFact;
            fHeight = 1 / bFact;
        }

        private async Task DelayedRemoveLine(int line)
        {
            var array = lines[line].ToArray();
            await (Watchdog.Watch = Task.Delay(100));
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
                        double xx = x * xFact;
                        double yy = line * yFact;
                        int c = (line << 8) | x;
                        double w, h;
                        if (height <= 0.5)
                        {
                            w = Data.Instance.SmallWidth.Value / fSmallWidth;
                            h = Data.Instance.SmallHeight.Value / fHeight;
                        }
                        else if (height <= 1)
                        {
                            w = Data.Instance.SmallWidth.Value / fWidth;
                            h = Data.Instance.SmallHeight.Value / fHeight;
                        }
                        else if (height <= 1.5)
                        {
                            w = Data.Instance.MediumWidth.Value / fWidth;
                            h = Data.Instance.MediumHeight.Value / fHeight;
                        }
                        else
                        {
                            w = Data.Instance.LargeWidth.Value / fWidth;
                            h = Data.Instance.LargeHeight.Value / fHeight;
                        }
                        Label label = new()
                        {
                            Padding = Padding,
                            TranslationX = xx - ( h / 6),
                            TranslationY = yy + h,
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
            grid.ScaleX = Width / bSize;
            grid.ScaleY = Height / bSize;            
        }

    }
}
