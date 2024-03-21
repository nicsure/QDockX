

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
        public static string Back { get; set; } = "Back";
        public static string Language { get; set; } = "Language";
        public static string Font { get; set; } = "Font Adjust";
        public static string Size { get; set; } = "Size";
        public static string Offset { get; set; } = "Offset";
        public static string Small { get; set; } = "Small";
        public static string Medium { get; set; } = "Medium";
        public static string Large { get; set; } = "Large";
        public static string AFGain { get; set; } = "AF Gain";
        public static string MicGain { get; set; } = "Mic Gain";
        public static string Edit { get; set; } = "Edit";
        public static string Apply { get; set; } = "Apply";
        public static string Delete { get; set; } = "Delete";
        public static string SelectAll { get; set; } = "Select All";
        public static string No { get; set; } = "No";
        public static string Yes { get; set; } = "Yes";
        public static string ConfirmDelLang { get; set; } = "Are you sure you wish to delete language:";

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
        public static string Button10 { get; set; } = "M 🅐";
        public static string Button11 { get; set; } = "↑ 🅑";
        public static string Button12 { get; set; } = "↓ 🅒";
        public static string Button13 { get; set; } = "← 🅓";
        public static string Button14 { get; set; } = "✱";
        public static string Button15 { get; set; } = "F #";
        public static string Button16 { get; set; } = "PTT";
        public static string ButtonF0 { get; set; } = "FMRA";
        public static string ButtonF1 { get; set; } = "BAND";
        public static string ButtonF2 { get; set; } = "A<>B";
        public static string ButtonF3 { get; set; } = "V<>M";
        public static string ButtonF4 { get; set; } = "CTSC";
        public static string ButtonF5 { get; set; } = "SLRC";
        public static string ButtonF6 { get; set; } = "POWR";
        public static string ButtonF7 { get; set; } = "VOXX";
        public static string ButtonF8 { get; set; } = "REVR";
        public static string ButtonF9 { get; set; } = "CALL";
        public static string ButtonF14 { get; set; } = "SACN";











        private static readonly Dictionary<string, PropertyInfo> properties = new();
        private static readonly System.Collections.ObjectModel.ObservableCollection<string> available = new();
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
            string file = Path.Combine(FileSystem.AppDataDirectory, $"{language.ToLower()}.lang");
            File.Delete(file);
            available.Remove(language.ToLower());
        }

        public static string GetLanguageData()
        {
            string s = string.Empty;
            foreach (var name in properties.Keys)
                s += $"{name}={(string)properties[name].GetValue(null)}\r\n";
            return s;
        }

        public static void SaveLanguage(string language, string data = null)
        {
            string file = Path.Combine(FileSystem.AppDataDirectory, $"{language.ToLower()}.lang");
            data ??= GetLanguageData();
            try
            {
                File.WriteAllText(file, data);
            }
            catch (Exception ex) { DebugLog.Exception(ex); return; }
        }

        private static bool firstRun = true;
        public static void LoadLanguage(string language)
        {
            string file = Path.Combine(FileSystem.AppDataDirectory, $"{language.ToLower()}.lang");
            if (language.ToLower().Equals("en") && firstRun)// && !File.Exists(file)) // TEMP
            {
                firstRun = false;
                SaveLanguage("en");
            }
            string[] lines;
            try
            {
                lines = File.ReadAllLines(file);
            }
            catch (Exception ex) { DebugLog.Exception(ex); return; }
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
    }
}
