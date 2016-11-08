using ClearUC.ListViewItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using LAPP.IO;

namespace LAP.Utils
{
    internal class GUI
    {
        private ListSubItem OpenItem;
        private ListSubItem ConfigItem;
        private ListSubItem CreatorItem;
        private ListSubItem ExitItem;
        private ListSubItem LogItem;
        private LAP.Dialogs.LogWindow LogWindow;

        private void RaiseEvent(LAPP.Player.Receiver.Action Action, params object[] Args)
        {
            LAPP.Player.Receiver.RaiseReceivedEvent(new LAPP.Player.Receiver.EventReceiveArgs(Action, Args));
        }
        private void RaiseEvent(LAPP.Player.Receiver.Action Action)
        {
            RaiseEvent(Action, null);
        }

        private MainWindow MW;

        public GUI(MainWindow MainWindow)
        {
            MW = MainWindow;
        }

        public void Initialize()
        {
            InitializeInterfaceAndConfig();
            InitializeTaskBarManager();
            InitializeLAPPanel();
            AssociateEvents();
            InitializeTabAndManager();
        }

        private void InitializeInterfaceAndConfig()
        {
            ClearUC.Dialogs.Dialog.DialogIcon = Utility.ToImageSource(Properties.Resources.Limpid_Audio_Player);
            MW.LibraryRoot.Visibility = Visibility.Visible;
            MW.MediaInformationRoot.Visibility = Visibility.Hidden;

            Config.ReadSetting(Paths.SettingFilePath);
            Config.ReadLanguage(Config.Setting.Paths.UsingLanguage);

            ApplyConfig();
        }

        public void ApplyConfig()
        {
            MW.library.SearchBoxVisible = Config.Setting.Boolean.SearchBox;
        }

        private void InitializeLAPPanel()
        {
            MW.OptionalView.ItemClicked += OptionalView_ItemClicked;
            MW.OptionalView.Items.Clear();

            double PerHeight = 35;

            OpenItem = new ListSubItem();
            OpenItem.MainLabelText = Utils.Config.Language.Strings.OptionalView.Open;
            OpenItem.SubLabelVisibility = Visibility.Hidden;
            OpenItem.Height = PerHeight;
            MW.OptionalView.Items.Add(OpenItem);

            MW.OptionalView.Items.Add(new Separator());

            ConfigItem = new ListSubItem();
            ConfigItem.MainLabelText = Utils.Config.Language.Strings.OptionalView.Config;
            ConfigItem.SubLabelVisibility = Visibility.Hidden;
            ConfigItem.Height = PerHeight;
            MW.OptionalView.Items.Add(ConfigItem);

            if (InstanceData.LogMode)
            {
                LogWindow = new LAP.Dialogs.LogWindow();
                LogItem = new ListSubItem();
                LogItem.MainLabelText = "Log";
                LogItem.SubLabelVisibility = Visibility.Hidden;
                LogItem.Height = PerHeight;
                MW.OptionalView.Items.Add(LogItem);
            }

            MW.OptionalView.Items.Add(new Separator());

            CreatorItem = new ListSubItem();
            CreatorItem.MainLabelText = Config.Language.Strings.OptionalView.Creator;
            CreatorItem.SubLabelVisibility = Visibility.Hidden;
            CreatorItem.Height = PerHeight;
            MW.OptionalView.Items.Add(CreatorItem);

            MW.OptionalView.Items.Add(new Separator());

            ExitItem = new ListSubItem();
            ExitItem.MainLabelText = Utils.Config.Language.Strings.OptionalView.Exit;
            ExitItem.SubLabelVisibility = Visibility.Hidden;
            ExitItem.Height = PerHeight;
            MW.OptionalView.Items.Add(ExitItem);

            double h = 0;
            for (int i = 0; MW.OptionalView.Items.Count > i; i++)
            {
                h += MW.OptionalView.Items[i].Height;
            }
            MW.OptionalGrid.Height = h;
        }

        private void AssociateEvents()
        {
            Program.NotImplementedException += Program_NotImplementedException;
            PluginManager.PluginChanged += PluginManager_PluginChanged;

            LAPP.Events.Notice += Events_Noticed;

            MW.MC.StopButton.MouseClicked += (sender, e) => { MW.StopFile(true); };
            MW.MC.LibraryButton.MouseClicked += LibraryButton_MouseClicked;
            MW.MC.FFButton.MouseClicked += (sender, e) => { MW.Manager.PlayNext(); };
            MW.MC.RewButton.MouseClicked += (sender, e) =>
            {
                if(MW.Renderer == null)
                {
                    MW.Manager.PlayLast();
                    return;
                }
                long min = MW.Renderer.AudioFileReader.Length / 30;
                if (MW.Renderer != null && MW.Renderer.StreamStatus != NWrapper.Audio.Status.Stopped && MW.Renderer.AudioFileReader.Position >= min)
                {
                    MW.Renderer.AudioFileReader.Position = 0;
                }
                else
                    MW.Manager.PlayLast();
            };

            MW.MediaInformationRoot.PreviewMouseUp += MediaInformationRoot_PreviewMouseUp;
            MW.MediaInformationRoot.PreviewMouseDown += MediaInformationRoot_PreviewMouseDown;

            MW.MC.FFButton.FastForward += FFButton_FastForward;
            MW.MC.RewButton.Rewind += RewButton_Rewind;
            MW.MC.PlayingStatus.MouseClick += PlayingStatus_MouseClick;
            MW.MC.MediaStateButton.MouseClicked += MediaStateButton_MouseClicked;
            MW.MC.Volume.ValueChanged += Volume_ValueChanged;
            MW.MC.Volume.MuteChanged += Volume_MuteChanged;
            
            MW.Shuffle.StateChanged += (sender, e) =>
            {
                MW.Manager.Shuffle = MW.Shuffle.ToggleState;
                RaiseEvent(LAPP.Player.Receiver.Action.Shuffle, MW.Manager.Shuffle);
            };

            MW.Repeat.StateChanged += Repeat_StateChanged;

            MW.Caption.OptionalButtonClick += Caption_OptionalButtonClick;

            MW.Closing += Window_Closing;
        }

        private void Events_Noticed(object sender, LAPP.Events.NotificationEventArgs e)
        {
            Notification notif = new Notification(MW.ParentGrid, e.Text, e.FillBrush);
            LAP.Dialogs.LogWindow.Append("[" + e.Assembly.GetName() + "] : " + e.Text);
            notif.ShowMessage();
        }

        FrameworkElement itemhidden;
        private void MediaInformationRoot_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mirmrf)
            {
                Animation.SwitchVisibility sv = new Animation.SwitchVisibility();
                if (MW.LyricsT.Visibility == Visibility.Hidden)
                {
                    sv.Animate(Config.Setting.Values.MediaInformationLyricsAnimationDuration, MW.LyricsT, MW.ArtworkI);
                    RaiseEvent(LAPP.Player.Receiver.Action.MediaLyrics, true);
                    RaiseEvent(LAPP.Player.Receiver.Action.MediaArtwork, false);
                }
                else
                {
                    sv.Animate(Config.Setting.Values.MediaInformationLyricsAnimationDuration, MW.ArtworkI, MW.LyricsT);
                    RaiseEvent(LAPP.Player.Receiver.Action.MediaLyrics, false);
                    RaiseEvent(LAPP.Player.Receiver.Action.MediaArtwork, true);
                }
            }
            mirmrf = false;

            if (mirmcf || mirmlf)
            {
                Animation.Visible va = new Animation.Visible();

                if(itemhidden != null)
                {
                    if(MW.LyricsT.Visibility == Visibility.Hidden &&
                        MW.ArtworkI.Visibility == Visibility.Hidden)
                    {
                        va.Animate(Config.Setting.Values.MediaInformationLyricsAnimationDuration, itemhidden, Visibility.Visible);
                        itemhidden = null;
                        mirmcf = false;
                        RaiseEvent(LAPP.Player.Receiver.Action.MediaHidden, false);
                        return;
                    }
                }

                switch (MW.LyricsT.Visibility)
                {
                    case Visibility.Visible:
                        va.Animate(Config.Setting.Values.MediaInformationLyricsAnimationDuration, MW.LyricsT, Visibility.Hidden);
                        itemhidden = MW.LyricsT;
                        RaiseEvent(LAPP.Player.Receiver.Action.MediaHidden, true);
                        break;
                }

                switch (MW.ArtworkI.Visibility)
                {
                    case Visibility.Visible:
                        va.Animate(Config.Setting.Values.MediaInformationLyricsAnimationDuration, MW.ArtworkI, Visibility.Hidden);
                        itemhidden = MW.ArtworkI;
                        RaiseEvent(LAPP.Player.Receiver.Action.MediaHidden, true);
                        break;
                }
            }
            mirmcf = false;
            mirmlf = false;
        }

        private bool mirmrf = false, mirmcf = false, mirmlf = false;
        private void MediaInformationRoot_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                mirmcf = true;
            if (e.RightButton == MouseButtonState.Pressed)
                mirmrf = true;
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.StylusDevice != null)
                    mirmlf = true;
            }
        }

        private void PluginManager_PluginChanged(object sender, PluginChangedEventArgs e)
        {
            if(e.Plugin.Instance.Pages != null)
            {
                for(int i = 0;e.Plugin.Instance.Pages.Count > i; i++)
                {
                    MW.Manager.Pages.Remove(e.Plugin.Instance.Pages[i]);
                }
            }

            MW.Tab.ActiveIndex = -1;
        }

        private void RewButton_Rewind(object sender, EventArgs e)
        {
            if (MW.Renderer != null && MW.Renderer.StreamStatus != NWrapper.Audio.Status.Stopped)
            {
                RaiseEvent(LAPP.Player.Receiver.Action.Rewind);
                long interval = MW.Renderer.AudioFileReader.Length / 20;
                if (MW.Renderer.AudioFileReader.Position - interval >= 0)
                {
                    MW.Renderer.AudioFileReader.Position -= interval;
                }
                else
                {
                    MW.Renderer.AudioFileReader.Position = 0;
                }
            }
        }

        private void FFButton_FastForward(object sender, EventArgs e)
        {
            if(MW.Renderer != null && MW.Renderer.StreamStatus != NWrapper.Audio.Status.Stopped)
            {
                RaiseEvent(LAPP.Player.Receiver.Action.FastForward);
                long interval = MW.Renderer.AudioFileReader.Length / 20;
                if (MW.Renderer.AudioFileReader.Position + interval <= MW.Renderer.AudioFileReader.Length)
                {
                    MW.Renderer.AudioFileReader.Position += interval;
                }
                else
                {
                    MW.Renderer.AudioFileReader.Position = MW.Renderer.AudioFileReader.Length;
                }
            }
        }

        private void Program_NotImplementedException(object sender, EventArgs e)
        {
            Notification na = new Notification(MW.ParentGrid, Config.Language.Strings.ExceptionMessage.NotImplemented,
                Config.Setting.Brushes.Notification.Message.Brush);
            na.ShowMessage();
        }

        private void Volume_MuteChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (MW.Renderer != null)
            {
                if (MW.Renderer.AudioFileReader != null)
                {
                    MW.ApplyVolume();
                }
            }
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MW.Renderer != null)
            {
                if (MW.Renderer.AudioFileReader != null)
                {
                    MW.ApplyVolume();
                }
            }
        }

        private void MediaStateButton_MouseClicked(object sender, MouseButtonEventArgs e)
        {
            if (MW.Renderer != null)
            {
                switch (MW.Renderer.StreamStatus)
                {
                    case NWrapper.Audio.Status.Stopped:
                        MW.Manager.PlayNext();
                        break;

                    case NWrapper.Audio.Status.Playing:
                        MW.PauseFile();
                        break;

                    case NWrapper.Audio.Status.Paused:
                        MW.RunFile();
                        break;
                }
            }
        }

        private void PlayingStatus_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (MW.Renderer != null)
            {
                if (MW.Renderer.StreamStatus != NWrapper.Audio.Status.Stopped)
                {
                    Animation.SwitchVisibility sv = new Animation.SwitchVisibility();
                    if (MW.MediaInformationRoot.Visibility == Visibility.Hidden)
                    {
                        sv.Animate(Config.Setting.Values.PlayingStatusAnimationDuration, MW.MediaInformationRoot, MW.LibraryRoot);
                        RaiseEvent(LAPP.Player.Receiver.Action.MediaInformation, true);
                    }
                    else
                    {
                        sv.Animate(Config.Setting.Values.PlayingStatusAnimationDuration, MW.LibraryRoot, MW.MediaInformationRoot);
                        RaiseEvent(LAPP.Player.Receiver.Action.MediaInformation, false);
                    }
                }
            }
        }

        private void Repeat_StateChanged(object sender, EventArgs e)
        {
            if (MW.Repeat.ToggleState == MVPUC.Toggles.Repeat.State.Repeat)
                MW.Manager.Loop = true;
            else
                MW.Manager.Loop = false;

            RaiseEvent(LAPP.Player.Receiver.Action.Repeat, (int)MW.Repeat.ToggleState);
        }

        private void TaskbarManager_PauseButtonClick(object sender, EventArgs e)
        {
            MW.PauseFile();
        }

        private void TaskbarManager_PlayButtonClick(object sender, EventArgs e)
        {
            MW.RunFile();
        }

        private void LibraryButton_MouseClicked(object sender, MouseButtonEventArgs e)
        {
            if (MW.MediaInformationRoot.Visibility == Visibility.Visible)
            {
                Animation.SwitchVisibility sv = new Animation.SwitchVisibility();
                sv.Animate(Config.Setting.Values.PlayingStatusAnimationDuration, MW.LibraryRoot, MW.MediaInformationRoot);
            }
            else
            {
                MW.Manager.SetTopPage();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MW.StopFile(true);
            MW.Manager.Dispose();

            Config.WriteSetting(Paths.SettingFilePath);
            Config.WriteLanguage(Config.Setting.Paths.UsingLanguage);

            RaiseEvent(LAPP.Player.Receiver.Action.WindowClosing);
        }

        private void InitializeTabAndManager()
        {
            MW.Manager = new Page.Manager(MW.library, MW.Tab);
            MW.Manager.Pages.AddRange(Pages.GetPages());
            if (MW.Tab.Items.Count > 0) MW.Tab.ActiveIndex = 0;
        }

        private void Manager_OrderEnded(object sender, EventArgs e)
        {
            if(MW.Repeat.ToggleState == MVPUC.Toggles.Repeat.State.Disable)
            {
                MW.StopFile(true);
            }
        }

        private void Manager_RendererDisposeRequest(object sender, EventArgs e)
        {
            MW.StopFile(false);
        }

        private void Manager_PlayFile(object sender, LAPP.RunFileEventArgs e)
        {
            MW.PlayFile(e.Item.File);
        }

        private void InitializeTaskBarManager()
        {
            MW.TaskbarManager = new Taskbar(MW);
            MW.TaskbarManager.PlayButtonClick += TaskbarManager_PlayButtonClick;
            MW.TaskbarManager.PauseButtonClick += TaskbarManager_PauseButtonClick;
            MW.TaskbarManager.AddButtons();
        }

        private void Caption_OptionalButtonClick(object sender, EventArgs e)
        {
            switch (MW.OptionalGrid.Visibility)
            {
                case Visibility.Visible:
                    MW.OptionalGrid.Visibility = Visibility.Hidden;
                    RaiseEvent(LAPP.Player.Receiver.Action.ToolStrip, false);
                    break;

                case Visibility.Hidden:
                    MW.OptionalGrid.Visibility = Visibility.Visible;
                    RaiseEvent(LAPP.Player.Receiver.Action.ToolStrip, true);
                    break;
            }
        }

        private void OptionalView_ItemClicked(object sender, ClearUC.ItemClickedEventArgs e)
        {
            if (e.Item == OpenItem)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                    MW.DirectPlay(ofd.FileName);
            }

            if (e.Item == ConfigItem)
            {
                new LAP.Dialogs.Config(MW).Show();
            }

            if (e.Item == CreatorItem)
            {
                new LAP.Dialogs.Creator().ShowDialog();
            }

            if (e.Item == ExitItem)
            {
                MW.Close();
            }

            if (e.Item == LogItem)
            {
                LogWindow.Show();
            }

            MW.OptionalGrid.Visibility = Visibility.Hidden;
        }
    }
}