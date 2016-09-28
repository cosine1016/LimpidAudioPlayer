using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LAPP.MTag
{
    public class TagCache
    {
        public const string CacheVersion = "1";

        List<Tag> Tags = new List<Tag>();
        Directory[] Dir { get; set; }
        string CacheDirectory = "";
        string CacheDirectoryFileName = "DirectoryCache";

        public TagCache(string CacheDirectory)
        {
            if (CacheDirectory.EndsWith(@"\") == false) CacheDirectory += @"\";
            this.CacheDirectory = CacheDirectory;
            Dir = ReadDirectoryCache();
        }
        
        public Tag GetTag(string FilePath)
        {
            string PD = Path.GetDirectoryName(FilePath);
            string Ext = Path.GetExtension(FilePath).ToLower();
            XmlSerializer ser = new XmlSerializer(typeof(Tag[]));
            if (TagReader.SupportedExtension.Contains(Ext) == true)
            {
                bool DirNotFound = true;
                int DirBI = -1;
                bool FileNotFound = true;
                int FileBI = -1;
                bool FileUpdated = false;
                for (int i = 0; Dir.Length > i; i++)
                {
                    if (Dir[i].DirectoryPath == PD)
                    {
                        DirNotFound = false;
                        DirBI = i;
                        if (Dir[i].Version != CacheVersion)
                        {
                            break;
                        }

                        StreamReader sr = null;
                        try
                        {
                            if (File.Exists(Dir[i].TagCachePath) == false) break;
                            sr = new StreamReader(Dir[i].TagCachePath, System.Text.Encoding.Default);
                            Tag[] pd = (Tag[])ser.Deserialize(sr);
                            sr.Close();

                            for(int ti = 0;pd.Length > ti; ti++)
                            {
                                if(FilePath == pd[ti].FilePath)
                                {
                                    FileBI = ti;
                                    if (File.GetLastWriteTime(FilePath) == pd[ti].LastWriteDate)
                                    {
                                        FileNotFound = false;
                                        return pd[ti];
                                    }
                                    else
                                    {
                                        FileUpdated = true;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            if (sr != null) sr.Close();
                        }
                    }
                }

                LAPP.MTag.Tag t = TagReader.GetTagFromFile(FilePath);
                if (t == null) return null;

                if (t.Artwork != null)
                {
                    int resizeWidth = ThumbnailWidth;
                    int resizeHeight = (int)(t.Artwork.Height * ((double)resizeWidth / (double)t.Artwork.Width));

                    Bitmap resizeBmp = new Bitmap(resizeWidth, resizeHeight);
                    Graphics g = Graphics.FromImage(resizeBmp);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(t.Artwork, 0, 0, resizeWidth, resizeHeight);
                    g.Dispose();

                    t.Thumbnail = resizeBmp;
                }

                Tag Tag = t;
                if(DirNotFound == true)
                {
                    List<Directory> Dirs = new List<Directory>(Dir);
                    Dirs.Add(new Directory(PD, CacheDirectory));
                    DirBI = Dirs.Count - 1;
                    Dir = Dirs.ToArray();
                    WriteDirectoryCache();
                }

                if(FileNotFound == true)
                {
                    if(File.Exists(Dir[DirBI].TagCachePath) == false)
                    {
                        Tag.FilePath = FilePath;
                        Tag.LastWriteDate = File.GetLastWriteTime(FilePath);

                        List<Tag> Tags = new List<Tag>();
                        Tags.Add(Tag);

                        StreamWriter sw = new StreamWriter(Dir[DirBI].TagCachePath, false, System.Text.Encoding.Default);
                        ser.Serialize(sw, Tags.ToArray());
                        sw.Close();
                    }
                    else
                    {
                        StreamReader sr = new StreamReader(Dir[DirBI].TagCachePath, System.Text.Encoding.Default);
                        Tag[] pd = (Tag[])ser.Deserialize(sr);
                        sr.Close();

                        Tag.FilePath = FilePath;
                        Tag.LastWriteDate = File.GetLastWriteTime(FilePath);

                        if (FileUpdated == true)
                        {
                            pd[FileBI] = Tag;
                        }
                        else
                        {
                            List<Tag> Tags = new List<Tag>(pd);
                            Tags.Add(Tag);
                            pd = Tags.ToArray();
                        }

                        StreamWriter sw = new StreamWriter(Dir[DirBI].TagCachePath, false, System.Text.Encoding.Default);
                        ser.Serialize(sw, pd);
                        sw.Close();
                    }
                }

                return Tag;
                
            }
            else
            {
                return null;
            }
        }

        public bool CacheOriginalArtwork { get; set; } = false;

        public int ThumbnailWidth { get; set; } = 100;

        private Directory[] ReadDirectoryCache()
        {
            string Path = CacheDirectory + CacheDirectoryFileName;
            System.IO.Directory.CreateDirectory(CacheDirectory);
            if(System.IO.File.Exists(Path) == true)
            {
                StreamReader sr = null;
                try
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Directory[]));
                    sr = new StreamReader(Path, System.Text.Encoding.Default);
                    Directory[] pd = (Directory[])ser.Deserialize(sr);
                    sr.Close();

                    return pd;
                }
                catch (Exception)
                {
                    return new Directory[0];
                }
                finally
                {
                    if (sr != null) sr.Close();
                }
            }
            else
            {
                return new Directory[0];
            }
        }

        private void WriteDirectoryCache()
        {
            string Path = CacheDirectory + CacheDirectoryFileName;
            System.IO.Directory.CreateDirectory(CacheDirectory);
            XmlSerializer ser = new XmlSerializer(typeof(Directory[]));
            StreamWriter sw = new StreamWriter(Path, false, System.Text.Encoding.Default);
            ser.Serialize(sw, Dir);
            sw.Close();
        }

        public class Tag
        {
            public string Title = "";
            public string Artist = "";
            public string Album = "";
            public string Date = "";
            public string Comment = "";
            public string Track = "";
            public string Genre = "";
            public string Artwork = "";
            public string Thumbnail = "";
            public string FilePath = "";
            public DateTime LastWriteDate;

            private static string ImageToBase64(Image image)
            {
                if (image == null) return null;
                using (MemoryStream ms = new MemoryStream())
                {
                    // Convert Image to byte[]
                    image.Save(ms, ImageFormat.Jpeg);
                    byte[] imageBytes = ms.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }

            private static Image Base64ToImage(string base64String)
            {
                if (string.IsNullOrEmpty(base64String) == true) return null;
                // Convert Base64 String to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0,
                  imageBytes.Length);

                // Convert byte[] to Image
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;
            }

            public static implicit operator Tag(LAPP.MTag.Tag Tag)
            {
                if (Tag == null) return null;
                Tag XTag = new TagCache.Tag();
                XTag.FilePath = Tag.FilePath;
                XTag.Title = Tag.Title;
                XTag.Artist = Tag.Artist;
                XTag.Album = Tag.Album;
                XTag.Date = Tag.Date;
                XTag.Comment = Tag.Comment;
                XTag.Track = Tag.Track;
                XTag.Genre = Tag.Genre;
                XTag.Artwork = ImageToBase64(Tag.Artwork);
                XTag.Thumbnail = ImageToBase64(Tag.Thumbnail);

                return XTag;
            }

            public static implicit operator LAPP.MTag.Tag(Tag XTag)
            {
                if (XTag == null) return null;
                LAPP.MTag.Tag t = new LAPP.MTag.Tag();
                t.FilePath = XTag.FilePath;
                t.Title = XTag.Title;
                t.Artist = XTag.Artist;
                t.Album = XTag.Album;
                t.Date = XTag.Date;
                t.Comment = XTag.Comment;
                t.Track = XTag.Track;
                t.Genre = XTag.Genre;
                t.Artwork = Base64ToImage(XTag.Artwork);
                t.Thumbnail = (Bitmap)Base64ToImage(XTag.Thumbnail);
                return t;
            }
        }

        public class Directory
        {
            public Directory() { }
            public Directory(string Path, string FileStoreDirectory)
            {
                Version = CacheVersion;
                DirectoryPath = Path;
                CacheFileDirectory = FileStoreDirectory;
                TagCachePath = CacheFileDirectory + System.IO.Path.GetRandomFileName();
            }

            public string Version = null;
            public string DirectoryPath;
            public string TagCachePath;
            public string CacheFileDirectory;
        }
    }
}
