using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP.Management;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Schema;

namespace BasicPlugin
{
    public class Config
    {
        public const string LoadingError_T = "Failed To Load Config";
        public const string LoadingError_M = "Unknown error has occured.\r\nPlease go to Config -> General" + 
            " and press Reset Config button to recreate config file.";

        private static Config cnf = new Config();
        public static Config Current
        {
            get { return cnf; }
            private set
            {
                cnf = value;
            }
        }

        public static void Load(string Path)
        {
            if (File.Exists(Path))
            {
                try
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Config));
                    using (StreamReader sr = new StreamReader(Path))
                        Current = (Config)ser.Deserialize(sr);
                }
                catch (Exception)
                {
                    ClearUC.Dialogs.Dialog.ShowMessageBox(ClearUC.Dialogs.Dialog.Buttons.OKOnly,
                        LoadingError_T, LoadingError_M);
                    Current = new Config();
                }
            }
            else
            {
                Current = new Config();
            }
        }

        public static void Save(string Path)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Config));
            using (StreamWriter sw = new StreamWriter(Path))
                ser.Serialize(sw, Current);
        }

        public ConfigDictionary<Enums.Path, string> Path
            = new ConfigDictionary<Enums.Path, string>();

        public ConfigDictionary<Enums.bValue, bool> bValue
            = new ConfigDictionary<Enums.bValue, bool>();
    }
}