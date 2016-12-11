using System.Windows;
using System.Windows.Controls;

namespace LAPP.Setting
{
    public interface ISettingItem : System.IDisposable
    {
        UIElement UIControl { get; set; }

        Border Border { get; set; }

        string Header { get; set; }

        ApplyInfo Apply();
    }

    public struct ApplyInfo
    {
        public ApplyInfo(bool Success) : this(Success, false, false, false) { }

        public ApplyInfo(bool Success, bool RestartApp, bool CloseDialog, bool RerenderFile)
        {
            this.Success = Success;
            this.RestartApp = RestartApp;
            this.CloseDialog = CloseDialog;
            this.RerenderFile = RerenderFile;
            ForceRestartApp = false;
        }

        public bool Success { get; }

        public bool RestartApp { get; }

        public bool ForceRestartApp { get; }

        public bool CloseDialog { get; }

        public bool RerenderFile { get; }
    }
}
