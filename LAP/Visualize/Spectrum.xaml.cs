using NAudio.Dsp;
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

namespace LAP.Visualize
{
    /// <summary>
    /// Spectrum.xaml の相互作用ロジック
    /// </summary>
    public partial class Spectrum : MVPUC.Visualize.Spectrum
    {
        private const double minDBValue = -100;
        private const double maxDBValue = 0;
        private const double dbScale = (maxDBValue - minDBValue);

        public Spectrum()
        {
            InitializeComponent();
        }

        public NWrapper.SampleAggregator SampleAggreator { get; set; } = null;

        public void AssociateEvent()
        {
            SampleAggreator.FftCalculated += SampleAggreator_FftCalculated;
        }

        public System.Windows.Threading.Dispatcher MainThreadDispatcher { get; set; } = null;

        private void SampleAggreator_FftCalculated(object sender, NWrapper.FftEventArgs e)
        {
            Update(e.Result);
        }

        private void Update(Complex[] fftResults)
        {
            float[] pows = new float[fftResults.Length / 2];
            for (int n = fftResults.Length / 2; n < fftResults.Length; n++)
            {
                pows[n - fftResults.Length / 2] = GetYPosLog(fftResults[n], 100);
            }

            AddResults(pows, false);
        }

        private void AddResults(float[] Power, bool Half)
        {
            MainThreadDispatcher.BeginInvoke(new Action(() =>
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
                        Bar Bar = new Bar(i, Pow);
                        Bar.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        Columns.Add(Bar);
                    }
                    else
                    {
                        Columns[i].Y = Pow;
                    }
                }
            }));
        }

        public void Start()
        {
            SampleAggreator.Enabled = true;
            SampleAggreator.PerformFFT = true;
        }

        public void Pause()
        {
            SampleAggreator.PerformFFT = false;
        }

        public static float GetYPosLog(Complex c, float Height)
        {
            float dbValue = 20 * (float)Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));
            float ret = (float)((dbValue - minDBValue) / dbScale) * Height;
            return ret > 0 ? ret : 0;
        }

        public void Dispose()
        {
            if (SampleAggreator != null) SampleAggreator.FftCalculated -= SampleAggreator_FftCalculated;
            Columns.Clear();
        }
    }
}