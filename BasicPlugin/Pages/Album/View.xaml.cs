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
using LAPP;

namespace BasicPlugin.Pages.Album
{
    /// <summary>
    /// View.xaml の相互作用ロジック
    /// </summary>
    public partial class View : UserControl
    {
        public event EventHandler UpdateRequest;
        public event EventHandler<PlayFileEventArgs> PlayFile;
        
        private PageItemCollection CurrentItems = new PageItemCollection();
        private AlbumData Current = null;
        private int VisibleIndex = -1;

        public View()
        {
            InitializeComponent();

            Children.CollectionChanged += Children_CollectionChanged;
        }

        double _size = 170;
        public double Size
        {
            get { return _size; }
            set
            {
                _size = value;
                UpdateMargin();
            }
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    Album alb = (Album)e.NewItems[0];
                    ItemGrid.Children.Add(alb);
                    alb.ItemClicked += Alb_ItemClicked;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    Album oalb = (Album)e.OldItems[0];
                    ItemGrid.Children.Remove(oalb);
                    oalb.ItemClicked -= Alb_ItemClicked;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    for (int i = 0; ItemGrid.Children.Count > i; i++)
                        ((Album)ItemGrid.Children[i]).ItemClicked -= Alb_ItemClicked;
                    ItemGrid.Children.Clear();
                    break;
            }

            UpdateMargin();
        }

        private void Alb_ItemClicked(object sender, EventArgs e)
        {
            Album alb = (Album)sender;

            CurrentItems.Clear();
            TrackView.Items.Clear();
            for (int i = 0; alb.Data.Tracks.Length > i; i++)
            {
                LAPP.IO.FileItem fi = CreateItem(alb.Data, i);
                TrackView.Items.Add(fi.ListItem);
                CurrentItems.Add(fi);
            }

            Current = alb.Data;

            ClearUC.Utils.AnimationHelper.Visible va = new ClearUC.Utils.AnimationHelper.Visible();
            int ind = Children.IndexOf(alb);
            if (ind == VisibleIndex)
            {
                VisibleIndex = -1;
                va.Animate(Config.Current.iValue[Enums.iValue.AlbumVisibleAnimation], TrackView, Visibility.Hidden);
            }
            else
            {
                VisibleIndex = ind;
                va.Animate(Config.Current.iValue[Enums.iValue.AlbumVisibleAnimation], TrackView, Visibility.Visible);
            }
        }

        private LAPP.IO.FileItem CreateItem(AlbumData Data, int Index)
        {
            ClearUC.ListViewItems.ListSubItem lsi = new ClearUC.ListViewItems.ListSubItem();
            lsi.MainLabelText = Data.Tracks[Index].Title;
            lsi.SideItem = ClearUC.ListViewItems.ListSubItem.SideItems.Number;

            if (Data.Tracks[Index].TrackNumber > 0)
            {
                lsi.NumberLabelText = Data.Tracks[Index].TrackNumber.ToString();
            }
            else
                lsi.NumberLabelText = null;

            string subl = "";

            if (!string.IsNullOrEmpty(Data.Artist))
            {
                subl += " - " + Data.Artist;
            }
            
            if (subl.Length > 3)
            {
                lsi.SubLabelVisibility = Visibility.Visible;
                lsi.SubLabelText = subl.Remove(0, 3);
            }
            else
                lsi.SubLabelVisibility = Visibility.Hidden;

            return new LAPP.IO.FileItem(new LAPP.IO.MediaFile(Data.Tracks[Index].Path), lsi, true);
        }

        public AlbumControlCollection Children { get; set; } = new AlbumControlCollection();

        private void UpdateMargin()
        {
            AlbumColumn.Width = new GridLength(Size + 20 + SystemParameters.VerticalScrollBarWidth, GridUnitType.Pixel);

            double top = 10;
            for(int i = 0;Children.Count > i; i++)
            {
                Album alb = Children[i];

                alb.Width = Size;
                alb.HorizontalAlignment = HorizontalAlignment.Left;
                alb.VerticalAlignment = VerticalAlignment.Top;
                alb.Margin = new Thickness(10, top, 10, 0);

                top += Size + 10;
            }
        }

        private void TrackView_ItemClicked(object sender, ClearUC.ItemClickedEventArgs e)
        {
            PageItemCollection PageItems = CurrentItems;
            OrderManager om = new OrderManager();

            LAPP.IO.FileItem playF = null;
            int index = TrackView.Items.IndexOf(e.Item);
            for(int i = 0; PageItems.Count > i; i++)
            {
                if(PageItems[i].ListItem == TrackView.Items[index])
                {
                    playF = (LAPP.IO.FileItem)PageItems[i];
                    break;
                }
            }

            if(playF != null)
            {
                om.Scan(PageItems);

                PlayFile?.Invoke(this, new PlayFileEventArgs() { Manager = om, File = playF });
            }
        }

        private void AddLabel_MouseClick(object sender, EventArgs e)
        {
            Dialogs.Album alb = new Dialogs.Album();
            alb.Initialize();

            alb.ShowDialog();

            UpdateRequest?.Invoke(this, new EventArgs());
        }
    }

    public class PlayFileEventArgs
    {
        public OrderManager Manager { get; set; }
        public LAPP.IO.FileItem File { get; set; }
    }

    public class AlbumControlCollection : System.Collections.ObjectModel.ObservableCollection<Album>
    {

    }
}
