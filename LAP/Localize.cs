using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LAP
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

    internal class Localize
    {
        public static string Get(string Key)
        {
            return CurrentLanguage.Strings[Key];
        }

        public static LAPP.Localize CurrentLanguage { get; set; }
        public static void Save(string Path)
        {
            LAPP.Localize.Save(Path, CurrentLanguage);
        }

        public static void Load(string Path)
        {
            CurrentLanguage = LAPP.Localize.Load(Path);
        }
    }
}
