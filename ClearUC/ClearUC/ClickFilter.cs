using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ClearUC
{
    public class ClickFilter
    {
        public event EventHandler<System.Windows.Input.MouseButtonEventArgs> MouseClick;
        public event EventHandler EnterColor;
        public event EventHandler DownColor;
        public event EventHandler DefaultColor;
        
        public System.Windows.Shapes.Rectangle Filter { get; set; } = new System.Windows.Shapes.Rectangle();

        public ClickFilter()
        {
            Filter.Fill =
                new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(1, 0, 0, 0));
            Filter.MouseEnter += Rectangle_MouseEnter;
            Filter.MouseLeave += Rectangle_MouseLeave;
            Filter.MouseDown += Rectangle_MouseDown;
            Filter.MouseUp += Rectangle_MouseUp;
        }

        private void Rectangle_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (f) MouseClick?.Invoke(this, e);
            f = false;
            EnterColor?.Invoke(this, new EventArgs());
        }

        bool f = false;
        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DownColor?.Invoke(this, new EventArgs());
            f = true;
        }

        private void Rectangle_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DefaultColor?.Invoke(this, new EventArgs());
            f = false;
        }

        private void Rectangle_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            EnterColor?.Invoke(this, new EventArgs());
            f = false;
        }
    }
}
