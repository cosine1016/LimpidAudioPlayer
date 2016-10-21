using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MVPUC.Visualize
{
    /// <summary>
    /// Spectrum.xaml の相互作用ロジック
    /// </summary>
    public class Spectrum : UserControl
    {
        private Grid parent = new Grid();

        public Spectrum()
        {
            InitializeComponent();
            Columns.CollectionChanged += Columns_CollectionChanged;
        }

        public ObservableCollection<Bar> Columns { get; set; } = new ObservableCollection<Bar>();

        public double Interval { get; set; } = 5;

        public double MaximumDuration { get; set; } = 1600;

        public double MaxY { get; set; } = 1;

        public bool OverrideMaxY { get; set; } = false;

        private double MaxX { get; set; } = 0;

        private void AddItems(IList Items)
        {
            for (int i = 0; Items.Count > i; i++)
            {
                Bar Bar = (Bar)Items[i];
                parent.Children.Add(Bar.Rect);
                Bar.XChanged += Bar_XChanged;
                Bar.YChanged += Bar_YChanged;

                CalcLoc(Bar);
            }
        }

        private void AnimateBar(Bar Bar, double Height)
        {
            if (Height > Bar.Rect.Height || double.IsNaN(Bar.Rect.Height))
            {
                Bar.Rect.Height = Height;

                double dur = Bar.Y / MaxY * MaximumDuration;

                if (Bar.HeightAnimator == null)
                {
                    Bar.HeightAnimator = new System.Windows.Media.Animation.DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(dur)));
                }
                else
                {
                    Bar.HeightAnimator.Duration = new Duration(TimeSpan.FromMilliseconds(dur));
                    Bar.HeightAnimator.From = Height;
                }

                Bar.Rect.BeginAnimation(HeightProperty, Bar.HeightAnimator);
            }
        }

        private void Bar_XChanged(object sender, EventArgs e)
        {
            CalcLoc((Bar)sender);
        }

        private void Bar_YChanged(object sender, EventArgs e)
        {
            CalcLoc((Bar)sender);
        }

        private void CalcLoc(Bar Bar)
        {
            double Width = (parent.ActualWidth - (5 + (Interval * (Columns.Count - 1)))) / Columns.Count;

            MaxX = (Bar.X > MaxX) ? Bar.X : MaxX;
            if (OverrideMaxY == false) MaxY = (Bar.ActualY > MaxY) ? Bar.ActualY : MaxY;

            if (Width > 0)
            {
                Bar.Rect.Margin = new Thickness(5 + ((Width * Bar.X) + (Interval * Bar.X)), 0, 0, 0);

                Bar.Rect.Width = Width;

                AnimateBar(Bar, parent.ActualHeight * (1.0 * Bar.ActualY / MaxY));
                Bar.ActualY = 0;
            }
        }

        private void ClearItems()
        {
            for (int i = 0; parent.Children.Count > i; i++)
            {
                Rectangle rec = (Rectangle)parent.Children[i];
                ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
                da.AnimationCompleted += Da_AnimationCompleted;
                da.Animate(rec.Height, 0, MaximumDuration * 2, null, HeightProperty, rec);
            }
        }

        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    ReplaceItems(e.NewItems, e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ClearItems();
                    break;
            }
        }

        private void Da_AnimationCompleted(object sender, ClearUC.Utils.AnimationHelper.AnimationEventArgs e)
        {
            Rectangle rec = (Rectangle)e.AnimatedObject;
            parent.Children.Remove(rec);
        }

        private void InitializeComponent()
        {
            SizeChanged += UserControl_SizeChanged;
            AddChild(parent);
        }

        private void ReplaceItems(IList Items, int StartIndex)
        {
            double Interval = (ActualWidth - (((Bar)Items[0]).Rect.Width)) / (Columns.Count + 1);
            double l = Interval;

            for (int i = 0; Items.Count > i; i++)
            {
                Bar Bar = (Bar)Items[i];

                if (OverrideMaxY == false) MaxY = (Bar.Y > MaxY) ? Bar.Y : MaxY;

                AnimateBar(Bar, parent.ActualHeight * (1.0 * Bar.Y / MaxY));
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int i = 0; Columns.Count > i; i++)
                {
                    CalcLoc(Columns[i]);
                }
            }));
        }

        public class Bar : DependencyObject
        {
            public event EventHandler XChanged;

            public event EventHandler YChanged;

            public static readonly DependencyProperty ActualYProperty = DependencyProperty.Register("ActualY", typeof(float), typeof(Bar));

            public static readonly DependencyProperty XProperty = DependencyProperty.Register("X", typeof(int), typeof(Bar));

            public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(float), typeof(Bar));

            internal System.Windows.Media.Animation.DoubleAnimation HeightAnimator;

            internal Rectangle Rect = new Rectangle();

            public Bar(int X, float Y)
            {
                this.X = X;
                this.Y = Y;
                Rect.HorizontalAlignment = HorizontalAlignment.Left;
                Rect.VerticalAlignment = VerticalAlignment.Bottom;
                Rect.Width = 40;
                Rect.Fill = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                Rect.Stroke = null;
                Rect.StrokeThickness = 0;
            }

            public float ActualY
            {
                get { return (float)GetValue(ActualYProperty); }
                set { SetValue(ActualYProperty, value); }
            }

            public Brush Background
            {
                get { return Rect.Fill; }
                set { Rect.Fill = value; }
            }

            public double Opacity
            {
                get { return Rect.Opacity; }
                set { Rect.Opacity = value; }
            }

            public Brush Stroke
            {
                get { return Rect.Stroke; }
                set { Rect.Stroke = value; }
            }

            public double StrokeThickness
            {
                get { return Rect.StrokeThickness; }
                set { Rect.StrokeThickness = value; }
            }

            public int X
            {
                get
                {
                    int val = (int)GetValue(XProperty);
                    return val;
                }
                set
                {
                    SetValue(XProperty, value);
                    XChanged?.Invoke(this, new EventArgs());
                }
            }

            public float Y
            {
                get
                {
                    float val = (float)GetValue(YProperty);
                    return val;
                }
                set
                {
                    SetValue(ActualYProperty, value);
                    SetValue(YProperty, value);
                    YChanged?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}