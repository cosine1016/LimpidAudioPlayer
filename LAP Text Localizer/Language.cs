using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace LAP_Text_Localizer
{
    public class Language
    {
        public string SupportVersion { get; set; }
        public int LCID { get; set; }
        public Dictionary<string, string> Strings { get; set; } = new Dictionary<string, string>();

        public static Language CurrentLanguage { get; set; }
        public static void Save(string Path)
        {
            StreamWriter sw = new StreamWriter(Path, false);

            string supVer = "SupportVersion=" + CurrentLanguage.SupportVersion.Replace(" ", "");
            string LCID = "LCID=" + CurrentLanguage.LCID;

            sw.Write("[INFO]\r\n" + supVer + "\r\n" + LCID + "\r\n");

            sw.Write("[STRINGS]\r\n");
            KeyValuePair<string, string>[] strs = CurrentLanguage.Strings.ToArray();
            for (int i = 0; strs.Length > i; i++)
                sw.Write(strs[i].Key + "=" + strs[i].Value + "\r\n");

            sw.Close();
        }

        public static void Load(string Path)
        {
            StreamReader sr = new StreamReader(Path);

            CurrentLanguage = new Language();
            bool IsINFO = false;
            bool IsSTRINGS = false;
            while(sr.Peek() > -1)
            {
                string line = sr.ReadLine().Replace("\r", "").Replace("\n", "");

                string Key, Value;
                GetPair(line, out Key, out Value);

                if (IsINFO && Key != null && Value != null)
                {
                    if (Key == "SupportVersion") CurrentLanguage.SupportVersion = Value;
                    if (Key == "LCID") CurrentLanguage.LCID = int.Parse(Value);
                }

                if (IsSTRINGS && Key != null && Value != null)
                {
                    CurrentLanguage.Strings.Add(Key, Value);
                }

                if (line == "[INFO]")
                {
                    IsINFO = true;
                    IsSTRINGS = false;
                }
                if(line == "[STRINGS]")
                {
                    IsSTRINGS = true;
                    IsINFO = false;
                }
            }

            sr.Close();
        }

        private static void GetPair(string Pair, out string Key, out string Value)
        {
            Key = null;
            Value = null;
            int ind = Pair.IndexOf("=");

            if(ind > -1)
            {
                Key = Pair.Substring(0, ind);
                Value = Pair.Substring(ind + 1, Pair.Length - ind - 1);
            }
        }
    }
}
