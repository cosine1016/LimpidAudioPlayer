using ClearUC.ListViewItems;
using LAPP;
using LAPP.IO;
using LAPP.Utils;
using NAudio.Wave;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using System;

namespace BasicPlugin.Pages.Album
{
    public class AlbumData
    {
        public string Album { get; set; } = "";

        public string Artist { get; set; } = "";

        [XmlIgnore()]
        public string Path { get; set; } = "";

        public int ArtworkIndex { get; set; } = 0;

        [XmlIgnore()]
        public System.Windows.Media.ImageSource Image { get; private set; }

        public bool ShowArtwork { get; set; } = false;

        public int TotalDiscs { get; set; } = 1;

        public Track[] Tracks { get; set; }

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

            if(Data.Tracks.Length - 1 >= Data.ArtworkIndex)
            {
                Data.Image = new MediaFile(Data.Tracks[Data.ArtworkIndex].Path).Artwork;
            }

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

            Data.Path = Path;
        }

        public class Track
        {
            public int DiscNumber { get; set; } = 0;

            public string Path { get; set; } = "";

            public string Title { get; set; } = "";

            public int TrackNumber { get; set; } = 0;
        }
    }

    public class Page : ManageablePage
    {
        private Border border = BorderHelper.GetBorderFromXAML(Resources.Shapes.Disc, 35, 35, new Thickness(0, 5, 0, 0));
        private ContainerItem ci = new ContainerItem(true, false, true);
        private View av = new View();
        
        public Page()
        {
            Items.Add(new PageItem(ci));
            av.UpdateRequest += Av_UpdateRequest;
            av.PlayFile += Av_PlayFile;
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            ci.Children.Add(av);
        }

        private void Av_PlayFile(object sender, PlayFileEventArgs e)
        {
            UpdateOrder(e.Manager);
            PlayFile(e.File);
        }

        private void Av_UpdateRequest(object sender, EventArgs e)
        {
            UpdateChildren();
        }

        private void UpdateChildren()
        {
            av.Children.Clear();
            string[] files = Directory.GetFiles(Config.Current.Path[Enums.Path.AlbumDirectory], "*.xml");
            for(int i = 0;files.Length > i; i++)
            {
                try
                {
                    AlbumData data = AlbumData.Read(files[i]);
                    av.Children.Add(new Album() { Data = data });
                }
                catch (Exception) { }
            }
        }

        public override Border Border { get; protected set; }

        public override string Title { get; protected set; } = "Album";

        public override void Dispose()
        {
        }

        public override void PlaybackStateChanged(PlaybackState PlaybackState)
        {
        }

        public override void Update()
        {
        }

        protected override PageItemCollection GetTopItems()
        {
            return Items;
        }

        protected override void Initialize(FileItem FileItem)
        {
        }

        protected override void InitializeTopItems()
        {
            UpdateChildren();
        }
    }
}