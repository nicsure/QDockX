using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDockX
{
    public static class FileFolders
    {
        public static string Documents { get; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }
}
