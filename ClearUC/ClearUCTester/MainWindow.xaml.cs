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
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClearUC;
using ClearUC.ListViewItems;

namespace ClearUCTester
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            int N = 13, I = 1, S = 0;
            while(true)
            {
                S = S + I;
                if (I == N)
                    break;
                else
                    I++;
            }

            Console.WriteLine(S);
        }

        private void editableLabel_EditingStatusChanged(object sender, EditableLabel.EditingStatusChangedEventArgs e)
        {
        }
    }
}
