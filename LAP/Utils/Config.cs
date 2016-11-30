using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LAP.Utils
{
    public static class Config
    {
        public static Setting Setting { get; set; }

        public static Library Library { get; set; }

        public static void ReadSetting(string Path)
        {
            if (InstanceData.UseDefaultSetting)
            {
                LAP.Dialogs.LogWindow.Append("Default Setting Applied");
                Setting = new Setting();
            }
            else
            {
                if (File.Exists(Path) == true)
                {
                    XmlSerializer xsr = new XmlSerializer(typeof(Setting));
                    StreamReader sr = new StreamReader(Path, Encoding.GetEncoding("shift-jis"));

                    try
                    {
                        Setting = (Setting)xsr.Deserialize(sr);

                        LAP.Dialogs.LogWindow.Append("Setting Deserialized");
                    }
                    catch (Exception ex)
                    {
                        LAP.Dialogs.LogWindow.Append(ex.ToString());
                        Setting = new Setting();
                        LAP.Dialogs.LogWindow.Append("Default Setting Applied");
                    }
                    finally
                    {
                        sr.Close();
                    }
                }
                else
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
                    Setting = new Setting();
                    LAP.Dialogs.LogWindow.Append("Default Setting Applied");
                }
            }

            Library = new Library(true);
        }

        public static void WriteSetting(string Path)
        {
            if (Utils.InstanceData.AutoSave == false) return;
            if (Directory.Exists(System.IO.Path.GetDirectoryName(Path)) == true)
            {
                XmlSerializer xsr = new XmlSerializer(typeof(Setting));
                StreamWriter sw = new StreamWriter(
                            Path, false, Encoding.GetEncoding("shift-jis"));

                try
                {
                    xsr.Serialize(sw, Setting);
                    LAP.Dialogs.LogWindow.Append("Setting Saved");
                }
                finally
                {
                    sw.Close();
                }
            }
            else
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
                WriteSetting(Path);
            }
        }
    }

    public class Setting
    {
        public Boolean Boolean = new Boolean();

        public Brushes Brushes = new Brushes();

        public WaveOut WaveOut = new WaveOut();

        public Pages Pages = new Pages();

        public Paths Paths = new Paths();

        public Values Values = new Values();
    }
}