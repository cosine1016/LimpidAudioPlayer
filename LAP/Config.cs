using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAP.Enums;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Schema;
using LAPP.Management;

namespace LAP
{
    public class Config
    {
        public const string LoadingError_T = "Failed To Load Config";
        public const string LoadingError_M = "Unknown error has occured.\r\nPlease go to Config -> General" + 
            " and press Reset Config button to recreate config file.";

        private Config()
        {
            InitMembers();
        }

        private static void InitMembers()
        {
            if(cnf != null)
            {
                cnf.Path.GettingValueFunction = PathFunc;
            }
        }

        private static Config cnf = new Config();
        public static Config Current
        {
            get { return cnf; }
            private set
            {
                cnf = value;
                InitMembers();
            }
        }

        public ConfigDictionary<Enums.Path, string> Path { get; set; } = new ConfigDictionary<Enums.Path, string>();

        public ConfigDictionary<Animation, int> Animation { get; set; } = new ConfigDictionary<Animation, int>();

        public ConfigDictionary<bValue, bool> bValue { get; set; } = new ConfigDictionary<bValue, bool>();

        public ConfigDictionary<iValue, int> iValue { get; set; } = new ConfigDictionary<iValue, int>();

        public ConfigDictionary<sValue, string> sValue { get; set; } = new ConfigDictionary<sValue, string>();

        public ConfigDictionary<sArrayValue, string[]> sArrayValue { get; set; } = new ConfigDictionary<sArrayValue, string[]>();

        public WaveOut Output { get; set; } = new WaveOut();

        public static void Load(string Path)
        {
            Path = PathFunc(Path);
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
                Dialogs.LogWindow.Append("Cannot Find Config File");
                Current = new Config();
            }

            Dialogs.LogWindow.Append("Config File Loaded");
        }

        public static void Save(string Path)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Config));
            using (StreamWriter sw = new StreamWriter(Path))
                ser.Serialize(sw, Current);
        }

        private static string PathFunc(string Value)
        {
            Value = Value.Replace("$LAP$",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\LAP\");
            Value = Value.Replace("$PRG$",
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\");


            return Value;
        }
    }
}