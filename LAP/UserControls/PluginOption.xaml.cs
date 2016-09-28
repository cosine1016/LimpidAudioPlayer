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
        }

        public void UpdateTab()
        {
            PluginT.Items.Clear();
            for (int i = 0; Utils.PluginManager.InitializedPlugin.Count > i; i++)
                PluginT.Items.Add(new ClearUC.Tab.TabItem(Utils.PluginManager.InitializedPlugin[i].Instance.Title));
            InfoGrid.Visibility = Visibility.Hidden;
        }

        private void PluginT_ActiveItemChanged(object sender, EventArgs e)
        {
            LAPP.LimpidAudioPlayerPlugin plg = GetActiveItem();
            if(plg == null)
            {
                InfoGrid.Visibility = Visibility.Hidden;
                return;
            }

            TitleL.Content = plg.Title;
            DescL.Content = plg.Description;
            AuthorL.Content = plg.Author;

            if (string.IsNullOrEmpty(plg.URL))
                URLB.Visibility = Visibility.Hidden;
            else
            {
                URLB.Visibility = Visibility.Visible;
                URLB.Content = plg.URL;
            }

            InfoGrid.Visibility = Visibility.Visible;
        }

        private LAPP.LimpidAudioPlayerPlugin GetActiveItem()
        {
            if (PluginT.ActiveIndex < 0) return null;
            return Utils.PluginManager.InitializedPlugin[PluginT.ActiveIndex].Instance;
        }

        private void URLB_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(GetActiveItem().URL);
        }
    }
}
