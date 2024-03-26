
using QDockX.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QDockX.Network
{
    public class ConnectionPreset
    {
        public static System.Collections.ObjectModel.ObservableCollection<string> PresetNames { get; } = new();
        private static readonly Dictionary<string, ConnectionPreset> presets = new();
        public static ConnectionPreset Get(string presetName) => presets.TryGetValue(presetName ?? string.Empty, out var preset) ? preset : null;
        public static void Delete(string name) { presets.Remove(name); PresetNames.Remove(name); }
        public static void Populate()
        {
            foreach(string s in Data.Instance.Presets.Value.Split(';'))
            {
                FromString(s);
            }            
        }
        public static void Serialize()
        {
            string s = string.Empty;
            foreach (var preset in presets.Values)
                s += $"{(s.Length > 0 ? ";" : string.Empty)}{preset}";
            Data.Instance.Presets.Value = s;
        }

        public string Host { get; }
        public int Port { get;}
        public string Password { get; }
        public string Name { get; }

        public ConnectionPreset(string host, int port, string password, string name)
        {
            Host = host;
            Port = port;
            Password = password;
            Name = name;
            if(!PresetNames.Contains(name))
                PresetNames.Add(name);
            presets[name] = this;
        }

        public override string ToString()
        {
            return $"{Regex.Escape(Name)},{Regex.Escape(Host)},{Port},{Regex.Escape(Password)}";
        }

        public static ConnectionPreset FromString(string s)
        {
            string[] p = s.Split(',');
            if(p.Length == 4)
            {
                string name = Regex.Unescape(p[0]);
                string host = Regex.Unescape(p[1]);
                if (int.TryParse(p[2], out int port))
                {
                    string password = Regex.Unescape(p[3]);
                    return new(host, port, password, name);
                }
            }
            return null;
        }
    }
}
