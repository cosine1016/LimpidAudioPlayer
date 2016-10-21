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

namespace ClearUC
{
    /// <summary>
    /// ColorPicker.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorSlider : UserControl
    {
        public event EventHandler HueChanged, SaturationChanged, LightnessChanged;

        public ColorSlider()
        {
            InitializeComponent();
            UpdateFill();
            UpdatePos();
        }

        public LinearGradientBrush BrightLGB =
            new LinearGradientBrush(new GradientStopCollection(new GradientStop[] 
            {
                new GradientStop(Color.FromArgb(255, 0, 0, 0), 0),
                new GradientStop(Color.FromArgb(255, 255, 255, 255), 1)
            }), 0.0);

        public enum GradientMode { Lightness, Saturation, Hue }

        private byte sat = 0;
        private int hue = 0;


        public static readonly DependencyProperty LightnessProperty = DependencyProperty.Register("Lightness", typeof(byte), typeof(ColorSlider));

        public byte Lightness
        {
            get { return (byte)GetValue(LightnessProperty); }
            set
            {
                SetValue(LightnessProperty, value);
                UpdateFill();
                UpdatePos();
                LightnessChanged?.Invoke(this, new EventArgs());
            }
        }


        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(int), typeof(ColorSlider));

        public int Hue
        {
            get
            {
                int val = (int)GetValue(HueProperty);

                return val;
            }
            set
            {
                SetValue(HueProperty, value);
                UpdateFill();
                UpdatePos();
                HueChanged?.Invoke(this, new EventArgs());
            }
        }


        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(byte), typeof(ColorSlider));

        public byte Saturation
        {
            get
            {
                byte val = (byte)GetValue(SaturationProperty);
                return val;
            }
            set
            {
                SetValue(SaturationProperty, value);
                UpdateFill();
                UpdatePos();
                SaturationChanged?.Invoke(this, new EventArgs());
            }
        }


        public static readonly DependencyProperty FillGradientProperty = DependencyProperty.Register("FillGradient", typeof(GradientMode), typeof(ColorSlider));

        public GradientMode FillGradient
        {
            get
            {
                GradientMode val = (GradientMode)GetValue(FillGradientProperty);
                return val;
            }
            set
            {
                SetValue(FillGradientProperty, value);
                UpdateFill();
                UpdatePos();
            }
        }

        private void UpdatePos()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                double pw = (ActualWidth - Thumb.Width) / Max;
                Thumb.Margin = new Thickness(pw * GetValue, 0, 0, 0);
            }));
        }

        private void UpdateFill()
        {
            switch (FillGradient)
            {
                case GradientMode.Lightness:
                    Fill.Fill = BrightLGB;
                    Fill.Stroke = ReverseBrush(BrightLGB);
                    break;
                case GradientMode.Hue:
                    LinearGradientBrush lgbh = GenerateHue(Lightness);
                    Fill.Fill = lgbh;
                    Fill.Stroke = ReverseBrush(lgbh);
                    break;
                case GradientMode.Saturation:
                    LinearGradientBrush lgbs = GenerateSaturation(Hue, Lightness);
                    Fill.Fill = lgbs;
                    Fill.Stroke = ReverseBrush(lgbs);
                    break;
            }
        }

        private int Max
        {
            get
            {
                switch (FillGradient)
                {
                    case GradientMode.Lightness:
                        return 255;
                    case GradientMode.Hue:
                        return 360;
                    case GradientMode.Saturation:
                        return 255;
                    default:
                        return 0;
                }
            }
        }

        private new int GetValue
        {
            get
            {
                switch (FillGradient)
                {
                    case GradientMode.Lightness:
                        return Lightness;
                    case GradientMode.Hue:
                        return Hue;
                    case GradientMode.Saturation:
                        return Saturation;
                    default:
                        return 0;
                }
            }
        }

        private LinearGradientBrush GenerateSaturation(int Hue, byte Lightness)
        {
            List<GradientStop> stops = new List<GradientStop>();

            stops.Add(new GradientStop(Utils.HslColor.ToRgb(new Utils.HslColor(Hue, 0, Lightness / 255f)), 0));
            stops.Add(new GradientStop(Utils.HslColor.ToRgb(new Utils.HslColor(Hue, 1, Lightness / 255f)), 1));

            LinearGradientBrush lgb = new LinearGradientBrush(new GradientStopCollection(stops));

            return lgb;
        }

        private LinearGradientBrush GenerateHue(byte Lightness)
        {
            const int StopsCount = 11;

            List<GradientStop> stops = new List<GradientStop>();

            int PerHue = 360 / (StopsCount - 1);
            double offset = 0.0;
            for(int i = 0;StopsCount > i; i++)
            {
                stops.Add(new GradientStop(Utils.HslColor.ToRgb(new Utils.HslColor(PerHue * i, 0.5f, Lightness / 255f)), offset));
                offset += 0.1;
            }

            LinearGradientBrush lgb = new LinearGradientBrush(new GradientStopCollection(stops));

            return lgb;
        }

        private LinearGradientBrush ReverseBrush(LinearGradientBrush BaseBrush)
        {
            LinearGradientBrush Brush = BaseBrush.Clone();
            for (int i = 0; Brush.GradientStops.Count > i; i++)
                Brush.GradientStops[i].Offset = Math.Abs(1.0 - Brush.GradientStops[i].Offset);

            return Brush;
        }

        private bool mdFlag = false;

        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            Thumb.Fill = GetEnterBrush();
        }

        private void Thumb_MouseLeave(object sender, MouseEventArgs e)
        {
            Thumb.Fill = GetNormalBrush();
        }

        private void Thumb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Thumb.Fill = GetClickBrush();
                mdFlag = true;
            }
        }

        private void Thumb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Thumb.Fill = GetEnterBrush();
            mdFlag = false;
        }

        private bool flag = false;
        private SolidColorBrush NormalSolc;

        private SolidColorBrush GetEnterBrush()
        {
            return GetBrush(50);
        }

        private SolidColorBrush GetClickBrush()
        {
            return GetBrush(100);
        }

        private SolidColorBrush GetBrush(int Param)
        {
            if(flag == false)
            {
                NormalSolc = (SolidColorBrush)Thumb.Fill;
                flag = true;
            }

            SolidColorBrush solc = GetNormalBrush();
            byte r, g, b, a = solc.Color.A;
            if (solc.Color.R + Param <= 255)
                r = (byte)(solc.Color.R + Param);
            else
                r = (byte)(solc.Color.R - Param);

            if (solc.Color.G + Param <= 255)
                g = (byte)(solc.Color.G + Param);
            else
                g = (byte)(solc.Color.G - Param);

            if (solc.Color.B + Param <= 255)
                b = (byte)(solc.Color.B + Param);
            else
                b = (byte)(solc.Color.B - Param);

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        private SolidColorBrush GetNormalBrush()
        {
            if (flag)
                return NormalSolc;
            else
                return (SolidColorBrush)Thumb.Fill;
        }

        private void Thumb_MouseMove(object sender, MouseEventArgs e)
        {
            if(mdFlag)
                CalcValueFromPos(e.GetPosition(this));
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            mdFlag = false;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mdFlag = false;
        }

        private void Fill_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Thumb.Fill = GetClickBrush();
                mdFlag = true;
                CalcValueFromPos(e.GetPosition(this));
            }
        }

        private void CalcValueFromPos(Point Position)
        {
            double pw = (ActualWidth - Thumb.Width) / Max;
            int Val = (int)(Position.X / pw);

            switch (FillGradient)
            {
                case GradientMode.Lightness:
                    Lightness = Val > Max ? (byte)Max : (byte)Val;
                    break;
                case GradientMode.Hue:
                    Hue = Val > Max ? Max : Val;
                    break;
                case GradientMode.Saturation:
                    Saturation = Val > Max ? (byte)Max : (byte)Val;
                    break;
            }
        }
    }
}
