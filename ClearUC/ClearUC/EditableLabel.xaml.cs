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
using System.ComponentModel;

namespace ClearUC
{
    /// <summary>
    /// EditableLabel.xaml の相互作用ロジック
    /// </summary>
    public partial class EditableLabel : TextBox
    {
        public class EditingStatusChangedEventArgs : EventArgs
        {
            public EditingStatusChangedEventArgs() { }
            public EditingStatusChangedEventArgs(bool Editable)
            {
                this.Editable = Editable;
            }

            public bool Handled { get; set; } = false;

            public bool Editable { get; } = false;
        }

        public event EventHandler<EditingStatusChangedEventArgs> EditingStatusChanged;

        public EditableLabel()
        {
            InitializeComponent();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IsReadOnly
        {
            get { return base.IsReadOnly; }
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditingStatusChangedEventArgs arg = new EditingStatusChangedEventArgs(true);
            EditingStatusChanged?.Invoke(this, arg);

            if (!arg.Handled)
            {
                base.IsReadOnly = false;
                Select(Text.Length, 0);
                Focus();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EditingStatusChangedEventArgs arg = new EditingStatusChangedEventArgs(false);
            EditingStatusChanged?.Invoke(this, arg);

            if (!arg.Handled)
            {
                base.IsReadOnly = true;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            EditingStatusChangedEventArgs arg = new EditingStatusChangedEventArgs(false);
            EditingStatusChanged?.Invoke(this, arg);

            if (!arg.Handled)
            {
                if (e.Key == Key.Enter && !AcceptsReturn)
                {
                    Keyboard.ClearFocus();
                    base.IsReadOnly = true;
                }
            }
        }
    }
}
