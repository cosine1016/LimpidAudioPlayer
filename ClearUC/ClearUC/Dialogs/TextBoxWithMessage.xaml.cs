using System.Windows;

namespace ClearUC.Dialogs
{
    /// <summary>
    /// TextBoxWithMessage.xaml の相互作用ロジック
    /// </summary>
    public partial class TextBoxWithMessage : Window
    {
        public class ResultData
        {
            public ResultData(Dialog.ClickedButton ClickedButton, int Number)
            {
                this.ClickedButton = ClickedButton;
                this.Number = Number;
            }

            public Dialog.ClickedButton ClickedButton = Dialog.ClickedButton.OK;

            public int Number { get; set; } = -1;
        }

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