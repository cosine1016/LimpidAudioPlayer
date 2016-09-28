using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Ext.MP4
{
    public class Reader
    {
        public string FilePath;

        public Reader(string FileName)
        {
            FilePath = FileName;
        }

        public class MP4
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

            public static explicit operator LAPP.MTag.Tag(MP4 val)
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

        public MP4 Read()
        {
            MP4 tag = new MP4();
            tag.FilePath = FilePath;

            List<Box> Boxes = SearchBox();

            byte[] bs = new byte[4];
            MemoryStream ms;
            string Name;
            foreach (Box b in Boxes)
            {
                ms = new MemoryStream(b.Type);
                ms.Read(bs, 0, 4);
                Name = ByteToStr(bs);
                if(Name == "moov")
                {
                    foreach(Box sb1 in b.SubBoxes)
                    {
                        ms = new MemoryStream(sb1.Type);
                        ms.Read(bs, 0, 4);
                        Name = ByteToStr(bs);
                        if(Name == "udta")
                        {
                            foreach (Box sb2 in sb1.SubBoxes)
                            {
                                ms = new MemoryStream(sb2.Type);
                                ms.Read(bs, 0, 4);
                                Name = ByteToStr(bs);
                                if (Name == "meta")
                                {
                                    tag = meta.Read(sb2);
                                }
                            }
                        }
                    }
                }
            }

            return tag;
        }

        private class meta
        {
            public static MP4 Read(Box Meta)
            {
                MP4 tag = new MP4();
                MemoryStream ms = new MemoryStream(Meta.Type);
                byte[] b = new byte[4];

                //Name
                ms.Read(b, 0, 4);
                string Name = ByteToStr(b);
                if (Name.ToLower() != "meta") return null;

                ms = new MemoryStream(Meta.Data);
                //hdlrをスキップ
                ms.Seek(4, SeekOrigin.Current);
                ms.Read(b, 0, 4);
                int size = ByteToSize(b);
                ms.Seek(size, SeekOrigin.Current);

                GC.Collect();
                //ilstを読み込む
                ms.Seek(4, SeekOrigin.Current);
                tag = ilstread(ms);

                return tag;
            }
            
            private static MP4 ilstread(MemoryStream ms)
            {
                int limit = 100;
                int count = 0;
                MP4 tag = new MP4();
                tag.TagName = "MP4";

                while (true)
                {
                    if (limit >= count)
                    {
                        byte[] bs = new byte[4];
                        ms.Read(bs, 0, 4);
                        int size = ByteToSize(bs) - 24;

                        if(size > 0)
                        {
                            ms.Read(bs, 0, 4);
                            string atom = ByteToStr(bs, Encoding.UTF7).ToLower();
                            ms.Seek(16, SeekOrigin.Current);

                            bs = new byte[size];
                            ms.Read(bs, 0, size);
                            switch (atom)
                            {
                                case "©alb":
                                    tag.Album = ByteToStr(bs);
                                    break;
                                case "©art":
                                    tag.Artist = ByteToStr(bs);
                                    break;
                                case "©cmt":
                                    tag.Comment = ByteToStr(bs);
                                    break;
                                case "©day":
                                    tag.Date = ByteToStr(bs);
                                    break;
                                case "©nam":
                                    tag.Title = ByteToStr(bs);
                                    break;
                                case "©lyr":
                                    tag.Lyrics = ByteToStr(bs);
                                    break;
                                case "©gen":
                                    tag.Genre = ByteToStr(bs);
                                    break;
                                case "gnre":
                                    tag.Genre = ByteToStr(bs);
                                    break;
                                case "trkn":
                                    byte[] trkn = new byte[4];
                                    Array.Copy(bs, trkn, 4);
                                    tag.Track = ByteToSize(trkn).ToString();
                                    break;
                                case "covr":
                                    ImageConverter imgconv = new ImageConverter();
                                    tag.Artwork = (Image)imgconv.ConvertFrom(bs);
                                    break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return tag;
            }

            private static string ByteToStr(byte[] b)
            {
                return Encoding.UTF8.GetString(b);
            }

            private static string ByteToStr(byte[] b, Encoding Encode)
            {
                return Encode.GetString(b);
            }

            private static int ByteToSize(byte[] Bytes)
            {
                Array.Reverse(Bytes);
                return BitConverter.ToInt32(Bytes, 0);
            }
        }

        public List<Box> SearchBox()
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);

            List<Box> Boxes = new List<Box>();

            int Limit = 30;
            int Count = 0;

            while (Limit >= Count)
            {
                Box b = new Box();
                ConvertedBox cb = null;

                fs.Read(b.Size, 0, 4);
                fs.Read(b.Type, 0, 4);

                cb = Convert(b);

                switch (cb.Type)
                {
                    case "moov":
                        if (cb.Size > 0)
                        {
                            b.Data = new byte[cb.Size - 4 - 4];
                            fs.Read(b.Data, 0, cb.Size - 4 - 4);

                            moov moov = new moov(this);

                            Boxes.Add(moov.Read(b));
                            return Boxes;
                        }
                        break;

                    default:
                        fs.Seek(cb.Size - 4 - 4, SeekOrigin.Current);
                        break;
                }

                Count++;
            }

            return Boxes;
        }

        public class moov
        {
            private Reader reader = null;

            public moov(Reader MP4Reader)
            {
                reader = MP4Reader;
            }

            public Box Read(Box moov)
            {
                Box moovBox = moov;

                Loop(moovBox);

                return moovBox;
            }

            private void Loop(Box b)
            {
                b = reader.BoxReader(b);

                for (int i = 0; b.SubBoxes.Count > i; i++)
                {
                    Box sb = b.SubBoxes[i];
                    Loop(sb);
                    b.SubBoxes[i] = sb;
                }
            }
        }

        public Box BoxReader(Box Box)
        {
            int limit = 100;
            int count = 0;

            MemoryStream ms = new MemoryStream(Box.Data);

            while (true)
            {
                if(limit >= count)
                {
                    Box b = new Box();
                    ConvertedBox cb = null;

                    ms.Read(b.Size, 0, 4);
                    ms.Read(b.Type, 0, 4);

                    cb = Convert(b);

                    if(cb.Type.Trim('\0') == "udta" || cb.Type.Trim('\0') == "meta")
                    {
                        if (cb.Size > 8 && cb.Type.Trim('\0') != "")
                        {
                            b.Data = new byte[cb.Size - 4 - 4];
                            ms.Read(b.Data, 0, cb.Size - 4 - 4);

                            Box.SubBoxes.Add(b);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        ms.Seek(cb.Size - 4 - 4, SeekOrigin.Current);
                    }

                    count++;
                }
                else
                {
                    break;
                }
            }

            return Box;
        }

        private string ByteToStr(byte[] b)
        {
            Encoding sjisEnc = System.Text.Encoding.UTF8;
            return sjisEnc.GetString(b).TrimEnd();
        }

        private int ByteToSize(byte[] Bytes)
        {
            return BitConverter.ToInt32(Bytes, 0);
        }

        public class Box
        {
            public byte[] Size = new byte[4];
            public byte[] Type = new byte[4];
            public byte[] Data;
            public List<Box> SubBoxes = new List<Box>();

            public static explicit operator ConvertedBox(Box Box)
            {
                ConvertedBox cb;
                LoopConvert(Box, out cb);

                return cb;
            }

            private static void LoopConvert(Box b, out ConvertedBox cbout)
            {
                cbout = new ConvertedBox();
                cbout = Convert(b);

                for (int i = 0; b.SubBoxes.Count > i; i++)
                {
                    cbout.SubBoxes.Add(Convert(b.SubBoxes[i]));

                    ConvertedBox[] cb;
                    loopsub(b.SubBoxes[i], out cb);
                    cbout.SubBoxes[i].SubBoxes.AddRange(cb);
                }
            }

            private static void loopsub(Box b, out ConvertedBox[] cbout)
            {
                List<ConvertedBox> cbs = new List<ConvertedBox>();
                for (int i = 0; b.SubBoxes.Count > i; i++)
                {
                    cbs.Add(Convert(b.SubBoxes[i]));
                    ConvertedBox[] cb;
                    loopsub(b.SubBoxes[i], out cb);
                    cbs[i].SubBoxes.AddRange(cb);
                }

                cbout = cbs.ToArray();
            }

            private static ConvertedBox Convert(Box b)
            {
                ConvertedBox cb = new ConvertedBox();
                Array.Reverse(b.Size);

                Encoding sjisEnc = System.Text.Encoding.UTF8;

                cb.Size = BitConverter.ToInt32(b.Size, 0);
                cb.Type = sjisEnc.GetString(b.Type).TrimEnd();
                cb.Data = b.Data;

                return cb;
            }
        }

        public class ConvertedBox
        {
            public int Size = 0;
            public string Type = null;
            public byte[] Data;
            public List<ConvertedBox> SubBoxes = new List<ConvertedBox>();
        }

        public ConvertedBox Convert(Box Box)
        {
            ConvertedBox cb = (ConvertedBox)Box;

            return cb;
        }
    }
}
