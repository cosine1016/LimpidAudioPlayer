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
using System.Windows.Shapes;

namespace BasicPlugin.Dialogs
{
    /// <summary>
    /// Album.xaml の相互作用ロジック
    /// </summary>
    public partial class Album : Window
    {
        bool TitleEdited = false;
        bool EditMode = false;

        System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        ClearUC.ListViewItems.ListSubItem AddItem = new ClearUC.ListViewItems.ListSubItem();
        ClearUC.ListViewItems.ListButtonsItem.ListButton RemoveButton;
        List<LAPP.IO.FileItem> items = new List<LAPP.IO.FileItem>();
        int aIndex = -1;

        public Album()
        {
            InitializeComponent();

            Localize.AddLanguageChangedAction(UpdateLocalize);
        }

        public void UpdateLocalize()
        {
            if (EditMode)
            {
                Caption.Title = Localize.Get("EDITALBUM");
            }
            else
            {
                Caption.Title = Localize.Get("CREATEALBUM");
            }
            ofd.Title = Localize.Get("0_OPEN");
            AddItem.MainLabelText = Localize.Get("ADDFILE");
            SaveB.Content = Localize.Get("SAVE");
        }

        public void Initialize()
        {
            InitItems();
        }

        public void Initialize(Pages.Album.AlbumData Data, string FilePath)
        {
            EditMode = true;
            InitItems();

            for(int i = 0;Data.Tracks.Length > i; i++)
            {
                
            }
        }

        public void InitItems()
        {
            FileView.Items.Clear();

            ofd.Multiselect = true;

            AddItem.SubLabelVisibility = Visibility.Hidden;
            AddItem.ItemClicked += AddItem_ItemClicked;

            FileView.Items.Add(AddItem);
        }

        private void AddItem_ItemClicked(object sender, ClearUC.ItemClickedEventArgs e)
        {
            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for(int i = 0;ofd.FileNames.Length > i; i++)
                {
                    LAPP.IO.FileItem item = CreateItem(new LAPP.IO.MediaFile(ofd.FileNames[i]));
                    items.Add(item);
                    FileView.Items.Add(item.ListItem);

                    if (!TitleEdited && !string.IsNullOrEmpty(item.File.Album))
                    {
                        editableLabel.Text = item.File.Album;
                        TitleEdited = true;
                    }

                    if(image.Source == null && item.File.Artwork != null)
                    {
                        image.Source = item.File.Artwork;
                        aIndex = i;
                    }
                }
            }
        }

        private LAPP.IO.FileItem CreateItem(LAPP.IO.MediaFile File)
        {
            ClearUC.ListViewItems.ListSubItem item = new ClearUC.ListViewItems.ListSubItem();
            item.MainLabelText = File.Title;
            item.SubLabelText = File.Path;

            if(File.Artwork != null)
            {
                item.SideItem = ClearUC.ListViewItems.ListSubItem.SideItems.Image;
                item.Image = File.Artwork;
            }

            return new LAPP.IO.FileItem(File, item, false);
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            Pages.Album.AlbumData data = new Pages.Album.AlbumData();
            data.ShowArtwork = false;
            data.ArtworkIndex = -1;

            if(image.Source != null)
            {
                data.ArtworkIndex = aIndex;
                data.ShowArtwork = true;
            }

            data.Tracks = new Pages.Album.AlbumData.Track[items.Count];
            for (int i = 0; items.Count > i; i++)
            {
                data.Tracks[i] = new Pages.Album.AlbumData.Track()
                {
                    DiscNumber = items[i].File.DiscNumber,
                    Path = items[i].File.Path,
                    Title = items[i].File.Title,
                    TrackNumber = items[i].File.Track
                };

                if(string.IsNullOrEmpty(data.Album) && !string.IsNullOrEmpty(items[i].File.Album))
                {
                    data.Album = items[i].File.Album;
                }

                if (string.IsNullOrEmpty(data.Artist) && !string.IsNullOrEmpty(items[i].File.Artist))
                {
                    data.Artist = items[i].File.Artist;
                }

                if (items[i].File.TotalDiscNumber > 0)
                    data.TotalDiscs = items[i].File.TotalDiscNumber;
            }

            System.IO.Directory.CreateDirectory(Config.Current.Path[Enums.Path.AlbumDirectory]);

            Random rnd = new Random();
            
            string saveP = "";
            string tempP = Config.Current.Path[Enums.Path.AlbumDirectory] + rnd.Next(99999).ToString().PadLeft(5, '0') + ".xml";
            while (true)
            {
                if (!System.IO.File.Exists(tempP))
                {
                    saveP = tempP;
                    break;
                }
                else
                    tempP = Config.Current.Path[Enums.Path.AlbumDirectory] + rnd.Next(99999).ToString().PadLeft(5, '0') + ".xml";
            }

            Pages.Album.AlbumData.Write(saveP, data);

            Close();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    BorderThickness = new Thickness(6);
                    break;
                case WindowState.Normal:
                    BorderThickness = new Thickness(0);
                    break;
            }
        }
    }
}
