using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace LAP.Page.Playlist
{
    public class PlaylistData
    {
        public PlaylistData(Playlist.PlaylistData Data, string Path)
        {
            this.Data = Data;
            this.Path = Path;
        }

        public Playlist.PlaylistData Data { get; set; }
        public string Path { get; set; }
    }

    public class Playlist
    {
        public class PlaylistData
        {
            public class Path
            {
                public string FilePath;
                public string DirectoryPath;
                public bool IsFile = true;
                public string[] Filter;
                public SearchOption SearchOption;
            }

            public string Title;
            public string Sticky;
            public Path[] Paths;
        }

        public static void Write(string Path, PlaylistData Data)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            XmlSerializer ser = new XmlSerializer(typeof(PlaylistData));
            StreamWriter sw = new StreamWriter(Path, false, System.Text.Encoding.UTF8);
            ser.Serialize(sw, Data);
            sw.Close();
        }

        public static PlaylistData Read(string Path)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            XmlSerializer ser = new XmlSerializer(typeof(PlaylistData));
            StreamReader sr = new StreamReader(Path, System.Text.Encoding.UTF8);
            PlaylistData pd = (PlaylistData)ser.Deserialize(sr);
            sr.Close();

            return pd;
        }


        public static PlaylistData[] GetDatas()
        {
            List<PlaylistData> ds = new List<PlaylistData>();
            string[] fs = Directory.GetFiles(Utils.Config.Setting.Paths.Playlist, "*" + Utils.Config.Setting.Paths.PlaylistExtension, SearchOption.TopDirectoryOnly);
            foreach (string path in fs)
            {
                ds.Add(Read(path));
            }

            return ds.ToArray();
        }
    }
}
