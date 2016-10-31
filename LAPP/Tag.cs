using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RWTag;

namespace LAPP
{
    public class MediaFile : IDisposable, ICloneable
    {
        private TagReader Reader = new TagReader();
        private Tag tag;
        private string fp = null;

        public MediaFile(string FilePath)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                fp = FilePath;
                tag = Reader.GetTag(fs, System.IO.Path.GetExtension(FilePath));
                if(tag.Image != null)
                    ms = new MemoryStream(tag.Image);
            }
        }

        private MediaFile(Tag TagBase)
        {
            tag = TagBase;
        }

        public string Title
        {
            get { return tag.Title; }
        }

        public string Album
        {
            get { return tag.Album; }
        }

        public string Artist
        {
            get { return tag.Artist; }
        }

        public string Genre
        {
            get { return tag.Genre; }
        }

        public int Track
        {
            get { return tag.Track; }
        }

        public int TotalTrack
        {
            get { return tag.TotalTrack; }
        }

        public int DiscNumber
        {
            get { return tag.DiscNumber; }
        }

        public int TotalDiscNumber
        {
            get { return tag.TotalDiscNumber; }
        }

        System.Windows.Media.Imaging.BitmapImage art = null;
        MemoryStream ms = null;
        public System.Windows.Media.ImageSource Artwork
        {
            get
            {
                if(art == null && ms != null)
                {
                    art = new System.Windows.Media.Imaging.BitmapImage();
                    art.BeginInit();
                    art.StreamSource = ms;
                    art.EndInit();
                }

                return art;
            }
        }

        public string Lyrics
        {
            get { return tag.Lyrics; }
        }

        public string Comment
        {
            get { return tag.Comment; }
        }

        public string Path
        {
            get { return fp; }
        }

        public void Dispose()
        {
            ms.Dispose();
        }

        public object Clone()
        {
            return new MediaFile(new Tag() { Album = Album, Artist = Artist,
                Comment = Comment, DiscNumber = DiscNumber, Genre = Genre,
                Lyrics = Lyrics, TotalDiscNumber = TotalDiscNumber,
                TotalTrack = TotalTrack, Track = Track, Title = Title, Image = tag.Image,
                Data = tag.Data, Date = tag.Date,
                Name = tag.Name, ImageDescription = tag.ImageDescription,
                ImageMIMEType = tag.ImageMIMEType });
        }
    }
}
