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
        public Album()
        {
            InitializeComponent();
        }

        public ImageSource Image
        {
            get { return image.Source; }
            set { image.Source = value; }
        }

        public string Title
        {
            get { return (string)label.Content; }
            set { label.Content = value; }
        }
    }
}
