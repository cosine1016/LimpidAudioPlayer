using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace LAPP.Utils
{
    public class TagReader
    {
        public class TaskStateChangedArgs : EventArgs
        {
            public string Path { get; set; }

            public bool IsDirectory { get; set; }
        }

        public event EventHandler TaskBegin;

        public event EventHandler TaskComplete;

        public event EventHandler<TaskStateChangedArgs> PartOfTask;

        public TagReader()
        {
            LoadArtworkCache();
        }

        public static bool ImageComp(Image image1, Image image2)
        {
            if (image1 == null || image2 == null) return false;
            Bitmap img1 = (Bitmap)image1;
            Bitmap img2 = (Bitmap)image2;

            //高さや幅が違えばfalse
            if (img1.Width != img2.Width || img1.Height != img2.Height) return false;
            //LockBitsでBitmapDataを取得
            BitmapData bd1 = img1.LockBits(new Rectangle(0, 0, img1.Width, img1.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img1.PixelFormat);
            BitmapData bd2 = img2.LockBits(new Rectangle(0, 0, img2.Width, img2.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img2.PixelFormat);
            if ((bd1.Stride * img1.Height) != (bd2.Stride * img2.Height)) return false;
            int bsize = bd1.Stride * img1.Height;
            byte[] byte1 = new byte[bsize];
            byte[] byte2 = new byte[bsize];
            //バイト配列にコピー
            Marshal.Copy(bd1.Scan0, byte1, 0, bsize);
            Marshal.Copy(bd2.Scan0, byte2, 0, bsize);
            //ロックを解除
            img1.UnlockBits(bd1);
            img2.UnlockBits(bd2);

            //MD5ハッシュを取る
            System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash1 = md5.ComputeHash(byte1);
            byte[] hash2 = md5.ComputeHash(byte2);
            //比較
            return hash1.SequenceEqual(hash2);
        }

        public async void Scan()
        {
            await Task.Run(() => DoScan());
        }

        private void SaveAlbum(Album Album)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Album));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                            Album.XMLPath, false, Encoding.UTF8);

            try
            {
                serializer.Serialize(sw, Album);
            }
            finally
            {
                sw.Close();
            }
        }

        public Tag GetTag(string FilePath)
        {
            if (Config.Library == null) Config.Library = new Library(true);
            if (Directory.Exists(Config.Setting.Paths.Cache + @"\artwork") == false)
                Directory.CreateDirectory(Config.Setting.Paths.Cache + @"\artwork");

            Tag nulltag = new Tag();
            nulltag.Title = Path.GetFileName(FilePath);

            if (!LAPP.MTag.TagReader.SupportedExtension.Contains(Path.GetExtension(FilePath).ToLower()))
            {
                Log.Append("Unsupported Tag File : " + FilePath);
                return nulltag;
            }

            Log.Append("Searching From Cache : " + FilePath);

            for (int i = 0; Config.Library.Files.Count > i; i++)
            {
                if (Config.Library.Files[i].Path == FilePath)
                {
                    if (System.IO.File.Exists(Config.Library.Files[i].XMLPath))
                    {
                        Album Alb = null;
                        XmlSerializer ser = new XmlSerializer(typeof(Album));
                        StreamReader sr = new StreamReader(Config.Library.Files[i].XMLPath, Encoding.UTF8);
                        try
                        {
                            Alb = (Album)ser.Deserialize(sr);
                        }
                        catch (Exception)
                        {
                            System.IO.File.Delete(Config.Library.Files[i].XMLPath);
                            i--;
                        }
                        finally
                        {
                            sr.Close();
                        }

                        for (int ai = 0; Alb.Track.Count > ai; ai++)
                        {
                            if (Alb.Track[ai].FilePath == FilePath)
                            {
                                if (Alb.Track[ai].LastWriteTime == System.IO.File.GetLastWriteTime(FilePath).ToString())
                                {
                                    if (System.IO.File.Exists(Alb.Track[ai].ArtworkCachePath) == false)
                                    {
                                        LAPP.MTag.Tag FArt = LAPP.MTag.TagReader.GetTagFromFile(FilePath);
                                        if (FArt.Artwork != null)
                                        {
                                            try
                                            {
                                                Bitmap bmp = new Bitmap(FArt.Artwork);
                                                bmp.Save(Alb.Track[ai].ArtworkCachePath, ImageFormat.Jpeg);
                                            }
                                            catch (Exception)
                                            { break; }
                                        }
                                        FArt.Dispose();
                                    }

                                    Log.Append("Found");
                                    return Alb.Track[ai];
                                }
                            }
                        }
                    }
                }
            }

            try
            {
                Log.Append("Creating Cache");

                LAPP.MTag.Tag OTag = LAPP.MTag.TagReader.GetTagFromFile(FilePath);
                Tag Ret = new Tag();
                Ret.Album = OTag.Album;
                Ret.Artist = OTag.Artist;
                Ret.Title = OTag.Title;
                Ret.Lyrics = OTag.Lyrics;
                Ret.Track = OTag.Track;
                Ret.FilePath = OTag.FilePath;
                Ret.LastWriteTime = System.IO.File.GetLastWriteTime(OTag.FilePath).ToString();

                char[] invailedchar = Path.GetInvalidFileNameChars();
                string albn = OTag.Album;
                foreach (char c in invailedchar)
                {
                    albn = albn.Replace(c, '-');
                }

                string AlbumCachePath = Config.Setting.Paths.Cache + @"\" + albn + ".xml";

                if (OTag.Artwork != null)
                {
                    string TagMD5 = ImageCRC(OTag.Artwork);

                    for (int i = 0; ArtworkCache.Count > i; i++)
                    {
                        if (ArtworkCache[i].MD5 == TagMD5)
                        {
                            Ret.ArtworkCachePath = ArtworkCache[i].Path;
                        }
                    }

                    if (string.IsNullOrEmpty(Ret.ArtworkCachePath))
                    {
                        if (Directory.Exists(Config.Setting.Paths.Cache + @"\artwork\") == false)
                            Directory.CreateDirectory(Config.Setting.Paths.Cache + @"\artwork\");

                        int i = 0;

                        while (System.IO.File.Exists(Config.Setting.Paths.Cache + @"\artwork\" + albn + "_" + i + ".jpg"))
                            i++;
                        string path = Config.Setting.Paths.Cache + @"\artwork\" + albn + "_" + i + ".jpg";

                        Bitmap bmp = new Bitmap(OTag.Artwork);
                        bmp.Save(path, ImageFormat.Jpeg);

                        Ret.ArtworkCachePath = path;
                        Artwork art = new Artwork();
                        art.Path = path;
                        art.MD5 = TagMD5;
                        ArtworkCache.Add(art);
                    }
                    else
                    {
                        int i = 0;

                        while (System.IO.File.Exists(Config.Setting.Paths.Cache + @"\artwork\" + albn + "_" + i + ".jpg"))
                            i++;

                        string path = Config.Setting.Paths.Cache + @"\artwork\" + albn + "_" + i + ".jpg";
                        if (System.IO.File.Exists(Ret.ArtworkCachePath) == false)
                        {
                            Bitmap bmp = new Bitmap(OTag.Artwork);
                            bmp.Save(path, ImageFormat.Jpeg);
                        }
                    }

                    XmlSerializer Art = new XmlSerializer(typeof(List<Artwork>));
                    StreamWriter Artsw = new StreamWriter(
                                    Config.Setting.Paths.Cache + @"\artwork\Artwork.xml", false, Encoding.UTF8);

                    try
                    {
                        Art.Serialize(Artsw, ArtworkCache);
                    }
                    finally
                    {
                        Artsw.Close();
                    }
                }

                if (System.IO.File.Exists(AlbumCachePath) == false)
                {
                    Album alb = new Album();
                    alb.Title = Ret.Title;
                    alb.XMLPath = AlbumCachePath;
                    alb.Track.Add(Ret);
                    SaveAlbum(alb);
                }
                else
                {
                    try
                    {
                        Album Alb = null;
                        XmlSerializer ser = new XmlSerializer(typeof(Album));
                        StreamReader sr = new StreamReader(AlbumCachePath);
                        try
                        {
                            Alb = (Album)ser.Deserialize(sr);
                        }
                        finally
                        {
                            sr.Close();
                        }

                        Alb.Track.Add(Ret);
                        SaveAlbum(Alb);
                    }
                    catch (Exception)
                    {
                    }
                }

                OTag.Dispose();

                Config.Library.Files.Add(new Library.File() { Path = FilePath, XMLPath = AlbumCachePath });

                Config.Library.Save();

                return Ret;
            }
            catch (Exception ex)
            {
                Log.Append("Tag Reading Error : " + FilePath + "\n" + ex.Message);
                return nulltag;
            }
        }

        private void DoScan()
        {
            TaskBegin?.Invoke(this, new EventArgs());

            for (int diri = 0; Config.Setting.Paths.ScanPaths.Length > diri; diri++)
            {
                string dir = Config.Setting.Paths.ScanPaths[diri];
                if (System.IO.Directory.Exists(dir))
                {
                    for (int fili = 0; Config.Setting.Paths.ScanFilters.Length > fili; fili++)
                    {
                        string[] files = Directory.GetFiles(dir, Config.Setting.Paths.ScanFilters[fili], System.IO.SearchOption.AllDirectories);
                        foreach (string File in files)
                        {
                            LAPP.MTag.Tag t = LAPP.MTag.TagReader.GetTagFromFile(File);
                            Cache(t);
                            PartOfTask?.Invoke(this, new TaskStateChangedArgs() { IsDirectory = false, Path = File });
                        }
                        Flash();
                    }
                }
                PartOfTask?.Invoke(this, new TaskStateChangedArgs() { IsDirectory = true, Path = dir });
            }

            Config.Library.Save();

            TaskComplete?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// キャッシュを構築します。
        /// タグ情報が返されますが、パフォーマンスが低下するのでタグの取得にはGetTagメソッドを使用してください。
        /// </summary>
        /// <param name="LAPP.MTag">キャッシュするタグ</param>
        /// <returns>キャッシュされたタグ</returns>
        public Tag Cache(MTag.Tag MTag)
        {
            char[] invailedchar = System.IO.Path.GetInvalidFileNameChars();
            string albn = MTag.Album;
            foreach (char c in invailedchar)
            {
                albn = albn.Replace(c, '-');
            }

            string AlbumCachePath = Config.Setting.Paths.Cache + @"\" + albn + ".xml";

            Tag Ret = new Tag();

            if (MTag.Album.Length == 0 || MTag.Title.Length == 0)
                return Ret;

            Ret.Album = MTag.Album;
            Ret.Artist = MTag.Artist;
            Ret.Title = MTag.Title;
            Ret.Lyrics = MTag.Lyrics;
            Ret.Track = MTag.Track;
            Ret.FilePath = MTag.FilePath;
            Ret.LastWriteTime = System.IO.File.GetLastWriteTime(MTag.FilePath).ToString();

            if (MTag.Artwork != null)
            {
                string TagMD5 = ImageCRC(MTag.Artwork);

                for (int i = 0; ArtworkCache.Count > i; i++)
                {
                    if (ArtworkCache[i].MD5 == TagMD5)
                    {
                        Ret.ArtworkCachePath = ArtworkCache[i].Path;
                    }
                }

                if (string.IsNullOrEmpty(Ret.ArtworkCachePath))
                {
                    if (System.IO.Directory.Exists(Config.Setting.Paths.Cache + @"\artwork\") == false)
                        System.IO.Directory.CreateDirectory(Config.Setting.Paths.Cache + @"\artwork\");

                    int i = 0;

                    while (System.IO.File.Exists(Config.Setting.Paths.Cache + @"\artwork\" + albn + "_" + i))
                        i++;
                    string path = Config.Setting.Paths.Cache + @"\artwork\" + albn + "_" + i + ".jpg";

                    Bitmap bmp = new Bitmap(MTag.Artwork);
                    MTag.Dispose();

                    bmp.Save(path, ImageFormat.Jpeg);

                    Ret.ArtworkCachePath = path;
                    Artwork art = new Artwork();
                    art.Path = path;
                    art.MD5 = TagMD5;
                    ArtworkCache.Add(art);
                }
                else
                {
                    int i = 0;

                    while (System.IO.File.Exists(Config.Setting.Paths.Cache + @"\artwork\" + albn + "_" + i + ".jpg"))
                        i++;

                    string path = Config.Setting.Paths.Cache + @"\artwork\" + albn + "_" + i + ".jpg";
                    if (System.IO.File.Exists(Ret.ArtworkCachePath) == false)
                    {
                        Bitmap bmp = new Bitmap(MTag.Artwork);
                        bmp.Save(path, ImageFormat.Jpeg);
                    }
                }
            }

            MTag.Dispose();

            bool Exist = false;
            for (int i = 0; Albums.Count > i; i++)
            {
                if (Albums[i].Title == Ret.Album)
                {
                    Exist = true;
                    Albums[i].Track.Add(Ret);
                }
            }

            if (Exist == false)
            {
                Album alb = new Album();
                alb.Title = Ret.Album;
                alb.XMLPath = AlbumCachePath;
                alb.Track.Add(Ret);
                Albums.Add(alb);
            }

            bool ex1 = false, ex2 = false;
            for (int i = 0; Config.Library.Files.Count > i; i++)
            {
                if (Config.Library.Files[i].Path == Ret.FilePath)
                    ex1 = true;

                if (Config.Library.Files[i].XMLPath == AlbumCachePath)
                    ex2 = true;

                if (ex1 == true && ex2 == false)
                {
                    Config.Library.Files.RemoveAt(i);
                    ex1 = false;
                    ex2 = false;
                }

                if (ex1 == false && ex2 == true)
                {
                    ex1 = false;
                    ex2 = false;
                }

                if (ex1 == true && ex2 == true)
                    break;
            }

            if (ex1 == false && ex2 == false)
                Config.Library.Files.Add(new Library.File() { Path = Ret.FilePath, XMLPath = AlbumCachePath });

            return Ret;
        }

        public void Flash()
        {
            if (Albums.Count > 0)
            {
                for (int i = 0; Albums.Count > i; i++)
                {
                    SaveAlbum(Albums[i]);
                }

                XmlSerializer Art = new XmlSerializer(typeof(List<Artwork>));
                StreamWriter Artsw = new StreamWriter(
                                Config.Setting.Paths.Cache + @"\artwork\Artwork.xml", false, Encoding.UTF8);

                try
                {
                    Art.Serialize(Artsw, ArtworkCache);
                }
                finally
                {
                    Artsw.Close();
                }
            }
        }

        public void LoadArtworkCache()
        {
            if (System.IO.File.Exists(Config.Setting.Paths.Cache + @"\artwork\Artwork.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<Artwork>));
                StreamReader sr = new StreamReader(Config.Setting.Paths.Cache + @"\artwork\Artwork.xml", Encoding.UTF8);

                try
                {
                    ArtworkCache = (List<Artwork>)ser.Deserialize(sr);
                }
                finally
                {
                    sr.Close();
                }
            }
        }

        public static string ImageCRC(Image image)
        {
            Bitmap img = (Bitmap)image;

            BitmapData bd = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            int bsize = bd.Stride * img.Height;
            byte[] bytes = new byte[bsize];
            Marshal.Copy(bd.Scan0, bytes, 0, bsize);
            img.UnlockBits(bd);

            System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(bytes);
            string ret = "";
            for (int i = 0; hash.Length > i; i++)
            {
                ret += hash[i];
            }
            return ret;
        }

        public class Artwork
        {
            public string Path { get; set; }

            public string MD5 { get; set; }
        }

        public class Album
        {
            public string Title { get; set; } = "";

            public List<Tag> Track { get; set; } = new List<Tag>();

            public string XMLPath { get; set; } = "";
        }

        public List<Artwork> ArtworkCache { get; set; } = new List<Artwork>();

        public List<Album> Albums { get; set; } = new List<Album>();
    }
}