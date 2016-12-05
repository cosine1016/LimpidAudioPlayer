using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LAP
{
    public enum Strings
    {
        Title, Open, Config, Creator, Log, Exit,
        Play, Pause, Stop, Next, Back,
        Version, Plugin
    }

    internal class Localize
    {
        public const string KEY_LCID = "LCID";

        public static event EventHandler LanguageChanged;

        public static bool ExportLog { get; set; } = true;

        public static string Get(string Key)
        {
            string str = Current.Strings[Key];

            if (Key == str)
                Dialogs.LogWindow.Append("Key Was Not Found : " + Key);

            return str;
        }

        public static string Get(Strings ID)
        {
            string ids = "UNKNOWN";
            switch (ID)
            {
                case Strings.Open:
                    ids = "0_OPEN";
                    break;
                case Strings.Config:
                    ids = "0_CONFIG";
                    break;
                case Strings.Log:
                    ids = "0_LOG";
                    break;
                case Strings.Creator:
                    ids = "0_CREATOR";
                    break;
                case Strings.Exit:
                    ids = "0_EXIT";
                    break;
                case Strings.Play:
                    ids = "1_PLAY";
                    break;
                case Strings.Pause:
                    ids = "1_PAUSE";
                    break;
                case Strings.Stop:
                    ids = "1_STOP";
                    break;
                case Strings.Next:
                    ids = "1_NEXT";
                    break;
                case Strings.Back:
                    ids = "1_BACK";
                    break;
                case Strings.Version:
                    ids = "2_VERSION";
                    break;
                case Strings.Plugin:
                    ids = "2_PLUGIN";
                    break;
            }

            return Get(ids);
        }

        public static LAPP.Localize Current { get; private set; }

        public static void Load(string Path)
        {
            try
            {
                if (Utils.InstanceData.OverrideLanguage)
                {
                    Dialogs.LogWindow.Append("Language was overridden");
                    Current = LAPP.Localize.Load(Utils.InstanceData.LocalizeFilePath);
                }
                else
                    Current = LAPP.Localize.Load(Path);
                LanguageChanged?.Invoke(null, null);
            }
            catch (Exception)
            {
                Dialogs.LogWindow.Append("Failed to loading localize file");
                Current = new LAPP.Localize();
                LanguageChanged?.Invoke(null, null);
            }
        }
    }
}
