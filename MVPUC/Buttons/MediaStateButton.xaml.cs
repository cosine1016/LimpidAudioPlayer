using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVPUC.Buttons
{
    /// <summary>
    /// MediaStateButton.xaml の相互作用ロジック
    /// </summary>
    public partial class MediaStateButton : ButtonBase
    {
        public enum State { Play, Pause }

        public event EventHandler MediaStateChanged;
        protected virtual void OnMediaStateChanged(EventArgs e)
        {
            MediaStateChanged?.Invoke(this, e);
        }

        State s = State.Pause;

        public MediaStateButton()
        {
            InitializeComponent();

            SwitchMediaStateShape();
            AppliedPropertyChanges += MediaStateButton_AppliedPropertyChanges;
        }

        public State MediaState
        {
            get { return s; }
            set
            {
                s = value;
                SwitchMediaStateShape();
                OnMediaStateChanged(new EventArgs());
            }
        }

        public State SwitchMediaState()
        {
            switch (s)
            {
                case State.Pause:
                    MediaState = State.Play;
                    return State.Play;
                case State.Play:
                    MediaState = State.Pause;
                    return State.Pause;
                default:
                    MediaState = State.Play;
                    return State.Play;
            }
        }

        private void SwitchMediaStateShape()
        {
            switch (s)
            {
                case State.Pause:
                    path.Visibility = Visibility.Hidden;
                    pause.Visibility = Visibility.Visible;
                    paused.Visibility = Visibility.Visible;
                    break;
                case State.Play:
                    path.Visibility = Visibility.Visible;
                    pause.Visibility = Visibility.Hidden;
                    paused.Visibility = Visibility.Hidden;
                    break;
                default:
                    s = State.Pause;
                    SwitchMediaStateShape();
                    break;
            }
        }

        private void MediaStateButton_AppliedPropertyChanges(object sender, EventArgs e)
        {
            path.Fill = ButtonBrush;
            pause.Fill = ButtonBrush;

            path.Stroke = ButtonStroke;
            pause.Stroke = ButtonStroke;

            path.StrokeThickness = StrokeThickness;
            pause.StrokeThickness = StrokeThickness;
        }

        private void path_MouseEnter(object sender, MouseEventArgs e)
        {
            Animate(path.Fill, MouseEnterBrush, path);
        }

        private void path_MouseLeave(object sender, MouseEventArgs e)
        {
            Animate(path.Fill, ButtonBrush, path);
        }

        private void path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Animate(path.Fill, MouseClickBrush, path);
        }

        private void path_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Animate(path.Fill, MouseEnterBrush, path);
        }

        private void paused_MouseEnter(object sender, MouseEventArgs e)
        {
            Animate(path.Fill, MouseEnterBrush, pause);
        }

        private void paused_MouseLeave(object sender, MouseEventArgs e)
        {
            Animate(path.Fill, ButtonBrush, pause);
        }

        private void paused_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Animate(path.Fill, MouseClickBrush, pause);
        }

        private void paused_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Animate(path.Fill, MouseEnterBrush, pause);
        }
    }
}
