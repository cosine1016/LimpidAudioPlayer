using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Ext.ID3
{
    public class Reader
    {
        public string FilePath;
        List<byte> AllData;

        public string[] FrameNames = new string[]
        { "COMM", "SYLT", "TALB", "TBPM", "TDAT", "TEXT", "TIT1", "TIT2", "TIT3", "TLAN", "TOAL", "TOFN", "TOLY", "TOPE", "TORY", "TRCK", "TYER", "WCOP" };

        public string[] Genres = new string[]
        {
            "ブルース",
            "Classic Rock",
            "カントリー",
            "ダンス",
            "ディスコ",
            "ファンク",
            "グランジ",
            "ヒップホップ",
            "ジャズ",
            "メタル",
            "ニューエイジ",
            "オールディーズ",
            "その他",
            "ポップ",
            "R&B",
            "ラップ",
            "レゲエ",
            "ロック",
            "テクノ",
            "Industrial",
            "オルタナティブ",
            "スカ",
            "デスメタル",
            "w:Pranks",
            "サウンドトラック",
            "Euro-Techno",
            "環境",
            "Trip-hop",
            "ボーカル",
            "Jazz+Funk",
            "フュージョン",
            "トランス",
            "クラシカル",
            "Instrumental",
            "Acid",
            "ハウス",
            "ゲームミュージック",
            "Sound Clip",
            "ゴスペル",
            "ノイズ",
            "Alt. Rock",
            "バス",
            "ソウル",
            "パンク",
            "Space",
            "Meditative",
            "Instrumental pop",
            "Instrumental rock",
            "フォークソング",
            "Gothic",
            "Darkwave",
            "Techno-Industrial",
            "Electronic",
            "Pop-Folk",
            "Eurodance",
            "Dream",
            "サザン・ロック",
            "喜劇",
            "Cult",
            "Gangsta",
            "Top ",
            "Christian Rap",
            "Pop/Funk",
            "Jungle",
            "Native American",
            "Cabaret",
            "ニューウェーブ",
            "Psychedelic",
            "Rave",
            "Showtunes",
            "Trailer",
            "Lo-Fi",
            "Tribal",
            "Acid Punk",
            "アシッドジャズ",
            "ポルカ",
            "Retro",
            "Musical",
            "Rock & Roll",
            "ハードロック",
            "フォーク",
            "フォークロック",
            "National Folk",
            "スウィング",
            "Fast Fusion",
            "ビバップ",
            "ラテン",
            "Revival",
            "Celtic",
            "ブルーグラス",
            "Avantgarde",
            "Gothic Rock",
            "プログレッシブ・ロック",
            "Psychedelic Rock",
            "Symphonic Rock",
            "Slow Rock",
            "Big Band",
            "Chorus",
            "Easy Listening",
            "アコースティック",
            "ユーモア",
            "Speech",
            "シャンソン",
            "オペラ",
            "Chamber Music",
            "ソナタ",
            "交響曲",
            "Booty Bass",
            "Primus",
            "Porn Groove",
            "Satire",
            "Slow Jam",
            "Club",
            "タンゴ",
            "Samba",
            "Folklore",
            "バラッド",
            "Power Ballad",
            "Rhythmic Soul",
            "Freestyle",
            "Duet",
            "パンク・ロック",
            "Drum Solo",
            "ア・カペラ",
            "Euro-House",
            "Dance Hall",
            "Goa",
            "ドラムンベース",
            "Club-House",
            "Hardcore",
            "Terror",
            "Indie",
            "ブリットポップ",
            "Negerpunk",
            "Polsk Punk",
            "Beat",
            "Christian gangsta rap",
            "ヘヴィメタル",
            "ブラックメタル",
            "クロスオーバー",
            "Contemporary Christian",
            "Christian Rock",
            "Merengue",
            "Salsa",
            "スラッシュメタル",
            "アニメ",
            "JPop",
            "Synthpop"
        };

        public Reader(string FileName)
        {
            FilePath = FileName;
        }

        public ID3 GetID3Tag(ID3Version Version, bool UseSystemDefaultEncoding)
        {
            ID3 i;

            try
            {
                switch (Version)
                {
                    case ID3Version.V1:
                        ID3v1 v1 = ReadID3v1();
                        i = ToID3(v1);
                        break;
                    case ID3Version.V1_1:
                        ID3v1_1 v1_1 = ReadID3v1_1();
                        i = ToID3(v1_1);
                        break;
                    case ID3Version.V2_3:
                        ID3v2_3 v2_3 = ReadID3v2();
                        i = ToID3(v2_3, UseSystemDefaultEncoding);
                        break;
                    default:
                        throw new Exception("対応していないタグバージョンです。");
                }
                i.FilePath = FilePath;
                return i;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ID3v1 ReadID3v1()
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);

            if (fs.Length >= 128)
            {
                AllData = new List<byte>();
                byte[] data = new byte[128];
                fs.Seek(fs.Length - 128, SeekOrigin.Begin);
                fs.Read(data, 0, data.Length);
                AllData.AddRange(data);
                data = null;
            }
            fs.Close();

            ID3v1 id = new ID3v1();

            byte[] TAG = new byte[128];
            Array.Copy(AllData.ToArray(), AllData.Count - 128, TAG, 0, 128);
            string str = ByteToStr(TAG);

            if (str.StartsWith("TAG"))
            {
                Array.Copy(TAG, 0, id.TAG, 0, 3);
                Array.Copy(TAG, 3, id.Title, 0, 30);
                Array.Copy(TAG, 33, id.Artist, 0, 30);
                Array.Copy(TAG, 63, id.Album, 0, 30);
                Array.Copy(TAG, 93, id.Date, 0, 4);
                Array.Copy(TAG, 97, id.Comment, 0, 30);
                Array.Copy(TAG, 127, id.Genre, 0, 1);
            }
            else
            {
                return null;
            }

            return id;
        }

        public ID3v1_1 ReadID3v1_1()
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);

            if (fs.Length >= 128)
            {
                AllData = new List<byte>();
                byte[] data = new byte[128];
                fs.Seek(fs.Length - 128, SeekOrigin.Begin);
                fs.Read(data, 0, data.Length);
                AllData.AddRange(data);
                data = null;
            }
            fs.Close();

            ID3v1_1 id = new ID3v1_1();

            byte[] TAG = new byte[128];
            Array.Copy(AllData.ToArray(), AllData.Count - 128, TAG, 0, 128);
            string str = ByteToStr(TAG);

            if (str.StartsWith("TAG"))
            {
                Array.Copy(TAG, 0, id.TAG, 0, 3);
                Array.Copy(TAG, 3, id.Title, 0, 30);
                Array.Copy(TAG, 33, id.Artist, 0, 30);
                Array.Copy(TAG, 63, id.Album, 0, 30);
                Array.Copy(TAG, 93, id.Date, 0, 4);
                Array.Copy(TAG, 97, id.Comment, 0, 28);
                Array.Copy(TAG, 125, id.Zero, 0, 1);
                Array.Copy(TAG, 126, id.Track, 0, 1);
                Array.Copy(TAG, 127, id.Genre, 0, 1);
            }
            else
            {
                return null;
            }

            return id;
        }

        public ID3v2_3 ReadID3v2()
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            byte[] DefTAG = new byte[10];
            fs.Read(DefTAG, 0, 10);

            ID3v2_3 id = new ID3v2_3();

            Array.Copy(DefTAG, 0, id.TAG, 0, 3);
            string str = ByteToStr(id.TAG);

            if (str.StartsWith("ID3"))
            {
                Array.Copy(DefTAG, 3, id.Version, 0, 2);
                Array.Copy(DefTAG, 5, id.Flag, 0, 1);
                Array.Copy(DefTAG, 6, id.Size, 0, 4);

                //このサイズにヘッダ（10byte）は含まれない
                long size = CombineSynchsafe(id.Size);

                AllData = new List<byte>();
                byte[] data = new byte[size];
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(data, 0, data.Length);
                AllData.AddRange(data);
                data = null;
                fs.Close();

                string flag = Convert.ToString(id.Flag[0], 2);
                for (int i = 0; i < flag.Length; i++)
                {
                    if (flag[i] == '1')
                    {
                        switch (i)
                        {
                            case 0:
                                id.Flags.Desynchronize = true;
                                break;
                            case 1:
                                id.Flags.ExtHeader = true;
                                break;
                            case 2:
                                id.Flags.Experiment = true;
                                break;
                            case 3:
                                id.Flags.Footer = true;
                                break;
                        }
                    }

                    byte[] FrameBytes = new byte[AllData.Count - 10 - 1];
                    Array.Copy(AllData.ToArray(), 10, FrameBytes, 0, FrameBytes.Length - 10 - 1);
                    id.Frames = SearchFrame(FrameBytes);
                }

            }
            else
            {
                fs.Close();
                return null;
            }

            return id;
        }

        string SynchNum = "";

        public void ByteToSynchsafe(byte Data)
        {
            string str = Convert.ToString(Data, 2);
            str = str.PadLeft(8, '0');
            str = str.Substring(1, 7);
            SynchNum += str;
        }

        public long CombineSynchsafe(byte[] Data)
        {
            for (int i = 0; Data.Length > i; i++)
            {
                ByteToSynchsafe(Data[i]);
            }

            long com = Convert.ToInt64(SynchNum, 2);
            SynchNum = "";
            return com;
        }

        public long CombineBin(byte[] Data)
        {
            string com = "";
            for (int i = 0; Data.Length > i; i++)
            {
                string str = Convert.ToString(Data[i], 2);
                str = str.PadLeft(8, '0');
                com += str;
            }

            return Convert.ToInt64(com, 2);
        }

        public string ToBin(byte[] Data)
        {
            string com = "";
            for (int i = 0; Data.Length > i; i++)
            {
                string str = Convert.ToString(Data[i], 2);
                str = str.PadLeft(8, '0');
                com += str;
            }

            return com;
        }

        public Frame[] SearchFrame(byte[] TagData)
        {
            List<Frame> Frames = new List<Frame>();
            long si = 0;

            Frame f = new Frame();
            for (int i = 1; TagData.Length > i; i++)
            {
                byte[] bs;
                if (i == si + 4)
                {
                    f = new Frame();
                    bs = new byte[4];
                    Array.Copy(TagData, si, bs, 0, 4);
                    string bin = ToBin(bs);

                    f.ID = ByteToStr(bs);
                }
                else if (i == si + 8)
                {
                    bs = new byte[4];
                    Array.Copy(TagData, si + 4, bs, 0, 4);
                    long size = CombineBin(bs);
                    f.Size = size;
                }
                else if (i == si + 10)
                {
                    bs = new byte[2];
                    Array.Copy(TagData, si + 8, bs, 0, 2);
                    f.Flag = ToBin(bs);
                }
                else if (i == si + 11)
                {
                    try
                    {
                        f.Data = new byte[f.Size];
                        Array.Copy(TagData, si + 10, f.Data, 0, f.Size);
                        si += 10 + f.Size;
                        Frames.Add(f);
                    }
                    catch (Exception)
                    {
                        si += 10 + f.Size;
                    }
                }
            }

            return Frames.ToArray();
        }

        public object GetFrameData(Frame F, bool UseDefEnc)
        {
            if (F.ID == "TALB" || F.ID == "TPE1" || F.ID == "TPE2" || F.ID == "TCOM" || F.ID == "TIT2"
                 || F.ID == "TCON" || F.ID == "TPUB")
            {
                string tex = ByteToStrTrimStart(F.Data, UseDefEnc);
                return tex;
            }
            if (F.ID == "COMM")
            {
                return FrameReader.COMMFrame(F.Data, UseDefEnc);
            }
            if (F.ID == "USLT")
            {
                return FrameReader.USLTFrame(F.Data, UseDefEnc);
            }
            if (F.ID == "TYER")
            {
                return ByteToStr(F.Data, true);
            }
            if (F.ID == "TRCK")
            {
                return ByteToStr(F.Data, true);
            }
            if (F.ID == "APIC")
            {
                return FrameReader.APICFrame(F.Data, UseDefEnc);
            }

            return F.Data;
        }

        public ID3Version GetID3Version()
        {
            ID3v2_3 idv2_3 = ReadID3v2();
            if (idv2_3 != null)
            {
                return ID3Version.V2_3;
            }
            else
            {
                ID3v1_1 v1_1 = ReadID3v1_1();
                if (v1_1 != null)
                {
                    if (v1_1.Zero[0] == 0)
                    {
                        return ID3Version.V1_1;
                    }
                    else
                    {
                        return ID3Version.V1;
                    }
                }
                else
                {
                    return ID3Version.UnSupported;
                }
            }

        }

        public ID3 ToID3(ID3v1 id3)
        {
            ID3 i = new ID3();
            i.TagName = "ID3v1";
            i.Title = ByteToStr(id3.Title, true);
            i.Artist = ByteToStr(id3.Artist, true);
            i.Album = ByteToStr(id3.Album, true);
            i.Date = ByteToStr(id3.Date, true);
            i.Comment = ByteToStr(id3.Comment, true);
            i.Genre = Genres[ByteToInt(id3.Genre)];

            return i;
        }

        public ID3 ToID3(ID3v1_1 id3)
        {
            ID3 i = new ID3();
            i.TagName = "ID3v1.1";
            i.Title = ByteToStr(id3.Title, true);
            i.Artist = ByteToStr(id3.Artist, true);
            i.Album = ByteToStr(id3.Album, true);
            i.Date = ByteToStr(id3.Date, true);
            i.Comment = ByteToStr(id3.Comment, true);
            i.Track = ByteToStr(id3.Track, true);
            i.Genre = Genres[ByteToInt(id3.Genre)];

            return i;
        }

        public ID3 ToID3(ID3v2_3 id3, bool UseDefEnc)
        {
            ID3 i = new ID3();
            if (id3 != null)
            {
                i.TagName = "ID3v2." + id3.Version[0].ToString() + "." + id3.Version[1].ToString();

                foreach (Frame F in id3.Frames)
                {
                    switch (F.ID)
                    {
                        case "TALB":
                            i.Album = (string)GetFrameData(F, UseDefEnc);
                            break;
                        case "TPE1":
                            i.Artist = (string)GetFrameData(F, UseDefEnc);
                            break;
                        case "TIT2":
                            i.Title = (string)GetFrameData(F, UseDefEnc);
                            break;
                        case "TRCK":
                            i.Track = (string)GetFrameData(F, UseDefEnc);
                            break;
                        case "COMM":
                            FrameReader.COMM comm = (FrameReader.COMM)GetFrameData(F, UseDefEnc);
                            i.Comment = comm.Text;
                            break;
                        case "TYER":
                            i.Date = (string)GetFrameData(F, UseDefEnc);
                            break;
                        case "TCON":
                            i.Genre = (string)GetFrameData(F, UseDefEnc);
                            break;
                        case "APIC":
                            FrameReader.APIC apic = (FrameReader.APIC)GetFrameData(F, UseDefEnc);
                            i.Artwork = apic.Picture;
                            break;
                        case "USLT":
                            FrameReader.USLT uslt = (FrameReader.USLT)GetFrameData(F, UseDefEnc);
                            i.Lyrics = uslt.Lyrics;
                            break;
                    }
                }

                return i;
            }
            else
            {
                return null;
            }
        }

        public enum ID3Version
        {
            UnSupported, V1, V1_1, V2, V2_2, V2_3, V2_4
        }

        /// <summary>
        /// バイト型配列をstringへ変換します
        /// </summary>
        /// <param name="b">文字列へ変換するバイト型配列</param>
        /// <returns></returns>
        private string ByteToStr(byte[] b)
        {
            Encoding sjisEnc = System.Text.Encoding.UTF8;
            return sjisEnc.GetString(b).TrimEnd();
        }

        private string ByteToStr(byte[] b, bool RemoveZero)
        {
            if (RemoveZero == true)
            {
                List<byte> bs = new List<byte>();
                for (int i = 0; b.Length > i; i++)
                {
                    if (b[i] != 0)
                    {
                        bs.Add(b[i]);
                    }
                }
                b = bs.ToArray();
            }
            Encoding sjisEnc = System.Text.Encoding.UTF8;
            return sjisEnc.GetString(b).TrimEnd();
        }

        private string ByteToStrTrimStart(byte[] Data, bool UseSystemDefaultEncode)
        {
            byte[] Alb = new byte[Data.Length - 3];
            byte[] Enc = new byte[3];
            Array.Copy(Data, 3, Alb, 0, Data.Length - 3);
            Array.Copy(Data, 0, Enc, 0, 3);

            switch (Enc[0])
            {
                case 0:
                    if (UseSystemDefaultEncode == true)
                    {
                        return ByteToStr(Data, true, Encoding.Default);
                    }
                    else
                    {
                        return ByteToStr(Data, false, Encoding.GetEncoding("iso-8859-1"));
                    }
                case 1:
                    byte[] unib = new byte[Data.Length - 1];
                    Array.Copy(Data, 1, unib, 0, Data.Length - 1);
                    return ByteToStr(unib, false, new UnicodeEncoding(false, true));
                case 2:
                    return ByteToStr(Alb, false, new UnicodeEncoding(true, false));
                case 3:
                    return ByteToStr(Data, false, Encoding.UTF8);
                default:
                    return ByteToStr(Data, false, Encoding.Default);
            }

        }

        public bool ArrayIsEqual(byte[] Ary1, byte[] Ary2)
        {
            return ((IStructuralEquatable)Ary1).Equals(Ary2,
                StructuralComparisons.StructuralEqualityComparer);
        }

        private string ByteToStr(byte[] b, bool RemoveZero, Encoding Encode)
        {
            if (RemoveZero == true)
            {
                List<byte> bs = new List<byte>();
                for (int i = 0; b.Length > i; i++)
                {
                    if (b[i] != 0)
                    {
                        bs.Add(b[i]);
                    }
                }
                b = bs.ToArray();
            }

            return Encode.GetString(b).TrimEnd();
        }

        private Encoding GetCharSet(byte[] Encode)
        {
            int n1, n2, n3;
            n1 = Convert.ToInt32(Encode[0]);
            n2 = Convert.ToInt32(Encode[1]);
            n3 = Convert.ToInt32(Encode[2]);
            return System.Text.Encoding.UTF8;
        }

        private int ByteToInt(byte[] b)
        {
            return Convert.ToInt32(b[0]);
        }

        public class ID3
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

            public static explicit operator LAPP.MTag.Tag(ID3 val)
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

        public class ID3v1
        {
            public byte[] TAG = new byte[3];
            public byte[] Title = new byte[30];
            public byte[] Artist = new byte[30];
            public byte[] Album = new byte[30];
            public byte[] Date = new byte[4];
            public byte[] Comment = new byte[30];
            public byte[] Genre = new byte[1];
        }

        public class ID3v1_1
        {
            public byte[] TAG = new byte[3];
            public byte[] Title = new byte[30];
            public byte[] Artist = new byte[30];
            public byte[] Album = new byte[30];
            public byte[] Date = new byte[4];
            public byte[] Comment = new byte[28];
            public byte[] Zero = new byte[1];
            public byte[] Track = new byte[1];
            public byte[] Genre = new byte[1];
        }

        public class ID3v2_3
        {
            public byte[] TAG = new byte[3];
            public byte[] Version = new byte[2];
            public byte[] Flag = new byte[1];
            public Flags Flags = new Flags();
            public byte[] Size = new byte[4];
            public Frame[] Frames;
        }

        public class Flags
        {
            public bool Desynchronize = false;
            public bool ExtHeader = false;
            public bool Experiment = false;
            public bool Footer = false;
        }

        public class Frame
        {
            public string ID;
            public long Size;
            public string Flag;
            public byte[] Data;
        }

        private class FrameReader
        {
            public static APIC APICFrame(byte[] Data, bool UseDefEnc)
            {
                APIC pict = new APIC();
                Bitmap bmp = new Bitmap(20, 20);
                byte Encode = Data[0];
                PictureType PT = PictureType.image_unknown;
                byte[] MIME;
                byte Type = 0;
                byte[] Expl;
                string exp = "";
                int expsi = 0;
                byte[] picture;
                Image img = null;

                bool mimeb = false;
                bool expb = false;
                bool flag = false;

                for (int i = 1; Data.Length > i; i++)
                {

                    if (mimeb == true && expb == true)
                    {
                        picture = new byte[Data.Length - i];
                        Array.Copy(Data, i, picture, 0, picture.Length);

                        ImageConverter imgconv = new ImageConverter();

                        try
                        {
                            img = (Image)imgconv.ConvertFrom(picture);
                        }
                        catch(Exception ex)
                        {
                            img = null;
                            Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                        }
                        break;
                    }

                    if (Data[i] == 0)
                    {
                        if (mimeb == false)
                        {
                            MIME = new byte[i - 1];
                            Array.Copy(Data, 1, MIME, 0, i - 1);
                            string mimetex = ByteToStr(MIME);
                            if (mimetex == "image/jpeg")
                            {
                                PT = PictureType.image_jpeg;
                            }
                            else if (mimetex == "image/png")
                            {
                                PT = PictureType.image_png;
                                i += 1;
                            }
                            else
                            {
                                PT = PictureType.image_unknown;
                            }

                            Type = Data[i + 1];
                            expsi = i + 2;
                            if (Data[expsi] == 0)
                            {
                                exp = "";
                                expb = true;
                                i += 2;
                            }
                            mimeb = true;
                            flag = true;
                        }

                        if (mimeb == true && flag == false && expb == false)
                        {
                            Expl = new byte[i - expsi];
                            Array.Copy(Data, expsi, Expl, 0, Expl.Length);
                            if (UseDefEnc == true)
                            {
                                exp = ByteToStr(Expl, true, Encoding.Default);
                            }
                            else
                            {
                                exp = ByteToStr(Expl, false, Encoding.GetEncoding("iso-8859-1"));
                            }

                            flag = true;
                            expb = true;
                        }
                    }
                    else { flag = false; }
                }

                pict.Encoding = Data[0];
                pict.Explain = exp;
                pict.MIME = PT;
                pict.Picture = img;
                pict.Type = Type;

                return pict;
            }

            public static COMM COMMFrame(byte[] Data, bool UseDefEnc)
            {
                COMM comment = new COMM();
                comment.Encoding = Data[0];
                byte[] lang = new byte[3];
                Array.Copy(Data, 1, lang, 0, 3);
                comment.Language = ByteToStr(lang);

                for (int i = 3; Data.Length > i; i++)
                {
                    if (Data[i] == 0)
                    {
                        if (Data.Length > 5)
                        {
                            byte[] expl = new byte[i - 4 + 1];
                            expl[0] = comment.Encoding;
                            Array.Copy(Data, 4, expl, 1, i - 4);
                            if (expl.Length >= 3)
                            {
                                comment.Explain = ByteToStrTrimStart(expl, true);
                            }

                            byte[] text;
                            if (Data[i + 1] == 0)
                            {
                                text = new byte[Data.Length - i - 1];
                                Array.Copy(Data, i + 1, text, 0, text.Length);
                            }
                            else
                            {
                                text = new byte[Data.Length - i];
                                Array.Copy(Data, i, text, 0, text.Length);
                            }
                            text[0] = comment.Encoding;

                            comment.Text = ByteToStrTrimStart(text, true);
                            break;
                        }
                    }
                }

                return comment;
            }

            public static USLT USLTFrame(byte[] Data, bool UseDefEnc)
            {
                USLT lyrics = new USLT();
                COMM lyr = COMMFrame(Data, UseDefEnc);

                lyrics.Encoding = lyr.Encoding;
                lyrics.Explain = lyr.Explain;
                lyrics.Language = lyr.Language;
                lyrics.Lyrics = lyr.Text;

                return lyrics;
            }

            public class APIC
            {
                public byte Encoding;
                public PictureType MIME = PictureType.image_unknown;
                public byte Type;
                public string Explain;
                public Image Picture;
            }

            public class COMM
            {
                public byte Encoding;
                public string Language;
                public string Explain;
                public string Text;
            }

            public class USLT
            {
                public byte Encoding;
                public string Language;
                public string Explain;
                public string Lyrics;
            }

            public enum PictureType
            {
                image_png, image_jpeg, image_unknown
            }

            private static bool ArrayIsEqual(byte[] Ary1, byte[] Ary2)
            {
                return ((IStructuralEquatable)Ary1).Equals(Ary2,
                    StructuralComparisons.StructuralEqualityComparer);
            }

            private static string ByteToStr(byte[] b)
            {
                Encoding sjisEnc = System.Text.Encoding.UTF8;
                return sjisEnc.GetString(b).TrimEnd();
            }

            private static string ByteToStr(byte[] b, bool RemoveZero)
            {
                if (RemoveZero == true)
                {
                    List<byte> bs = new List<byte>();
                    for (int i = 0; b.Length > i; i++)
                    {
                        if (b[i] != 0)
                        {
                            bs.Add(b[i]);
                        }
                    }
                    b = bs.ToArray();
                }
                Encoding sjisEnc = System.Text.Encoding.UTF8;
                return sjisEnc.GetString(b).TrimEnd();
            }

            private static string ByteToStrTrimStart(byte[] Data, bool UseSystemDefaultEncode)
            {
                byte[] Alb = new byte[Data.Length - 3];
                byte[] Enc = new byte[3];
                Array.Copy(Data, 3, Alb, 0, Data.Length - 3);
                Array.Copy(Data, 0, Enc, 0, 3);

                switch (Enc[0])
                {
                    case 0:
                        if (UseSystemDefaultEncode == true)
                        {
                            return ByteToStr(Data, true, Encoding.Default);
                        }
                        else
                        {
                            return ByteToStr(Data, false, Encoding.GetEncoding("iso-8859-1"));
                        }
                    case 1:
                        return ByteToStr(Alb, false, new UnicodeEncoding(false, true));
                    case 2:
                        return ByteToStr(Alb, false, new UnicodeEncoding(true, false));
                    case 3:
                        return ByteToStr(Data, false, Encoding.UTF8);
                    default:
                        return ByteToStr(Data, false, Encoding.Default);
                }

            }

            private static string ByteToStr(byte[] b, bool RemoveZero, Encoding Encode)
            {
                if (RemoveZero == true)
                {
                    List<byte> bs = new List<byte>();
                    for (int i = 0; b.Length > i; i++)
                    {
                        if (b[i] != 0)
                        {
                            bs.Add(b[i]);
                        }
                    }
                    b = bs.ToArray();
                }

                return Encode.GetString(b).TrimEnd();
            }
        }

    }

    public static class ArrayEx
    {
        public static int IndexOf<T>(this T[] source, T[] pattern)
        {
            var table = MakeTable(pattern);
            return QuickSearch(table, source, pattern);
        }

        private static Dictionary<T, int> MakeTable<T>(T[] pattern)
        {
            var table = new Dictionary<T, int>(pattern.Length);
            for (var i = 0; i < pattern.Length; i++)
            {
                table[pattern[i]] = pattern.Length - i;
            }
            return table;
        }

        private static int QuickSearch<T>(Dictionary<T, int> table, T[] source, T[] pattern)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            var sLen = source.Length;
            var pLen = pattern.Length;
            for (var sPos = 0; sPos <= sLen - pLen; sPos += Shift(table, pLen, source[sPos + pLen]))
            {
                var pPos = 0;
                for (; pPos < pLen; pPos++)
                    if (!comparer.Equals(source[sPos + pPos], pattern[pPos]))
                        break;
                if (pPos == pLen)
                    return sPos; // found
                if (sPos == sLen - pLen)
                    break;
            }
            return -1; // not found
        }

        private static int Shift<T>(Dictionary<T, int> table, int patternLength, T next)
        {
            int s;
            return (table.TryGetValue(next, out s)) ? s : patternLength + 1;
        }
    }
}
