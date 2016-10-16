using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace LAP.Utils
{
    class VersionInfo
    {
        public VersionInfo(Exception UnhandledException)
        {
            this.UnhandledException = UnhandledException;
            AccessFailed = true;
            LatestVersion = CurrentVersion;
        }

        public VersionInfo(Version LatestVersion) { this.LatestVersion = LatestVersion; }

        public VersionInfo(Version LatestVersion, string ShortMessage)
            : this(LatestVersion) { this.ShortMessage = ShortMessage; }

        public VersionInfo(Version LatestVersion, string ShortMessage, string LongMessage)
            : this(LatestVersion, ShortMessage) { this.LongMessage = LongMessage; }

        public enum ComparingResult { Older, Latest }

        public Version LatestVersion { get; set; }

        public Version CurrentVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        /// <summary>
        /// バージョンを比較したときにCurrentが最新か古いかを判断します
        /// </summary>
        /// <param name="Current">現在のバージョン</param>
        /// <param name="Latest">最新のバージョン</param>
        /// <returns></returns>
        public ComparingResult CompareVersions(Version Current, Version Latest)
        {
            bool maj = false, min =false, rev = false, bui = false;
            maj = Latest.Major > Current.Major;
            min = Latest.Minor > Current.Minor;
            rev = Latest.Revision > Current.Revision;
            bui = Latest.Build > Current.Build;

            if (maj || min || rev || bui)
                return ComparingResult.Older;
            else
                return ComparingResult.Latest;
        }

        /// <summary>
        /// 現在のインスタンスでバージョンを比較します
        /// </summary>
        /// <returns></returns>
        public ComparingResult CompareVersions()
        {
            return CompareVersions(CurrentVersion, LatestVersion);
        }

        public string ShortMessage { get; set; }

        public string LongMessage { get; set; }

        public bool AccessFailed { get; set; } = false;

        public Exception UnhandledException { get; set; }
    }

    class Update
    {
        public const string BaseURL = @"https://mods-ksprogram.ssl-lolipop.jp/";

        public const string GetAddressPHP = BaseURL + @"GetAddress.php";

        public const string TitleName = "title";

        public const string LatestValue = "lap_latest";

        public const string DownloadValue = "lap_download";

        public const string ClientUpdaterFile = "ClientUpdater.exe";

        public VersionInfo CheckUpdate()
        {
            LAP.Dialogs.LogWindow.Append("Checking update");
            try
            {
                string Value = NetworkTools.ReadString(new NetworkTools.PHP(GetAddressPHP,
                    new NetworkTools.PHP.Argument[] { new NetworkTools.PHP.Argument(TitleName, LatestValue) }));

                if (!string.IsNullOrEmpty(Value))
                {
                    Latest latest = null;

                    try
                    {
                        XmlSerializer des = new XmlSerializer(typeof(Latest));
                        using (StringReader sr = new StringReader(NetworkTools.ReadString(Value)))
                        {
                            latest = (Latest)des.Deserialize(sr);
                        }

                        Version Latest = new Version(latest.Major, latest.Minor, latest.Build, latest.Revision);
                        VersionInfo vi = new VersionInfo(Latest, latest.ShortMessage);
                        return vi;
                    }
                    catch (Exception ex) { return new VersionInfo(ex); }
                }
                else
                    return null;
            }
            catch(Exception ex) { return new VersionInfo(ex); }
        }

        public UpdateFiles GetUpdateFiles()
        {
            string Value = NetworkTools.ReadString(new NetworkTools.PHP(GetAddressPHP,
                new NetworkTools.PHP.Argument[] { new NetworkTools.PHP.Argument(TitleName, DownloadValue) }));

            try
            {
                UpdateFiles Files = null;
                XmlSerializer des = new XmlSerializer(typeof(UpdateFiles));
                using (StringReader sr = new StringReader(NetworkTools.ReadString(Value)))
                {
                    Files = (UpdateFiles)des.Deserialize(sr);
                }

                return Files;
            }
            catch (Exception) { return null; }
        }

        public void AutoUpdateAsync(bool Silent = true)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                VersionInfo vi = CheckUpdate();
                if (!vi.AccessFailed && vi.CompareVersions() == VersionInfo.ComparingResult.Older)
                {
                    LAP.Dialogs.LogWindow.Append("Newer version available");
                    //TODO Dialog
                    if (ClearUC.Dialogs.Dialog.ShowMessageBox(ClearUC.Dialogs.Dialog.Buttons.YesNo,
                        "Newer Version Available",
                        "New Version of LAP is Available(" + vi.LatestVersion.ToString() + ")\r\nDo You Want to Update It?")
                        == ClearUC.Dialogs.Dialog.ClickedButton.Yes)
                    {
                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(ClientUpdaterFile);
                        psi.Arguments = GenerateArgsForUpdater(GetUpdateFiles());
                        psi.UseShellExecute = false;

                        InstanceData.UpdateProcessInfo = psi;
                        InstanceData.UpdateMode = true;

                        System.Windows.Application.Current.Shutdown(Program.UpdateModeExitCode);
                    }
                }
                else
                {
                    LAP.Dialogs.LogWindow.Append("You're using latest version");
                    //TODO UpdateNotAvailableDialog
                    if (!Silent)
                    {

                    }
                }
            }));
        }

        private string GenerateArgsForUpdater(UpdateFiles Files, bool ExportIcon = true, bool UseUIXML = true)
        {
            List<string> Args = new List<string>();

            string uip = Path.GetTempFileName();
            ClientUpdater.DialogUI uid = new ClientUpdater.DialogUI();
            uid.AutoClose = false;

            ClientUpdater.DialogModule.InitInfo = uid;
            ClientUpdater.DialogModule.SaveXML(uip);
            Args.Add("\"UI:" + uip + "\"");

            if (ExportIcon)
            {
                string iconp = Path.GetTempFileName();

                FileStream fs = new FileStream(iconp, FileMode.Open, FileAccess.Write);
                byte[] buffer = ClientUpdater.DialogModule.IconToBytes(Properties.Resources.Limpid_Audio_Player);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();

                Args.Add("\"icon:" + iconp + "\"");
                Args.Add("\"rm:" + iconp + "\"");
            }

            if (UseUIXML)
            {
                string xml = Path.GetTempFileName();
                ClientUpdater.DialogUI ui = new ClientUpdater.DialogUI();
                ui.AutoClose = true;
            }

            Args.Add("\"Title:Limpid Audio Player\"");

            string file = "\"url:";
            for (int i = 0; Files.Files.Length > i; i++)
            {
                file += BaseURL + Files.Files[i] + ",";
                file += Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\" + Path.GetFileName(Files.Files[i]);
                if (Files.Files.Length - 1 > i) file += ",";
            }
            file += "\"";
            Args.Add(file);
            Args.Add("\"Boot:" + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\" + Files.Boot + "\"");
            Args.Add("\"Work:" + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\"");

            Console.WriteLine(string.Join(" ", Args));
            return string.Join(" ", Args);
        }
    }

    static class NetworkTools
    {
        public class PHP
        {
            public PHP() { }
            public PHP(string URL)
            {
                this.URL = URL;
            }
            public PHP(string URL, Argument[] Arguments) : this(URL)
            {
                this.Arguments.AddRange(Arguments);
            }

            public struct Argument
            {
                public Argument(string Name, string Value)
                {
                    this.Name = Name;
                    this.Value = Value;
                }

                public string Name { get; set; }

                public string Value { get; set; }
            }

            public List<Argument> Arguments { get; set; } = new List<Argument>();

            public string URL { get; set; } = null;

            public string GetURL()
            {
                string Radr = URL;
                if (!Radr.EndsWith("?")) Radr += "?";
                for(int i = 0;Arguments.Count > i; i++)
                {
                    Radr += Arguments[i].Name + "=" + Arguments[i].Value;
                    if (i < Arguments.Count - 1) Radr += "&";
                }
                return Radr;
            }
        }

        public static string ReadString(PHP PHP)
        {
            return ReadString(PHP.GetURL());
        }

        public static string ReadString(string Address, bool ReturnActualValue = false)
        {
            HttpWebResponse st = URLToStream(Address);
            if (st != null)
            {
                StreamReader sr = new StreamReader(st.GetResponseStream(), Encoding.GetEncoding(st.CharacterSet));
                string str = sr.ReadToEnd();
                st.Close();
                sr.Close();

                if (ReturnActualValue)
                    return str;
                else if (str == "-1")
                    return null;
                else
                    return str;
            }
            else
                return null;
        }

        public static HttpWebResponse URLToStream(string URL)
        {
            if (!CheckConnection()) return null;
            HttpWebRequest webreq = null;
            HttpWebResponse wres = null;
            try
            {
                webreq = (HttpWebRequest)WebRequest.Create(URL);
                wres = (HttpWebResponse)webreq.GetResponse();
                if ((int)wres.StatusCode >= 200 && (int)wres.StatusCode < 300)
                {
                    return wres;
                }
            }
            catch (WebException ex)
            {
                LAP.Dialogs.LogWindow.Append(Enum.GetName(typeof(WebExceptionStatus), ex.Status));

                HttpWebResponse res = ex.Response as HttpWebResponse;
                if(res != null)
                {
                    LAP.Dialogs.LogWindow.Append(res.StatusCode + " - " + res.StatusDescription);
                }

                if (wres != null)
                    wres.Close();

                throw ex;
            }

            return null;
        }

        public static bool CheckConnection()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }
    }

    [XmlRoot("Latest")]
    public partial class Latest
    {
        [XmlElement("Major")]
        public byte Major { get; set; }
        [XmlElement("Minor")]
        public byte Minor { get; set; }
        [XmlElement("Build")]
        public byte Build { get; set; }
        [XmlElement("Revision")]
        public byte Revision { get; set; }
        [XmlElement("ShortMsg")]
        public string ShortMessage { get; set; }
        [XmlElement("LongMsg")]
        public string LongMessage { get; set; }
    }
    
    [XmlRoot("UpdateFiles")]
    public partial class UpdateFiles
    {
        [XmlElement("Boot")]
        public string Boot { get; set; }
        [XmlElement("File")]
        public string[] Files { get; set; }
    }
}
