using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Ext.Flac
{
    public class Reader
    {
        public string FilePath;

        public Reader(string FileName)
        {
            FilePath = FileName;
        }

        public Tag GetTag()
        {
            Tag t = new Tag();
            t.FilePath = FilePath;
            fLaC flac = ReadTag();

            for(int i = 0;flac.Blocks.Length > i; i++)
            {
                switch (flac.Blocks[i].MetaDataBlockKind)
                {
                    case MetaDataKinds.VORBIS_COMMENT:
                        BlockReader.VORBIS_COMMENT(ref t, flac.Blocks[i].Data);
                        break;
                    case MetaDataKinds.PICTURE:
                        BlockReader.PICTURE(ref t, flac.Blocks[i].Data);
                        break;
                }
            }

            return t;
        }

        public fLaC ReadTag()
        {
            fLaC Tag = new fLaC();
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            fs.Read(Tag.Marker, 0, 4);
            if (ArrayIsEqual(Tag.Marker, new byte[] { 0x66, 0x4C, 0x61, 0x43 }) == true)
            {
                List<MetaDataBlock> mbs = new List<MetaDataBlock>();
                while (true)
                {
                    byte[] f = new byte[1];
                    fs.Read(f, 0, 1);
                    byte[] size = new byte[3];
                    fs.Read(size, 0, 3);
                    byte[] data = new byte[SizeToInt(size)];
                    fs.Read(data, 0, data.Length);
                    MetaDataBlock mdb = SearchBlocks(f[0], size, data);
                    mbs.Add(mdb);
                    if(mdb.LastMetaDataFlag == 1)
                    {
                        Tag.Blocks = mbs.ToArray();
                        break;
                    }
                }
                return Tag;
            }
            else
            {
                fs.Close();
                return null;
            }
        }



        private int ByteToInt(byte[] b)
        {
            return Convert.ToInt32(b[0]);
        }

        public MetaDataBlock SearchBlocks(byte Flag, byte[] Size, byte[] Data)
        {
            MetaDataBlock mb = new MetaDataBlock();
            string oneb = ToBin(Flag);
            mb.LastMetaDataFlag = int.Parse(oneb.Substring(0, 1));
            int t = Convert.ToInt32(oneb.Substring(1, 7), 2);
            mb.MetaDataBlockKind = (MetaDataKinds)t;
            mb.size = Size;
            mb.Data = Data;

            return mb;
        }

        public string ToBin(byte Data)
        {
            string str = Convert.ToString(Data, 2);
            str = str.PadLeft(8, '0');

            return str;
        }

        public int SizeToInt(byte[] Size)
        {
            string str = BitConverter.ToString(Size).Replace("-", string.Empty); ;

            return Convert.ToInt32(str, 16);
        }

        private string ByteToStr(byte[] b)
        {
            Encoding sjisEnc = System.Text.Encoding.ASCII;
            return sjisEnc.GetString(b).TrimEnd();
        }

        public bool ArrayIsEqual(byte[] Ary1, byte[] Ary2)
        {
            return ((IStructuralEquatable)Ary1).Equals(Ary2,
                StructuralComparisons.StructuralEqualityComparer);
        }

        public class fLaC
        {
            public byte[] Marker = new byte[4];
            public MetaDataBlock[] Blocks;
        }

        public class MetaDataBlock
        {
            public int LastMetaDataFlag;
            public MetaDataKinds MetaDataBlockKind;
            public byte[] size = new byte[3];
            public byte[] Data;
        }

        public class Tag
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
            public string Lyrics = "";

            public static explicit operator LAPP.MTag.Tag(Tag val)
            {
                LAPP.MTag.Tag tag = new LAPP.MTag.Tag();
                tag.TagName = val.TagName;
                tag.FilePath = val.FilePath;
                tag.Title = val.Title;
                tag.Artist = val.Artist;
                tag.Album = val.Album;
                tag.Date = val.Date;
                tag.Comment = val.Comment;
                tag.Track = val.Track;
                tag.Genre = val.Genre;
                tag.Artwork = val.Artwork;
                tag.Lyrics = val.Lyrics;
                return tag;
            }
        }

        private class BlockReader
        {
            public static void VORBIS_COMMENT(ref Tag TagData, byte[] Data)
            {
                TagData.TagName = "fLaC";
                List<string> Coms = new List<string>();
                MemoryStream ms = new MemoryStream(Data);

                bool ven = true;
                while (ms.Position < ms.Length)
                {
                    byte[] len = new byte[4];
                    ms.Read(len, 0, 4);
                    int l = GetCount(len, true);
                    byte[] texb = new byte[l];
                    ms.Read(texb, 0, l);
                    Coms.Add(ByteToStr(texb));
                    if(ven == true)
                    {
                        ms.Seek(4, SeekOrigin.Current);
                        ven = false;
                    }
                }

                foreach(string Line in Coms)
                {
                    string upper = Line.ToUpper();
                    int i = upper.IndexOf('=');
                    if(i > -1)
                    {
                        string Title = upper.Substring(0, i);
                        string Text = Line.Substring(i + 1, Line.Length - i - 1);

                        switch (Title)
                        {
                            case "TITLE":
                                TagData.Title = Text;
                                break;
                            case "ARTIST":
                                TagData.Artist = Text;
                                break;
                            case "ALBUM ARTIST":
                                TagData.Artist = Text;
                                break;
                            case "ALBUM":
                                TagData.Album = Text;
                                break;
                            case "DATE":
                                TagData.Date = Text;
                                break;
                            case "COMMENT":
                                TagData.Comment = Text;
                                break;
                            case "GENRE":
                                TagData.Genre = Text;
                                break;
                            case "YEAR":
                                TagData.Date = Text;
                                break;
                            case "LYRICS":
                                TagData.Lyrics = Text;
                                break;
                            case "UNSYNCEDLYRICS":
                                TagData.Lyrics = Text;
                                break;
                            case "TRACKNUMBER":
                                TagData.Track = Text;
                                break;
                        }
                    }
                }
            }

            public static void PICTURE(ref Tag TagData, byte[] Data)
            {
                MemoryStream ms = new MemoryStream(Data);
                ms.Seek(4, SeekOrigin.Begin);
                byte[] len1 = new byte[4];
                ms.Read(len1, 0, 4);
                int cou1 = GetCount(len1, false);
                ms.Seek(cou1, SeekOrigin.Current);
                byte[] len2 = new byte[4];
                ms.Read(len2, 0, 4);
                int cou2 = GetCount(len2, false);
                ms.Seek(cou2, SeekOrigin.Current);
                ms.Seek(16, SeekOrigin.Current);

                byte[] sizeb = new byte[4];
                ms.Read(sizeb, 0, 4);
                int size = GetCount(sizeb, false);

                byte[] pict = new byte[size];
                ms.Read(pict, 0, pict.Length);

                ImageConverter imgconv = new ImageConverter();

                TagData.Artwork = (Image)imgconv.ConvertFrom(pict);

            }

            private static int GetCount(byte[] Data, bool FlipArray)
            {
                if (FlipArray)
                {
                    Array.Reverse(Data);
                }
                return SizeToInt(Data);
            }

            private static int SizeToInt(byte[] Size)
            {
                string str = BitConverter.ToString(Size).Replace("-00", "").Replace("-", "");
                
                return Convert.ToInt32(str, 16);
            }

            private static string ByteToStr(byte[] b)
            {
                Encoding sjisEnc = System.Text.Encoding.ASCII;
                return sjisEnc.GetString(b).TrimEnd();
            }

            private static int ByteToInt(byte[] b)
            {
                return Convert.ToInt32(b[0]);
            }
        }
        public enum MetaDataKinds
        {
            STREAMINFO,
            PADDING,
            APPLICATION,
            SEEKTABLE,
            VORBIS_COMMENT,
            CUESHEET,
            PICTURE
        }
    }
}
