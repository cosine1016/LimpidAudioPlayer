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


        public class WaveOut
        {
            internal static object DeepCopy(object target)
            {
                object result;
                BinaryFormatter b = new BinaryFormatter();

                MemoryStream mem = new MemoryStream();

                try
                {
                    b.Serialize(mem, target);
                    mem.Position = 0;
                    result = b.Deserialize(mem);
                }
                finally
                {
                    mem.Close();
                }

                return result;
            }

            public enum Devices
            {
                ASIO, WASAPI, Wave, DirectSound
            }

            public Devices OutputDevice { get; set; } = Devices.WASAPI;

            public DirectSoundConfig DirectSound { get; set; } = new DirectSoundConfig();

            public ASIOConfig ASIO { get; set; } = new ASIOConfig();

            public WASAPIConfig WASAPI { get; set; } = new WASAPIConfig(NAudio.CoreAudioApi.AudioClientShareMode.Shared);

            public float Amplify { get; set; } = 0;

            [Serializable()]
            public class DirectSoundConfig : ICloneable
            {
                public int Latency { get; set; } = 300;

                public object Clone()
                {
                    return DeepCopy(this);
                }
            }

            [Serializable()]
            public class ASIOConfig : ICloneable
            {
                public string DriverName { get; set; }

                public object Clone()
                {
                    return DeepCopy(this);
                }
            }

            [Serializable()]
            public class WASAPIConfig : ICloneable
            {
                public WASAPIConfig()
                {
                }

                public WASAPIConfig(NAudio.CoreAudioApi.AudioClientShareMode ShareMode)
                {
                    this.ShareMode = ShareMode;
                }

                public NAudio.CoreAudioApi.AudioClientShareMode ShareMode { get; set; } = NAudio.CoreAudioApi.AudioClientShareMode.Shared;

                public int Latency { get; set; } = 50;

                public string DeviceFriendlyName { get; set; }

                public int DeviceIndex { get; set; } = 0;

                public object Clone()
                {
                    return DeepCopy(this);
                }
            }
        }
    }
}