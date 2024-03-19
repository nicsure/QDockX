using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QDockX.UI
{
    public static class MonospaceFont
    {
        public static string Name { get; } = SelectByPlatform();
        private static string SelectByPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Consolas";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "Menlo";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Monospace";
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return "monospace";
            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return "Menlo";
            return "monospace";
        }
    }
}
