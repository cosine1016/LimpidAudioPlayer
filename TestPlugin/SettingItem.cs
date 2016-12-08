using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LAPP.Setting;

namespace TestPlugin
{
    public class SettingItem : ISettingItem
    {
        public Border Border { get; set; }

        public string Header { get; set; } = "Test";

        public UIElement UIControl { get; set; } = new Button() { Content = "Test" };

        public ApplyInfo Apply()
        {
            return new ApplyInfo(true);
        }

        public void Dispose()
        {
        }
    }
}
