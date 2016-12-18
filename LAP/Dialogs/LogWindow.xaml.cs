using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LAP.Dialogs
{
    /// <summary>
    /// LogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LogWindow : Window
    {
        private static event EventHandler LogChanged;

        internal static string LogStr { get; private set; } = "";

        internal static void Append(string Msg)
        {
            if (Msg.EndsWith("\n") == false) Msg += "\n";
            LogStr += "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + Msg;
            LogChanged?.Invoke(null, null);
        }

        internal static void ExportLog(string Path)
        {
            string dir = System.IO.Path.GetDirectoryName(Path);
            string savep = Path;

            if (Path == "Default")
                savep = "LAP_Log_" + Utils.Utility.GetUnixTime(DateTime.Now) + ".log";

            if (Path.IndexOf("{num}") > -1)
            {
                int num = 0, lmt = 1000;
                while (true)
                {
                    if (System.IO.File.Exists(Path.Replace("{num}", num.ToString())) == false)
                    {
                        savep = Path.Replace("{num}", num.ToString());
                        break;
                    }
                    else
                        num++;

                    if (num == lmt)
                        savep = "LAP_Log_" + Utils.Utility.GetUnixTime(DateTime.Now) + ".log";
                }
            }

            try
            {
                System.IO.StreamWriter dammy = new System.IO.StreamWriter(savep);
                dammy.Write("Dammy");
                dammy.Close();
            }
            catch (Exception)
            {
                savep = "LAP_Log_" + Utils.Utility.GetUnixTime(DateTime.Now) + ".log";
            }

            try
            {
                Append("Exporting Log : " + savep);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(savep, false, Encoding.UTF8);
                sw.Write(LogStr);

                Append("Log Exported");
                sw.Close();
            }
            catch (Exception ex)
            {
                Append(ex.ToString());
                Append("Failed to Export Log File");
            }
        }

        public LogWindow()
        {
            InitializeComponent();
            Caption.Title = Localize.Get(Strings.Log);

            Closing += LogWindow_Closing;
            Closed += LogWindow_Closed;
            Loaded += LogWindow_Loaded;
            LogChanged += LogWindow_LogChanged;
        }

        private void LogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void LogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Text = LogStr;
        }

        private void LogWindow_Closed(object sender, EventArgs e)
        {
            LogChanged -= LogWindow_LogChanged;
        }

        private void LogWindow_LogChanged(object sender, EventArgs e)
        {
            Log.Text = LogStr;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    Root.Margin = new Thickness(6);
                    break;

                default:
                    Root.Margin = new Thickness(0);
                    break;
            }
        }
    }
}