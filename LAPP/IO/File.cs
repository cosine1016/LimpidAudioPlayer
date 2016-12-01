using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ClearUC.ListViewItems;
using System.IO;
using RWTag;

namespace LAPP.IO
{
    internal static class BitmapCacher
    {
        internal class Cache
        {
            internal byte[] Bytes { get; set; }
            public System.Windows.Media.Imaging.BitmapImage Image { get; internal set; }
        }

        private static List<Cache> Caches { get; } = new List<Cache>();

        public static System.Windows.Media.Imaging.BitmapImage DoCache(byte[] Image, string Album)
        {
            if (string.IsNullOrEmpty(Album))
                return GetImage(Image);
            else
            {
                for (int i = 0; Caches.Count > i; i++)
                {
                    if (Caches[i].Bytes.SequenceEqual(Image))
                        return Caches[i].Image;
                }

                Cache cache = new Cache()
                {
                    Bytes = Image,
                    Image = GetImage(Image)
                };

                Caches.Add(cache);

                return cache.Image;
            }
        }

        private static System.Windows.Media.Imaging.BitmapImage GetImage(byte[] Image)
        {
            using (WrappingStream stream = new WrappingStream(new MemoryStream(Image)))
            {
                System.Windows.Media.Imaging.BitmapImage art;
                art = new System.Windows.Media.Imaging.BitmapImage();
                art.BeginInit();
                art.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                art.StreamSource = stream;
                art.EndInit();

                return art;
            }
        }
    }

    public class FileItem : PageItem
    {
        public FileItem(MediaFile File, ListItem ListItem, bool Playable) : base(ListItem)
        {
            this.File = File;
            this.Playable = Playable;
        }

        public bool Playable { get; set; } = false;
        
        public MediaFile File { get; set; }
    }

    public class MediaFile : ICloneable
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
                if (tag.Image != null)
                {
                    art = BitmapCacher.DoCache(tag.Image, tag.Album);
                }
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
        public System.Windows.Media.Imaging.BitmapImage Artwork
        {
            get { return art; }
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

    public class WrappingStream : Stream
    {
        Stream m_streamBase;

        public override bool CanRead
        {
            get { return m_streamBase.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_streamBase.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return m_streamBase.CanWrite; }
        }

        public override long Length
        {
            get { return m_streamBase.Length; }
        }

        public override long Position
        {
            get { return m_streamBase.Position; }
            set { m_streamBase.Position = value; }
        }

        public WrappingStream(Stream streamBase)
        {
            if (streamBase == null)
            {
                throw new ArgumentNullException("streamBase");
            }
            m_streamBase = streamBase;
        }
        
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return m_streamBase.ReadAsync(buffer, offset, count, cancellationToken);
        }
        public new Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            ThrowIfDisposed();
            return m_streamBase.ReadAsync(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_streamBase.Dispose();
                m_streamBase = null;
            }
            base.Dispose(disposing);
        }
        private void ThrowIfDisposed()
        {
            if (m_streamBase == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public override void Flush()
        {
            m_streamBase.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_streamBase.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            m_streamBase.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_streamBase.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_streamBase.Write(buffer, offset, count);
        }
    }
}