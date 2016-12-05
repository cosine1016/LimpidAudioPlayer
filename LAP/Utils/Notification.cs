using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearUC;

namespace LAP.Utils
{
    public class Notification
    {
        bool flag = false;
        NotificationBar NB;
        Panel P;

        public event EventHandler Click;

        public NotificationBar NotificationBar { get { return NB; } }

        public Notification(Panel Parent, string Message, string EnterLabelText, Brush BackgroundBrush)
        {
            NB = new NotificationBar();
            P = Parent;
            NB.BackgroundBrush = BackgroundBrush;
            NB.Animate = false;
            NB.ShowEnterLabel = true;
            NB.EnterLabelText = EnterLabelText;
            NB.HorizontalAlignment = HorizontalAlignment.Stretch;
            NB.VerticalAlignment = VerticalAlignment.Top;
            P.Children.Add(NB);
            NB.Minimize();
            NB.Message = Message;
            NB.AnimationDuration = Config.Current.Animation[Enums.Animation.Notification];
            NB.ShowingDuration = Config.Current.Animation[Enums.Animation.Notification];
            NB.Click += NB_Click;
        }

        public Notification(Panel Parent, string Message, Brush BackgroundBrush) : this(Parent, Message, null, BackgroundBrush) { }

        public Brush BackgroundBrush
        {
            get { return NB.BackgroundBrush; }
            set { NB.BackgroundBrush = value; }
        }

        public void ShowMessage()
        {
            NB.ShowMessage();
            NB.MessageMinimized += NB_MessageMinimized;
            flag = true;
        }

        public void Maximize()
        {
            NB.Maximize();
        }

        public void Minimize()
        {
            NB.Minimize();
        }

        private void NB_Click(object sender, EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        private void NB_MessageMinimized(object sender, EventArgs e)
        {
            if(flag == true)
            {
                NB.MessageMinimized -= NB_MessageMinimized;
                P.Children.Remove(NB);
            }
        }
    }
}
