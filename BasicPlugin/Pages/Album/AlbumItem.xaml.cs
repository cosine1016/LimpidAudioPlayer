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
    /// ListAlbumItem.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAlbumItem : UserControl
    {
        int maximum = 5, width = 10;

        public ListAlbumItem()
        {
            InitializeComponent();
        }

        public AlbumCollection Albums { get; } = new AlbumCollection();

        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (maximum != value)
                {
                    maximum = value;
                    UpdateGrid(Maximum);
                }
            }
        }

        public int WidthPerAlbum
        {
            get { return width; }
            set
            {
                if(width != value)
                {
                    width = value;
                    UpdateGrid(Maximum);
                }
            }
        }

        private void UpdateGrid(int Max)
        {
            Parent.ColumnDefinitions.Clear();
            GridLength padding = new GridLength(5);

            Parent.ColumnDefinitions.Add(new ColumnDefinition() { Width = padding });
            for(int i = 0; Max > i; i++)
            {
                GridLength perW = new GridLength(width, GridUnitType.Star);
                Parent.ColumnDefinitions.Add(new ColumnDefinition() { Width = perW });
            }
            Parent.ColumnDefinitions.Add(new ColumnDefinition() { Width = padding });
        }
    }

    public class AlbumCollection : System.Collections.ObjectModel.ObservableCollection<Album>
    {
        public void AddRange(Album[] items)
        {
            for (int i = 0; items.Length > i; i++)
                Add(items[i]);
        }
    }
}
