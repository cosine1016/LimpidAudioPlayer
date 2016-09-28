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
    /// FastForward.xaml の相互作用ロジック
    /// </summary>
    public partial class FastForwardButton : ButtonBase
    {
        public event EventHandler FastForward;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        bool mode = false;

        public FastForwardButton()
        {
            InitializeComponent();

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            AppliedPropertyChanges += FastForward_AppliedPropertyChanges;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            mode = true;
            FastForward?.Invoke(this, new EventArgs());
        }

        private void FastForward_AppliedPropertyChanges(object sender, EventArgs e)
        {
            path.Fill = ButtonBrush;
            path.Stroke = ButtonStroke;
            path.StrokeThickness = StrokeThickness;
        }

        private void rect_MouseEnter(object sender, MouseEventArgs e)
        {
            Animate(path.Fill, MouseEnterBrush, path);
            timer.Stop();
        }

        private void rect_MouseLeave(object sender, MouseEventArgs e)
        {
            Animate(path.Fill, ButtonBrush, path);
            timer.Stop();
        }

        private void rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Animate(path.Fill, MouseClickBrush, path);
            timer.Start();
        }

        private void rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Animate(path.Fill, MouseEnterBrush, path);
            timer.Stop();
        }

        protected override void OnMouseClicked(MouseButtonEventArgs e)
        {
            if (mode == false)
                base.OnMouseClicked(e);

            mode = false;
        }
    }
}
