using System.Windows;
using System.Windows.Controls;

namespace LAPP.Setting
{
    public interface ISettingItem : System.IDisposable
    {
        UIElement UIControl { get; set; }
        Border Border { get; set; }
        string Header { get; set; }

        void Apply();
    }
}
