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
                if (ContainsKey(Key))
                    return base[Key];
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
        public LocalizeDictionary Info { get; set; } = new LocalizeDictionary();
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
                    loc.Info.Add(Key, Value);
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

            if(LocalizeData.Info.Count > 0)
            {
                sw.Write("[INFO]\r\n");
                KeyValuePair<string, string>[] strs = LocalizeData.Strings.ToArray();
                for (int i = 0; strs.Length > i; i++)
                    sw.Write(strs[i].Key + "=" + strs[i].Value + "\r\n");
            }

            if (LocalizeData.Strings.Count > 0)
            {
                sw.Write("[STRINGS]\r\n");
                KeyValuePair<string, string>[] strs = LocalizeData.Strings.ToArray();
                for (int i = 0; strs.Length > i; i++)
                    sw.Write(strs[i].Key + "=" + strs[i].Value + "\r\n");
            }

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
