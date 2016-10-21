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

        public double EnterOpacity { get; set; } = 0.9;

        public double ClickOpacity { get; set; } = 1;

        public bool ShowEnterLabel { get; set; } = true;

        public bool Animate { get; set; } = true;

        public double AnimationDuration { get; set; } = 200;

        public int ShowingDuration { get; set; } = 2000;

        public Thickness MaximizedMargin { get; set; } = new Thickness(0);

        public async void ShowMessage()
        {
            if (Animate == false)
            {
                Maximize();

                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(ShowingDuration);
                    if (entering == true)
                    {
                        while (entering == true) { }
                    }
                });

                Minimize();
            }
            else
            {
                Maximize();
            }
        }

        public void Minimize()
        {
            if (Animate == true)
            {
                Utils.AnimationHelper.Thickness ta = new Utils.AnimationHelper.Thickness();
                ta.AnimationCompleted += Ta_AnimationCompleted;
                ta.Animate(Margin, new Thickness(Margin.Left, Margin.Top - Height, Margin.Right, Margin.Bottom),
                    AnimationDuration, null, new PropertyPath(MarginProperty), this);
            }
            else
            {
                Margin = new Thickness(Margin.Left, Margin.Top - Height, Margin.Right, Margin.Bottom);
                if (MessageMinimized != null) MessageMinimized(this, new EventArgs());
            }
        }

        private void Ta_AnimationCompleted(object sender, Utils.AnimationHelper.AnimationEventArgs e)
        {
            MessageMinimized?.Invoke(this, new EventArgs());
        }

        public void Maximize()
        {
            if (Animate == true)
            {
                Utils.AnimationHelper.Thickness ta = new Utils.AnimationHelper.Thickness();
                ta.AnimationCompleted += AnimationCompleted;
                ta.Animate(Margin, MaximizedMargin, AnimationDuration, null, new PropertyPath(MarginProperty), this);
            }
            else
            {
                Margin = MaximizedMargin;
                MessageMaximized?.Invoke(this, new EventArgs());
            }
        }

        private async void AnimationCompleted(object sender, Utils.AnimationHelper.AnimationEventArgs e)
        {
            if (MessageMaximized != null) MessageMaximized(this, new EventArgs());

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(ShowingDuration);
                if (entering == true)
                {
                    while (entering == true) { }
                }
            });

            Minimize();
        }

        private bool entering = false;

        private void drain_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ShowEnterLabel == true) EnterLabel.Visibility = Visibility.Visible;
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