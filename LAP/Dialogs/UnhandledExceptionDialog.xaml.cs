using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static LAP.Utils.Update;

namespace LAP.Dialogs
{
    /// <summary>
    /// UnhandledExceptionDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class UnhandledExceptionDialog : Window
    {
        internal bool ExitApp { get; private set; } = true;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int VK_F4 = 0x73;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_CLOSE = 0xF060;

        public const string ErrorPostValue = "LAP_ErrorPost";

        /// <summary>
        /// Loadedイベントハンドラ
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(handle, GWL_STYLE);
            style = style & (~WS_SYSMENU);
            SetWindowLong(handle, GWL_STYLE, style);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((msg == WM_SYSKEYDOWN) &&
                (wParam.ToInt32() == VK_F4))
            {
                handled = true;
            }

            if ((msg == WM_SYSCOMMAND) &&
                (wParam.ToInt32() == SC_CLOSE))
            {
                handled = true;
            }

            return IntPtr.Zero;
        }

        private DialogLanguage JpnLang =
            new DialogLanguage()
            {
                Title = "予期しないエラーが発生しました",
                ErrorMsg = "予期しないエラーによりアプリケーションは終了しました、よろしければ以下のボタンからエラーを報告してください。ご迷惑をおかけし申し訳ございません",
                CantainsData = "このアプリはOS、CPU、メモリ、アプリの実行パスとエラーに関する情報のみを送信します。個人情報は一切送信しません。",
                ReportError = "エラーを報告する",
                DoNotReportError = "エラーを報告しない",
                NetworkHasNotAvailable = "ネットワークの接続が確認できませんでした。可能であればsupport@ksprogram.mods.jpまでメールで報告してください。",
                UploadFailed = "エラー報告に失敗しました。可能であればsupport@ksprogram.mods.jpまでメールで報告してください。",
                IgnoreTheError = "エラーを無視する"
            };

        private DialogLanguage EngLang =
            new DialogLanguage()
            {
                Title = "This app has crashed caused by unknown error",
                ErrorMsg = "An error has occured. Please click the button below to report the error.",
                CantainsData = "This app will send informations about your OS, CPU, memory, application's path, and the error info only.",
                ReportError = "Report this error",
                DoNotReportError = "Do not report this error",
                NetworkHasNotAvailable = "Network has not available. Please send an email about this error to support@ksprogram.mods.jp",
                UploadFailed = "Failed to send report. Please send an email about this error to support@ksprogram.mods.jp",
                IgnoreTheError = "Ignore the error"
            };

        private DialogLanguage CurrentLanguage;

        public UnhandledExceptionDialog()
        {
            InitializeComponent();
            ApplyLang(EngLang);
        }

        private void English_Click(object sender, RoutedEventArgs e)
        {
            ApplyLang(EngLang);
        }

        private void Japanese_Click(object sender, RoutedEventArgs e)
        {
            ApplyLang(JpnLang);
        }

        private void ApplyLang(DialogLanguage Lang)
        {
            Title = Lang.Title;
            textBlock.Text = Lang.ErrorMsg;
            ContainsData.Text = Lang.CantainsData;
            Report.Content = Lang.ReportError;
            DoNotReport.Content = Lang.DoNotReportError;
            IgnoreB.Content = Lang.IgnoreTheError;

            CurrentLanguage = Lang;
        }

        private class DialogLanguage
        {
            public string Title { get; set; }
            public string ErrorMsg { get; set; }
            public string CantainsData { get; set; }
            public string ReportError { get; set; }
            public string DoNotReportError { get; set; }
            public string NetworkHasNotAvailable { get; set; }
            public string UploadFailed { get; set; }
            public string IgnoreTheError { get; set; }
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            ExitApp = true;
            SendReport(ErrorMsg.Text);
        }

        private void DoNotReport_Click(object sender, RoutedEventArgs e)
        {
            ExitApp = true;
            Close();
        }

        private void SendReport(string Msg)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {
                    string adr = Utils.NetworkTools.ReadString(new Utils.NetworkTools.PHP(GetAddressPHP,
                        new Utils.NetworkTools.PHP.Argument[] { new Utils.NetworkTools.PHP.Argument(TitleName, ErrorPostValue) }));

                    if (adr != "-1")
                    {
                        string path = Path.GetTempFileName();

                        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write);
                        byte[] buffer = Encoding.UTF8.GetBytes(Msg);
                        fs.Write(buffer, 0, buffer.Length);
                        fs.Close();

                        WebClient wc = new WebClient();
                        wc.UploadFile(adr, path);
                    }

                    Close();
                }
                catch (Exception)
                {
                    MessageBox.Show(CurrentLanguage.UploadFailed);
                }
            }
            else
            {
                MessageBox.Show(CurrentLanguage.NetworkHasNotAvailable);
            }
        }

        private void IgnoreB_Click(object sender, RoutedEventArgs e)
        {
            ExitApp = false;
            Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
#if DEBUG
            IgnoreB.Visibility = Visibility.Visible;
#endif
        }
    }
}