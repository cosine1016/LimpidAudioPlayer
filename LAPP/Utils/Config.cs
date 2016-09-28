using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LAPP.Utils
{
    public static class Config
    {
        public static Setting Setting { get; set; }

        internal static Library Library { get; set; }
    }

    public class Setting
    {
        public Paths Paths = new Paths();
    }

    public class Paths
    {
        public string Cache { get; set; }

        public string LibraryPath { get; set; }

        public string[] ScanPaths { get; set; } = new string[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
        };

        public string[] ScanFilters { get; set; } = new string[]
        {
            "*.m4a"
        };
    }
}