using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace LAP
{
    internal class Program
    {
        internal static event EventHandler NotImplementedException;

        internal static ExceptionInfo ExceptionInformation { get; set; } = new ExceptionInfo();

        internal const int UpdateModeExitCode = 10;

        static App App = null;
        internal static Utils.Update UpdateMan = new Utils.Update();

        [STAThread]
        public static void Main(string[] arg)
        {
            Process CurrentProcess = Process.GetCurrentProcess();
            CurrentProcess.PriorityBoostEnabled = true;
            CurrentProcess.PriorityClass = ProcessPriorityClass.High;

            bool Priority = false;
            bool LogExport = false;
            foreach (string Arg in arg)
            {
                bool Processed = true;
                switch (Arg)
                {
                    case "-SafeMode":
                        Utils.InstanceData.UseDefaultSetting = true;
                        Utils.InstanceData.SafeMode = true;
                        break;

                    case "-Default":
                        Utils.InstanceData.UseDefaultSetting = true;
                        break;

                    case "-DisableAS":
                        Utils.InstanceData.AutoSave = false;
                        break;

                    case "-RaiseError":
                        Utils.InstanceData.ErrorRaise = true;
                        break;

                    case "-Priotity":
                        Priority = true;
                        break;

                    case "-Log":
                        Utils.InstanceData.LogMode = true;
                        break;

                    case "-LogExport":
                        LogExport = true;
                        break;

                    case "-DoNotInitialize":
                        Utils.InstanceData.DoNotInitialize = true;
                        break;

                    case "-InitPluginDir":
                        Utils.InstanceData.DoNotInitialize = true;
                        Initialize();
                        System.IO.Directory.CreateDirectory(Config.Current.Path[Enums.Path.PluginDirectory]);
                        break;

                    case "-Hash":
#if DEBUG == false
                        byte[] hash = Utils.InstanceData.SrtLib.Auth.ComputeHash(new System.Security.Cryptography.SHA256Managed(),
                            Process.GetCurrentProcess().Modules[0].FileName);

                        Console.WriteLine(BitConverter.ToString(hash).Replace("-", ""));
#endif
                        break;

                    default:
                        if (Priority)
                        {
                            switch (Arg)
                            {
                                case "RealTime":
                                    CurrentProcess.PriorityClass = ProcessPriorityClass.RealTime;
                                    break;

                                case "AboveNormal":
                                    CurrentProcess.PriorityClass = ProcessPriorityClass.AboveNormal;
                                    break;

                                case "Normal":
                                    CurrentProcess.PriorityClass = ProcessPriorityClass.Normal;
                                    break;

                                case "Low":
                                    CurrentProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                                    break;
                            }
                            Priority = false;
                        }
                        else if (LogExport)
                        {
                            Utils.InstanceData.LogExp = true;
                            Utils.InstanceData.LogExpPath = Arg;
                        }
                        else if (Arg.StartsWith("-Output"))
                        {
                            OutputParser(Arg);
                        }
                        else if (Arg.StartsWith("-Loc"))
                        {
                            Utils.InstanceData.OverrideLanguage = true;
                            int ind = Arg.IndexOf("=");
                            Utils.InstanceData.LocalizeFilePath =
                                Arg.Substring(ind + 1, Arg.Length - ind - 1);
                        }
                        else
                        {
                            Priority = false;
                            LogExport = false;
                            Processed = false;
                        }
                        break;
                }

                if (!Processed)
                    Dialogs.LogWindow.Append(Arg + " : Unknown Arg");
                else
                    Dialogs.LogWindow.Append(Arg + " : Processed");
            }

            if (Utils.InstanceData.DoNotInitialize == false)
            {
                Initialize();

                if(Config.Current.bValue[Enums.bValue.AutoUpdate])
                    UpdateMan.AutoUpdate(true);

                App = new App();
                App.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
                App.Exit += App_Exit;
                App.DispatcherUnhandledException += App_DispatcherUnhandledException;
                App.InitializeComponent();

                LAPP.Events.AppendLog += Events_AppendLog;
                Dialogs.LogWindow.Append("LAP Initialized");

                App.Run();
            }
        }

        private static void Initialize()
        {
            //この順番は変更するとエラー起こす
            Localize.Load(Config.Current.Path[Enums.Path.LanguageFile]);
            Config.Load(Config.Current.Path[Enums.Path.SettingFile]);
        }

        private static void OutputParser(string Arg)
        {
            Arg = Arg.ToLower();
            Arg = Arg.Replace("-output:", "");

            Utils.InstanceData.OverrideOutput = true;

            switch (Arg)
            {
                case "wave":
                case "wav":
                    Utils.InstanceData.OverrideDevice = Config.WaveOut.Devices.Wave;
                    break;
                case "directsound":
                case "ds":
                    Utils.InstanceData.OverrideDevice = Config.WaveOut.Devices.DirectSound;
                    break;
                case "wasapi":
                    Utils.InstanceData.OverrideDevice = Config.WaveOut.Devices.WASAPI;
                    break;
                case "asio":
                    Utils.InstanceData.OverrideDevice = Config.WaveOut.Devices.ASIO;
                    break;
                default:
                    Utils.InstanceData.OverrideOutput = false;
                    break;
            }
        }

        private static void Events_AppendLog(object sender, LAPP.Events.LogEventArgs e)
        {
            Dialogs.LogWindow.Append(e.Msg);
        }

        private static void App_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            LAPP.Player.Receiver.RaiseReceivedEvent(new LAPP.Player.Receiver.EventReceiveArgs(LAPP.Player.Receiver.Action.Exited, e.ApplicationExitCode));
            Dialogs.LogWindow.Append("Application is Shutting Down(Code " + e.ApplicationExitCode + ")");

            if (Utils.InstanceData.DoNotInitialize == false)
                App.DispatcherUnhandledException -= App_DispatcherUnhandledException;

            if (Utils.InstanceData.LogExp)
                Dialogs.LogWindow.ExportLog(Utils.InstanceData.LogExpPath);

            if(e.ApplicationExitCode == UpdateModeExitCode)
                Process.Start(Utils.InstanceData.UpdateProcessInfo);
        }

        private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Handled == false)
            {
                if (e.Exception.GetType() == typeof(NotImplementedException))
                {
                    e.Handled = true;
                    NotImplementedException?.Invoke(sender, e);
                    return;
                }

                if (Config.Current != null && Utils.PluginManager.InitializedPlugin.Count > 0)
                {
                    for(int i = 0;Utils.PluginManager.InitializedPlugin.Count > i; i++)
                    {
                        System.Reflection.Assembly asm = Utils.PluginManager.InitializedPlugin[i].Asm;
                        if(asm.GetName().Name == e.Exception.Source)
                        {
                            e.Handled = true;

                            Utils.PluginManager.UnloadPlugin(Utils.PluginManager.InitializedPlugin[i]);
                            ClearUC.Dialogs.Dialog.ShowMessageBox(ClearUC.Dialogs.Dialog.Buttons.OKOnly, "Plugin Error",
                                asm.GetName().Name + " is not compatible plugin\nThe plugin was unloaded");
                            return;
                        }
                    }
                }
                
                try
                {
                    if (e.Handled) return;
                    System.Management.ManagementClass mc =
                        new System.Management.ManagementClass("Win32_OperatingSystem");
                    System.Management.ManagementObjectCollection moc = mc.GetInstances();
                    foreach (System.Management.ManagementObject mo in moc)
                    {
                        ExceptionInformation.OSName = mo["Name"].ToString();
                        ExceptionInformation.OSVersion = mo["Version"].ToString();
                        ExceptionInformation.OSLocale = mo["Locale"].ToString();
                        ExceptionInformation.OSLanguage = mo["OSLanguage"].ToString();
                        ExceptionInformation.OSBuildNumber = mo["BuildNumber"].ToString();

                        int.TryParse(mo["TotalVisibleMemorySize"].ToString(), out ExceptionInformation.TotalVisibleMemorySize);
                        int.TryParse(mo["FreePhysicalMemory"].ToString(), out ExceptionInformation.FreePhysicalMemory);
                        int.TryParse(mo["TotalVirtualMemorySize"].ToString(), out ExceptionInformation.TotalVisibleMemorySize);
                        int.TryParse(mo["FreeVirtualMemory"].ToString(), out ExceptionInformation.FreeVirtualMemorySize);
                        int.TryParse(mo["FreeSpaceInPagingFiles"].ToString(), out ExceptionInformation.FreeSpaceInPagingFiles);
                        int.TryParse(mo["SizeStoredInPagingFiles"].ToString(), out ExceptionInformation.SizeStoredInPagingFiles);
                        object o = mo["TotalSwapSpaceSize"];
                        if (o != null)
                            int.TryParse(o.ToString(), out ExceptionInformation.TotalSwapSpaceSize);

                        mo.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionInformation.Message += "Failed to get some informations.\r\n" + ex.ToString();
                }

                ExceptionInformation.Exception = e.Exception;
                ExceptionInformation.Handled = e.Handled;
                ExceptionInformation.Log = Dialogs.LogWindow.LogStr;

                Dialogs.UnhandledExceptionDialog dlg = new Dialogs.UnhandledExceptionDialog();
                dlg.ErrorMsg.Text = ExceptionInformation.ToString();
                dlg.ShowDialog();
                if (dlg.ExitApp)
                    Process.GetCurrentProcess().Kill();
                else
                    e.Handled = true;
            }
        }

        internal class ExceptionInfo
        {
            public Exception Exception = null;

            public bool Handled = false;
            public string OSName = "";
            public string OSVersion = "";
            public string OSLocale = "";
            public string OSLanguage = "";
            public string OSBuildNumber = "";

            public int TotalVisibleMemorySize = 0;
            public int FreePhysicalMemory = 0;
            public int TotalVirtualMemorySize = 0;
            public int FreeVirtualMemorySize = 0;
            public int FreeSpaceInPagingFiles = 0;
            public int SizeStoredInPagingFiles = 0;
            public int TotalSwapSpaceSize = 0;

            public string Message = "";
            public string Log = "";

            public override string ToString()
            {
                string str = "";

                str +=
                    "AppPath : " + System.Reflection.Assembly.GetExecutingAssembly().Location +
                    "\r\nOSName : " + OSName +
                    "\r\nOSVersion : " + OSVersion +
                    "\r\nOSLocale : " + OSLocale +
                    "\r\nOSLanguage : " + OSLanguage +
                    "\r\nOSBuildNumber : " + OSBuildNumber +
                    "\r\nTotalVisibleMemorySize : " + TotalVisibleMemorySize +
                    "\r\nFreePhysicalMemory : " + FreePhysicalMemory +
                    "\r\nTotalVirtualMemorySize : " + TotalVirtualMemorySize +
                    "\r\nFreeVirtualMemorySize : " + FreeVirtualMemorySize +
                    "\r\nFreeSpaceInPagingFiles : " + FreeSpaceInPagingFiles +
                    "\r\nSizeStoredInPagingFiles : " + SizeStoredInPagingFiles +
                    "\r\nTotalSwapSpaceSize : " + TotalSwapSpaceSize + "\r\n";

                if (Exception != null)
                {
                    str += "Exception : " + Exception.GetType().ToString() +
                        "\r\nHResult : " + Exception.HResult +
                        "\r\nTargetSite : " + Exception.TargetSite +
                        "\r\nMessage : " + Exception.Message +
                        "\r\nStackTrace : " + Exception.StackTrace + "\r\n";
                }
                else
                {
                    str += "Exception : null\r\n";
                }

                if (string.IsNullOrEmpty(Message))
                {
                    str += "Another Message : null\r\n";
                }
                else
                {
                    str += "Another Message : " + Message + "\r\n";
                }

                return str;
            }
        }
    }
}