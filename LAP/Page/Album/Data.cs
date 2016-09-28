using ClearUC;
using ClearUC.ListViewItems;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace LAP.Page.Album
{
    public class AlbumData
    {
        public class Track
        {
            public string Path { get; set; } = "";

            public string Title { get; set; } = "";

            public int TrackNumber { get; set; } = 0;

            public int DiscNumber { get; set; } = 0;
        }

        public string Album { get; set; } = "";

        public string Artist { get; set; } = "";

        public int TotalDiscs { get; set; } = 1;

        public Track[] Tracks { get; set; }

        public bool ShowArtwork { get; set; } = false;

        public string Path { get; set; } = "";

        public static AlbumData Read(string Path)
        {
            XmlSerializer des = new XmlSerializer(typeof(AlbumData));
            StreamReader sr = new StreamReader(Path, System.Text.Encoding.UTF8);
            AlbumData Data = null;
            try
            {
                Data = (AlbumData)des.Deserialize(sr);
            }
            catch (System.Exception) { return null; }
            finally
            {
                sr.Close();
            }

            Data.Path = Path;

            return Data;
        }

        public static void Write(string Path, AlbumData Data)
        {
            XmlSerializer ser = new XmlSerializer(typeof(AlbumData));
            StreamWriter sw = new StreamWriter(Path, false, System.Text.Encoding.UTF8);

            try
            {
                ser.Serialize(sw, Data);
            }
            finally
            {
                sw.Close();
            }
        }
    }
}
