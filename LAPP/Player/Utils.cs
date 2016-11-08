using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.Player
{
    public static class Utils
    {
        public static void Notice(string Text, System.Windows.Media.Brush FillBrush)
        {
            Events.DoNotice(new Events.NotificationEventArgs() { Text = Text,
                FillBrush = FillBrush,
                Assembly = System.Reflection.Assembly.GetCallingAssembly() });
        }

        public static void AppendLog(string Text)
        {
            Events.AppendMsgToLog(Text);
        }
    }
}
