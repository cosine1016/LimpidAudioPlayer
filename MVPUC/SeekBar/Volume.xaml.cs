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

namespace MVPUC.SeekBar
{
    /// <summary>
    /// Volume.xaml の相互作用ロジック
    /// </summary>
    public partial class Volume : UserControl
    {
        public event EventHandler<MouseButtonEventArgs> MouseClicked;

        protected virtual void OnMouseClicked(MouseButtonEventArgs e)
        {
            MouseClicked?.Invoke(this, e);
        }

        public event EventHandler AppliedPropertyChanges;

        protected virtual void OnAppliedPropertyChanges(EventArgs e)
        {
            AppliedPropertyChanges?.Invoke(this, e);
        }

        private bool mf = false;

        public Brush ButtonBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));

        public Brush MouseEnterBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

        public Brush MouseClickBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));

        public Brush ButtonStroke { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));

        public double StrokeThickness { get; set; } = 1;

        public double AnimationDuration { get; set; } = 50;

        public object Data { get; set; } = null;

        public Type DataType { get; set; }

        public void ApplyPropertyChanges()
        {
            OnAppliedPropertyChanges(new EventArgs());
        }

        public void Animate(Brush Before, Brush After, double Duration, Shape Shape)
        {
            if (Before == null | After == null) return;

            ClearUC.Utils.AnimationHelper.Brush ba = new ClearUC.Utils.AnimationHelper.Brush();
            ba.Animate(Before, After, Duration, Shape, new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty));
        }

        public void Animate(Brush Before, Brush After, Shape Shape)
        {
            Animate(Before, After, AnimationDuration, Shape);
        }

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ValueChanged;

        public event EventHandler<RoutedPropertyChangedEventArgs<bool>> MuteChanged;

        private bool enterF = false;

        public Volume()
        {
            InitializeComponent();

            timer.Interval = 3000;
            timer.Tick += Timer_Tick;
            AppliedPropertyChanges += LibraryButton_AppliedPropertyChanges;
            MouseClicked += Volume_MouseClicked;
            MouseLeave += Volume_MouseLeave;

            switch (Mute)
            {
                case true:
                    path16.Visibility = Visibility.Hidden;
                    path18.Visibility = Visibility.Hidden;
                    path20.Visibility = Visibility.Hidden;
                    Cross.Visibility = Visibility.Visible;
                    break;

                case false:
                    UpdateIcon();
                    break;
            }

            VolumeBar.ValueChanged += VolumeBar_ValueChanged;
        }

        public bool ShowVolumeBarOnMouseEnter { get; set; } = true;

        private void Volume_MouseLeave(object sender, MouseEventArgs e)
        {
            timer.Stop();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ClearUC.Utils.AnimationHelper.Thickness ta = new ClearUC.Utils.AnimationHelper.Thickness();
                ta.Animate(VolumeBar.Margin, new Thickness(0, 0, ActualWidth - drain.ActualWidth, 0),
                    300, null, new PropertyPath(MarginProperty), VolumeBar);
            }));
            timer.Stop();
        }

        private void VolumeBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Mute = false;

            UpdateIcon();

            ValueChanged?.Invoke(this, e);
        }

        private void UpdateIcon()
        {
            Cross.Visibility = Visibility.Hidden;
            path16.Visibility = Visibility.Hidden;
            path18.Visibility = Visibility.Hidden;
            path20.Visibility = Visibility.Hidden;

            if (VolumeBar.Value > 25)
            {
                path20.Visibility = Visibility.Visible;
                if (VolumeBar.Value > 50)
                {
                    path16.Visibility = Visibility.Visible;
                    if (VolumeBar.Value > 75)
                    {
                        path18.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Volume_MouseClicked(object sender, MouseButtonEventArgs e)
        {
            if (ToggleOnClick)
            {
                Mute = !Mute;
            }
        }

        private bool _Mute = false;

        public bool Mute
        {
            get { return _Mute; }
            set
            {
                if (Mute == value) return;
                _Mute = value;
                switch (value)
                {
                    case true:
                        path16.Visibility = Visibility.Hidden;
                        path18.Visibility = Visibility.Hidden;
                        path20.Visibility = Visibility.Hidden;
                        Cross.Visibility = Visibility.Visible;
                        break;

                    case false:
                        UpdateIcon();
                        break;
                }

                MuteChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<bool>(!_Mute, value));
            }
        }

        public bool ToggleOnClick { get; set; } = true;

        public double Value
        {
            get { return VolumeBar.Value; }
            set { VolumeBar.Value = value; }
        }

        private void LibraryButton_AppliedPropertyChanges(object sender, EventArgs e)
        {
            path14.Fill = ButtonBrush;
            path16.Fill = ButtonBrush;
            path18.Fill = ButtonBrush;
            path20.Fill = ButtonBrush;
            Cross.Fill = ButtonBrush;
            path14.Stroke = ButtonStroke;
            path16.Stroke = ButtonStroke;
            path18.Stroke = ButtonStroke;
            path20.Stroke = ButtonStroke;
            Cross.Stroke = ButtonStroke;
            path14.StrokeThickness = StrokeThickness;
            path16.StrokeThickness = StrokeThickness;
            path18.StrokeThickness = StrokeThickness;
            path20.StrokeThickness = StrokeThickness;
            Cross.StrokeThickness = StrokeThickness;
        }

        private void drain_MouseEnter(object sender, MouseEventArgs e)
        {
            enterF = true;
            Animate(path14.Fill, MouseEnterBrush, path14);

            if (!Mute)
            {
                Animate(path16.Fill, MouseEnterBrush, path16);
                Animate(path18.Fill, MouseEnterBrush, path18);
                Animate(path20.Fill, MouseEnterBrush, path20);
            }
            else
            {
                Animate(Cross.Fill, MouseEnterBrush, Cross);
            }

            if (ShowVolumeBarOnMouseEnter)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ClearUC.Utils.AnimationHelper.Thickness ta = new ClearUC.Utils.AnimationHelper.Thickness();
                    ta.Animate(VolumeBar.Margin, new Thickness(0),
                        300, null, new PropertyPath(MarginProperty), VolumeBar);
                }));
            }
            timer.Stop();
        }

        private void drain_MouseLeave(object sender, MouseEventArgs e)
        {
            mf = false;
            enterF = false;
            Animate(path14.Fill, ButtonBrush, path14);

            if (!Mute)
            {
                Animate(path16.Fill, ButtonBrush, path16);
                Animate(path18.Fill, ButtonBrush, path18);
                Animate(path20.Fill, ButtonBrush, path20);
            }
            else
            {
                Animate(Cross.Fill, ButtonBrush, Cross);
            }
        }

        private void drain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mf = true;
            Animate(path14.Fill, MouseClickBrush, path14);

            if (!Mute)
            {
                Animate(path16.Fill, MouseClickBrush, path16);
                Animate(path18.Fill, MouseClickBrush, path18);
                Animate(path20.Fill, MouseClickBrush, path20);
            }
            else
            {
                Animate(Cross.Fill, MouseClickBrush, Cross);
            }
        }

        private void drain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Animate(path14.Fill, MouseEnterBrush, path14);

            if (!Mute)
            {
                Animate(path16.Fill, MouseEnterBrush, path16);
                Animate(path18.Fill, MouseEnterBrush, path18);
                Animate(path20.Fill, MouseEnterBrush, path20);
            }
            else
            {
                Animate(Cross.Fill, MouseEnterBrush, Cross);
            }

            if (mf)
            {
                mf = false;
                MouseClicked?.Invoke(this, e);
            }
        }
    }
}