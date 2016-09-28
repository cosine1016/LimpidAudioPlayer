using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClearUC.ListViewItems;

namespace LAP.UserControls.ListViewItems
{
    /// <summary>
    /// TextBoxWithButton.xaml の相互作用ロジック
    /// </summary>
    public partial class TextBoxWithButton : ListItem
    {
        public event EventHandler<RoutedEventArgs> ButtonClick;
        protected virtual void OnButtonClick(RoutedEventArgs e)
        {
            if (ButtonClick != null) ButtonClick(this, e);
        }

        public TextBoxWithButton()
        {
            InitializeComponent();
        }

        public string TextBoxText
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public object ButtonContent
        {
            get { return button.Content; }
            set { button.Content = value; }
        }

        public double TextBoxOpacity
        {
            get { return textBox.Opacity; }
            set { textBox.Opacity = value; }
        }

        public double ButtonOpacity
        {
            get { return button.Opacity; }
            set { button.Opacity = value; }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OnButtonClick(e);
        }
    }
}
