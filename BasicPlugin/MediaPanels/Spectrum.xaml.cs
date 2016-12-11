using NAudio.Dsp;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
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

namespace BasicPlugin.MediaPanels
{
    /// <summary>
    /// Spectrum.xaml の相互作用ロジック
    /// </summary>
    public partial class Spectrum : UserControl
    {
        private const double minDBValue = -100;
        private const double maxDBValue = 0;
        private const double dbScale = (maxDBValue - minDBValue);

        private float _ym = 0;
        private float YMax
        {
            get
            {
                if (_ym <= 1) return 1;
                else return _ym;
            }
            set
            {
                _ym = value;
            }
        }

        public Spectrum()
        {
            InitializeComponent();
            Columns.CollectionChanged += Columns_CollectionChanged;
            MainThreadDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        }

        public static float GetYPosLog(Complex c, float Height)
        {
            float dbValue = 20 * (float)Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));
            float ret = (float)((dbValue - minDBValue) / dbScale) * Height;
            return ret > 0 ? ret : 0;
        }

        public ObservableCollection<Bar> Columns { get; set; } = new ObservableCollection<Bar>();

        public System.Windows.Threading.Dispatcher MainThreadDispatcher { get; set; } = null;

        private Providers.SampleAggregator prov = null;
        public Providers.SampleAggregator SampleAggreator
        {
            get { return prov; }
            set
            {
                if (prov != null) prov.FftCalculated -= Prov_FftCalculated;
                prov = value;
                prov.FftCalculated += Prov_FftCalculated;
            }
        }

        private void Prov_FftCalculated(object sender, Providers.SampleAggregator.FftEventArgs e)
        {
            MainThreadDispatcher.BeginInvoke(new Action(() =>{ UpdateRes(e.Result); }));   
        }

        private void UpdateRes(Complex[] fftResults)
        {
            float[] pows = new float[fftResults.Length / 2];
            for (int n = fftResults.Length / 2; n < fftResults.Length; n++)
            {
                pows[n - fftResults.Length / 2] = GetYPosLog(fftResults[n], 100);
            }

            AddResults(pows, true);
        }

        private void AddResults(float[] Power, bool Half)
        {
            int LoopC = 0;
            if (Half) LoopC = Power.Length / 2;
            else LoopC = Power.Length;

            for (int i = 0; LoopC > i; i++)
            {
                float Pow = 0;
                if (Half) Pow = Power[i + LoopC];
                else Pow = Power[i];

                if (i >= Columns.Count)
                {
                    Bar Bar = new Bar(Pow);
                    Bar.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    Columns.Add(Bar);
                }
                else
                {
                    Columns[i].Y = Pow;
                }

                YMax = Columns[i].Y > YMax ? Columns[i].Y : YMax;
                CalcLoc(i);
            }
        }

        public double Interval { get; set; } = 5;

        public double MaximumDuration { get; set; } = 1600;

        private void AddItems(IList Items)
        {
            for (int i = 0; Items.Count > i; i++)
            {
                Bar Bar = (Bar)Items[i];
                Parent.Children.Add(Bar.Rect);
                CalcLoc(i);
            }
        }

        private void AnimateBar(Bar Bar, double Height)
        {
            if (Height > Bar.Rect.Height || double.IsNaN(Bar.Rect.Height))
            {
                double dur = Bar.Y / YMax * MaximumDuration;

                Bar.HeightAnimator.Duration = new Duration(TimeSpan.FromMilliseconds(dur));
                Bar.HeightAnimator.From = Height;

                Bar.Rect.BeginAnimation(HeightProperty, Bar.HeightAnimator);
            }
        }

        private void CalcLoc(int Index)
        {
            Bar Bar = Columns[Index];
            double Width = (Parent.ActualWidth - (5 + (Interval * (Columns.Count - 1)))) / Columns.Count;

            if (Width > 0)
            {
                Bar.Rect.Margin = new Thickness(5 + ((Width * Index) + (Interval * Index)), 0, 0, 0);

                Bar.Rect.Width = Width;

                AnimateBar(Bar, Parent.ActualHeight * (1.0 * Bar.Y / YMax));
            }
        }

        private void ClearItems()
        {
            for (int i = 0; Parent.Children.Count > i; i++)
            {
                Rectangle rec = (Rectangle)Parent.Children[i];
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
            Rectangle rec = (Rectangle)e.AnimatedElement;
            Parent.Children.Remove(rec);
        }

        private void ReplaceItems(IList Items, int StartIndex)
        {
            double Interval = (ActualWidth - (((Bar)Items[0]).Rect.Width)) / (Columns.Count + 1);
            double l = Interval;

            for (int i = 0; Items.Count > i; i++)
            {
                Bar Bar = (Bar)Items[i];
                AnimateBar(Bar, Parent.ActualHeight * (1.0 * Bar.Y / YMax));
            }
        }

        public class Bar : DependencyObject
        {
            public static readonly DependencyProperty YProperty = DependencyProperty.Register("Y", typeof(float), typeof(Bar));

            internal System.Windows.Media.Animation.DoubleAnimation HeightAnimator = new System.Windows.Media.Animation.DoubleAnimation();

            internal Rectangle Rect = new Rectangle();

            public Bar(float Y)
            {
                this.Y = Y;
                Rect.HorizontalAlignment = HorizontalAlignment.Left;
                Rect.VerticalAlignment = VerticalAlignment.Bottom;
                Rect.Width = 40;
                Rect.Fill = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                Rect.Stroke = null;
                Rect.StrokeThickness = 0;
                HeightAnimator.To = 0;
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

            public float Y
            {
                get
                {
                    float val = (float)GetValue(YProperty);
                    return val;
                }
                set
                {
                    SetValue(YProperty, value);
                }
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainThreadDispatcher.BeginInvoke(new Action(() =>
            {
                for (int i = 0; Columns.Count > i; i++)
                    CalcLoc(i);
            }));
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (SampleAggreator != null)
                SampleAggreator.Enabled = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SampleAggreator != null)
                SampleAggreator.Enabled = true;
        }
    }
}