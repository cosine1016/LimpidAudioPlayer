using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;

namespace MVPUC.Visualize
{
    /// <summary>
    /// Spectrum.xaml の相互作用ロジック
    /// </summary>
    public class Spectrum : UserControl
    {
        public class Bar
        {
            public event EventHandler XChanged;
            public event EventHandler YChanged;

            public Bar(double X, double Y)
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

            internal Rectangle Rect = new Rectangle();

            double x, y;
            public double X
            {
                get { return x; }
                set
                {
                    x = value;
                    if (XChanged != null) XChanged(this, new EventArgs());
                }
            }

            public double Y
            {
                get { return y; }
                set
                {
                    y = value;
                    if (YChanged != null) YChanged(this, new EventArgs());
                }
            }

            public Brush Background
            {
                get { return Rect.Fill; }
                set { Rect.Fill = value; }
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

            public double Opacity
            {
                get { return Rect.Opacity; }
                set { Rect.Opacity = value; }
            }
        }

        private Grid parent = new Grid();

        public Spectrum()
        {
            InitializeComponent();
            Columns.CollectionChanged += Columns_CollectionChanged;
        }

        private void InitializeComponent()
        {
            SizeChanged += UserControl_SizeChanged;
            AddChild(parent);
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

        public bool Animate { get; set; } = false;

        public double Duration { get; set; } = 50;

        public double Interval { get; set; } = 5;

        public bool OverrideMaxY { get; set; } = false;


        public double MaxY { get; set; } = 1;

        private double MaxX { get; set; } = 0;

        private void CalcLoc(Bar Bar)
        {
            double Width = (parent.ActualWidth - (5 + (Interval * (Columns.Count - 1)))) / Columns.Count;

            MaxX = (Bar.X > MaxX) ? Bar.X : MaxX;
            if (OverrideMaxY == false) MaxY = (Bar.Y > MaxY) ? Bar.Y : MaxY;

            if (Width < 0)
            {

            }
            else
            {
                Bar.Rect.Width = Width;
                if (Animate)
                {
                    if (double.IsNaN(Bar.Rect.Height)) Bar.Rect.Height = 0;
                    Bar.Rect.Margin = new Thickness(5 + ((Width * Bar.X) + (Interval * Bar.X)), 0, 0, 0);
                    ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
                    da.Animate(Bar.Rect.Height, parent.ActualHeight * (1.0 * Bar.Y / MaxY), Duration, null, HeightProperty, Bar.Rect);
                }
                else
                {
                    Bar.Rect.Margin = new Thickness(5 + ((Width * Bar.X) + (Interval * Bar.X)), 0, 0, 0);
                    Bar.Rect.Height = parent.ActualHeight * (1.0 * Bar.Y / MaxY);
                }
            }
        }

        private void AddItems(IList Items)
        {
            for(int i = 0; Items.Count > i; i++)
            {
                Bar Bar = (Bar)Items[i];
                parent.Children.Add(Bar.Rect);
                Bar.XChanged += Bar_XChanged;
                Bar.YChanged += Bar_YChanged;

                CalcLoc(Bar);
            }
        }

        private void ReplaceItems(IList Items, int StartIndex)
        {
            double Interval = (ActualWidth - (((Bar)Items[0]).Rect.Width)) / (Columns.Count + 1);
            double l = Interval;

            for (int i = 0; Items.Count > i; i++)
            {
                Bar Bar = (Bar)Items[i];

                if (OverrideMaxY == false) MaxY = (Bar.Y > MaxY) ? Bar.Y : MaxY;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Animate)
                    {
                        if (double.IsNaN(Bar.Rect.Height)) Bar.Rect.Height = 0;
                        Bar.Rect.Margin = new Thickness(l, 0, 0, 0);
                        ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
                        da.Animate(Bar.Rect.Height, parent.ActualHeight * (1.0 * Bar.Y / MaxY), Duration, null, HeightProperty, Bar.Rect);
                    }
                    else
                    {
                        Bar.Rect.Margin = new Thickness(l, 0, 0, 0);
                        Bar.Rect.Height = parent.ActualHeight * (1.0 * Bar.Y / MaxY);
                    }

                    l += Bar.Rect.Width + Interval;
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void ClearItems()
        {
            if (Animate)
            {
                for (int i = 0; parent.Children.Count > i; i++)
                {
                    Rectangle rec = (Rectangle)parent.Children[i];
                    ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
                    da.AnimationCompleted += Da_AnimationCompleted;
                    da.Animate(rec.Height, 0, Duration * 2, null, HeightProperty, rec);
                }
            }
            else
            {
                parent.Children.Clear();
            }
        }

        private void Da_AnimationCompleted(object sender, ClearUC.Utils.AnimationHelper.AnimationEventArgs e)
        {
            Rectangle rec = (Rectangle)e.AnimatedObject;
            parent.Children.Remove(rec);
        }

        private void Bar_YChanged(object sender, EventArgs e)
        {
            CalcLoc((Bar)sender);
        }

        private void Bar_XChanged(object sender, EventArgs e)
        {
            CalcLoc((Bar)sender);
        }

        public ObservableCollection<Bar> Columns { get; set; } = new ObservableCollection<Bar>();

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            for (int i = 0; Columns.Count > i; i++)
            {
                CalcLoc(Columns[i]);
            }
        }
    }
}
