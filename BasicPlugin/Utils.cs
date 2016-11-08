using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace BasicPlugin
{
    public static class Utils
    {
        public static void ShowExplorerWithFile(string File)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE",
                @"/select,""" + File + "\"");
        }

        public static void ShowExplorerWithDirectory(string Directory)
        {
            System.Diagnostics.Process.Start(Directory);
        }
    }
}
