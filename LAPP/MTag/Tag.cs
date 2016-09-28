using System;
using System.Drawing;

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

    public enum TagType
    {
        ID3, Flac, M4A, Unknown
    }
}