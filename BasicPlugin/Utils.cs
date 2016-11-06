using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin
{
    class Utils
    {
        internal static void ShowExplorerWithFile(string File)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE",
                @"/select,""" + File + "\"");
        }

        internal static void ShowExplorerWithDirectory(string Directory)
        {
            System.Diagnostics.Process.Start(Directory);
        }
    }
}
