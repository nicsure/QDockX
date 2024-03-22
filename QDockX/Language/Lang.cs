

using QDockX.Context;
using QDockX.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.Language
{
    public static class Lang
    {        
        public static string NonCrititalExp { get; set; } = "Non Critital Exception";
        public static string InvalidConv { get; set; } = "Invalid Conversion Key";
        public static string QDNHHost { get; set; } = "QDNH Host";
        public static string QDNHPort { get; set; } = "QDNH Port";
        public static string QDNHPassword { get; set; } = "QDNH Password";
        public static string LCDBackground { get; set; } = "LCD Background";
        public static string LCDForeground { get; set; } = "LCD Foreground";
        public static string User1 { get; set; } = "User Button 1";
        public static string User2 { get; set; } = "User Button 2";
        public static string Latency { get; set; } = "Audio Latency";
        public static string Exit { get; set; } = "Exit";
        public static string Back { get; set; } = "◀";
        public static string Language { get; set; } = "Language";
        public static string Font { get; set; } = "Font";
        public static string Size { get; set; } = "Size";
        public static string Offset { get; set; } = "Offset";
        public static string Small { get; set; } = "Small";
        public static string Medium { get; set; } = "Medium";
        public static string Large { get; set; } = "Large";
        public static string AFGain { get; set; } = "🔊";
        public static string MicGain { get; set; } = "🎤";
        public static string Edit { get; set; } = "🖊";
        public static string Apply { get; set; } = "✔️";
        public static string Delete { get; set; } = "🗑";
        public static string Factory { get; set; } = "↺";
        public static string SelectAll { get; set; } = "Select All";
        public static string No { get; set; } = "No";
        public static string Yes { get; set; } = "Yes";
        public static string ConfirmDelLang { get; set; } = "Are you sure you wish to delete language:";
        public static string ConfirmFactory { get; set; } = "ARE YOU SURE YOU WANT TO RESTORE DEFAULT SETTINGS?";

        public static string Button0 { get; set; } = "0";
        public static string Button1 { get; set; } = "1";
        public static string Button2 { get; set; } = "2";
        public static string Button3 { get; set; } = "3";
        public static string Button4 { get; set; } = "4";
        public static string Button5 { get; set; } = "5";
        public static string Button6 { get; set; } = "6";
        public static string Button7 { get; set; } = "7";
        public static string Button8 { get; set; } = "8";
        public static string Button9 { get; set; } = "9";
        public static string Button10 { get; set; } = "𝑴";
        public static string Button11 { get; set; } = "⬆";
        public static string Button12 { get; set; } = "⬇";
        public static string Button13 { get; set; } = "⇦";
        public static string Button14 { get; set; } = "✱";
        public static string Button15 { get; set; } = "𝑓";
        public static string Button16 { get; set; } = "PTT";
        public static string ButtonF0 { get; set; } = "📻";
        public static string ButtonF1 { get; set; } = "〰️";
        public static string ButtonF2 { get; set; } = "⇅";
        public static string ButtonF3 { get; set; } = "📖";
        public static string ButtonF4 { get; set; } = "🔭";
        public static string ButtonF5 { get; set; } = "📊";
        public static string ButtonF6 { get; set; } = "📡";
        public static string ButtonF7 { get; set; } = "🗣";
        public static string ButtonF8 { get; set; } = "⇆";
        public static string ButtonF9 { get; set; } = "🔔";
        public static string ButtonF14 { get; set; } = "🔍";










        private static List<string> enData = null;
        private static readonly Dictionary<string, PropertyInfo> properties = new();
        private static readonly System.Collections.ObjectModel.ObservableCollection<string> available = new() { "en" };
        public static System.Collections.ObjectModel.ObservableCollection<string> Available => available;
        static Lang()
        {
            var type = typeof(Lang);
            PropertyInfo[] info = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo propertyInfo in info)
            {
                if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.PropertyType == typeof(string))
                {
                    properties[propertyInfo.Name] = propertyInfo;
                }
            }
            FindAvailable();
        }

        public static void FindAvailable()
        {
            foreach (var langFile in Directory.GetFiles(FileSystem.AppDataDirectory, "*.lang"))
            {
                string lang = Path.GetFileNameWithoutExtension(langFile).ToLower();
                if(!available.Contains(lang))
                    available.Add(lang);
            }
        }

        public static void DeleteLanguge(string language)
        {
            string file = Path.Combine(FileSystem.AppDataDirectory, language.LangFile());
            File.Delete(file);
            available.Remove(language.ToLower());
        }

        public static List<string> GetLanguageData()
        {
            List<string> s = new();
            foreach (var name in properties.Keys)
                s.Add($"{name}={(string)properties[name].GetValue(null)}");
            return s;
        }

        public static void SaveLanguage(string language, List<string> data = null)
        {
            string file = Path.Combine(FileSystem.AppDataDirectory, language.LangFile());
            data ??= GetLanguageData();
            try
            {
                File.WriteAllLines(file, data);
            }
            catch (Exception ex) { DebugLog.Exception(ex); return; }
        }
        
        public static void LoadLanguage(string language)
        {
            string[] lines;
            if (language.ToLower().Equals("en"))
            {
                enData ??= GetLanguageData();
                lines = enData.ToArray();
            }
            else
            {
                string file = Path.Combine(FileSystem.AppDataDirectory, language.LangFile());
                try
                {
                    lines = File.ReadAllLines(file);
                }
                catch (Exception ex) { DebugLog.Exception(ex); return; }
            }
            foreach (string line in lines) 
            {
                string[] ke = line.Split('=');
                if (ke.Length > 1)
                {
                    string key = ke[0];
                    if (properties.ContainsKey(key))
                    {
                        string ele = string.Join("=", ke, 1, ke.Length - 1);
                        properties[key].SetValue(null, ele);
                    }
                }
            }
        }

        public static string LangFile(this string language) => $"{language.ToLower()}.lang";


    }
}
