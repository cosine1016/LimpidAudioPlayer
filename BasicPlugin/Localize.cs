using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BasicPlugin
{
    public enum Strings
    {
        Title, Open, Config, Creator, Log, Exit, Unknown,
        Play, Pause, Stop, Next, Back,
        Version, Plugin
    }

    internal class Localize
    {
        private static List<Action> ChangedActions = new List<Action>();

        public static void AddLanguageChangedAction(Action Action)
        {
            ChangedActions.Add(Action);
            Action();
        }

        public static void RemoveLanguageChangedAction(Action Action)
        {
            ChangedActions.Remove(Action);
        }

        ~Localize()
        {
            ChangedActions.Clear();
        }

        public static bool ExportLog { get; set; } = true;

        public static string Get(string Key)
        {
            string str = Current.Strings[Key];

            if (Key == str)
                LAPP.Utils.Log.Append("Key Was Not Found : " + Key);

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
                case Strings.Unknown:
                    ids = "0_UNKNOWN";
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

        public static LAPP.Management.Localize Current { get; private set; }

        public static string CurrentFilePath { get; private set; }

        public static void Load(string Path)
        {
            try
            {
                Current = LAPP.Management.Localize.Load(Path);
                CurrentFilePath = Path;
                ChangeLanguage();
            }
            catch (Exception)
            {
                Current = new LAPP.Management.Localize();
                CurrentFilePath = null;
                ChangeLanguage();
            }
        }

        private static void ChangeLanguage()
        {
            for(int i = 0;ChangedActions.Count > i; i++)
            {
                try
                {
                    ChangedActions[i]();
                }
                catch (Exception) { }
            }
        }
    }
}
