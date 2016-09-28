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
            UsingLanguage = UsingLanguage.Replace("{Base}", BaseSettingPath);
            Equalizer = Equalizer.Replace("{Base}", BaseSettingPath);
            Playlist = Playlist.Replace("{Base}", BaseSettingPath);
            Album = Album.Replace("{Base}", BaseSettingPath);
            Cache = Cache.Replace("{Base}", BaseSettingPath);
            LibraryPath = LibraryPath.Replace("{Base}", BaseSettingPath);
        }

        public static string SettingFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/LAP/Config/Setting.limpidcnf";

        public static string BaseSettingPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public string UsingLanguage { get; set; } = @"{Base}\LAP\Languages\English.xml";

        public string Equalizer { get; set; } = @"{Base}\LAP\Equalizer\";

        public string Playlist { get; set; } = @"{Base}\LAP\Playlists\";

        public string Album { get; set; } = @"{Base}\LAP\Album\";

        public string Cache { get; set; } = @"{Base}\LAP\Cache\";

        public string EqualizerExtension { get; set; } = @".limpideq";

        public string PlaylistExtension { get; set; } = @".limpidpf";

        public string AlbumExtension { get; set; } = @".limpidal";

        public string LibraryPath { get; set; } = @"{Base}\LAP\Library.limpidlb";

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
