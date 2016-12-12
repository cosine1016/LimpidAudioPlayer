using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP
{
    /// <summary>
    /// LAPが使用するイベントが定義されています。
    /// プラグイン製作者はこのクラスは使用しないでください。予期せぬ動作をする可能性があります。
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class Events
    {
        public static void AppendMsgToLog(string Msg)
        {
            AppendLog?.Invoke(null, new LogEventArgs(Msg));
        }

        internal static int LastLCID = 0;
        public static void LanguageUpdated(int LCID)
        {
            LastLCID = LCID;
            LanguageChanged?.Invoke(null, new Utils.TypeEventArgs<int>(LCID));
        }

        internal static void DoNotice(NotificationEventArgs e)
        {
            Notice?.Invoke(null, e);
        }

        public static event EventHandler<LogEventArgs> AppendLog;
        public static event EventHandler<NotificationEventArgs> Notice;
        internal static event EventHandler<Utils.TypeEventArgs<int>> LanguageChanged;

        public class LogEventArgs : EventArgs
        {
            public LogEventArgs(string Message)
            {
                Msg = Message;
            }

            public string Msg;
        }


        public class NotificationEventArgs : EventArgs
        {
            public System.Windows.Media.Brush FillBrush { get; set; }
            public string Text { get; set; }
            public System.Reflection.Assembly Assembly { get; set; }
        }
    }
}
