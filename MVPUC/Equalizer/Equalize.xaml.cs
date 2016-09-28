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

namespace MVPUC.Equalizer
{
    /// <summary>
    /// Equalize.xaml の相互作用ロジック
    /// </summary>
    public partial class Equalize : UserControl
    {
        public event EventHandler<GainChangedEventArgs> GainChanged;

        public class GainChangedEventArgs : EventArgs
        {
            public GainChangedEventArgs(int ChangedIndex, float Gain)
            {
                this.ChangedIndex = ChangedIndex;
                this.Gain = Gain;
            }

            public int ChangedIndex { get; private set; } = -1;

            public float Gain { get; set; } = 0;
        }

        public Equalize()
        {
            InitializeComponent();
            Sliders.CollectionChanged += Sliders_CollectionChanged;
            Maximum = max;
            Minimum = min;
        }

        private void Sliders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            parent.Children.Clear();
            SliderControls.Clear();
            
            for (int i = 0; Sliders.Count > i; i++)
            {
                Slider s = new Slider();
                s.Visibility = Visibility.Hidden;
                s.Value = Sliders[i];
                s.Minimum = Minimum;
                s.Maximum = Maximum;
                s.ValueChanged += Slider_ValueChanged;
                parent.Children.Add(s);
                SliderControls.Add(s);
            }

            CalcMargin();
        }

        private void CalcMargin()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                double Interval = (parent.ActualWidth - (18 * Sliders.Count)) / (Sliders.Count + 1);
                if (Interval < 0) Interval = 0;
                double l = Interval;
                for (int i = 0; SliderControls.Count > i; i++)
                {
                    SliderControls[i].Orientation = Orientation.Vertical;
                    SliderControls[i].VerticalAlignment = VerticalAlignment.Stretch;
                    SliderControls[i].HorizontalAlignment = HorizontalAlignment.Left;
                    SliderControls[i].Width = 18;
                    SliderControls[i].Margin = new Thickness(l, 0, 0, 0);
                    SliderControls[i].Visibility = Visibility.Visible;
                    l += SliderControls[i].Width + Interval;
                }
            })
            , System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GainChanged?.Invoke(this, new GainChangedEventArgs(parent.Children.IndexOf((UIElement)sender), (float)e.NewValue));
        }

        float max = 30;
        float min = -30;

        public float Maximum
        {
            get { return max; }
            set
            {
                max = value;
                for (int i = 0; parent.Children.Count > i; i++)
                {
                    Slider s = parent.Children[i] as Slider;
                    s.Maximum = max;
                }
            }
        }

        public float Minimum
        {
            get { return min; }
            set
            {
                min = value;
                for (int i = 0; parent.Children.Count > i; i++)
                {
                    Slider s = parent.Children[i] as Slider;
                    s.Minimum = min;
                }
            }
        }

        private List<Slider> SliderControls = new List<Slider>();

        public ClearUC.Collections.ObservableRangeCollection<float> Sliders { get; set; } = new ClearUC.Collections.ObservableRangeCollection<float>();

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalcMargin();
        }
    }
}
