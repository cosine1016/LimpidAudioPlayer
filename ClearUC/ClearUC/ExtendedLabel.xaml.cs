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
    /// ExtendedLabel.xaml の相互作用ロジック
    /// </summary>
    public partial class ExtendedLabel : Label
    {
        public event EventHandler MouseClick;

        private Brush curFore = null;

        public ExtendedLabel()
        {
            InitializeComponent();
            Foreground = base.Foreground;
        }

        public Brush Entered { get; set; } = new SolidColorBrush(new Color { A = 255, B = 200, G = 200, R = 200 });

        public Brush Clicked { get; set; } = new SolidColorBrush(new Color { A = 255, B = 150, G = 150, R = 150 });

        public new Brush Foreground
        {
            get { return base.Foreground; }
            set
            {
                curFore = value.Clone();
                base.Foreground = value;
            }
        }

        bool mousel = false;
        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            base.Foreground = Entered;
            mousel = false;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            base.Foreground = curFore;
            mousel = false;
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.Foreground = Clicked;
            mousel = true;
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mousel)
                MouseClick?.Invoke(this, new EventArgs());

            base.Foreground = Entered;
            mousel = false;
        }
    }
}
