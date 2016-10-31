using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using cnf = LAP.Utils.Config;
using LAPP.Setting;

namespace LAP.Dialogs
{
    /// <summary>
    /// Config.xaml の相互作用ロジック
    /// </summary>
    public partial class Config : Window
    {
        private readonly ISettingItem[] Category = null;
        private MainWindow MW;

        internal Config(MainWindow MainWindow, ISettingItem[] Category = null)
        {
            InitializeComponent();

            Caption.Title = cnf.Language.Strings.Config;
            MW = MainWindow;

            if (Category != null && Category.Length > 0)
                this.Category = Category;
            else
            {
                this.Category = new ISettingItem[3];
                
                OutputCategory output = new OutputCategory();
                this.Category[0] = output;

                Plugin plg = new Plugin();
                this.Category[1] = plg;
            }

            TabContent.Children.Clear();
            Tab.Items.Clear();
            for (int i = 0; this.Category.Length > i; i++)
            {
                this.Category[i].UIControl.Visibility = Visibility.Hidden;

                TabContent.Children.Add(this.Category[i].UIControl);
                Tab.Items.Add(new ClearUC.Tab.TabItem(this.Category[i].Header, this.Category[i].Border));
            }

            for(int i = 0; Utils.PluginManager.InitializedPlugin.Count > i; i++)
            {
                Utils.PluginManager.Plugin plg = Utils.PluginManager.InitializedPlugin[i];
                if (plg.Enabled)
                {
                    for(int s = 0;plg.Instance.SettingItems.Count > s; s++)
                    {
                        plg.Instance.SettingItems[s].UIControl.Visibility = Visibility.Hidden;
                        Tab.Items.Add(new ClearUC.Tab.TabItem(plg.Instance.SettingItems[s].Header, plg.Instance.SettingItems[s].Border));
                    }
                }
            }

            Tab.ActiveItemChanged += Tab_ActiveItemChanged;

            if (Tab.Items.Count > 0) Tab.ActiveIndex = 0;
        }

        private void Tab_ActiveItemChanged(object sender, EventArgs e)
        {
            for (int i = 0; TabContent.Children.Count > i; i++)
                TabContent.Children[i].Visibility = Visibility.Hidden;

            if (Tab.ActiveIndex > -1) Category[Tab.ActiveIndex].UIControl.Visibility = Visibility.Visible;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; Category.Length > i; i++)
            {
                Category[i].Apply();
            }

            cnf.WriteSetting(Utils.Paths.SettingFilePath);

            cnf.UpdateLAPP();

            MW.ReRenderFile(true, true);
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; Category.Length > i; i++)
            {
                Category[i].Dispose();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    internal class OutputCategory : ISettingItem
    {
        public OutputCategory()
        {
            Header = cnf.Language.Strings.ConfigWindow.Output.Header;
            UIControl = aos;
        }

        public Border Border { get; set; }

        public string Header { get; set; }

        public UIElement UIControl { get; set; }

        private UserControls.AudioOutSelector aos = new UserControls.AudioOutSelector();

        public void Apply()
        {
            cnf.Setting.WaveOut.OutputDevice = aos.SelectedDevice;
            cnf.Setting.WaveOut.ASIO = aos.ASIOConfig;
            cnf.Setting.WaveOut.WASAPI = aos.WASAPIConfig;
            cnf.Setting.WaveOut.DirectSound = aos.DSConfig;
        }

        public void Dispose()
        {
        }
    }

    internal class Plugin : ISettingItem
    {
        public Plugin()
        {
            UIControl = new UserControls.PluginOption();
        }

        public Border Border { get; set; }

        public string Header { get; set; } = "Plugin";

        public UIElement UIControl { get; set; }

        public void Apply()
        {
        }

        public void Dispose()
        {
        }
    }
}