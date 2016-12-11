using System.Windows;

namespace ClearUC.Dialogs
{
    /// <summary>
    /// TextBoxWithMessage.xaml の相互作用ロジック
    /// </summary>
    internal partial class TextBoxWithMessage : Window
    {
        public TextBoxWithMessage()
        {
            InitializeComponent();
            Result = new ResultData(Dialog.ClickedButton.Cancel, -1);
        }

        public ResultData Result { get; set; }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Result = new ResultData(Dialog.ClickedButton.OK, NumUpDown.Value);
            Close();
        }
    }
}