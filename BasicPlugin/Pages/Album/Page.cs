using ClearUC.ListViewItems;
using LAPP;
using LAPP.IO;
using LAPP.Utils;
using NAudio.Wave;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using System;

namespace BasicPlugin.Pages.Album
{
    public class AlbumData
    {
        public string Album { get; set; } = "";

        public string Artist { get; set; } = "";

        public string Path { get; set; } = "";

        public bool ShowArtwork { get; set; } = false;

        public int TotalDiscs { get; set; } = 1;

        public Track[] Tracks { get; set; }

        public static AlbumData Read(string Path)
        {
            XmlSerializer des = new XmlSerializer(typeof(AlbumData));
            StreamReader sr = new StreamReader(Path, System.Text.Encoding.UTF8);
            AlbumData Data = null;
            try
            {
                Data = (AlbumData)des.Deserialize(sr);
            }
            catch (System.Exception) { return null; }
            finally
            {
                sr.Close();
            }

            Data.Path = Path;

            return Data;
        }

        public static void Write(string Path, AlbumData Data)
        {
            XmlSerializer ser = new XmlSerializer(typeof(AlbumData));
            StreamWriter sw = new StreamWriter(Path, false, System.Text.Encoding.UTF8);

            try
            {
                ser.Serialize(sw, Data);
            }
            finally
            {
                sw.Close();
            }
        }

        public class Track
        {
            public int DiscNumber { get; set; } = 0;
            public string Path { get; set; } = "";

            public string Title { get; set; } = "";

            public int TrackNumber { get; set; } = 0;
        }
    }
    public class AlbumItem : PageItem
    {
        internal event EventHandler<AlbumData> Selected;

        public AlbumItem(AlbumData Data)
        {
            ListItem = GetItem(Data);
            this.Data = Data;
        }

        public AlbumData Data { get; protected set; }

        private ListAnimativeItem GetItem(AlbumData Data)
        {
            ListAnimativeItem Lai = new ListAnimativeItem(true);

            ListAlbumItem album = new ListAlbumItem();
            ListSubItem lsi = new ListSubItem();
            Lai.ItemsHeight = lsi.Height;
            lsi.MainLabelText = Data.Album;

            if (string.IsNullOrEmpty(Data.Artist))
                lsi.SubLabelText = Data.Tracks.Count().ToString();
            else
                lsi.SubLabelText = Data.Tracks.Count() + " - " + Data.Artist;

            Lai.SearchText = lsi.MainLabelText;

            Lai.FirstItem = lsi;

            ListButtonsItem lbi = new ListButtonsItem();

            ListButtonsItem.ListButton Edit = new ListButtonsItem.ListButton(Lai);
            Edit.Content = Common.Setting.Edit;
            Edit.Click += Edit_Click;
            lbi.Add(Edit);

            ListButtonsItem.ListButton Remove = new ListButtonsItem.ListButton(Lai);
            Remove.Content = Common.Setting.Remove;
            Remove.Click += Remove_Click;
            lbi.Add(Remove);


            Lai.SecondItem = lbi;

            Lai.ItemClicked += Lai_ItemClicked;

            return Lai;
        }

        int height = 60;
        public int EachHeight
        {
            get { return height; }
            set { height = value; }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Lai_ItemClicked(object sender, ClearUC.ItemClickedEventArgs e)
        {
            if(e.MouseButtonEventArgs.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                Selected?.Invoke(this, Data);
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListButtonsItem.ListButton lb = (ListButtonsItem.ListButton)sender;
                ListAnimativeItem lai = (ListAnimativeItem)lb.ParentItem;
                File.Delete(Data.Path);
            }
            catch(Exception ex)
            { LAPP.Player.Utils.Notice(ex.Message, System.Windows.Media.Brushes.Red); }
        }
    }

    public class Page : ManageablePage
    {
        private Border border = BorderHelper.GetBorderFromXAML(Resources.Shapes.Disc, 35, 35, new Thickness(0, 5, 0, 0));
        private ListSubItem CreateAlbumItem = new ListSubItem();
        private FileItem LastFile = null;
        
        private PageItemCollection TopItems = new PageItemCollection();

        public Page()
        {
        }

        protected override void InitializeTopItems()
        {
            TopItems.CollectionChanged += TopItems_CollectionChanged;
            TopItems.Clear();

            if (Directory.Exists(Setting.Current.AlbumDirectory))
            {
                string[] Paths = Directory.GetFiles(Setting.Current.AlbumDirectory, "*" + Setting.AlbumExtension);
                foreach (string Path in Paths)
                    AddAlbum(Path);

                TopItems.Add(new ClearUC.ListViewItems.Separator());
            }

            CreateAlbumItem.ItemClicked += CreateAlbumItem_ItemClicked;
            CreateAlbumItem.MainLabelText = Setting.Current.CreateAlbumStr;
            CreateAlbumItem.SubLabelVisibility = Visibility.Hidden;
            TopItems.Add(CreateAlbumItem);
        }

        private void CreateAlbumItem_ItemClicked(object sender, ClearUC.ItemClickedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override Border Border
        {
            get { return null; }
            protected set { border = value; }
        }

        public override string Title { get; protected set; } = "Album";

        public override void PlaybackStateChanged(PlaybackState PlaybackState)
        {
            FileItem Item = GetPlayingItem();

            for (int i = 0; Items.Count > i; i++)
            {
                ListSubItem lsi = GetSubItem(Items[i]);
                if (lsi != null)
                {
                    lsi.StatusLabelText = null;

                    if (Item == Items[i])
                    {
                        switch (PlaybackState)
                        {
                            case PlaybackState.Playing:
                                lsi.StatusLabelText = Common.Setting.Play;
                                break;
                            case PlaybackState.Paused:
                                lsi.StatusLabelText = Common.Setting.Pause;
                                break;
                        }
                    }
                }
            }
        }

        private ListSubItem GetSubItem(PageItem Item)
        {
            ListAnimativeItem lai = Item.ListItem as ListAnimativeItem;
            ListSubItem lsi = lai?.FirstItem as ListSubItem;

            return lsi;
        }

        public override void Update()
        {
            if (PageLevel == Level.Top)
                UpdatePage(PageLevel);
        }

        protected override PageItemCollection GetTopItems()
        {
            return TopItems;
        }

        private void AddAlbum(string Path)
        {
            ListAnimativeItem Lai = new ListAnimativeItem(true);
            AlbumData Data = AlbumData.Read(Path);
            if (Data == null) return;

            TopItems.Add(new AlbumItem(Data));
        }

        private void TopItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AlbumItem item;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    item = e.NewItems[0] as AlbumItem;
                    if (item != null) item.Selected += Item_Selected;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    item = e.OldItems[0] as AlbumItem;
                    if (item != null) item.Selected -= Item_Selected;
                    break;
            }
        }

        public void CreateTrackPage(AlbumData Data)
        {
            ListMenuItem MI = new ListMenuItem();
            MI.MainLabelText = Data.Album;
            MI.SubLabelText = Data.Artist;
            MI.ImageSources = null;

            for (int i = 0; Data.Tracks.Length > i; i++)
            {
                ListAnimativeItem Lai = new ListAnimativeItem(true);
                Lai.SearchText = Data.Tracks[i].Title;
                Lai.ItemClicked += Lai_ItemClicked;

                ListSubItem lsi = new ListSubItem();
                Lai.ItemsHeight = lsi.Height;
                lsi.MainLabelText = Data.Tracks[i].Title;
                if (string.IsNullOrEmpty(lsi.MainLabelText)) lsi.MainLabelText = System.IO.Path.GetFileNameWithoutExtension(Data.Tracks[i].Path);
                lsi.SubLabelVisibility = Visibility.Hidden;
                lsi.StatusLabelVisibility = Visibility.Visible;

                if (Data.Tracks[i].TrackNumber > 0)
                {
                    lsi.LeftItem = ListSubItem.LeftItems.Number;
                    lsi.NumberLabelText = Data.Tracks[i].TrackNumber.ToString();
                }

                Lai.FirstItem = lsi;

                ListButtonsItem lbi = new ListButtonsItem();

                ListButtonsItem.ListButton Explorer = new ListButtonsItem.ListButton(Lai);
                Explorer.Click += Explorer_Click;
                Explorer.Content = Common.Setting.ShowInExplorer;
                lbi.Add(Explorer);

                Lai.SecondItem = lbi;

                MediaFile file = new MediaFile(Data.Tracks[i].Path);

                FileItem fileItem = new FileItem(file, Lai, true);

                if(MI.ImageSources == null && fileItem.File.Artwork != null)
                {
                    System.Windows.Media.ImageSource Image = fileItem.File.Artwork;
                    ListItem.ImageSourceList ISL = new ListItem.ImageSourceList();
                    ISL.Add(Image);
                    MI.ImageSources = ISL;
                    MI.ImageIndex = 0;

                    Items.Insert(0, MI);
                }

                Items.Add(fileItem);
            }
        }

        private void Lai_ItemClicked(object sender, ClearUC.ItemClickedEventArgs e)
        {
            UpdateOrder(new OrderManager(Items));
        }

        private void Explorer_Click(object sender, RoutedEventArgs e)
        {
            ListButtonsItem.ListButton lb = sender as ListButtonsItem.ListButton;
            if (lb != null)
            {
                FileItem file = FindPageItem(lb.ParentItem) as FileItem;
                if(file != null)
                    Utils.ShowExplorerWithFile(file.File.Path);
            }
        }

        private void Item_Selected(object sender, AlbumData e)
        {
            Items.Clear();
            CreateTrackPage(e);
            UpdatePage(Level.Current);
        }

        protected override void Initialize(FileItem FileItem)
        {
            LastFile = FileItem;
        }

        public override void Dispose()
        {
        }
    }
}