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

namespace LAP
{
    public class ConfigDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public ConfigDictionary() { }
        public ConfigDictionary(Func<TKey, TValue> KeyConverter) { this.KeyConverter = KeyConverter; }
        public ConfigDictionary(Func<TKey, TValue> KeyConverter, TKey[] Keys, TValue[] Values) : this(KeyConverter)
        {
            if (Keys.Length != Values.Length)
                throw new Exception("Keys' Length must be the same as Values'");

            for (int i = 0; Keys.Length > i; i++)
            {
                SetDefaultValue(Keys[i], Values[i]);
                this[Keys[i]] = Values[i];
            }
        }

        public Func<TKey, TValue> KeyConverter { get; set; } = null;
        public Func<TValue, TValue> SettingValueFunction { get; set; } = null;
        public Func<TValue, TValue> GettingValueFunction { get; set; } = null;

        private Dictionary<TKey, TValue> DefValues = new Dictionary<TKey, TValue>();
        public new TValue this[TKey Key]
        {
            get
            {
                if (ContainsKey(Key))
                {
                    TValue ret = base[Key];
                    if (GettingValueFunction != null)
                        ret = GettingValueFunction(ret);

                    return ret;
                }
                else
                    return GetDefaultValue(Key);
            }
            set
            {
                TValue val = value;
                if (SettingValueFunction != null)
                    val = SettingValueFunction(val);

                if (!ContainsKey(Key))
                {
                    if (KeyConverter != null)
                        DefValues[Key] = KeyConverter(Key);
                    else
                        DefValues[Key] = default(TValue);
                }

                base[Key] = val;
            }
        }

        public void SetDefaultValue(TKey Key, TValue Value)
        {
            DefValues[Key] = Value;
        }

        public TValue GetDefaultValue(TKey Key)
        {
            if (!DefValues.ContainsKey(Key))
            {
                if (KeyConverter != null)
                    DefValues[Key] = KeyConverter(Key);
                else
                    DefValues[Key] = default(TValue);
            }

            if (GettingValueFunction != null)
                return GettingValueFunction(DefValues[Key]);
            else
                return DefValues[Key];
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                KeyValue kv = serializer.Deserialize(reader) as KeyValue;
                if (kv != null)
                    Add(kv.Key, kv.Value);
            }
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            if(Keys.Count > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(KeyValue));
                foreach (var key in Keys)
                {
                    serializer.Serialize(writer, new KeyValue(key, this[key]));
                }
            }
        }

        public class KeyValue
        {
            public KeyValue() { }
            public KeyValue(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }
    }

    public class Config
    {
        public const string LoadingError_T = "T_LOADCNF";
        public const string LoadingError_M = "M_LOADCNF";

        public Config()
        {
            Path.GettingValueFunction = PathFunc;
        }

        public static Config Current { get; private set; } = new Config();

        public ConfigDictionary<Enums.Path, string> Path { get; set; } = new ConfigDictionary<Enums.Path, string>(
            new Func<Enums.Path, string>((Key) => { return Key.ToString(); }),
            new Enums.Path[] 
            {
                Enums.Path.LanguageDirectory,
                Enums.Path.LanguageFile,
                Enums.Path.SettingFile
            },
            new string[]
            {
                @"$LAP$Languages\English.loc",
                @"$LAP$Languages\English.loc",
                @"$LAP$Setting.lcnf"
            });

        public ConfigDictionary<Animation, int> Animation { get; set; } = new ConfigDictionary<Animation, int>(
            new Func<Enums.Animation, int>((key) => { return 0; }),
            new Animation[]
            {
                Enums.Animation.Default,
                Enums.Animation.Notification
            },
            new int[]
            {
                200,
                3000
            });

        public ConfigDictionary<iValue, int> iValue { get; set; } = new ConfigDictionary<iValue, int>();

        public ConfigDictionary<sValue, string> sValue { get; set; } = new ConfigDictionary<sValue, string>();

        public ConfigDictionary<sArrayValue, string[]> sArrayValue { get; set; } = new ConfigDictionary<sArrayValue, string[]>();

        public WaveOut Output { get; set; } = new WaveOut();

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
                        Localize.Get(LoadingError_T), Localize.Get(LoadingError_M));
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

        private string PathFunc(string Value)
        {
            Value = Value.Replace("$LAP$", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\LAP\");
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