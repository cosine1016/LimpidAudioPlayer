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
        public PluginOption()
        {
            InitializeComponent();
            UpdateTab();

            EnableL.Content = Localize.Get("ENABLE");
        }

        public void UpdateTab()
        {
            PluginT.Items.Clear();

            Utils.PluginManager.Plugin[] plgs = Utils.PluginManager.GetPlugins();
            for (int i = 0; plgs.Length > i; i++)
                PluginT.Items.Add(new ClearUC.Tab.TabItem(plgs[i].Instance.Title));
            InfoGrid.Visibility = Visibility.Hidden;
        }

        private void PluginT_ActiveItemChanged(object sender, EventArgs e)
        {
            Utils.PluginManager.Plugin plg = GetActiveItem();
            if(plg == null)
            {
                InfoGrid.Visibility = Visibility.Hidden;
                return;
            }

            EnableB.State = plg.Enabled;
            TitleL.Content = plg.Instance.Title;
            DescL.Content = plg.Instance.Description;
            AuthorL.Content = plg.Instance.Author + " - " + plg.Instance.Version.ToString();

            if (string.IsNullOrEmpty(plg.Instance.URL))
                URLB.Visibility = Visibility.Hidden;
            else
            {
                URLB.Visibility = Visibility.Visible;
                URLB.Content = plg.Instance.URL;
            }

            InfoGrid.Visibility = Visibility.Visible;
        }

        private LAP.Utils.PluginManager.Plugin GetActiveItem()
        {
            if (PluginT.ActiveIndex < 0) return null;
            return Utils.PluginManager.GetPlugins()[PluginT.ActiveIndex];
        }

        private void URLB_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(GetActiveItem().Instance.URL);
        }

        private void EnableB_ToggleStateChanged(object sender, EventArgs e)
        {
            if (GetActiveItem() == null) return;
            GetActiveItem().Enabled = EnableB.State;
        }
    }
}
