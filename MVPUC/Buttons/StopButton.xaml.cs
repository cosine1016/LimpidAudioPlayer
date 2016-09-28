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

namespace MVPUC.Buttons
{
    /// <summary>
    /// StopButton.xaml の相互作用ロジック
    /// </summary>
    public partial class StopButton : ButtonBase
    {
        public StopButton()
        {
            InitializeComponent();

            AppliedPropertyChanges += StopButton_AppliedPropertyChanges;
        }

        private void StopButton_AppliedPropertyChanges(object sender, EventArgs e)
        {
            stop.Fill = ButtonBrush;
            stop.Stroke = ButtonStroke;
            stop.StrokeThickness = StrokeThickness;
        }

        private void stop_MouseEnter(object sender, MouseEventArgs e)
        {
            Animate(stop.Fill, MouseEnterBrush, stop);
        }

        private void stop_MouseLeave(object sender, MouseEventArgs e)
        {
            Animate(stop.Fill, ButtonBrush, stop);
        }

        private void stop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Animate(stop.Fill, MouseClickBrush, stop);
        }

        private void stop_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Animate(stop.Fill, MouseEnterBrush, stop);
        }
    }
}
