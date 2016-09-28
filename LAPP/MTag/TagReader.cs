using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace LAPP.MTag
{
    public class TagReader
    {
        private static Ext.ID3.Reader ID3Reader;
        private static Ext.Flac.Reader FlacReader;
        private static Ext.MP4.Reader MP4Reader;
        private static TagCache TagCache;

        public static string[] SupportedExtension { get; private set; } = new string[] { ".mp3", ".flac", ".m4a" };

        public static int ThumbnailWidth { get; set; } = 200;

        public static bool CacheArtwork { get; set; } = false;

        public static bool Caching { get; set; } = true;

        public static string CacheFileDirectory { get; set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\LAPP.MTag\TagCache\";

        public static Tag GetTagFromFile(string FilePath)
        {
            Tag t = new Tag();
            switch (Path.GetExtension(FilePath.ToLower()))
            {
                case ".mp3":
                    ID3Reader = new Ext.ID3.Reader(FilePath);
                    Ext.ID3.Reader.ID3 id3 = ID3Reader.GetID3Tag(ID3Reader.GetID3Version(), true);
                    t = (Tag)id3;
                    break;
                case ".flac":
                    FlacReader = new Ext.Flac.Reader(FilePath);
                    Ext.Flac.Reader.Tag flac = FlacReader.GetTag();
                    t = (Tag)flac;
                    break;
                case ".m4a":
                    MP4Reader = new Ext.MP4.Reader(FilePath);
                    Ext.MP4.Reader.MP4 m4a = MP4Reader.Read();
                    t = (Tag)m4a;
                    break;
            }

            t.FilePath = FilePath;

            if (ImageComp(t.Artwork))
            {
                t.Artwork.Dispose();
                t.Artwork = BaseCompareImage;
            }

            return t;
        }

        public static Tag GetTag(string FilePath)
        {
            try
            {
                Tag t = new Tag();

                if (Caching == true)
                {
                    if (TagCache == null)
                    {
                        TagCache = new TagCache(CacheFileDirectory);
                        TagCache.CacheOriginalArtwork = false;
                    }

                    t = TagCache.GetTag(FilePath);

                    if (ImageComp(t.Artwork))
                    {
                        t.Artwork.Dispose();
                        t.Artwork = BaseCompareImage;
                    }

                    return t;
                }
                else
                {
                    switch (Path.GetExtension(FilePath.ToLower()))
                    {
                        case ".mp3":
                            ID3Reader = new Ext.ID3.Reader(FilePath);
                            Ext.ID3.Reader.ID3 id3 = ID3Reader.GetID3Tag(ID3Reader.GetID3Version(), true);
                            t = (Tag)id3;
                            break;
                        case ".flac":
                            FlacReader = new Ext.Flac.Reader(FilePath);
                            Ext.Flac.Reader.Tag flac = FlacReader.GetTag();
                            t = (Tag)flac;
                            break;
                        case ".m4a":
                            MP4Reader = new Ext.MP4.Reader(FilePath);
                            Ext.MP4.Reader.MP4 m4a = MP4Reader.Read();
                            t = (Tag)m4a;
                            break;
                    }

                    return t;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// アートワークを比較し、同じアートワークであれば
        /// CompareImageBaseを参照するように変更します（メモリ削減効果があります）
        /// </summary>
        public static bool CompareImage { get; set; } = true;
        
        public static Image BaseCompareImage { get; set; }

        public static bool ImageComp(Image image2)
        {
            if (CompareImage == false) return false;
            if (BaseCompareImage == null || image2 == null) return false;
            Bitmap img1 = (Bitmap)BaseCompareImage;
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

        public static TagType GetTagType(string FilePath)
        {
            try
            {
                switch (Path.GetExtension(FilePath).ToLower())
                {
                    case ".mp3":
                        Ext.ID3.Reader r = new Ext.ID3.Reader(FilePath);
                        Ext.ID3.Reader.ID3Version idv = r.GetID3Version();
                        r.GetID3Tag(idv, true);
                        ID3Reader = r;
                        return TagType.ID3;

                    case ".flac":
                        return TagType.Flac;

                    case ".m4a":
                        return TagType.M4A;
                }
            }
            catch (Exception)
            {
                return TagType.Unknown;
            }
            return TagType.Unknown;
        }

        public static Ext.ID3.Reader.ID3Version GetID3Version()
        {
            return ID3Reader.GetID3Version();
        }

        public static Ext.ID3.Reader.ID3 GetID3Tag(Ext.ID3.Reader.ID3Version ID3Version, bool UseDefaultEncoding)
        {
            return ID3Reader.GetID3Tag(ID3Version, UseDefaultEncoding);
        }
    }
}