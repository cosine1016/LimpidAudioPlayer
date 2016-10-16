using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                App = new App();
                App.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
                App.Exit += App_Exit;
                App.DispatcherUnhandledException += App_DispatcherUnhandledException;
                App.InitializeComponent();

                LAPP.Events.AppendLog += Events_AppendLog;
                Dialogs.LogWindow.Append("LAP Initialized");
                
                UpdateMan.AutoUpdateAsync();

                App.Run();
            }
        }

        private static void Events_AppendLog(object sender, LAPP.Events.LogEventArgs e)
        {
            Dialogs.LogWindow.Append(e.Msg);
        }

        private static void App_Exit(object sender, System.Windows.ExitEventArgs e)
        {
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

                try
                {
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
                Process.GetCurrentProcess().Kill();
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