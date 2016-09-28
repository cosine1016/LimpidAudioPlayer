using System;
using System.Windows.Media;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LAPP.Utils
{
    public class Tag
    {
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public string Title { get; set; } = "";
        public string Lyrics { get; set; } = "";
        public string ArtworkCachePath { get; set; } = "";
        public string Track { get; set; } = "";
        public string LastWriteTime { get; set; } = "";
        public string FilePath { get; set; } = "";

        public ImageSource GetArtwork()
        {
            return Utils.Converter.ToImageSource((Bitmap)Image.FromFile(ArtworkCachePath));
        }
    }

    public class File : ICloneable
    {
        public File(string Path, Tag Tag)
        {
            this.Path = Path;
            this.Tag = Tag;
        }

        public string Path { get; set; }

        public ImageSource Artwork { get; set; }

        public Tag Tag { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
