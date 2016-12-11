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

namespace BasicPlugin.MediaPanels
{
    /// <summary>
    /// Artwork.xaml の相互作用ロジック
    /// </summary>
    public partial class Artwork : UserControl
    {
        public Artwork()
        {
            InitializeComponent();
        }

        public ImageSource Source
        {
            get { return image.Source; }
            set { image.Source = value; }
        }

        public string Label
        {
            get { return (string)label.Content; }
            set { label.Content = value; }
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(Label))
            {
                LabelParent.Visibility = Visibility.Hidden;
                return;
            }

            ClearUC.Utils.AnimationHelper.Visible va = new ClearUC.Utils.AnimationHelper.Visible();
            va.Animate(300, LabelParent, Visibility.Visible);
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(Label))
            {
                LabelParent.Visibility = Visibility.Hidden;
                return;
            }

            ClearUC.Utils.AnimationHelper.Visible va = new ClearUC.Utils.AnimationHelper.Visible();
            va.Animate(300, LabelParent, Visibility.Hidden);
        }
    }
}
