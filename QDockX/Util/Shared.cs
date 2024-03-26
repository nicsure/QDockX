using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Util
{
    public static class Shared
    {
        public static Editor LanguageEditor { get; set; } = null;
        public static Page Page { get; set; } = null;

        public static string LinesToString(this List<string> lines)
        {
            string s = string.Empty;
            foreach (string line in lines)
                s += $"{line}\r\n";
            return s;
        }

        public static List<string> StringtoLines(this string s)
        {
            return s.Replace('\r', '\n').Replace("\n\n", "\n").Split('\n').ToList();
        }

        public static void Dispatch(Action action)
        {
            (_ = LanguageEditor)?.Dispatcher.Dispatch(action);
        }

        public static async Task Alert(string title, string message, string button)
        {
            using var task = Page.DisplayAlert(title, message, button);
            await task;
        }
    }
}
