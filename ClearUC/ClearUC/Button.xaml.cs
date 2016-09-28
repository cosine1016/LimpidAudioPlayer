using System;
using System.Windows.Input;
using System.Windows.Media;

namespace ClearUC
{
    /// <summary>
    /// Button.xaml の相互作用ロジック
    /// </summary>
    public partial class Button : System.Windows.Controls.Button
    {
        public event EventHandler<MouseButtonEventArgs> RightClick;

        public Button()
        {
            InitializeComponent();
        }

        public Brush MouseOver
        {
            get { return (Brush)Resources["MouseOver"]; }
            set { Resources["MouseOver"] = value; }
        }

        public Brush MouseOverBorder
        {
            get { return (Brush)Resources["MouseOverBorder"]; }
            set { Resources["MouseOverBorder"] = value; }
        }

        private bool downf = false;

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            downf = false;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            downf = false;
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (downf == true)
            {
                if (RightClick != null) RightClick(sender, e);
            }
            downf = false;
        }

        private void Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            downf = true;
        }
    }
}