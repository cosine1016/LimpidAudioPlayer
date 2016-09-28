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
    /// PickColorPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class PickColorPanel : UserControl
    {
        public PickColorPanel()
        {
            InitializeComponent();
            UpdateFill();
        }

        public event EventHandler ColorChanged;

        private void UpdateFill()
        {
            if (H == null || S == null || L == null) return;
            S.Hue = H.Hue;
            S.Lightness = L.Lightness;
        }

        public byte Lightness
        {
            get { return L.Lightness; }
            set
            {
                L.Lightness = value;
                UpdateFill();
            }
        }

        public int Hue
        {
            get { return H.Hue; }
            set
            {
                H.Hue = value;
                UpdateFill();
            }
        }

        public byte Saturation
        {
            get { return S.Saturation; }
            set
            {
                S.Saturation = value;
                UpdateFill();
            }
        }

        public void SetColor(Color Color)
        {
            Utils.HslColor hsl = Utils.HslColor.FromRgb(Color);
            Hue = (int)hsl.H;
            Saturation = (byte)(hsl.S * 255);
            Lightness = (byte)(hsl.L * 255);
        }

        public Color GetColor()
        {
            return Utils.HslColor.ToRgb(new Utils.HslColor(Hue, Saturation / 255f, Lightness / 255f));
        }

        private void L_LightnessChanged(object sender, EventArgs e)
        {
            ColorChanged?.Invoke(this, e);
            UpdateFill();
        }

        private void H_HueChanged(object sender, EventArgs e)
        {
            ColorChanged?.Invoke(this, e);
            UpdateFill();
        }

        private void S_SaturationChanged(object sender, EventArgs e)
        {
            ColorChanged?.Invoke(this, e);
            UpdateFill();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Lightness = 255 / 2;
            Hue = 360 / 2;
            Saturation = 255 / 2;
        }
    }
}
