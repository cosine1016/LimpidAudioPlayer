using ClearUC.ListViewItems;
using NWrapper;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Threading.Tasks;

namespace LAP
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        internal LAPP.IO.MediaFile PlayingFile = null;
        internal Page.Manager Manager;
        internal Audio Renderer = null;
        internal Timer seekt = new Timer();
        internal Utils.Taskbar TaskbarManager;
        internal Utils.GUI GUIMan = null;
        private NAudio.Wave.PlaybackState LastPlaybackState = NAudio.Wave.PlaybackState.Stopped;

        private void RaiseEvent(LAPP.Player.Receiver.Action Action, params object[] Args)
        {
            LAPP.Player.Receiver.RaiseReceivedEvent(new LAPP.Player.Receiver.EventReceiveArgs(Action, Args));
        }
        private void RaiseEvent(LAPP.Player.Receiver.Action Action)
        {
            RaiseEvent(Action, null);
        }

        private void SetPlaybackState(NAudio.Wave.PlaybackState State)
        {
            if (LastPlaybackState != State)
            {
                LastPlaybackState = State;
                Manager.PlaybackStateChanged(State);
                RaiseEvent(LAPP.Player.Receiver.Action.PlaybackState, State);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            if (Utils.InstanceData.ErrorRaise)
                throw new Exception("-ErrorRaiseが引数として与えられました");

            RaiseEvent(LAPP.Player.Receiver.Action.Boot);
        }

        private void PluginManager_PluginChanged(object sender, EventArgs e)
        {
            if(Renderer != null)
            {
                ReRenderFile(true, true);
            }
        }

        private void Manager_RunFile(object sender, LAPP.RunFileEventArgs e)
        {
            if (e.Item.Playable)
                e.Success = PlayFile(e.Item.File);
        }

        private void Manager_Stop(object sender, EventArgs e)
        {
            StopFile(true);
        }

        public void DirectPlay(string FilePath)
        {
            StopFile(false);

            LAPP.IO.MediaFile file = new LAPP.IO.MediaFile(FilePath);
            RenderFile(file);
        }

        public bool RenderFile(LAPP.IO.MediaFile File, bool AutoRun = true)
        {
            try
            {
                InitializeRenderer(File.Path);

                TitleT.Content = File.Title;
                ArtistT.Content = File.Artist;
                AlbumT.Content = File.Album;
                //TODO Plugin Events
            }
            catch (Audio.ASIOException)
            {
                StopFile(true);
                Utils.Notification na = new Utils.Notification(ParentGrid,
                    Localize.Get("ASIO_EXCEPTION"), Constants.ErrorBrush);
                na.ShowMessage();
                return false;
            }
            catch (Exception ex)
            {
                Dialogs.LogWindow.Append(ex.Message);
                StopFile(true);
                Utils.Notification na = new Utils.Notification(ParentGrid,
                    Localize.Get("RENDERING_ERROR"), Constants.ErrorBrush);
                na.ShowMessage();
                return false;
            }

            SeekBar.Maximum = Renderer.AudioFileReader.Length;
            ApplyVolume();

            Utils.Animation.Visible ta = new Utils.Animation.Visible();
            ta.Animate(Config.Current.Animation[Enums.Animation.Default], TimeL, Visibility.Visible);

            seekt.Interval = 20;
            seekt.Start();
            seekt.Tick += Seekt_Tick;

            MC.MediaStateButton.SwitchMediaState();

            bgImage.Image = File.Artwork;

            if (bgImage.Visibility == Visibility.Hidden)
            {
                Utils.Animation.Visible va = new Utils.Animation.Visible();
                va.Animate(Config.Current.Animation[Enums.Animation.BackgroundImage], bgImage, Visibility.Visible);
            }

            MC.PlayingStatus.Title = File.Title;
            MC.PlayingStatus.Album = File.Album;
            MC.PlayingStatus.Image = File.Artwork;
            MC.VisibleStatus();

            PlayingFile = File;

            TaskbarManager.VisibleButtons();

            if (AutoRun) RunFile();

            return true;
        }

        public void ReRenderFile(bool StayPosition, bool StayStatus)
        {
            if (PlayingFile == null) return;

            long pos = 0;
            Audio.Status status = Audio.Status.Playing;

            if (Renderer != null && Renderer.AudioFileReader != null)
            {
                if (StayPosition) pos = Renderer.AudioFileReader.Position;
                if (StayStatus)
                {
                    status = Renderer.StreamStatus;
                    if (status == Audio.Status.Stopped) return;
                }
            }

            Renderer.StreamStatus = Audio.Status.Stopped;
            try
            {
                InitializeRenderer(PlayingFile.Path);
            }
            catch (Audio.ASIOException)
            {
                StopFile(true);
                Utils.Notification na = new Utils.Notification(ParentGrid,
                    Localize.Get("ASIO_EXCEPTION"), Constants.ErrorBrush);
                na.ShowMessage();
                return;
            }
            catch (Exception ex)
            {
                Dialogs.LogWindow.Append(ex.Message);
                StopFile(true);
                Utils.Notification na = new Utils.Notification(ParentGrid,
                    Localize.Get("RENDERING_ERROR"), Constants.ErrorBrush);
                na.ShowMessage();
                return;
            }

            Renderer.AudioFileReader.Position = pos;

            seekt.Interval = 20;
            seekt.Start();
            seekt.Tick += Seekt_Tick;

            ApplyVolume();

            switch (status)
            {
                case Audio.Status.Playing:
                    RunFile();
                    break;
                case Audio.Status.Paused:
                    PauseFile();
                    break;
            }
        }

        internal void ApplyVolume()
        {
            if (MC.Volume.Mute == false)
            {
                Renderer.AudioFileReader.Volume = (float)MC.Volume.Value / 100;
            }
            else
            {
                Renderer.AudioFileReader.Volume = 0;
            }

            RaiseEvent(LAPP.Player.Receiver.Action.VolumeChanged, Renderer.AudioFileReader.Volume);
        }

        private void DisposeRenderer()
        {
            seekt.Stop();
            seekt.Tick -= Seekt_Tick;

            Utils.Animation.Visible va = new Utils.Animation.Visible();
            va.Animate(Config.Current.Animation[Enums.Animation.Default], TimeL, Visibility.Hidden);
            TimeL.Content = "00:00 / 00:00";

            if (Renderer != null)
            {
                Renderer.PlaybackStopped -= Renderer_PlaybackStopped;

                Renderer.StreamStatus = Audio.Status.Stopped;
                Renderer.Dispose();
                Renderer = null;
                SetPlaybackState(NAudio.Wave.PlaybackState.Stopped);
            }

            RaiseEvent(LAPP.Player.Receiver.Action.RendererDisposed);
        }

        private void InitializeRenderer(string FilePath)
        {
            if (Renderer != null) DisposeRenderer();

            RaiseEvent(LAPP.Player.Receiver.Action.Render);
            Renderer = new Audio();

            Renderer.OpenFile(FilePath,
                new Utils.Classes.AudioFileReader(FilePath), Utils.Utility.CreateSoundDevice());

            Dialogs.LogWindow.Append("File Open : " + FilePath);

            RaiseEvent(LAPP.Player.Receiver.Action.Rendered);

            Renderer.PlaybackStopped += Renderer_PlaybackStopped;

            Dialogs.LogWindow.Append("Renderer : " + Renderer.AudioFileReader.ToString());
        }

        private void Renderer_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                Utils.Notification ne = new Utils.Notification(ParentGrid, e.Exception.Message, null,
                    Constants.ErrorBrush);
                ne.ShowMessage();
            }

            if (Repeat.ToggleState == MVPUC.Toggles.Repeat.State.SingleRepeat) ReRenderFile(false, true);
            else Manager.PlayNext();
        }

        internal bool PlayFile(LAPP.IO.MediaFile File)
        {
            return RenderFile(File);
        }

        internal void RunFile()
        {
            Dialogs.LogWindow.Append("Run");
            MC.MediaStateButton.MediaState = MVPUC.Buttons.MediaStateButton.State.Pause;
            Renderer.StreamStatus = Audio.Status.Playing;
            TaskbarManager.State = Utils.Taskbar.ButtonState.Pause;
            SetPlaybackState(NAudio.Wave.PlaybackState.Playing);
        }

        internal void PauseFile()
        {
            Dialogs.LogWindow.Append("Pause");
            MC.MediaStateButton.MediaState = MVPUC.Buttons.MediaStateButton.State.Play;
            Renderer.StreamStatus = Audio.Status.Paused;
            TaskbarManager.State = Utils.Taskbar.ButtonState.Play;
            SetPlaybackState(NAudio.Wave.PlaybackState.Paused);
        }

        internal void StopFile(bool ClearImage)
        {
            Dialogs.LogWindow.Append("Stop");
            MC.MediaStateButton.MediaState = MVPUC.Buttons.MediaStateButton.State.Play;

            DisposeRenderer();

            if (ClearImage)
            {
                Utils.Animation.SwitchVisibility sv = new Utils.Animation.SwitchVisibility();
                if (MediaInformationRoot.Visibility == Visibility.Visible)
                    sv.Animate(Config.Current.Animation[Enums.Animation.Default], LibraryRoot, MediaInformationRoot);

                if (bgImage.Image != null)
                {
                    Utils.Animation.Visible va = new Utils.Animation.Visible();
                    EventHandler handler = null;
                    handler = (sender, e) =>
                    {
                        bgImage.Image = null;
                        Dialogs.LogWindow.Append("Image Removed");
                        va.AnimationCompleted -= handler;
                    };
                    va.AnimationCompleted += handler;

                    va.Animate(Config.Current.Animation[Enums.Animation.Default], bgImage, Visibility.Hidden);
                }
                MC.HideStatus();

                SetPlaybackState(NAudio.Wave.PlaybackState.Stopped);
            }

            SeekBar.Value = SeekBar.Minimum;
            if (TaskbarManager != null) TaskbarManager.HideButtons();
        }

        private void SeekBar_ValueChanged(object sender, ClearUC.SeekBar.ValueChangedEventArgs e)
        {
            if (e.ChangeType == ClearUC.SeekBar.ValueChangedEventArgs.ChangedType.ManualEnd && Renderer != null)
            {
                seekt.Stop();
                Renderer.AudioFileReader.Position = SeekBar.Value;
                RaiseEvent(LAPP.Player.Receiver.Action.Seek, Renderer.AudioFileReader.Position);
                seekt.Start();
            }
        }

        private void Seekt_Tick(object sender, EventArgs e)
        {
            if (Renderer != null && Renderer.AudioFileReader != null)
            {
                SeekBar.Value = Renderer.AudioFileReader.Position;
                SetTime(Renderer.AudioFileReader.CurrentTime, Renderer.AudioFileReader.TotalTime);
                if (Renderer.AudioFileReader.Position >= Renderer.AudioFileReader.Length - 3000)
                    if (Repeat.ToggleState == MVPUC.Toggles.Repeat.State.SingleRepeat) ReRenderFile(false, true);
                    else Manager.PlayNext();
            }
        }

        private void SetTime(TimeSpan Remain, TimeSpan Duration)
        {
            TimeL.Content = Remain.ToString(@"mm\:ss") + " / " + Duration.ToString(@"mm\:ss");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                GUIMan = new Utils.GUI(this);
                GUIMan.Initialize();
                
                Manager.RunFile += Manager_RunFile;
                Manager.Stop += Manager_Stop;
                Utils.PluginManager.PluginChanged += PluginManager_PluginChanged;
            }));
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    Root.Margin = new Thickness(8);
                    break;

                default:
                    Root.Margin = new Thickness(0);
                    break;
            }

            RaiseEvent(LAPP.Player.Receiver.Action.WindowState, WindowState);
        }
    }
}