using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BasicPlugin
{
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
