using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Windows;
using System.Windows.Interop;

namespace LAP.Utils
{
    public class Taskbar
    {
        private ThumbnailToolBarButton BackButton;

        private ButtonState bs = ButtonState.Play;

        private ThumbnailToolBarButton NextButton;

        private Window ParentWindow;

        private ThumbnailToolBarButton PlayPauseButton;

        public Taskbar(Window ParentWindow)
        {
            this.ParentWindow = ParentWindow;
        }

        public event EventHandler BackButtonClick;

        public event EventHandler NextButtonClick;

        public event EventHandler PauseButtonClick;

        public event EventHandler PlayButtonClick;

        public enum ButtonState { Play, Pause }

        public ButtonState State
        {
            get { return bs; }
            set
            {
                bs = value;
                switch (bs)
                {
                    case ButtonState.Play:
                        PlayPauseButton.Icon = ToIcon(Resources.Taskbar.Play);
                        PlayPauseButton.Tooltip = Utils.Config.Language.Strings.Status.Playing;
                        break;

                    case ButtonState.Pause:
                        PlayPauseButton.Icon = ToIcon(Resources.Taskbar.Pause);
                        PlayPauseButton.Tooltip = Utils.Config.Language.Strings.Status.Pause;
                        break;
                }
            }
        }

        public void AddButtons()
        {
            PlayPauseButton = new ThumbnailToolBarButton(ToIcon(Resources.Taskbar.Play), Utils.Config.Language.Strings.Status.Playing);
            PlayPauseButton.Click += PlayPauseButton_Click;
            PlayPauseButton.Visible = false;

            NextButton = new ThumbnailToolBarButton(ToIcon(Resources.Taskbar.FastForward), Utils.Config.Language.Strings.MediaControl.Next);
            NextButton.Click += NextButton_Click;
            NextButton.Visible = false;

            BackButton = new ThumbnailToolBarButton(ToIcon(Resources.Taskbar.Rewind), Utils.Config.Language.Strings.MediaControl.Back);
            BackButton.Click += BackButton_Click;
            BackButton.Visible = false;

            TaskbarManager.Instance.ThumbnailToolBars.AddButtons(new WindowInteropHelper(ParentWindow).Handle,
                new ThumbnailToolBarButton[] { BackButton, PlayPauseButton, NextButton });
        }

        public void HideButtons()
        {
            PlayPauseButton.Visible = false;
            NextButton.Visible = false;
            BackButton.Visible = false;
        }

        public void VisibleButtons()
        {
            PlayPauseButton.Visible = true;
            NextButton.Visible = true;
            BackButton.Visible = true;
        }

        private void BackButton_Click(object sender, ThumbnailButtonClickedEventArgs e)
        {
            BackButtonClick?.Invoke(this, new EventArgs());
        }

        private void NextButton_Click(object sender, ThumbnailButtonClickedEventArgs e)
        {
            NextButtonClick?.Invoke(this, new EventArgs());
        }

        private void PlayPauseButton_Click(object sender, ThumbnailButtonClickedEventArgs e)
        {
            switch (State)
            {
                case ButtonState.Pause:
                    PauseButtonClick?.Invoke(this, new EventArgs());
                    break;

                case ButtonState.Play:
                    if (PlayButtonClick != null) PlayButtonClick(this, new EventArgs());
                    break;
            }
        }

        private System.Drawing.Icon ToIcon(System.Drawing.Bitmap Bitmap)
        {
            return System.Drawing.Icon.FromHandle(Bitmap.GetHicon());
        }
    }
}