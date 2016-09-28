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
    public partial class ColorPicker : UserControl
    {
        public event EventHandler SelectedColorChanged;

        public ColorPicker()
        {
            InitializeComponent();
        }

        private bool ips = false;
        public bool IsPanelShown
        {
            get { return ips; }
            set
            {
                ips = value;
                if (value == true)
                {
                    PickColorPanel.Visibility = Visibility.Visible;
                    DropDownB.Content = 5;
                }
                else
                {
                    PickColorPanel.Visibility = Visibility.Hidden;
                    DropDownB.Content = 6;
                }
            }
        }

        public Color SelectedColor
        {
            get { return PickColorPanel.GetColor(); }
            set { PickColorPanel.SetColor(value); }
        }

        private void PickColorPanel_ColorChanged(object sender, EventArgs e)
        {
            Fill.Fill = new SolidColorBrush(PickColorPanel.GetColor());
            SelectedColorChanged?.Invoke(this, e);
        }

        private void DropDownB_Click(object sender, RoutedEventArgs e)
        {
            IsPanelShown = !IsPanelShown;
        }
    }
}
