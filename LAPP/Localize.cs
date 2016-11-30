using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LAPP
{
    public class LocalizeDictionary : Dictionary<string, string>
    {
        public new string this[string Key]
        {
            get
            {
                string str;
                if (TryGetValue(Key, out str))
                    return str;
                else
                    return Key;
            }
            set
            {
                base[Key] = value;
            }
        }
    }

    public class Localize
    {
        public string SupportVersion { get; set; }
        public int LCID { get; set; }
        public LocalizeDictionary Strings { get; set; } = new LocalizeDictionary();

        public static Localize Load(string Path)
        {
            StreamReader sr = new StreamReader(Path);

            Localize loc = new Localize();
            bool IsINFO = false;
            bool IsSTRINGS = false;
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine().Replace("\r", "").Replace("\n", "");

                string Key, Value;
                GetPair(line, out Key, out Value);

                if (IsINFO && Key != null && Value != null)
                {
                    if (Key == "SupportVersion") loc.SupportVersion = Value;
                    if (Key == "LCID") loc.LCID = int.Parse(Value);
                }

                if (IsSTRINGS && Key != null && Value != null)
                {
                    loc.Strings.Add(Key, Value);
                }

                if (line == "[INFO]")
                {
                    IsINFO = true;
                    IsSTRINGS = false;
                }
                if (line == "[STRINGS]")
                {
                    IsSTRINGS = true;
                    IsINFO = false;
                }
            }

            sr.Close();

            return loc;
        }
        public static void Save(string Path, Localize LocalizeData)
        {
            StreamWriter sw = new StreamWriter(Path, false);

            if (string.IsNullOrEmpty(LocalizeData.SupportVersion))
                throw new Exception("Support Version is null");

            string supVer = "SupportVersion=" + LocalizeData.SupportVersion.Replace(" ", "");
            string LCID = "LCID=" + LocalizeData.LCID;

            sw.Write("[INFO]\r\n" + supVer + "\r\n" + LCID + "\r\n");

            if (LocalizeData.Strings == null)
                throw new Exception("Strings are not found");

            sw.Write("[STRINGS]\r\n");
            KeyValuePair<string, string>[] strs = LocalizeData.Strings.ToArray();
            for (int i = 0; strs.Length > i; i++)
                sw.Write(strs[i].Key + "=" + strs[i].Value + "\r\n");

            sw.Close();
        }
        private static void GetPair(string Pair, out string Key, out string Value)
        {
            Key = null;
            Value = null;
            int ind = Pair.IndexOf("=");

            if (ind > -1)
            {
                Key = Pair.Substring(0, ind);
                Value = Pair.Substring(ind + 1, Pair.Length - ind - 1);
            }
        }
    }
}
