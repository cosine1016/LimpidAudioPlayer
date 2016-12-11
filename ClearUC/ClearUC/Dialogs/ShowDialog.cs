namespace ClearUC.Dialogs
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

    public class Dialog
    {
        public class DialogEventArgs : System.EventArgs
        {
            public DialogEventArgs(DialogState State)
            {
                this.State = State;
            }

            public enum DialogState { Show, Close }

            public DialogState State { get; set; }
        }

        public static event System.EventHandler<DialogEventArgs> DialogStateChanged;

        public static string OKOnlyRightButtonText { get; set; } = "OK";

        public static string OKCancelLeftButtonText { get; set; } = "OK";

        public static string OKCancelRightButtonText { get; set; } = "Cancel";

        public static string AcceptDeclineLeftButtonText { get; set; } = "Accept";

        public static string AcceptDeclineRightButtonText { get; set; } = "Decline";

        public static string YesNoLeftButtonText { get; set; } = "Yes";

        public static string YesNoRightButtonText { get; set; } = "No";

        public static string CustomLeftButtonText { get; set; } = "OK";

        public static string CustomRightButtonText { get; set; } = "Cancel";

        public enum Buttons
        {
            OKOnly, OKCancel, AcceptDecline, YesNo, Custom
        }

        public enum ClickedButton
        {
            OK, Cancel, Accept, Decline, Yes, No, CustomLeft, CustomRight, Unknown
        }

        public static System.Windows.Media.ImageSource DialogIcon { get; set; }

        public static ResultData ShowMessageBoxWithNumeric(string Title, string Message, int DefaultValue, bool ShowInTaskbar = true)
        {
            TextBoxWithMessage Box = new TextBoxWithMessage();
            Box.TL.Content = Title;
            Box.ML.Text = Message;
            Box.NumUpDown.Value = DefaultValue;

            Box.Icon = DialogIcon;

            Box.ShowInTaskbar = ShowInTaskbar;

            DialogStateChanged?.Invoke(Box, new DialogEventArgs(DialogEventArgs.DialogState.Show));

            Box.ShowDialog();

            DialogStateChanged?.Invoke(Box, new DialogEventArgs(DialogEventArgs.DialogState.Close));

            return Box.Result;
        }

        public static ClickedButton ShowMessageBox(Buttons Button, string Title, string Message, bool ShowInTaskbar = true)
        {
            MessageBox msg = new MessageBox();
            switch (Button)
            {
                case Buttons.OKOnly:
                    msg.LB.Visibility = System.Windows.Visibility.Hidden;
                    msg.RB.Content = OKOnlyRightButtonText;
                    break;

                case Buttons.OKCancel:
                    msg.LB.Content = OKCancelLeftButtonText;
                    msg.RB.Content = OKCancelRightButtonText;
                    break;

                case Buttons.AcceptDecline:
                    msg.LB.Content = AcceptDeclineLeftButtonText;
                    msg.RB.Content = AcceptDeclineRightButtonText;
                    break;

                case Buttons.YesNo:
                    msg.LB.Content = YesNoLeftButtonText;
                    msg.RB.Content = YesNoRightButtonText;
                    break;

                case Buttons.Custom:
                    msg.LB.Content = CustomLeftButtonText;
                    msg.RB.Content = CustomRightButtonText;
                    break;
            }
            msg.TL.Content = Title;
            msg.ML.Text = Message;

            msg.Icon = DialogIcon;

            msg.Buttons = Button;

            msg.ShowInTaskbar = ShowInTaskbar;

            DialogStateChanged?.Invoke(msg, new DialogEventArgs(DialogEventArgs.DialogState.Show));

            msg.ShowDialog();

            DialogStateChanged?.Invoke(msg, new DialogEventArgs(DialogEventArgs.DialogState.Close));

            return msg.Result;
        }
    }
}