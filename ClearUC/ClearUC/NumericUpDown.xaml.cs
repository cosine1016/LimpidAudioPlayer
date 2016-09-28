using System.Windows;
using System.Windows.Controls;

namespace ClearUC
{
    /// <summary>
    /// NumericUpDown.xaml の相互作用ロジック
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            InitializeComponent();
            TotalDiscsN.ValueChanged += TotalDiscsN_ValueChanged;
        }

        private void TotalDiscsN_ValueChanged(object sender, System.EventArgs e)
        {
            ValueChanged?.Invoke(sender, e);
        }

        public event System.EventHandler ValueChanged;

        private void NumUp_Click(object sender, RoutedEventArgs e)
        {
            if (TotalDiscsN.Maximum >= TotalDiscsN.Value + 1) TotalDiscsN.Value += 1;
        }

        private void NumDown_Click(object sender, RoutedEventArgs e)
        {
            if (TotalDiscsN.Value - 1 >= TotalDiscsN.Minimum) TotalDiscsN.Value -= 1;
        }

        public new bool IsEnabled
        {
            get { return NumUp.IsEnabled; }
            set
            {
                NumUp.IsEnabled = value;
                NumDown.IsEnabled = value;
                TotalDiscsN.IsEnabled = value;
            }
        }

        public int Minimum
        {
            get { return TotalDiscsN.Minimum; }
            set { TotalDiscsN.Minimum = value; }
        }

        public int Maximum
        {
            get { return TotalDiscsN.Maximum; }
            set { TotalDiscsN.Maximum= value; }
        }

        public int Value
        {
            get { return TotalDiscsN.Value; }
            set { TotalDiscsN.Value = value; }
        }
    }
}