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

        public static string[] SupportedExtension { get; private set; } = new string[] { ".mp3", ".flac", ".m4a" };

        public static string CacheFileDirectory { get; set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\LAPP.MTag\TagCache\";

        public static Tag GetTag(string FilePath)
        {
            try
            {
                Tag t = null;
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
            catch (Exception)
            {
                return null;
            }
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