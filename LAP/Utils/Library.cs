using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LAP.Utils
{
    public class Library
    {
        public class File
        {
            public string Path { get; set; } = "";

            public string XMLPath { get; set; } = "";
        }

        public Library(bool Load)
        {
            if (Load) this.Load();
        }

        public List<File> Files { get; set; } = new List<File>();

        public void Save()
        {
            XmlSerializer Ser = new XmlSerializer(typeof(List<File>));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(Config.Setting.Paths.LibraryPath);

            try
            {
                Ser.Serialize(sw, Files);
            }
            finally
            {
                sw.Close();
            }
        }

        public void Load()
        {
            if (System.IO.File.Exists(Config.Setting.Paths.LibraryPath))
            {
                XmlSerializer Ser = new XmlSerializer(typeof(List<File>));
                System.IO.StreamReader sr = new System.IO.StreamReader(Config.Setting.Paths.LibraryPath);

                try
                {
                    Files = (List<File>)Ser.Deserialize(sr);
                }
                finally
                {
                    sr.Close();
                }
            }
        }
    }
}
