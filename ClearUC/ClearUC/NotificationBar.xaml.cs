using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClearUC
{
    /// <summary>
    /// NotificationBar.xaml の相互作用ロジック
    /// </summary>
    public partial class NotificationBar : UserControl
    {
        private const int visible_d = 300;
        private const int wait_d = 1000;
        private const double EnterOpacity = 0.8, ClickOpacity = 1.0;

        public event EventHandler Click;

        public event EventHandler MessageMaximized;

        public event EventHandler MessageMinimized;

        public NotificationBar()
        {
            InitializeComponent();
            bo = bg.Opacity;
        }

        private double bo;

        public double BackgroundOpacity
        {
            get { return bo; }
            set
            {
                bg.Opacity = value;
                bo = value;
            }
        }

        public double BackgroundStrokeThickness
        {
            get { return bg.StrokeThickness; }
            set { bg.StrokeThickness = value; }
        }

        public Brush BackgroundBrush
        {
            get { return bg.Fill; }
            set { bg.Fill = value; }
        }

        public Brush BackgroundStroke
        {
            get { return bg.Stroke; }
            set { bg.Stroke = value; }
        }

        public string Message
        {
            get { return (string)label.Content; }
            set { label.Content = value; }
        }

        public string EnterLabelText
        {
            get { return (string)EnterLabel.Content; }
            set { EnterLabel.Content = value; }
        }

        public bool VisibleEnterLabel { get; set; } = true;

        public Thickness MaximizedMargin { get; set; } = new Thickness(0);

        public void ShowMessage()
        {
            Maximize();
        }

        public void Minimize()
        {
            Utils.AnimationHelper.Thickness ta = new Utils.AnimationHelper.Thickness();
            ta.AnimationCompleted += Ta_AnimationCompleted;
            ta.Animate(Margin, new Thickness(Margin.Left, Margin.Top - Height, Margin.Right, Margin.Bottom),
                visible_d, null, new PropertyPath(MarginProperty), this);
        }

        private void Ta_AnimationCompleted(object sender, Utils.AnimationHelper.AnimationEventArgs e)
        {
            MessageMinimized?.Invoke(this, new EventArgs());
        }

        public void Maximize()
        {
            Utils.AnimationHelper.Thickness ta = new Utils.AnimationHelper.Thickness();
            ta.AnimationCompleted += AnimationCompleted;
            ta.Animate(Margin, MaximizedMargin, visible_d, null, new PropertyPath(MarginProperty), this);
        }

        private void AnimationCompleted(object sender, Utils.AnimationHelper.AnimationEventArgs e)
        {
            MessageMaximized?.Invoke(this, new EventArgs());

            Dispatcher.BeginInvoke(new Action(() =>
            {
                System.Threading.Thread.Sleep(wait_d);
                while (entering == true) { }
                Minimize();
            }));
        }

        private bool entering = false;

        private void drain_MouseEnter(object sender, MouseEventArgs e)
        {
            if (VisibleEnterLabel == true) EnterLabel.Visibility = Visibility.Visible;
            bg.Opacity = EnterOpacity;
            entering = true;
        }

        private void drain_MouseLeave(object sender, MouseEventArgs e)
        {
            EnterLabel.Visibility = Visibility.Hidden;
            bg.Opacity = BackgroundOpacity;
            entering = false;
        }

        private bool downf = false;

        private void drain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bg.Opacity = ClickOpacity;
            if (e.ChangedButton == MouseButton.Left)
            {
                downf = true;
            }
        }

        private void drain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bg.Opacity = ClickOpacity;
            downf = true;
        }

        private void drain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bg.Opacity = EnterOpacity;
            if (downf == true)
            {
                Click?.Invoke(this, new EventArgs());
            }
        }
    }
}