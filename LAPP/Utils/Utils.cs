using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.Utils
{
    public static class Player
    {
        public static void Notice(string Text, System.Windows.Media.Brush FillBrush)
        {
            Events.DoNotice(new Events.NotificationEventArgs() { Text = Text, FillBrush = FillBrush });
        }
    }
}
