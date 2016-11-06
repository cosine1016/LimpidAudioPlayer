using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin.Pages.Album
{
    public class Setting
    {
        internal const string AlbumSettingFileName = "Album.cnf";
        internal const string AlbumExtension = ".limpidal";
        public string AlbumDirectory { get; set; } = LAPP.Utils.Path.GetPath(LAPP.Utils.Path.LAPSetting + @"Album");
    }
}
