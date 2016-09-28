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

namespace LAP.UserControls
{
    /// <summary>
    /// PSEOption.xaml の相互作用ロジック
    /// </summary>
    public partial class PSEOption : UserControl
    {
        public PSEOption()
        {
            InitializeComponent();
            checkBox.IsChecked = Utils.Config.Setting.Boolean.PSE;
        }

        public bool PSEEnable = false;

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            PSEEnable = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PSEEnable = false;
        }
    }
}
