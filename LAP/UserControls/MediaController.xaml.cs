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
    /// MediaController.xaml の相互作用ロジック
    /// </summary>
    public partial class MediaController : UserControl
    {
        public MediaController()
        {
            InitializeComponent();
            SizeChanged += MediaController_SizeChanged;
        }

        private bool vis = false;

        private void MediaController_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < StatusVisibleWidth && e.PreviousSize.Width >= StatusVisibleWidth)
            {
                PlayingStatus.Visibility = Visibility.Hidden;
            }

            if (e.PreviousSize.Width < StatusVisibleWidth & e.NewSize.Width >= StatusVisibleWidth && vis)
            {
                PlayingStatus.Opacity = 1;
                PlayingStatus.Visibility = Visibility.Visible;
            }
        }

        public void VisibleStatus()
        {
            if (vis) return;
            vis = true;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ActualWidth >= StatusVisibleWidth)
                {
                    Utils.Animation.Visible va = new Utils.Animation.Visible();
                    va.Animate(Config.Current.Animation[Enums.Animation.Default], PlayingStatus, Visibility.Visible);
                }
            }));
        }

        public void HideStatus()
        {
            if (vis == false) return;
            vis = false;

            Utils.Animation.Visible va = new Utils.Animation.Visible();
            va.Animate(Config.Current.Animation[Enums.Animation.Default], PlayingStatus, Visibility.Hidden);
        }

        public int StatusVisibleWidth { get; set; } = 930;
    }
}