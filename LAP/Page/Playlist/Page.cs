using ClearUC.ListViewItems;
using LAP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;

namespace LAP.Page.Playlist
{
    public class Page : ListViewPage
    {
        private ListSubItem CreatePlaylistItem = null;
        private List<ListItem> TopPage = new List<ListItem>();
        private List<ListItem> OnlyFiles = new List<ListItem>();
        private List<ListItem> PageItem = new List<ListItem>();
        private PlaylistData ShowingPlaylist;
        private PlaylistData PlayingPlaylist;

        private List<LAPP.MTag.File> Files = new List<LAPP.MTag.File>();

        internal Page()
        {
            Border = Shapes.GetBorderFromXAML(Resources.Shapes.Playlist, 35, 35, new Thickness(0));
            ItemSelected += Page_ItemSelected;
        }

        private void Page_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            if (e.Item == CreatePlaylistItem)
            {
                Dialogs.Playlist pd = new Dialogs.Playlist(Dialogs.Playlist.DialogMode.Create);
                pd.PlaylistCreated += Pd_PlaylistCreated;
                pd.ShowDialog();
            }
            else
            {
                if (e.Item.DataType == typeof(PlaylistData))
                {
                    RequestClearPage();
                    CreateTrackPage(((PlaylistData)e.Item.Data).Data);
                    ShowingPlaylist = (PlaylistData)e.Item.Data;
                }

                if (e.Item.DataType == typeof(LAPP.MTag.File))
                {
                    int ind = Files.IndexOf((LAPP.MTag.File)e.Item.Data);
                    OnPlayFile(Files.ToArray(), ind);
                    PlayingPlaylist = ShowingPlaylist;
                }
            }
        }

        private void CreateTrackPage(Playlist.PlaylistData Data)
        {
            PageItem.Clear();
            OnlyFiles.Clear();
            Files.Clear();

            LabelSeparator ls = new LabelSeparator();
            ls.Sticky = Converter.StringToBrush(Data.Sticky);
            ls.Label = Data.Title;
            PageItem.Add(ls);
            Add(ls);

            for (int i = 0; Data.Paths.Length > i; i++)
            {
                CreateItem(Data.Paths[i]);
            }
        }

        private void CreateItem(Playlist.PlaylistData.Path Data)
        {
            if(Data.IsFile == true)
            {
                CreateItemFromPath(Data.FilePath, true);
            }
            else
            {
                List<string> paths = new List<string>();
                for(int i = 0;Data.Filter.Length > i; i++)
                {
                    paths.AddRange(Directory.GetFiles(Data.DirectoryPath, Data.Filter[i], Data.SearchOption));
                }

                for(int i = 0;paths.Count > i; i++)
                {
                    CreateItemFromPath(paths[i], false);
                }
            }
        }

        private void CreateItemFromPath(string Path, bool FileMode)
        {
            bool IsFile = FileMode;
            ListAnimativeItem Lai = new ListAnimativeItem(true);

            LAPP.MTag.TagEx tag = GetTag(Path);
            LAPP.MTag.File File = new LAPP.MTag.File(Path, tag);
            File.Artwork = Utility.ArtworkManager.GetArtwork(tag.ArtworkCachePath);

            Lai.Data = File;
            Lai.DataType = typeof(LAPP.MTag.File);

            ListSubItem lsi = new ListSubItem();
            Lai.ItemsHeight = lsi.Height;
            lsi.MainLabelText = System.IO.Path.GetFileNameWithoutExtension(Path);
            lsi.SubLabelVisibility = Visibility.Hidden;
            lsi.StatusLabelVisibility = Visibility.Visible;

            if (string.IsNullOrEmpty(File.Tag.Title) == false)
            {
                lsi.MainLabelText = File.Tag.Title;
            }
            else
            {
                lsi.MainLabelText = System.IO.Path.GetFileNameWithoutExtension(Path);
            }

            if (string.IsNullOrEmpty(File.Tag.Album) == false)
            {
                lsi.SubLabelVisibility = Visibility.Visible;
                lsi.SubLabelText = File.Tag.Album;
                if (string.IsNullOrEmpty(File.Tag.Artist) == false)
                {
                    lsi.SubLabelText += " - " + File.Tag.Artist;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(File.Tag.Artist) == false)
                {
                    lsi.SubLabelVisibility = Visibility.Visible;
                    lsi.SubLabelText = File.Tag.Artist;
                }
            }

            if (File.Artwork != null)
            {
                lsi.LeftItem = ListSubItem.LeftItems.Image;
                ClearUC.ListViewItems.ListItem.ImageSourceList isl = new ListItem.ImageSourceList();
                isl.Add(File.Artwork);
                lsi.ImageSources = isl;
                lsi.ImageIndex = 0;
            }

            Lai.FirstItem = lsi;

            ListButtonsItem lbi = new ListButtonsItem();
            ListButtonsItem.ListButton Explorer = new ListButtonsItem.ListButton(Lai);
            Explorer.Click += Explorer_Click;
            Explorer.Content = Config.Language.Strings.ContextMenu.ShowInExplorer;
            lbi.Add(Explorer);

            if (FileMode == true)
            {
                //ListButtonsItem.ListButton Delete = new ListButtonsItem.ListButton(Lai);
                //Delete.Click += Delete_Click;
                //Delete.Content = Config.Language.Strings.ContextMenu.Delete;
                //lbi.Add(Delete);
            }

            //ListButtonsItem.ListButton Property = new ListButtonsItem.ListButton(Lai);
            //Property.Click += Property_Click;
            //Property.Content = Utils.Config.Language.Strings.ContextMenu.Property;
            //lbi.Add(Property);

            Lai.SecondItem = lbi;

            Lai.SearchText = lsi.MainLabelText;

            PageItem.Add(Lai);
            OnlyFiles.Add(Lai);
            Add(Lai);
            Files.Add(File);
        }

        private void Property_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ListButtonsItem.ListButton lb = sender as ListButtonsItem.ListButton;
            if (lb != null)
            {
                if (lb.ParentItem.DataType == typeof(LAPP.MTag.File))
                {
                    LAPP.MTag.File File = (LAPP.MTag.File)lb.ParentItem.Data;
                    List<Playlist.PlaylistData.Path> Paths = new List<Playlist.PlaylistData.Path>();
                    Paths.AddRange(ShowingPlaylist.Data.Paths);

                    Paths.RemoveAt(OnlyFiles.IndexOf(lb.ParentItem));
                    ShowingPlaylist.Data.Paths = Paths.ToArray();

                    Playlist.Write(ShowingPlaylist.Path, ShowingPlaylist.Data);
                    Remove(lb.ParentItem);
                    PageItem.Remove(lb.ParentItem);

                    if (PlayingPlaylist != null && ShowingPlaylist == PlayingPlaylist)
                    {
                        Files.Remove((LAPP.MTag.File)lb.ParentItem.Data);
                        MakeOrder(Files.ToArray(), PlayingIndex);
                    }
                }
            }
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

        private void Pd_PlaylistCreated(object sender, Classes.PlaylistEventArgs e)
        {
            Add(GetPlaylistItem(e.Path));
        }

        public override System.Windows.Controls.Border Border { get; protected set; } = null;

        public override string Title { get; protected set; } = "Playlist";

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
            CreatePlaylistItem = GetCreatePlaylistItem();
            AddPlaylists(Config.Setting.Paths.Playlist);
        }

        private ListSubItem GetCreatePlaylistItem()
        {
            ListSubItem lsi = new ListSubItem(false, false);
            lsi.MainLabelText = Config.Language.Strings.CreatePlaylist;
            lsi.SubLabelVisibility = Visibility.Hidden;
            lsi.LeftItem = ListSubItem.LeftItems.Shape;

            Shape s = Utils.Shapes.GetShape(Utils.Shapes.ShapeData.Plus, 20, 20, new Thickness(0));
            s.HorizontalAlignment = HorizontalAlignment.Stretch;
            s.VerticalAlignment = VerticalAlignment.Stretch;

            lsi.ShapeItem = s;

            TopPage.Add(lsi);
            return lsi;
        }

        private void AddPlaylists(string PlaylistDirectory)
        {
            if (Directory.Exists(PlaylistDirectory) == false) Directory.CreateDirectory(PlaylistDirectory);
            string[] files = Directory.GetFiles(PlaylistDirectory, "*" + Utils.Config.Setting.Paths.PlaylistExtension, SearchOption.TopDirectoryOnly);
            foreach (string s in files)
            {
                TopPage.Add(GetPlaylistItem(s));
            }
        }

        public ListAnimativeItem GetPlaylistItem(string Path)
        {
            Playlist.PlaylistData Data = Playlist.Read(Path);

            ListAnimativeItem Lai = new ListAnimativeItem(true);
            Lai.Data = new PlaylistData(Data, Path);
            Lai.DataType = typeof(PlaylistData);

            ListSubItem lsi = new ListSubItem();
            Lai.ItemsHeight = lsi.Height;
            lsi.MainLabelText = Data.Title;
            lsi.SubLabelVisibility = Visibility.Hidden;
            Lai.FirstItem = lsi;

            ListButtonsItem lbi = new ListButtonsItem();

            ListButtonsItem.ListButton edit = new ListButtonsItem.ListButton(Lai);
            edit.Opacity = 0.7;
            edit.Click += Edit_Click;
            edit.Content = Utils.Config.Language.Strings.ContextMenu.Edit;

            ListButtonsItem.ListButton remove = new ListButtonsItem.ListButton(Lai);
            remove.Opacity = 0.7;
            remove.Click += Remove_Click;
            remove.Content = Utils.Config.Language.Strings.ContextMenu.Remove;

            lbi.Add(edit);
            lbi.Add(remove);
            Lai.SecondItem = lbi;

            Lai.SearchText = lsi.MainLabelText;

            return Lai;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            ListButtonsItem.ListButton lb = sender as ListButtonsItem.ListButton;
            if (lb != null)
            {
                if (lb.ParentItem.DataType == typeof(PlaylistData))
                {
                    PlaylistData pd = lb.ParentItem.Data as PlaylistData;
                    if (pd != null)
                    {
                        Dialogs.Playlist dialog = new Dialogs.Playlist(Dialogs.Playlist.DialogMode.Edit);
                        dialog.LoadPlaylist(pd.Data, pd.Path);
                        dialog.PlaylistEdited += Dialog_PlaylistEdited;
                        dialog.ShowDialog();
                    }
                }
            }
        }

        private void Dialog_PlaylistEdited(object sender, Classes.PlaylistEventArgs e)
        {
            for (int i = 0; TopPage.Count > i; i++)
            {
                PlaylistData pd = TopPage[i].Data as PlaylistData;
                if (pd != null)
                {
                    if (pd.Path == e.Path)
                    {
                        ListAnimativeItem lai = TopPage[i] as ListAnimativeItem;
                        lai.Data = new PlaylistData(e.Data, e.Path);
                        ListSubItem fi = lai.FirstItem as ListSubItem;
                        fi.MainLabelText = e.Data.Title;

                        lai.FrontItem = ListAnimativeItem.Item.First;
                    }
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ListButtonsItem.ListButton lb = sender as ListButtonsItem.ListButton;
            if (lb != null)
            {
                if (lb.ParentItem.DataType == typeof(PlaylistData))
                {
                    PlaylistData pd = lb.ParentItem.Data as PlaylistData;
                    if (pd != null)
                    {
                        File.Delete(pd.Path);
                        TopPage.Remove(lb.ParentItem);
                        Remove(lb.ParentItem);
                    }
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
    }
}