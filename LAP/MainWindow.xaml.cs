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
        internal LAPP.MediaFile PlayingFile = null;
        internal LAPP.MediaFile LastFile = null;
        internal Page.Manager Manager;
        internal Audio Renderer = null;
        internal Timer seekt = new Timer();
        internal Utils.Taskbar TaskbarManager;
        internal Utils.GUI GUIMan = null;

        public MainWindow()
        {
            InitializeComponent();

            if (Utils.InstanceData.ErrorRaise)
                throw new Exception("-ErrorRaiseが引数として与えられました");
        }

        public void DirectPlay(string FilePath)
        {
            StopFile(false);
            Manager.OnPlayStateChanged(Audio.Status.Stopped, null);

            LAPP.MediaFile file = new LAPP.MediaFile(FilePath);
            RenderFile(file);
        }

        public void RenderFile(LAPP.MediaFile File, bool KeepState = false, bool AutoRun = true)
        {
            try
            {
                InitializeRenderer(File.Path);

                TitleT.Content = File.Title;
                ArtistT.Content = File.Artist;
                AlbumT.Content = File.Album;
                LyricsT.Text = File.Lyrics;
                ArtworkI.Source = File.Artwork;
            }
            catch (Audio.ASIOException)
            {
                StopFile(true);
                Utils.Notification na = new Utils.Notification(ParentGrid,
                    Utils.Config.Language.Strings.ExceptionMessage.ASIOException, Utils.Config.Setting.Brushes.Notification.Error.Brush);
                na.ShowMessage();
                return;
            }
            catch (Exception ex)
            {
                Dialogs.LogWindow.Append(ex.Message);
                StopFile(true);
                Utils.Notification na = new Utils.Notification(ParentGrid,
                    Utils.Config.Language.Strings.ExceptionMessage.RenderingError, Utils.Config.Setting.Brushes.Notification.Error.Brush);
                na.ShowMessage();
                return;
            }

            SeekBar.Maximum = Renderer.WaveStream.Length;
            ApplyVolume();

            seekt.Interval = 20;
            seekt.Start();
            seekt.Tick += Seekt_Tick;

            MC.MediaStateButton.SwitchMediaState();

            if (!KeepState)
                SetFile(File);

            bgImage.Image = File.Artwork;

            if (bgImage.Visibility == Visibility.Hidden)
            {
                Utils.Animation.Visible va = new Utils.Animation.Visible();
                va.Animate(Utils.Config.Setting.Values.BackgroundImageAnimationDuration, bgImage, Visibility.Visible);
            }

            MC.PlayingStatus.Title = File.Title;
            MC.PlayingStatus.Album = File.Album;
            MC.PlayingStatus.Image = File.Artwork;
            MC.VisibleStatus();

            TaskbarManager.VisibleButtons();

            FFTCalcDicision();

            if (AutoRun) RunFile();
        }

        public void ReRenderFile(bool StayPosition, bool StayStatus)
        {
            if (PlayingFile == null) return;

            long pos = 0;
            Audio.Status status = Audio.Status.Playing;

            if (Renderer != null && Renderer.WaveStream != null)
            {
                if (StayPosition) pos = Renderer.WaveStream.Position;
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
                    Utils.Config.Language.Strings.ExceptionMessage.ASIOException, Utils.Config.Setting.Brushes.Notification.Error.Brush);
                na.ShowMessage();
                return;
            }
            catch (Exception ex)
            {
                Dialogs.LogWindow.Append(ex.Message);
                StopFile(true);
                Utils.Notification na = new Utils.Notification(ParentGrid,
                    Utils.Config.Language.Strings.ExceptionMessage.RenderingError, Utils.Config.Setting.Brushes.Notification.Error.Brush);
                na.ShowMessage();
                return;
            }

            Renderer.WaveStream.Position = pos;

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
                Renderer.WaveStream.Volume = (float)MC.Volume.Value / 100;
            }
            else
            {
                Renderer.WaveStream.Volume = 0;
            }
        }

        private void DisposeRenderer()
        {
            seekt.Stop();
            seekt.Tick -= Seekt_Tick;

            if (Renderer != null)
            {
                if (Renderer.SampleAggregator != null)
                {
                    Renderer.SampleAggregator.Enabled = false;
                    Renderer.SampleAggregator.PerformFFT = false;

                    Spectrum.SampleAggreator = null;
                }

                if (Renderer.PSEMicMixer != null)
                    Renderer.PSEMicMixer.PSE.ExtractedDegreeOfRisk -= PSE_ExtractedDegreeOfRisk;

                Renderer.PlaybackStopped -= Renderer_PlaybackStopped;

                Renderer.StreamStatus = Audio.Status.Stopped;
                Renderer.Dispose();
                Renderer = null;
            }
        }

        private void InitializeRenderer(string FilePath)
        {
            if (Renderer != null) DisposeRenderer();

            Renderer = new Audio();
            Renderer.WaveStream = new Utils.Classes.AudioFileReader(FilePath);

            Renderer.fftLenght = Utils.Config.Setting.Values.SpectrumBarCount * 2;

            Renderer.WavePlayer = Utils.Utility.CreateSoundDevice();
            Dialogs.LogWindow.Append("Output : " + Utils.Config.Setting.WaveOut.OutputDevice.ToString());

            Renderer.OpenFile(FilePath, Utils.Utility.GetCaptureDevice());
            Dialogs.LogWindow.Append("File Open : " + FilePath);

            Renderer.SetEqualizerBand(Utils.Equalizer.Bands);

            Spectrum.OverrideMaxY = true;
            Spectrum.MaxY = 100;
            Spectrum.MainThreadDispatcher = Dispatcher;
            Spectrum.SampleAggreator = Renderer.SampleAggregator;
            Spectrum.AssociateEvent();

            Renderer.PSEMicMixer.Enabled = Utils.Config.Setting.Boolean.PSE;
            Renderer.PSEMicMixer.PSE.ExtractedDegreeOfRisk += PSE_ExtractedDegreeOfRisk;
            Renderer.PlaybackStopped += Renderer_PlaybackStopped;

            Dialogs.LogWindow.Append("Renderer : " + Renderer.WaveStream.ToString());
        }

        private void Renderer_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                Utils.Notification ne = new Utils.Notification(ParentGrid, e.Exception.Message, null,
                    Utils.Config.Setting.Brushes.Notification.Error.Brush);
                ne.ShowMessage();
            }

            if (Repeat.ToggleState == MVPUC.Toggles.Repeat.State.SingleRepeat) ReRenderFile(false, true);
            else Manager.PlayNextFile();
        }

        internal void PlayFile(LAPP.MediaFile File)
        {
            SetFile(File);
            RenderFile(File);
        }

        private void SetFile(LAPP.MediaFile File)
        {
            if(LastFile != null)
            {
                LastFile.Dispose();
            }
            LastFile = PlayingFile;
            PlayingFile = File;
        }

        internal void RunFile()
        {
            if (MediaInformationRoot.Visibility == Visibility.Hidden)
            {
                Renderer.SampleAggregator.Enabled = true;
            }
            else
            {
                Renderer.SampleAggregator.Enabled = false;
            }

            Dialogs.LogWindow.Append("Run");
            MC.MediaStateButton.MediaState = MVPUC.Buttons.MediaStateButton.State.Pause;
            Renderer.StreamStatus = Audio.Status.Playing;
            TaskbarManager.State = Utils.Taskbar.ButtonState.Pause;
            Manager.OnPlayStateChanged(Audio.Status.Playing, PlayingFile);

            Spectrum.Start();
        }

        internal void PauseFile()
        {
            Dialogs.LogWindow.Append("Pause");
            MC.MediaStateButton.MediaState = MVPUC.Buttons.MediaStateButton.State.Play;
            Renderer.StreamStatus = Audio.Status.Paused;
            TaskbarManager.State = Utils.Taskbar.ButtonState.Play;
            Manager.OnPlayStateChanged(Audio.Status.Paused, PlayingFile);

            Spectrum.Pause();
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
                    sv.Animate(Utils.Config.Setting.Values.PlayingStatusAnimationDuration, LibraryRoot, MediaInformationRoot);

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

                    va.Animate(Utils.Config.Setting.Values.BackgroundImageAnimationDuration, bgImage, Visibility.Hidden);
                }

                LastFile?.Dispose();
                PlayingFile?.Dispose();
                LastFile = null;
                PlayingFile = null;
                MC.HideStatus();
            }

            Manager.OnPlayStateChanged(Audio.Status.Stopped, null);

            SeekBar.Value = SeekBar.Minimum;
            if (TaskbarManager != null) TaskbarManager.HideButtons();
        }

        private void PSE_ExtractedDegreeOfRisk(object sender, PerilousSoundEventArgs e)
        {
            if (Renderer.StreamStatus != Audio.Status.Stopped && e.DangerLevel == DegreeOfRisk.High && Utils.Config.Setting.Boolean.PSE)
            {
                Dialogs.LogWindow.Append("Perilous Sound Detected");
                Renderer.Fade.BeginFadeOut(1);
                Renderer.PSEMicMixer.Enabled = true;
            }
        }

        private void SeekBar_ValueChanged(object sender, ClearUC.SeekBar.ValueChangedEventArgs e)
        {
            if (e.ChangeType == ClearUC.SeekBar.ValueChangedEventArgs.ChangedType.ManualEnd && Renderer != null)
            {
                seekt.Stop();
                Renderer.WaveStream.Position = SeekBar.Value;
                seekt.Start();
            }
        }

        private void Seekt_Tick(object sender, EventArgs e)
        {
            if (Renderer != null && Renderer.WaveStream != null)
            {
                SeekBar.Value = Renderer.WaveStream.Position;
                MC.PlayingStatus.SetTime(Renderer.WaveStream.CurrentTime, Renderer.WaveStream.TotalTime);
                if (Renderer.WaveStream.Position >= Renderer.WaveStream.Length - 3000)
                    if (Repeat.ToggleState == MVPUC.Toggles.Repeat.State.SingleRepeat) ReRenderFile(false, true);
                    else Manager.PlayNextFile();
            }
        }

        private void WaitVolTimer_Tick(object sender, EventArgs e)
        {
            Renderer.Fade.BeginFadeIn(1000);
            Renderer.PSEMicMixer.Enabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                GUIMan = new Utils.GUI(this);
                GUIMan.Initialize();
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

            FFTCalcDicision();
        }

        private void FFTCalcDicision()
        {
            if (Renderer != null)
            {
                if (Renderer.SampleAggregator != null)
                {
                    bool perform = false;
                    bool active = Utils.Config.Setting.Values.CalculateFFTWindowState.HasFlag(Utils.Values.WindowState.Activated);
                    bool deactive = Utils.Config.Setting.Values.CalculateFFTWindowState.HasFlag(Utils.Values.WindowState.Deactivated);

                    if (IsActive && active)
                    {
                        perform = true;
                    }
                    else if (!IsActive && deactive)
                    {
                        perform = true;
                    }

                    bool max = Utils.Config.Setting.Values.CalculateFFTWindowState.HasFlag(Utils.Values.WindowState.Maximized);
                    bool min = Utils.Config.Setting.Values.CalculateFFTWindowState.HasFlag(Utils.Values.WindowState.Minimized);
                    bool nor = Utils.Config.Setting.Values.CalculateFFTWindowState.HasFlag(Utils.Values.WindowState.Normal);

                    if (max || min || nor)
                        perform = true;

                    Renderer.SampleAggregator.PerformFFT = perform;
                }
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            FFTCalcDicision();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            FFTCalcDicision();
        }
    }
}