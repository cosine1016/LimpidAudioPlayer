using ClearUC.ListViewItems;
using LAP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LAP.Page.Album
{
    internal class Page : ListViewPage
    {
        private ListSubItem CreateAlbumItem = null;
        private List<ListItem> TopPage = new List<ListItem>();
        private List<ListItem> PageItem = new List<ListItem>();

        private List<LAPP.MTag.File> Files = new List<LAPP.MTag.File>();

        internal Page()
        {
            ItemSelected += Page_ItemSelected;
            CreateAlbumItem = GetCreateAlbumItem();
            Border = Shapes.GetBorderFromXAML(Resources.Shapes.Disc, 35, 35, new Thickness(0, 3, 0, 0));
        }

        private void Page_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            if (CreateAlbumItem == e.Item)
            {
                Dialogs.Album Album = new Dialogs.Album();
                Album.AlbumCreated += Album_AlbumCreated;
                Album.ShowDialog();
            }
            else
            {
                if (e.Item.DataType == typeof(AlbumData))
                {
                    Opened = true;
                    RequestClearPage();
                    CreateTrackPage((AlbumData)e.Item.Data);
                    for (int i = 0; PageItem.Count > i; i++)
                        Add(PageItem[i]);
                }

                if (e.Item.DataType == typeof(LAPP.MTag.File))
                {
                    int ind = Files.IndexOf((LAPP.MTag.File)e.Item.Data);
                    OnPlayFile(Files.ToArray(), ind);
                }
            }
        }

        public override void PlayAnyFile()
        {
            if (Files.Count > 0)
            {
                if (Shuffle)
                    OnPlayFile(Files.ToArray(), new Random().Next(0, Files.Count - 1));
                else
                    OnPlayFile(Files.ToArray(), 0);
            }
        }

        public void CreateTrackPage(AlbumData Data)
        {
            Files.Clear();

            ListMenuItem MI = new ListMenuItem();
            MI.MainLabelText = Data.Album;
            MI.SubLabelText = Data.Artist;

            for (int i = 0; Data.Tracks.Length > i; i++)
            {
                if (Data.ShowArtwork)
                {
                    if (MI.ImageSources == null)
                    {
                        LAPP.MTag.TagEx Tag = GetTag(Data.Tracks[i].Path);
                        if (string.IsNullOrEmpty(Tag.ArtworkCachePath) == false)
                        {
                            System.Windows.Media.ImageSource Image = Utility.ArtworkManager.GetArtwork(Tag.ArtworkCachePath);
                            ListItem.ImageSourceList ISL = new ListItem.ImageSourceList();
                            ISL.Add(Image);
                            MI.ImageSources = ISL;
                            MI.ImageIndex = 0;
                            break;
                        }
                    }
                }
            }

            if(MI.ImageSources != null && !string.IsNullOrEmpty(Data.Album) && !string.IsNullOrEmpty(Data.Artist))
                PageItem.Add(MI);

            for (int i = 0; Data.Tracks.Length > i; i++)
            {
                ListAnimativeItem Lai = new ListAnimativeItem(true);
                Lai.SearchText = Data.Tracks[i].Title;

                ListSubItem lsi = new ListSubItem();
                Lai.ItemsHeight = lsi.Height;
                lsi.MainLabelText = Data.Tracks[i].Title;
                if (string.IsNullOrEmpty(lsi.MainLabelText)) lsi.MainLabelText = System.IO.Path.GetFileNameWithoutExtension(Data.Tracks[i].Path);
                lsi.SubLabelVisibility = Visibility.Hidden;
                lsi.StatusLabelVisibility = Visibility.Visible;

                if(Data.Tracks[i].TrackNumber > 0)
                {
                    lsi.LeftItem = ListSubItem.LeftItems.Number;
                    lsi.NumberLabelText = Data.Tracks[i].TrackNumber.ToString();
                }

                Lai.FirstItem = lsi;

                ListButtonsItem lbi = new ListButtonsItem();

                ListButtonsItem.ListButton Explorer = new ListButtonsItem.ListButton(Lai);
                Explorer.Click += Explorer_Click;
                Explorer.Content = Config.Language.Strings.ContextMenu.ShowInExplorer;
                lbi.Add(Explorer);

                //ListButtonsItem.ListButton Remove = new ListButtonsItem.ListButton(Lai);
                //Remove.Content = Utils.Config.Language.Strings.ContextMenu.Remove;
                //lbi.Add(Remove);

                Lai.SecondItem = lbi;

                LAPP.MTag.TagEx tag = GetTag(Data.Tracks[i].Path);
                LAPP.MTag.File File = new LAPP.MTag.File(Data.Tracks[i].Path, tag);
                File.Artwork = Utility.ArtworkManager.GetArtwork(tag.ArtworkCachePath);

                Lai.Data = File;
                Lai.DataType = typeof(LAPP.MTag.File);

                Files.Add(File);

                PageItem.Add(Lai);
            }
        }

        private void Album_AlbumCreated(object sender, Dialogs.Album.AlbumCreatedEventArgs e)
        {
            Update();
            RequestClearPage();
            for (int i = 0; TopPage.Count > i; i++) Add(TopPage[i]);
        }

        private ListSubItem GetCreateAlbumItem()
        {
            ListSubItem lsi = new ListSubItem(false, true);
            lsi.MainLabelText = Utils.Config.Language.Strings.CreateAlbum;
            lsi.SubLabelVisibility = Visibility.Hidden;
            lsi.LeftItem = ListSubItem.LeftItems.Shape;

            Shape s = Utils.Shapes.GetShape(Utils.Shapes.ShapeData.Plus, 20, 20, new Thickness(0));
            s.HorizontalAlignment = HorizontalAlignment.Stretch;
            s.VerticalAlignment = VerticalAlignment.Stretch;

            lsi.ShapeItem = s;

            return lsi;
        }

        private void AddAlbum(string Path)
        {
            ListAnimativeItem Lai = new ListAnimativeItem(true);
            AlbumData Data = AlbumData.Read(Path);
            if (Data == null) return;

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

            ListButtonsItem.ListButton Remove = new ListButtonsItem.ListButton(Lai);
            Remove.Content = Config.Language.Strings.ContextMenu.Remove;
            Remove.Click += Remove_Click;
            lbi.Add(Remove);

            Lai.SecondItem = lbi;

            Lai.Data = Data;
            Lai.DataType = typeof(AlbumData);

            TopPage.Add(Lai);
        }

        private void Explorer_Click(object sender, RoutedEventArgs e)
        {
            ListButtonsItem.ListButton lb = sender as ListButtonsItem.ListButton;
            if (lb != null)
            {
                if (lb.ParentItem.DataType == typeof(LAPP.MTag.File))
                {
                    LAPP.MTag.File File = (LAPP.MTag.File)lb.ParentItem.Data;
                    Utility.ShowExplorerWithFile(File.Path);
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ListButtonsItem.ListButton lb = (ListButtonsItem.ListButton)sender;
            ListAnimativeItem lai = (ListAnimativeItem)lb.ParentItem;
            AlbumData alb = (AlbumData)lai.Data;
            File.Delete(alb.Path);
            Remove(lai);
        }

        public override string Title { get; protected set; } = "Album";

        public override System.Windows.Controls.Border Border { get; protected set; } = null;

        public override void Dispose()
        {
            RequestClearPage();
            TopPage.Clear();
            PageItem.Clear();
        }

        public override ListItem[] GetPageItems()
        {
            return PageItem.ToArray();
        }

        public override ListItem[] GetTopPageItems()
        {
            return TopPage.ToArray();
        }

        public override void Update()
        {
            Opened = false;
            TopPage.Clear();
            PageItem.Clear();

            Files.Clear();

            CreateAlbumItem = GetCreateAlbumItem();
            Directory.CreateDirectory(Config.Setting.Paths.Album);
            string[] Paths = Directory.GetFiles(Config.Setting.Paths.Album, "*" + Utils.Config.Setting.Paths.AlbumExtension);
            foreach (string Path in Paths)
            {
                AddAlbum(Path);
            }

            TopPage.Add(new Separator());
            TopPage.Add(CreateAlbumItem);
        }
    }
}