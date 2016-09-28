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
        public static Language Language { get; set; }

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

            UpdateLAPP();

            Library = new Library(true);
        }

        public static void UpdateLAPP()
        {
            LAPP.Utils.Config.Setting = new LAPP.Utils.Setting();
            LAPP.Utils.Config.Setting.Paths.Cache = Setting.Paths.Cache;
            LAPP.Utils.Config.Setting.Paths.LibraryPath = Setting.Paths.LibraryPath;
            LAPP.Utils.Config.Setting.Paths.ScanPaths = Setting.Paths.ScanPaths;
            LAPP.Utils.Config.Setting.Paths.ScanFilters = Setting.Paths.ScanFilters;
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

        public static void ApplySetting(MainWindow MainWindow)
        {
        }

        public static void ReadLanguage(string Path)
        {
            if (File.Exists(Path) == true)
            {
                XmlSerializer xsr = new XmlSerializer(typeof(Language));
                StreamReader sr = new StreamReader(Path, Encoding.GetEncoding("shift-jis"));

                try
                {
                    Language = (Language)xsr.Deserialize(sr);
                    LAP.Dialogs.LogWindow.Append("Language Deserialized : " + Path);
                }
                catch (Exception)
                {
                    Language = new Language();
                    LAP.Dialogs.LogWindow.Append("Default Language Applied");
                }
                finally
                {
                    sr.Close();
                }
            }
            else
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
                Language = new Language();
                LAP.Dialogs.LogWindow.Append("Default Language Applied");
            }

            OperatingSystem os = Environment.OSVersion;
            if (os.Version.Major < 6)
            {
                LAP.Dialogs.LogWindow.Append("Unsupported OS Detected (" + os.ToString() + ")");
                ClearUC.Dialogs.Dialog.ShowMessageBox(ClearUC.Dialogs.Dialog.Buttons.OKOnly,
                    Language.Strings.ExceptionMessage.UnsupportedOS[0], Language.Strings.ExceptionMessage.UnsupportedOS[1]);
                Utility.SafeClose();
            }
        }

        public static void WriteLanguage(string Path)
        {
            if (Directory.Exists(System.IO.Path.GetDirectoryName(Path)) == true)
            {
                XmlSerializer xsr = new XmlSerializer(typeof(Language));
                StreamWriter sw = new StreamWriter(
                            Path, false, Encoding.GetEncoding("shift-jis"));

                try
                {
                    xsr.Serialize(sw, Language);
                }
                finally
                {
                    sw.Close();
                }
            }
            else
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
                WriteLanguage(Path);
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

    public class Language
    {
        public string Name { get; set; } = new CultureInfo("en-US").NativeName;

        public Strings Strings { get; set; } = new Strings();
    }
}