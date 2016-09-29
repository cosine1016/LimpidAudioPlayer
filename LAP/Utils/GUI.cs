using ClearUC.ListViewItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace LAP.Utils
{
    internal class GUI
    {
        private ListSubItem OpenItem;
        private ListSubItem OpenDiscItem;
        private ListSubItem ConfigItem;
        private ListSubItem CreatorItem;
        private ListSubItem EqualizerItem;
        private ListSubItem ExitItem;
        private ListSubItem LogItem;
        private LAP.Dialogs.LogWindow LogWindow;

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

            //OpenDiscItem = new ListSubItem();
            //OpenDiscItem.MainLabelText = Utils.Config.Language.Strings.OptionalView.DiscOpen;
            //OpenDiscItem.SubLabelVisibility = Visibility.Hidden;
            //OpenDiscItem.Height = PerHeight;
            //MW.OptionalView.Items.Add(OpenDiscItem);

            MW.OptionalView.Items.Add(new Separator());

            ConfigItem = new ListSubItem();
            ConfigItem.MainLabelText = Utils.Config.Language.Strings.OptionalView.Config;
            ConfigItem.SubLabelVisibility = Visibility.Hidden;
            ConfigItem.Height = PerHeight;
            MW.OptionalView.Items.Add(ConfigItem);

            EqualizerItem = new ListSubItem();
            EqualizerItem.MainLabelText = Config.Language.Strings.OptionalView.Equalizer;
            EqualizerItem.SubLabelVisibility = Visibility.Hidden;
            EqualizerItem.Height = PerHeight;
            MW.OptionalView.Items.Add(EqualizerItem);

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
            PluginManager.PluginEnableChanged += PluginManager_PluginEnableChanged;
            MW.MC.StopButton.MouseClicked += (sender, e) => { MW.StopFile(true); };
            MW.MC.LibraryButton.MouseClicked += LibraryButton_MouseClicked;
            MW.MC.FFButton.MouseClicked += (sender, e) => { MW.Manager.PlayNextFile(); };
            MW.MC.RewButton.MouseClicked += (sender, e) =>
            {
                if(MW.Renderer == null)
                {
                    MW.Manager.PlayLastFile();
                    return;
                }
                long min = MW.Renderer.WaveStream.Length / 30;
                if (MW.Renderer != null && MW.Renderer.StreamStatus != NWrapper.Audio.Status.Stopped && MW.Renderer.WaveStream.Position >= min)
                {
                    MW.Renderer.WaveStream.Position = 0;
                }
                else
                    MW.Manager.PlayLastFile();
            };

            MW.MediaInformationRoot.PreviewMouseUp += MediaInformationRoot_PreviewMouseUp;
            MW.MediaInformationRoot.PreviewMouseDown += MediaInformationRoot_PreviewMouseDown;

            MW.MC.FFButton.FastForward += FFButton_FastForward;
            MW.MC.RewButton.Rewind += RewButton_Rewind;
            MW.MC.PlayingStatus.MouseClick += PlayingStatus_MouseClick;
            MW.MC.MediaStateButton.MouseClicked += MediaStateButton_MouseClicked;
            MW.MC.Volume.ValueChanged += Volume_ValueChanged;
            MW.MC.Volume.MuteChanged += Volume_MuteChanged;

            MW.QueueButton.Click += QueueButton_Click;
            MW.Shuffle.StateChanged += (sender, e) => { MW.Manager.Shuffle = MW.Shuffle.ToggleState; };
            MW.Repeat.StateChanged += Repeat_StateChanged;

            Equalizer.EqualizerChanged += (sender, e) =>
            {
                if (MW.Renderer != null && MW.Renderer.Equalizer != null)
                    MW.Renderer.SetEqualizerBand(Equalizer.Bands);
            };

            MW.Caption.OptionalButtonClick += Caption_OptionalButtonClick;

            MW.Closing += Window_Closing;
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
                }
                else
                {
                    sv.Animate(Config.Setting.Values.MediaInformationLyricsAnimationDuration, MW.ArtworkI, MW.LyricsT);
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
                        return;
                    }
                }

                switch (MW.LyricsT.Visibility)
                {
                    case Visibility.Visible:
                        va.Animate(Config.Setting.Values.MediaInformationLyricsAnimationDuration, MW.LyricsT, Visibility.Hidden);
                        itemhidden = MW.LyricsT;
                        break;
                }

                switch (MW.ArtworkI.Visibility)
                {
                    case Visibility.Visible:
                        va.Animate(Config.Setting.Values.MediaInformationLyricsAnimationDuration, MW.ArtworkI, Visibility.Hidden);
                        itemhidden = MW.ArtworkI;
                        break;
                }
            }
            mirmcf = false;
            mirmlf = false;
        }

        private bool mirmrf = false, mirmcf = false, mirmlf = false;
        private int lastTimestamp = -1;
        private void MediaInformationRoot_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                mirmcf = true;
            if (e.RightButton == MouseButtonState.Pressed)
                mirmrf = true;
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.StylusDevice != null)
                {
                    if(e.Timestamp - lastTimestamp < 700)
                    {
                        mirmlf = true;
                    }
                }
                lastTimestamp = e.Timestamp;
            }
        }

        private void PluginManager_PluginEnableChanged(object sender, EventArgs e)
        {
            MW.Manager.Pages.Clear();
            MW.Manager.Pages.AddRange(Pages.GetPages());
            if (MW.Tab.Items.Count > 0) MW.Tab.ActiveIndex = 0;
        }

        private void RewButton_Rewind(object sender, EventArgs e)
        {
            if (MW.Renderer != null && MW.Renderer.StreamStatus != NWrapper.Audio.Status.Stopped)
            {
                long interval = MW.Renderer.WaveStream.Length / 20;
                if (MW.Renderer.WaveStream.Position - interval >= 0)
                {
                    MW.Renderer.WaveStream.Position -= interval;
                }
                else
                {
                    MW.Renderer.WaveStream.Position = 0;
                }
            }
        }

        private void FFButton_FastForward(object sender, EventArgs e)
        {
            if(MW.Renderer != null && MW.Renderer.StreamStatus != NWrapper.Audio.Status.Stopped)
            {
                long interval = MW.Renderer.WaveStream.Length / 20;
                if (MW.Renderer.WaveStream.Position + interval <= MW.Renderer.WaveStream.Length)
                {
                    MW.Renderer.WaveStream.Position += interval;
                }
                else
                {
                    MW.Renderer.WaveStream.Position = MW.Renderer.WaveStream.Length;
                }
            }
        }

        private void QueueButton_Click(object sender, RoutedEventArgs e)
        {
            MW.Dispatcher.BeginInvoke(new Action(() =>
            {
                Animation.Visible va = new Animation.Visible();
                if (MW.QueueWindow.Visibility == Visibility.Visible)
                {
                    va.Animate(Config.Setting.Values.QueueWindowAnimationDuration, MW.QueueWindow, Visibility.Hidden);
                }
                else
                {
                    va.Animate(Config.Setting.Values.QueueWindowAnimationDuration, MW.QueueWindow, Visibility.Visible);
                }
            }));
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
                if (MW.Renderer.WaveStream != null)
                {
                    MW.ApplyVolume();
                }
            }
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MW.Renderer != null)
            {
                if (MW.Renderer.WaveStream != null)
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
                        MW.Manager.PlayNextFile();
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
                        MW.Renderer.SampleAggregator.Enabled = true;
                        sv.Animate(Config.Setting.Values.PlayingStatusAnimationDuration, MW.MediaInformationRoot, MW.LibraryRoot);
                    }
                    else
                    {
                        MW.Renderer.SampleAggregator.Enabled = false;
                        sv.Animate(Config.Setting.Values.PlayingStatusAnimationDuration, MW.LibraryRoot, MW.MediaInformationRoot);
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
        }

        private void Tab_ActiveItemChanged(object sender, EventArgs e)
        {
            ListItem[] items = null;
            MW.library.Items.Clear();
            if (MW.Manager.Pages[MW.Tab.ActiveIndex].Opened == false)
            {
                MW.Manager.Pages[MW.Tab.ActiveIndex].Update();
                items = MW.Manager.Pages[MW.Tab.ActiveIndex].GetTopPageItems();
                if (items != null) MW.library.Items.AddRange(items);
            }
            else
            {
                items = MW.Manager.Pages[MW.Tab.ActiveIndex].GetPageItems();
                if (items != null) MW.library.Items.AddRange(items);
            }
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
                MW.library.Items.Clear();
                MW.Manager.Pages[MW.Tab.ActiveIndex].Update();

                ListItem[] items = MW.Manager.Pages[MW.Tab.ActiveIndex].GetTopPageItems();
                if(items != null) MW.library.Items.AddRange(items);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MW.StopFile(true);
            MW.Manager.Dispose();

            Config.WriteSetting(Paths.SettingFilePath);
            Config.WriteLanguage(Config.Setting.Paths.UsingLanguage);
        }

        private void InitializeTabAndManager()
        {
            MW.Tab.ActiveItemChanged += Tab_ActiveItemChanged;
            MW.Manager = new Page.Manager(MW.library, MW.PlayQueue, MW.Tab);
            MW.Manager.Pages.AddRange(Pages.GetPages());
            if (MW.Tab.Items.Count > 0) MW.Tab.ActiveIndex = 0;

            MW.Manager.PlayFile += Manager_PlayFile;
            MW.Manager.RendererDisposeRequest += Manager_RendererDisposeRequest;
            MW.Manager.OrderEnded += Manager_OrderEnded;
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

        private void Manager_PlayFile(object sender, Page.PlayFileEventArgs e)
        {
            MW.PlayFile(e.File);
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
                    break;

                case Visibility.Hidden:
                    MW.OptionalGrid.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void OptionalView_ItemClicked(object sender, ListItem.ItemClickedEventArgs e)
        {
            if (e.Item == OpenItem)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    MW.DirectPlay(ofd.FileName);
            }

            if(e.Item == OpenDiscItem)
            {

            }

            if (e.Item == ConfigItem)
            {
                new LAP.Dialogs.Config(MW).Show();
            }

            if (e.Item == EqualizerItem)
            {
                new LAP.Dialogs.Equalizer().Show();
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