using QDockX.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QDockX.Config
{
    public class ConfigFile
    {
        public static string Root { get; set; } = FileSystem.AppDataDirectory;
        public static string Config { get; set; } = "default";
        private readonly string path;
        private readonly Dictionary<string, string> entries = new();
        public ConfigFile(string name)
        {
            path = Path.Combine(Root, Config, $"{name.ToLower()}.conf");
            try
            {
                Directory.CreateDirectory(Path.Combine(Root, Config));
                var lines = File.ReadAllLines(path);
                int len = lines.Length & 0x7ffffffe;
                for (int i = 0; i < len; i += 2)
                    entries[lines[i]] = Regex.Unescape(lines[i + 1]);
            }
            catch (Exception ex) { DebugLog.Exception(ex); }
        }

        public void Save()
        {
            List<string> lines = new();
            foreach(var key in entries.Keys)
            {
                lines.Add(key);
                lines.Add(Regex.Escape(entries[key]));
            }
            try
            {
                File.WriteAllLines(path, lines);
            }
            catch(Exception ex) { DebugLog.Exception(ex); }
        }

        public T Read<T>(string key, T def)
        {
            if (key != null && entries.TryGetValue(key, out string val))
            {
                switch (typeof(T))
                {
                    case Type n when n == typeof(string):
                        return (T)(object)val;
                    case Type n when n == typeof(double):
                        return double.TryParse(val, out double d) ? (T)(object)d : def;
                    case Type n when n == typeof(int):
                        return int.TryParse(val, out int i) ? (T)(object)i : def;
                    case Type n when n == typeof(bool):
                        return bool.TryParse(val, out bool b) ? (T)(object)b : def;
                    case Type n when n == typeof(float):
                        return float.TryParse(val, out float f) ? (T)(object)f : def;
                    case Type n when n == typeof(Color):
                        return (T)(object)Color.FromArgb(val);
                }
            }
            return def;
        }

        public void Write<T>(string key, T val)
        {
            if (key != null)
            {
                switch (typeof(T))
                {
                    case Type n when n == typeof(string):
                        entries[key] = (string)(object)val;
                        break;
                    case Type n when n == typeof(double):
                        entries[key] = ((double)(object)val).ToString();
                        break;
                    case Type n when n == typeof(int):
                        entries[key] = ((int)(object)val).ToString();
                        break;
                    case Type n when n == typeof(bool):
                        entries[key] = ((bool)(object)val).ToString();
                        break;
                    case Type n when n == typeof(float):
                        entries[key] = ((float)(object)val).ToString();
                        break;
                    case Type n when n == typeof(Color):
                        entries[key] = ((Color)(object)val).ToArgbHex();
                        break;
                }
            }
        }
    }
}
