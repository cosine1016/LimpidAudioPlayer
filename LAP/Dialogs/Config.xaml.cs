using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using cnf = LAP.Config;
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

        private void UpdateLanguage()
        {
            Caption.Title = Localize.Get("0_CONFIG");
            Apply.Content = Localize.Get("APPLY");
            Cancel.Content = Localize.Get("CANCEL");
            UpdateTab();
        }

        private void UpdateTab()
        {
            TabContent.Children.Clear();
            Tab.Items.Clear();
            for (int i = 0; this.Category.Length > i; i++)
            {
                if (this.Category[i] != null)
                {
                    this.Category[i].UIControl.Visibility = Visibility.Hidden;

                    TabContent.Children.Add(this.Category[i].UIControl);
                    Tab.Items.Add(new ClearUC.Tab.TabItem(this.Category[i].Header, this.Category[i].Border));
                }
            }

            Tab.ActiveIndex = 0;
        }

        internal Config(MainWindow MainWindow, ISettingItem[] Category = null)
        {
            InitializeComponent();

            MW = MainWindow;

            if (Category != null && Category.Length > 0)
                this.Category = Category;
            else
            {
                List<ISettingItem> set = new List<ISettingItem>();

                GeneralCategory general = new GeneralCategory();
                set.Add(general);

                OutputCategory output = new OutputCategory();
                set.Add(output);

                Plugin plg = new Plugin();
                set.Add(plg);

                this.Category = set.ToArray();
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

            Localize.AddLanguageChangedAction(UpdateLanguage);
        }

        private void Tab_ActiveItemChanged(object sender, EventArgs e)
        {
            for (int i = 0; TabContent.Children.Count > i; i++)
                TabContent.Children[i].Visibility = Visibility.Hidden;

            if (Tab.ActiveIndex > -1) Category[Tab.ActiveIndex].UIControl.Visibility = Visibility.Visible;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            bool Restart = false, CloseDlg = false, Rerender = false;
            for (int i = 0; Category.Length > i; i++)
            {
                ApplyInfo ai = Category[i].Apply();

                if (ai.RestartApp)
                    Restart = true;

                if (ai.CloseDialog)
                    CloseDlg = true;

                if (ai.RerenderFile)
                    Rerender = true;
            }

            cnf.Save(cnf.Current.Path[Enums.Path.SettingFile]);

            if (Restart)
            {
                Application.Current.Shutdown();
                System.Windows.Forms.Application.Restart();
                return;
            }

            if (CloseDlg)
            {
                Close();
            }

            if (Rerender)
            {
                MW.ReRenderFile(true, true);
            }
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

    internal class GeneralCategory : ISettingItem
    {
        public GeneralCategory()
        {
            Action = new Action(() =>
            {
                Header = Localize.Get("GENERAL");
            });
            Localize.AddLanguageChangedAction(Action);

            UIControl = gen;
        }

        private Action Action;

        public Border Border { get; set; }

        public string Header { get; set; }

        public UIElement UIControl { get; set; }

        private UserControls.General gen = new UserControls.General();

        public ApplyInfo Apply()
        {
            return gen.Apply();
        }

        public void Dispose()
        {
            Localize.RemoveLanguageChangedAction(Action);
        }
    }

    internal class OutputCategory : ISettingItem
    {
        public OutputCategory()
        {
            Action = new Action(() =>
            {
                Header = Localize.Get("CONFIG_OUTPUT");
            });
            Localize.AddLanguageChangedAction(Action);

            UIControl = aos;
        }

        private Action Action;

        public Border Border { get; set; }

        public string Header { get; set; }

        public UIElement UIControl { get; set; }

        private UserControls.AudioOutSelector aos = new UserControls.AudioOutSelector();

        public ApplyInfo Apply()
        {
            try
            {
                cnf.Current.Output.OutputDevice = aos.SelectedDevice;
                cnf.Current.Output.ASIO = aos.ASIOConfig;
                cnf.Current.Output.WASAPI = aos.WASAPIConfig;
                cnf.Current.Output.DirectSound = aos.DSConfig;
                cnf.Current.Output.Amplify = (float)aos.AmplifyN.Value / 100;

                return new ApplyInfo(true, false, false, aos.RerenderFile);
            }
            catch (Exception) { return new ApplyInfo(false); }
        }

        public void Dispose()
        {
            Localize.RemoveLanguageChangedAction(Action);
        }
    }

    internal class Plugin : ISettingItem
    {
        public Plugin()
        {
            Action = new Action(() =>
            {
                Header = Localize.Get(Strings.Plugin);
            });
            Localize.AddLanguageChangedAction(Action);

            UIControl = new UserControls.PluginOption();
        }

        private Action Action;

        public Border Border { get; set; }

        public string Header { get; set; }

        public UIElement UIControl { get; set; }

        public ApplyInfo Apply()
        {
            ApplyInfo ai = new ApplyInfo(true);

            return ai;
        }

        public void Dispose()
        {
            Localize.RemoveLanguageChangedAction(Action);
        }
    }
}