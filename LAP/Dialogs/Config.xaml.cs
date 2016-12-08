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

            this.Category = Category;
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
}