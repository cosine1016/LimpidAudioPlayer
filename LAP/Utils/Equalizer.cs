//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Serialization;
//using NWrapper;

//namespace LAP.Utils
//{
//    public class Equalizer
//    {
//        public static event EventHandler EqualizerChanged;

//        public static void OnEqualizerChanged() { EqualizerChanged?.Invoke(null, new EventArgs()); }

//        public static readonly NWrapper.Equalizer.EqualizerBand[] DefaultBands = new NWrapper.Equalizer.EqualizerBand[]
//        {
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 100, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 150, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 300, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 400, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 600, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1000, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 2400, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 3600, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 4800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 7200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 9600, Gain = 0},
//        };

//        static NWrapper.Equalizer.EqualizerBand[] bs = new NWrapper.Equalizer.EqualizerBand[]
//        {
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 100, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 150, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 300, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 400, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 600, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1000, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 2400, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 3600, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 4800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 7200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 9600, Gain = 0},
//        };

//        static NWrapper.Equalizer.EqualizerBand[] tbs = new NWrapper.Equalizer.EqualizerBand[]
//        {
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 100, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 150, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 300, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 400, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 600, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1000, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 1800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 2400, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 3600, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 4800, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 7200, Gain = 0},
//            new NWrapper.Equalizer.EqualizerBand {Bandwidth = 0.8f, Frequency = 9600, Gain = 0},
//        };

//        public static NWrapper.Equalizer.EqualizerBand[] Bands
//        {
//            get { return bs; }
//            set { bs = value; }
//        }

//        public static NWrapper.Equalizer.EqualizerBand[] TempBands
//        {
//            get { return tbs; }
//            set { tbs = value; }
//        }

//        public static string[] GetEqualizerFileNames(out int Index)
//        {
//            Index = 0;
//            System.IO.Directory.CreateDirectory();
//            string[] paths = System.IO.Directory.GetFiles(Config.Setting.Paths.Equalizer,
//                "*" + Config.Setting.Paths.EqualizerExtension, System.IO.SearchOption.TopDirectoryOnly);
//            for(int i = 0;paths.Length > i; i++)
//            {
//                paths[i] = System.IO.Path.GetFileNameWithoutExtension(paths[i]);
//                if (paths[i] == Config.Setting.Paths.EQPath) Index = i + 2;
//            }
//            return paths;
//        }

//        public static void ReadBandFile(string FileName)
//        {
//            XmlSerializer xs = new XmlSerializer(typeof(NWrapper.Equalizer.EqualizerBand[]));

//            System.IO.StreamReader sr = new System.IO.StreamReader(Config.Setting.Paths.Equalizer + FileName + Config.Setting.Paths.EqualizerExtension);
//            Bands = (NWrapper.Equalizer.EqualizerBand[])xs.Deserialize(sr);
//            sr.Close();
//        }

//        public static void WriteBandFile(string FileName, NWrapper.Equalizer.EqualizerBand[] Band)
//        {
//            XmlSerializer xs = new XmlSerializer(typeof(NWrapper.Equalizer.EqualizerBand[]));

//            System.IO.StreamWriter sw = new System.IO.StreamWriter(Config.Setting.Paths.Equalizer + FileName + Config.Setting.Paths.EqualizerExtension);
//            xs.Serialize(sw, Band);
//            sw.Close();
//        }
//    }
//}
