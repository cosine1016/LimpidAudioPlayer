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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MVPUC.Buttons
{
    /// <summary>
    /// ButtonBase.xaml の相互作用ロジック
    /// </summary>
    public partial class ButtonBase : UserControl
    {
        public event EventHandler<MouseButtonEventArgs> MouseClicked;

        protected virtual void OnMouseClicked(MouseButtonEventArgs e)
        {
            MouseClicked?.Invoke(this, e);
        }

        public event EventHandler AppliedPropertyChanges;

        protected virtual void OnAppliedPropertyChanges(EventArgs e)
        {
            AppliedPropertyChanges?.Invoke(this, e);
        }

        public ButtonBase()
        {
            MouseDown += ButtonBase_MouseDown;
            MouseUp += ButtonBase_MouseUp;
            MouseLeave += ButtonBase_MouseLeave;
        }

        private bool mf = false;

        private void ButtonBase_MouseLeave(object sender, MouseEventArgs e)
        {
            mf = false;
        }

        private void ButtonBase_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mf == true)
            {
                OnMouseClicked(e);
            }
        }

        private void ButtonBase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mf = true;
        }

        public Brush ButtonBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));

        public Brush MouseEnterBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

        public Brush MouseClickBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));

        public Brush ButtonStroke { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));

        public double StrokeThickness { get; set; } = 1;

        public double AnimationDuration { get; set; } = 50;

        public object Data { get; set; } = null;

        public Type DataType { get; set; }

        public void ApplyPropertyChanges()
        {
            OnAppliedPropertyChanges(new EventArgs());
        }

        public void Animate(Brush Before, Brush After, double Duration, Shape Shape)
        {
            if (Before == null | After == null) return;

            ClearUC.Utils.AnimationHelper.Brush ba = new ClearUC.Utils.AnimationHelper.Brush();
            ba.Animate(Before, After, Duration, Shape, new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty));
        }

        public void Animate(Brush Before, Brush After, Shape Shape)
        {
            Animate(Before, After, AnimationDuration, Shape);
        }
    }
}