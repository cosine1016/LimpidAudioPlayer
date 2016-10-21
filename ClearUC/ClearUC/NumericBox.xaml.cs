using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClearUC
{
    /// <summary>
    /// NumericBox.xaml の相互作用ロジック
    /// </summary>
    public partial class NumericBox : TextBox
    {
        public event EventHandler ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null) ValueChanged(this, e);
        }

        public NumericBox()
        {
            InitializeComponent();
            Value = Minimum;
            InputMethod.SetIsInputMethodSuspended(this, true);
            DataObject.AddPastingHandler(this, OnPaste);
            ValueChanged += NumericBox_ValueChanged;
        }

        private void NumericBox_ValueChanged(object sender, EventArgs e)
        {
            Text = Value.ToString();
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText) return;
            string text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            if (!reg.IsMatch(text))
            {
                e.CancelCommand();
                e.Handled = true;
            }
            else
            {
                bool s = false;
                int ret = TryParse(text, out s);
                if (ret > Maximum) ret = Maximum;
                if (ret < Minimum) ret = Minimum;

                Value = ret;
            }
        }

        public bool EnableKeyUp { get; set; } = true;

        public bool EnableKeyDown { get; set; } = true;

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }


        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(NumericUpDown), new PropertyMetadata(100));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                OnValueChanged(new EventArgs());
            }
        }

        private Regex reg = new Regex("^[0-9\\-]*$");

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool s = false;
            int ret = TryParse(Text, out s);

            switch (e.Key)
            {
                case Key.Up:
                    if (s && EnableKeyUp) ret += 1;
                    break;

                case Key.Down:
                    if (s && EnableKeyDown) ret -= 1;
                    break;

                default:
                    e.Handled = true;
                    break;
            }

            if (ret > Maximum) ret = Maximum;
            if (ret < Minimum) ret = Minimum;

            Value = ret;
        }

        private int TryParse(string s, out bool Success)
        {
            if (Text == "-")
            {
                Success = true;
                return -1;
            }

            int ret = 0;
            Success = int.TryParse(s, out ret);
            if (Success == false) ret = Minimum;
            return ret;
        }
    }
}