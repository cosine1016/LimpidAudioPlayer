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
    /// VolumeBar.xaml の相互作用ロジック
    /// </summary>
    public partial class VolumeBar : UserControl
    {
        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ValueChanged;

        public VolumeBar()
        {
            InitializeComponent();
        }

        private void CustomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ValueChanged?.Invoke(this, e);
        }

        public double Value
        {
            get { return CustomSlider.Value; }
            set { CustomSlider.Value = value; }
        }
    }
}