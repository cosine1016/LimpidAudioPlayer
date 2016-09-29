using System;
using System.Windows;
using System.Windows.Media;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// ListMenuItem.xaml の相互作用ロジック
    /// </summary>
    public partial class ListMenuItem : ListItem
    {
        public ListMenuItem()
        {
            InitializeComponent();

            ImageIndexChanged += ListMenuItem_ImageIndexChanged;
        }

        private void ListMenuItem_ImageIndexChanged(object sender, EventArgs e)
        {
            if (ImageIndex > -1) image.Source = ImageSources[ImageIndex];
            else image.Source = null;
        }

        public Brush MainLabelBrush
        {
            get { return MainL.Foreground; }
            set { MainL.Foreground = value; }
        }

        public Brush SubLabelBrush
        {
            get { return SubL.Foreground; }
            set { SubL.Foreground = value; }
        }

        public string MainLabelText
        {
            get { return (string)MainL.Content; }
            set
            {
                MainL.Content = value;
                SearchText = value;
            }
        }

        public string SubLabelText
        {
            get { return (string)SubL.Content; }
            set { SubL.Content = value; }
        }

        public Visibility SubLabelVisibility
        {
            get { return SubL.Visibility; }
            set { SubL.Visibility = value; }
        }

        public Visibility ImageVisibility
        {
            get { return image.Visibility; }
            set { image.Visibility = value; }
        }
    }
}