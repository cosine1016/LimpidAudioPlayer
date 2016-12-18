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

namespace BasicPlugin.Pages.Album
{
    /// <summary>
    /// Album.xaml の相互作用ロジック
    /// </summary>
    public partial class Album : UserControl
    {
        public event EventHandler ItemClicked;

        LAPP.IO.FileItem fi = null;

        public Album()
        {
            InitializeComponent();
        }

        bool Changing = false;
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (Changing)
            {
                Changing = false;
                return;
            }

            if(e.Property == HeightProperty)
            {
                Changing = true;
                Width = (double)e.NewValue;
            }

            if(e.Property == WidthProperty)
            {
                Changing = true;
                Height = (double)e.NewValue;
            }
        }

        AlbumData alb = null;
        public AlbumData Data
        {
            get { return alb; }
            set
            {
                if(value != null)
                {
                    label.Content = value.Album;

                    LAPP.IO.MediaFile file = new LAPP.IO.MediaFile(value.Tracks[value.ArtworkIndex].Path);
                    if (file.Artwork != null)
                        image.Source = file.Artwork;
                }
                else
                {
                    label.Content = null;
                    image.Source = null;
                }

                alb = value;

                visible();
            }
        }

        private void visible()
        {
            if(image.Source != null)
            {
                image.Visibility = Visibility.Visible;
                label.Visibility = Visibility.Hidden;
            }
            else
            {
                image.Visibility = Visibility.Hidden;
                label.Visibility = Visibility.Visible;
            }
        }

        public LAPP.IO.FileItem GetFileItem()
        {
            if(fi == null)
            {

            }

            return fi;
        }

        bool flag = false;
        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            rect.Opacity = 0.5;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
            da.Animate(rect.Opacity, 0.1, Config.Current.iValue[Enums.iValue.DefaultAnimation], null, OpacityProperty, rect);

            flag = false;
        }

        private void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rect.Opacity = 0.7;
            flag = true;
        }

        private void rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (flag)
            {
                ItemClicked?.Invoke(this, new EventArgs());
            }

            ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
            da.Animate(rect.Opacity, 0.5, Config.Current.iValue[Enums.iValue.DefaultAnimation], null, OpacityProperty, rect);
        }
    }
}
