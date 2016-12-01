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

            Caption.Title = Localize.Get("0_CONFIG");
            MW = MainWindow;

            if (Category != null && Category.Length > 0)
                this.Category = Category;
            else
            {
                this.Category = new ISettingItem[2];
                
                OutputCategory output = new OutputCategory();
                this.Category[0] = output;

                Plugin plg = new Plugin();
                this.Category[1] = plg;
            }

            TabContent.Children.Clear();
            Tab.Items.Clear();
            for (int i = 0; this.Category.Length > i; i++)
            {
                if(this.Category[i] != null)
                {
                    this.Category[i].UIControl.Visibility = Visibility.Hidden;

                    TabContent.Children.Add(this.Category[i].UIControl);
                    Tab.Items.Add(new ClearUC.Tab.TabItem(this.Category[i].Header, this.Category[i].Border));
                }
            }

            System.Collections.ObjectModel.Collection<ISettingItem> sets
                = Utils.PluginManager.GetSettings();
            for (int s = 0; sets.Count > s; s++)
            {
                sets[s].UIControl.Visibility = Visibility.Hidden;
                Tab.Items.Add(new ClearUC.Tab.TabItem(sets[s].Header, sets[s].Border));
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
            Header = Localize.Get("CONFIG_OUTPUT");
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
            cnf.Setting.WaveOut.Amplify = (float)aos.AmplifyN.Value / 100;
        }

        public void Dispose()
        {
        }
    }

    internal class Plugin : ISettingItem
    {
        public Plugin()
        {
            Header = Localize.Get(Strings.Plugin);
            UIControl = new UserControls.PluginOption();
        }

        public Border Border { get; set; }

        public string Header { get; set; }

        public UIElement UIControl { get; set; }

        public void Apply()
        {
        }

        public void Dispose()
        {
        }
    }
}