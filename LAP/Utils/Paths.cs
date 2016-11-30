using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    public class Paths
    {
        public Paths()
        {
            UsingLanguage = LAPP.Utils.Path.GetPath(UsingLanguage);
            Equalizer = LAPP.Utils.Path.GetPath(Equalizer);
            Cache = LAPP.Utils.Path.GetPath(Cache);
            LibraryPath = LAPP.Utils.Path.GetPath(LibraryPath);
            PluginInfoPath = LAPP.Utils.Path.GetPath(PluginInfoPath);
        }

        public static string SettingFilePath = LAPP.Utils.Path.GetPath(@"{LAP}\Setting.limpidcnf");

        public string UsingLanguage { get; set; } = @"{LAP}\Languages\English.loc";

        public string Equalizer { get; set; } = @"{LAP}\Equalizer\";

        public string Cache { get; set; } = @"{LAP}\Cache\";

        public string EqualizerExtension { get; set; } = @".limpideq";

        public string PlaylistExtension { get; set; } = @".limpidpf";

        public string AlbumExtension { get; set; } = @".limpidal";

        public string LibraryPath { get; set; } = @"{LAP}\Library.limpidlb";

        public string PluginInfoPath { get; set; } = @"{LAP}\PluginInfo.xml";

        public string PlaylistCache { get; set; } = "PlaylistCache";

        public string EQPath { get; set; } = "";

        public string[] PictureExtensions { get; set; } = new string[] { ".jpg", ".jpeg", ".bmp", ".png" };

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
