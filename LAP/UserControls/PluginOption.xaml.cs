using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LAP.UserControls
{
    /// <summary>
    /// PluginOption.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginOption : UserControl
    {
        List<Utils.PluginManager.Plugin> CurrentPlugins = new List<Utils.PluginManager.Plugin>();
        List<Utils.PluginManager.PluginFunction> CurrentFunctions = new List<Utils.PluginManager.PluginFunction>();
        List<bool> PluginEnabled = new List<bool>();
        ClearUC.Tab.TabItem FunctionItem = new ClearUC.Tab.TabItem("");
        ClearUC.ListViewItems.ListSubItem CleanUpItem = new ClearUC.ListViewItems.ListSubItem() { SubLabelVisibility = Visibility.Hidden };

        public PluginOption()
        {
            InitializeComponent();
            UpdateTab();

            PluginT.Items.Clear();

            CurrentPlugins.Clear();
            PluginEnabled.Clear();

            CurrentFunctions.Clear();

            CurrentPlugins.AddRange(Utils.PluginManager.GetPlugins(false));
            CurrentFunctions.AddRange(Utils.PluginManager.GetFunctions());
            for (int i = 0; CurrentPlugins.Count > i; i++)
            {
                PluginEnabled.Add(CurrentPlugins[i].Enabled);
                PluginT.Items.Add(new ClearUC.Tab.TabItem(CurrentPlugins[i].Instance.Title));
            }
            PluginT.Items.Add(FunctionItem);

            CleanUpItem.ItemClicked += CleanUpItem_ItemClicked;
        }

        private void CleanUpItem_ItemClicked(object sender, ClearUC.ItemClickedEventArgs e)
        {
            Utils.PluginManager.CleanUp();
        }

        public void UpdateTab()
        {
            InfoGrid.Visibility = Visibility.Hidden;
            FunctionGrid.Visibility = Visibility.Hidden;
            EnableL.Content = Localize.Get("ENABLE");
            FunctionItem.Title = Localize.Get("FUNCTION");
            CleanUpItem.MainLabelText = Localize.Get("CLEANUP");
        }

        private void PluginT_ActiveItemChanged(object sender, EventArgs e)
        {
            EnableB.ToggleStateChanged -= EnableB_ToggleStateChanged;

            if(PluginT.ActiveItem == FunctionItem)
            {
                InfoGrid.Visibility = Visibility.Hidden;
                FunctionGrid.Visibility = Visibility.Visible;

                FunctionList.Items.Clear();

                FunctionList.Items.Add(CleanUpItem);

                System.Collections.ObjectModel.Collection<Utils.PluginManager.PluginFunction> funcs
                    = Utils.PluginManager.GetFunctions();

                for(int i = 0;funcs.Count > i; i++)
                {
                    ClearUC.ListViewItems.ListToggleItem lti = new ClearUC.ListViewItems.ListToggleItem();
                    lti.ListItem.MainLabelText = funcs[i].Title;
                    lti.ToggleButton.State = funcs[i].Enabled;

                    FunctionList.Items.Add(lti);
                }
            }
            else
            {
                FunctionGrid.Visibility = Visibility.Hidden;

                Utils.PluginManager.Plugin plg = GetActiveItem();
                if (plg == null)
                {
                    InfoGrid.Visibility = Visibility.Hidden;
                    return;
                }

                EnableB.State = plg.Enabled;
                TitleL.Content = plg.Instance.Title;
                DescL.Content = plg.Instance.Description;
                AuthorL.Content = plg.Instance.Author + " - " + plg.Instance.Version.ToString();

                EnableB.ToggleStateChanged += EnableB_ToggleStateChanged;

                if (string.IsNullOrEmpty(plg.Instance.URL))
                    URLB.Visibility = Visibility.Hidden;
                else
                {
                    URLB.Visibility = Visibility.Visible;
                    URLB.Content = plg.Instance.URL;
                }

                InfoGrid.Visibility = Visibility.Visible;
            }
        }

        private Utils.PluginManager.Plugin GetActiveItem()
        {
            if (PluginT.ActiveIndex < 0) return null;
            return Utils.PluginManager.GetPlugins(false)[PluginT.ActiveIndex];
        }

        private void URLB_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(GetActiveItem().Instance.URL);
        }

        internal bool Apply()
        {
            bool restart = false;
            for (int i = 0; CurrentPlugins.Count > i; i++)
            {
                if (CurrentPlugins[i].Enabled != PluginEnabled[i])
                    restart = true;

                CurrentPlugins[i].Enabled = PluginEnabled[i];
            }

            if(FunctionList.Items.Count >= CurrentFunctions.Count)
            {
                for (int i = 0; CurrentFunctions.Count > i; i++)
                {
                    if(FunctionList.Items[i] != CleanUpItem)
                    {
                        ClearUC.ListViewItems.ListToggleItem lti = (ClearUC.ListViewItems.ListToggleItem)FunctionList.Items[i];
                        if (lti.ToggleButton.State != CurrentFunctions[i].Enabled)
                            restart = true;

                        CurrentFunctions[i].Enabled = lti.ToggleButton.State;
                    }
                }
            }

            return restart;
        }

        private void EnableB_ToggleStateChanged(object sender, EventArgs e)
        {
            int index = CurrentPlugins.IndexOf(GetActiveItem());
            PluginEnabled[index] = EnableB.State;
        }
    }
}
