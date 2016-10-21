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
            NumN.ValueChanged += TotalDiscsN_ValueChanged;
        }

        private void TotalDiscsN_ValueChanged(object sender, System.EventArgs e)
        {
            ValueChanged?.Invoke(sender, e);
        }

        public event System.EventHandler ValueChanged;

        private void NumUp_Click(object sender, RoutedEventArgs e)
        {
            if (NumN.Maximum >= NumN.Value + 1) NumN.Value += 1;
        }

        private void NumDown_Click(object sender, RoutedEventArgs e)
        {
            if (NumN.Value - 1 >= NumN.Minimum) NumN.Value -= 1;
        }

        public new bool IsEnabled
        {
            get { return NumUp.IsEnabled; }
            set
            {
                NumUp.IsEnabled = value;
                NumDown.IsEnabled = value;
                NumN.IsEnabled = value;
            }
        }

        public int Minimum
        {
            get { return NumN.Minimum; }
            set { NumN.Minimum = value; }
        }

        public int Maximum
        {
            get { return NumN.Maximum; }
            set { NumN.Maximum = value; }
        }

        public int Value
        {
            get { return NumN.Value; }
            set { NumN.Value = value; }
        }
    }
}