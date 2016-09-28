using ClearUC.ListViewItems;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LAP.Dialogs
{
    /// <summary>
    /// CreatePlaylist.xaml の相互作用ロジック
    /// </summary>
    public partial class Playlist : Window
    {
        public event EventHandler<Utils.Classes.PlaylistEventArgs> PlaylistCreated;

        public event EventHandler<Utils.Classes.PlaylistEventArgs> PlaylistEdited;

        private string FileName = "";
        private string EdFilePath = "";
        private LabelSeparator TitleItem = new LabelSeparator();

        public enum DialogMode { Create, Edit };

        public Playlist(DialogMode Mode)
        {
            InitializeComponent();
            this.Mode = Mode;
            InitChrome();
            GetPlaylistName();
            ApplyLanguage();
            TitleItem.Sticky = new SolidColorBrush(StickyColorPicker.SelectedColor);
            PlaylistItems.Items.Add(TitleItem);
            TitleItem.Label = NameT.Text;
        }

        public void LoadPlaylist(Page.Playlist.Playlist.PlaylistData PlaylistData, string FilePath)
        {
            EdFilePath = FilePath;
            PlaylistItems.Items.Clear();

            TitleItem.Sticky = Utils.Converter.StringToBrush(PlaylistData.Sticky);
            TitleItem.Label = PlaylistData.Title;
            PlaylistItems.Items.Add(TitleItem);

            StickyColorPicker.SelectedColor = Utils.Converter.StringToBrush(PlaylistData.Sticky).Color;
            NameT.Text = PlaylistData.Title;

            FileT.Text = null;
            DirectoryT.Text = null;

            for (int i = 0; PlaylistData.Paths.Length > i; i++)
            {
                PlaylistItems.Items.Add(CreateSubItem(PlaylistData.Paths[i]));
            }
        }

        public DialogMode Mode { get; set; } = DialogMode.Create;

        /// <summary>
        ///
        /// </summary>
        private void InitChrome()
        {
            Chrome.CaptionHeight = Caption.Height;
            Chrome.ResizeBorderThickness = new Thickness(1);

            switch (Mode)
            {
                case DialogMode.Create:
                    Caption.Title = Utils.Config.Language.Strings.CreatePlaylist;
                    break;

                case DialogMode.Edit:
                    Caption.Title = Utils.Config.Language.Strings.EditPlaylist;
                    break;
            }
        }

        public async void GetPlaylistName()
        {
            await Task.Run(() =>
            {
                int ret = Utils.Converter.GenerateRandomValue(9999);
                string FileName = string.Format("{0:D4}", ret) + Utils.Config.Setting.Paths.PlaylistExtension;
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

        private void NameT_TextChanged(object sender, TextChangedEventArgs e)
        {
            TitleItem.Label = NameT.Text;
        }

        private ListItem CreateSubItem(Page.Playlist.Playlist.PlaylistData.Path Path)
        {
            ListAnimativeItem Lai = new ListAnimativeItem(true);
            Lai.DataType = typeof(Page.Playlist.Playlist.PlaylistData.Path);
            Lai.Data = Path;

            ListSubItem lsi = new ListSubItem();
            Lai.ItemsHeight = lsi.Height;
            lsi.SubLabelVisibility = Visibility.Visible;
            lsi.SubLabelText = "Unknown";

            switch (Path.IsFile)
            {
                case true:
                    lsi.MainLabelText = System.IO.Path.GetFileName(Path.FilePath) + " - " + Utils.Config.Language.Strings.Path.File;
                    lsi.SubLabelText = Path.FilePath;
                    break;

                case false:
                    lsi.MainLabelText = System.IO.Path.GetFileName(Path.DirectoryPath) + " - " + Utils.Config.Language.Strings.Path.Directory;
                    lsi.SubLabelText = Path.DirectoryPath + " - Filter : " + string.Join(" , ", Path.Filter);
                    break;
            }
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
                PlaylistItems.Items.Remove(lb.ParentItem);
            }
        }

        private void ApplyLanguage()
        {
            switch (Mode)
            {
                case DialogMode.Create:
                    Title = Utils.Config.Language.Strings.CreatePlaylist;
                    CreateB.Content = Utils.Config.Language.Strings.Window.Playlist.CreateButton;
                    break;

                case DialogMode.Edit:
                    Title = Utils.Config.Language.Strings.EditPlaylist;
                    CreateB.Content = Utils.Config.Language.Strings.Window.Playlist.EditButton;
                    break;
            }

            NameL.Content = Utils.Config.Language.Strings.Window.Playlist.NameLabel;
            StickyL.Content = Utils.Config.Language.Strings.Window.Playlist.StickyLabel;

            FileL.Content = Utils.Config.Language.Strings.Window.Playlist.FileLabel;
            FileOpenB.Content = Utils.Config.Language.Strings.Window.Playlist.OpenButton;
            FileAddB.Content = Utils.Config.Language.Strings.Window.Playlist.AddButton;

            DirectoryL.Content = LAP.Utils.Config.Language.Strings.Window.Playlist.DirectoryLabel;
            DirectoryOpenB.Content = Utils.Config.Language.Strings.Window.Playlist.OpenButton;
            DirectoryAddB.Content = Utils.Config.Language.Strings.Window.Playlist.AddButton;
            DirectoryFilterL.Content = Utils.Config.Language.Strings.Window.Playlist.FilterLabel;
            TopDirectoryOnlyRadio.Content = Utils.Config.Language.Strings.Window.Playlist.TopDirectoryOnlyRadio;
            AllDirectoriesRadio.Content = Utils.Config.Language.Strings.Window.Playlist.AllDirectoriesRadio;
        }

        private void FileOpenB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileNames.Length > 1)
                {
                    foreach (string p in ofd.FileNames)
                    {
                        Page.Playlist.Playlist.PlaylistData.Path path = new Page.Playlist.Playlist.PlaylistData.Path();
                        path.IsFile = true;
                        path.FilePath = p;

                        PlaylistItems.Items.Add(CreateSubItem(path));
                    }
                }
                else
                {
                    FileT.Text = ofd.FileName;
                }
            }
        }

        private void DirectoryOpenB_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryT.Text = fbd.SelectedPath;
            }
        }

        private void FileAddB_Click(object sender, RoutedEventArgs e)
        {
            if (FileT.Text.Length > 0)
            {
                char[] invalidChars = System.IO.Path.GetInvalidPathChars();

                if (FileT.Text.IndexOfAny(invalidChars) < 0)
                {
                    Page.Playlist.Playlist.PlaylistData.Path path = new Page.Playlist.Playlist.PlaylistData.Path();
                    path.IsFile = true;
                    path.FilePath = FileT.Text;

                    PlaylistItems.Items.Add(CreateSubItem(path));

                    FileT.Text = "";
                }
                else
                {
                    Utils.Notification not = new Utils.Notification(parent, Utils.Config.Language.Strings.ExceptionMessage.PathException.UsingInvalidChars,
                        Utils.Config.Setting.Brushes.Notification.Error.Brush);
                    not.ShowMessage();
                }
            }
            else
            {
                Utils.Notification not = new Utils.Notification(parent, Utils.Config.Language.Strings.ExceptionMessage.PathException.IncorrectPath,
                    Utils.Config.Setting.Brushes.Notification.Error.Brush);
                not.ShowMessage();
            }
        }

        private void DirectoryAddB_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoryT.Text.Length > 0)
            {
                char[] invalidChars = System.IO.Path.GetInvalidPathChars();

                if (DirectoryT.Text.IndexOfAny(invalidChars) < 0)
                {
                    Page.Playlist.Playlist.PlaylistData.Path path = new Page.Playlist.Playlist.PlaylistData.Path();
                    path.IsFile = false;
                    path.DirectoryPath = DirectoryT.Text;

                    if (FilterT.Text.Length == 0)
                    {
                        Utils.Notification not = new Utils.Notification(parent, Utils.Config.Language.Strings.ExceptionMessage.PathException.SetToDefaultFilter,
                            Utils.Config.Setting.Brushes.Notification.Message.Brush);
                        not.ShowMessage();
                        path.Filter = new string[] { "*.*" };
                    }
                    else
                    {
                        path.Filter = FilterT.Text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    PlaylistItems.Items.Add(CreateSubItem(path));

                    DirectoryT.Text = "";
                }
                else
                {
                    Utils.Notification not = new Utils.Notification(parent, Utils.Config.Language.Strings.ExceptionMessage.PathException.UsingInvalidChars,
                        Utils.Config.Setting.Brushes.Notification.Error.Brush);
                    not.ShowMessage();
                }
            }
            else
            {
                Utils.Notification not = new Utils.Notification(parent, Utils.Config.Language.Strings.ExceptionMessage.PathException.IncorrectPath,
                    Utils.Config.Setting.Brushes.Notification.Error.Brush);
                not.ShowMessage();
            }
        }

        private void StickyColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            TitleItem.Sticky = new SolidColorBrush(StickyColorPicker.SelectedColor);
        }

        private void CreateB_Click(object sender, RoutedEventArgs e)
        {
            Page.Playlist.Playlist.PlaylistData pd = new Page.Playlist.Playlist.PlaylistData();
            List<Page.Playlist.Playlist.PlaylistData.Path> Paths = new List<Page.Playlist.Playlist.PlaylistData.Path>();

            foreach (ListItem li in PlaylistItems.Items)
            {
                if (li.DataType == typeof(Page.Playlist.Playlist.PlaylistData.Path))
                {
                    Paths.Add((Page.Playlist.Playlist.PlaylistData.Path)li.Data);
                }
            }

            pd.Paths = Paths.ToArray();
            pd.Sticky = Utils.Converter.BrushToString(new SolidColorBrush(StickyColorPicker.SelectedColor));
            pd.Title = NameT.Text;

            switch (Mode)
            {
                case DialogMode.Create:
                    Page.Playlist.Playlist.Write(Utils.Config.Setting.Paths.Playlist + FileName, pd);
                    PlaylistCreated?.Invoke(this, new Utils.Classes.PlaylistEventArgs(Utils.Config.Setting.Paths.Playlist + FileName, pd));
                    break;

                case DialogMode.Edit:
                    Page.Playlist.Playlist.Write(EdFilePath, pd);
                    PlaylistEdited?.Invoke(this, new Utils.Classes.PlaylistEventArgs(EdFilePath, pd));
                    break;
            }

            Close();
        }
    }
}