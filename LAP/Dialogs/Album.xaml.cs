using ClearUC.ListViewItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace LAP.Dialogs
{
    /// <summary>
    /// Album.xaml の相互作用ロジック
    /// </summary>
    public partial class Album : Window
    {
        private int MaxDiscs { get; set; } = 1;
        private int NowDisc { get; set; } = 1;
        private OpenFileDialog OFD = new OpenFileDialog();

        public class AlbumCreatedEventArgs : EventArgs
        {
            public AlbumCreatedEventArgs(string Path)
            {
                this.Path = Path;
            }

            public string Path { get; set; }
        }

        public event EventHandler<AlbumCreatedEventArgs> AlbumCreated;

        private int LastNum = 0;
        private string FileName = "";
        private ListMenuItem MenuItem = new ListMenuItem();

        public Album()
        {
            InitializeComponent();

            Caption.Title = Utils.Config.Language.Strings.CreateAlbum;

            AddMenuItem();

            GetPlaylistName();
        }

        public void AddMenuItem()
        {
            MenuItem.MainLabelText = AlbumT.Text;
            MenuItem.SubLabelText = ArtistT.Text;

            ListView.Items.Add(MenuItem);
        }

        private void AlbumT_TextChanged(object sender, TextChangedEventArgs e)
        {
            MenuItem.MainLabelText = AlbumT.Text;
        }

        private void ArtistT_TextChanged(object sender, TextChangedEventArgs e)
        {
            MenuItem.SubLabelText = ArtistT.Text;
        }

        public async void GetPlaylistName()
        {
            await Task.Run(() =>
            {
                int ret = Utils.Converter.GenerateRandomValue(9999);
                string FileName = string.Format("{0:D4}", ret) + Utils.Config.Setting.Paths.AlbumExtension;
                if (System.IO.File.Exists(Utils.Config.Setting.Paths.Playlist + FileName) == false)
                {
                    this.FileName = FileName;
                }
                else
                {
                    GetPlaylistName();
                }
            });
        }

        private void FileOpenB_Click(object sender, RoutedEventArgs e)
        {
            OFD.Multiselect = true;

            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LastNum++;
                ClearUC.Dialogs.TextBoxWithMessage.ResultData Result = ClearUC.Dialogs.Dialog.ShowMessageBoxWithNumeric(
                    Utils.Config.Language.Strings.ExceptionMessage.SelectDiscNumber[0], Utils.Config.Language.Strings.ExceptionMessage.SelectDiscNumber[1], LastNum);
                int DNum = 0;
                if (Result.ClickedButton == ClearUC.Dialogs.Dialog.ClickedButton.OK) DNum = Result.Number;

                foreach (string p in OFD.FileNames)
                {
                    int Num = -1;

                    Page.Album.AlbumData.Track Track = new Page.Album.AlbumData.Track();

                    if (LAPP.MTag.TagReader.SupportedExtension.Contains(System.IO.Path.GetExtension(p).ToLower()))
                    {
                        LAPP.MTag.Tag Tag = LAPP.MTag.TagReader.GetTagFromFile(p);
                        if (Tag != null)
                        {
                            if (MenuItem.ImageSources == null)
                            {
                                if (Tag.Artwork != null)
                                {
                                    ImageSource Image =
                                        Utils.Converter.ToImageSource((System.Drawing.Bitmap)Tag.Artwork);
                                    ClearUC.ListViewItems.ListItem.ImageSourceList ISL
                                        = new ClearUC.ListViewItems.ListItem.ImageSourceList();
                                    ISL.Items.Add(Image);
                                    MenuItem.ImageSources = ISL;
                                    MenuItem.ImageIndex = 0;
                                }
                            }

                            if (string.IsNullOrEmpty(AlbumT.Text) && string.IsNullOrEmpty(Tag.Album) == false)
                                AlbumT.Text = Tag.Album;

                            if (string.IsNullOrEmpty(Tag.Title) == false)
                            {
                                Track.Title = Tag.Title;
                                if (string.IsNullOrEmpty(ArtistT.Text))
                                    ArtistT.Text = Tag.Artist;
                            }
                            else
                            {
                                Track.Title = System.IO.Path.GetFileNameWithoutExtension(p);
                            }

                            if (string.IsNullOrEmpty(Tag.Track) == false)
                            {
                                bool Success = int.TryParse(Tag.Track, out Num);
                                if (Success == false) Num = -1;
                            }
                        }
                    }

                    Track.DiscNumber = DNum;
                    Track.TrackNumber = Num;
                    Track.Path = p;

                    ListView.Items.Add(CreateSubItem(Track));
                }
            }
        }

        private ClearUC.ListViewItems.ListItem CreateSubItem(Page.Album.AlbumData.Track Track)
        {
            ListAnimativeItem Lai = new ListAnimativeItem(true);
            Lai.DataType = typeof(Page.Album.AlbumData.Track);
            Lai.Data = Track;

            ListSubItem lsi = new ListSubItem();

            if (Track.TrackNumber > 0)
            {
                lsi.LeftItem = ListSubItem.LeftItems.Number;
                lsi.NumberLabelText = Track.TrackNumber.ToString();
            }

            Lai.ItemsHeight = lsi.Height;
            lsi.SubLabelVisibility = Visibility.Visible;
            lsi.SubLabelText = "Unknown";

            lsi.MainLabelText = System.IO.Path.GetFileName(Track.Path) + " - " + Utils.Config.Language.Strings.Path.File;
            lsi.SubLabelText = Utils.Config.Language.Strings.Window.Album.Disc + ":" + Track.DiscNumber + " (" + Track.Path + ")";

            Lai.FirstItem = lsi;

            ListButtonsItem lbi = new ListButtonsItem();
            ListButtonsItem.ListButton Remove = new ListButtonsItem.ListButton(Lai);
            Remove.Click += Remove_Click;
            Remove.Content = Utils.Config.Language.Strings.ContextMenu.Remove;
            lbi.Add(Remove);
            Lai.SecondItem = lbi;

            return Lai;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ListButtonsItem.ListButton lb = sender as ListButtonsItem.ListButton;
            if (lb != null)
            {
                ListView.Items.Remove(lb.ParentItem);
            }
        }

        private void CreateAlbumB_Click(object sender, RoutedEventArgs e)
        {
            Page.Album.AlbumData AD = new Page.Album.AlbumData();

            List<Page.Album.AlbumData.Track> Tracks = new List<Page.Album.AlbumData.Track>();
            for (int i = 0; ListView.Items.Count > i; i++)
            {
                Page.Album.AlbumData.Track Track = ListView.Items[i].Data as Page.Album.AlbumData.Track;
                if (Track != null)
                {
                    Tracks.Add(Track);
                    if (Track.DiscNumber > AD.TotalDiscs) AD.TotalDiscs = Track.DiscNumber;
                }
            }

            AD.Album = AlbumT.Text;
            AD.Artist = ArtistT.Text;
            AD.Tracks = Tracks.ToArray();

            AD.ShowArtwork = (bool)ArtworkC.IsChecked;

            Page.Album.AlbumData.Write(Utils.Config.Setting.Paths.Album + FileName, AD);
            if (AlbumCreated != null) AlbumCreated(this, new AlbumCreatedEventArgs(Utils.Config.Setting.Paths.Album + FileName));
            Close();
        }

        private void ArtworkC_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem.ImageVisibility = Visibility.Visible;
        }

        private void ArtworkC_Unchecked(object sender, RoutedEventArgs e)
        {
            MenuItem.ImageVisibility = Visibility.Hidden;
        }
    }
}