using System.Windows;

namespace ClearUC.Dialogs
{
    /// <summary>
    /// MessageBox.xaml の相互作用ロジック
    /// </summary>
    public partial class MessageBox : Window
    {
        public MessageBox()
        {
            InitializeComponent();

            switch (Buttons)
            {
                case Dialog.Buttons.OKCancel:
                    Result = Dialog.ClickedButton.Cancel;
                    break;

                case Dialog.Buttons.AcceptDecline:
                    Result = Dialog.ClickedButton.Decline;
                    break;

                case Dialog.Buttons.YesNo:
                    Result = Dialog.ClickedButton.No;
                    break;

                case Dialog.Buttons.Custom:
                    Result = Dialog.ClickedButton.CustomRight;
                    break;
            }
        }

        public Dialog.Buttons Buttons { get; set; } = Dialog.Buttons.OKCancel;

        public Dialog.ClickedButton Result { get; set; }

        private void LB_Click(object sender, RoutedEventArgs e)
        {
            switch (Buttons)
            {
                case Dialog.Buttons.OKCancel:
                    Result = Dialog.ClickedButton.OK;
                    break;

                case Dialog.Buttons.AcceptDecline:
                    Result = Dialog.ClickedButton.Accept;
                    break;

                case Dialog.Buttons.YesNo:
                    Result = Dialog.ClickedButton.Yes;
                    break;

                case Dialog.Buttons.Custom:
                    Result = Dialog.ClickedButton.CustomLeft;
                    break;
            }

            Close();
        }

        private void RB_Click(object sender, RoutedEventArgs e)
        {
            switch (Buttons)
            {
                case Dialog.Buttons.OKCancel:
                    Result = Dialog.ClickedButton.Cancel;
                    break;

                case Dialog.Buttons.AcceptDecline:
                    Result = Dialog.ClickedButton.Decline;
                    break;

                case Dialog.Buttons.YesNo:
                    Result = Dialog.ClickedButton.No;
                    break;

                case Dialog.Buttons.Custom:
                    Result = Dialog.ClickedButton.CustomRight;
                    break;
            }

            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}