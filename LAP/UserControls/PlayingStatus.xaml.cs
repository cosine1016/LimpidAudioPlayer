using ClearUC;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LAP.UserControls
{
    /// <summary>
    /// PlayingStatus.xaml の相互作用ロジック
    /// </summary>
    public partial class PlayingStatus : UserControl
    {
        private const double TimeH = 30;

        public event EventHandler<MouseButtonEventArgs> MouseClick;

        public PlayingStatus()
        {
            InitializeComponent();

            if (Image == null)
            {
                TimeL.Height = Height;
                TimeL.Background.Opacity = 0;
            }
            else
            {
                TimeL.Height = TimeH;
                TimeL.Background.Opacity = 0.7;
            }
        }

        public object Title
        {
            get { return TitleL.Content; }
            set { TitleL.Content = value; }
        }

        public object Album
        {
            get { return AlbumL.Content; }
            set { AlbumL.Content = value; }
        }

        public ImageSource Image
        {
            get { return image.Source; }
            set
            {
                image.Source = value;

                if (value == null)
                {
                    TimeL.Height = Height;
                    TimeL.Background.Opacity = 0;
                }
                else
                {
                    TimeL.Height = TimeH;
                    TimeL.Background.Opacity = 0.7;
                }
            }
        }

        public void SetTime(TimeSpan Remain, TimeSpan Duration)
        {
            TimeL.Content = Remain.ToString(@"mm\:ss") + " / " + Duration.ToString(@"mm\:ss");
        }

        private bool f = false;

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            f = true;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            f = false;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            f = false;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (f)
            {
                MouseClick?.Invoke(this, e);
            }
            f = false;
        }
    }
}