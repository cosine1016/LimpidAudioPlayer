using System;
using System.Drawing;
using System.Windows.Media;

namespace LAPP.MTag
{
    public class Tag : IDisposable
    {
        public string TagName = "";
        public string FilePath = "";
        public string Title = "";
        public string Artist = "";
        public string Album = "";
        public string Date = "";
        public string Comment = "";
        public string Track = "";
        public string Genre = "";
        public Image Artwork = null;
        public Bitmap Thumbnail = null;
        public string Lyrics = "";

        public Tag CreateThumbnail(int Width)
        {
            if (Artwork != null)
            {
                int resizeWidth = Width;
                int resizeHeight = (int)(Artwork.Height * ((double)resizeWidth / (double)Artwork.Width));

                Bitmap resizeBmp = new Bitmap(resizeWidth, resizeHeight);
                Graphics g = Graphics.FromImage(resizeBmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(Artwork, 0, 0, resizeWidth, resizeHeight);
                g.Dispose();

                Thumbnail = resizeBmp;
            }

            return this;
        }

        public void Dispose()
        {
            if (Thumbnail != null) Thumbnail.Dispose();
            if (Artwork != null) Artwork.Dispose();
        }

        ~Tag()
        {
            Dispose();
        }
    }

    public class TagEx
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
            if (string.IsNullOrEmpty(ArtworkCachePath))
                return null;
            return ToImageSource((Bitmap)Image.FromFile(ArtworkCachePath));
        }


        public static ImageSource ToImageSource(Bitmap bmp)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
                IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }
    }
    
    public class File : ICloneable
    {
        public File(string Path, TagEx Tag)
        {
            this.Path = Path;
            this.Tag = Tag;
        }

        public string Path { get; set; }

        public ImageSource Artwork { get; set; }

        public TagEx Tag { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public enum TagType
    {
        ID3, Flac, M4A, Unknown
    }
}