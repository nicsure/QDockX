using Microsoft.Maui.Controls;
using QDockX.Language;
using QDockX.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    public interface IAutoFontSizable
    {
        event EventHandler SizeChanged;
        event EventHandler MeasureInvalidated;
        double FontSize { get; set; }
        double Width { get; }
        double Height { get; }
        string Group { get; }
        SizeRequest Measure(double width, double height, MeasureFlags flags = MeasureFlags.None);
        


        private static readonly Dictionary<string, long> groupTimer = new();
        private static readonly Dictionary<string, List<IAutoFontSizable>> groups = new();
        public static void Register(object b, object o, object n)
        {
            if (b is IAutoFontSizable element && n is string group)
            {
                if ("None".Equals(group)) group = null;
                element.SizeChanged -= Element_MeasureChanged;
                element.MeasureInvalidated -= Element_MeasureChanged;
                element.SizeChanged += Element_MeasureChanged;
                element.MeasureInvalidated += Element_MeasureChanged;
                if (group != null)
                {
                    if (!groups.TryGetValue(group, out List<IAutoFontSizable> list))
                        groups[group] = list = new();
                    list.Add(element);
                }
                if(o is string oldGroup)
                {
                    if (groups.TryGetValue(oldGroup, out List<IAutoFontSizable> list))
                    {
                        list.Remove(element);
                        if(list.Count == 0)
                            groups.Remove(oldGroup);
                    }
                }
            }
        }
        private static void ResizeElement(IAutoFontSizable element)
        {
            element.MeasureInvalidated -= Element_MeasureChanged;
            element.FontSize = 0.1;
            Size requiredSize;
            do
            {
                element.FontSize += 1;
                requiredSize = element.Measure(double.PositiveInfinity, double.PositiveInfinity).Request;
            }
            while (element.FontSize < 1000 && requiredSize.Width < element.Width && requiredSize.Height < element.Height);
            element.FontSize -= 1;
            element.MeasureInvalidated += Element_MeasureChanged;
        }
        private static void ResizeGroup(string group)
        {
            if (groups.TryGetValue(group, out List<IAutoFontSizable> list))
            {
                double min = double.MaxValue;
                foreach (var member in list)
                {
                    member.MeasureInvalidated -= Element_MeasureChanged;
                    if (member.FontSize < min)
                        min = member.FontSize;
                }
                foreach (var member in list)
                {
                    member.FontSize = min;
                    member.MeasureInvalidated += Element_MeasureChanged;
                }
            }
        }       
        private static void Element_MeasureChanged(object sender, EventArgs e)
        {
            if (sender is IAutoFontSizable element)
            {
                ResizeElement(element);
                if (groups.ContainsKey(element.Group))
                    Watchdog.Watch = TimedGroupChange(element.Group);
            }
        }
        private static async Task TimedGroupChange(string group)
        {
            long release = DateTime.Now.Ticks + 50000;
            bool hold = !groupTimer.ContainsKey(group);
            groupTimer[group] = release;
            if(hold)
            {
                while(DateTime.Now.Ticks < release)
                {
                    await Watchdog.Delay(50); 
                }
                ResizeGroup(group);
                groupTimer.Remove(group);
            }
        }
    }
}
