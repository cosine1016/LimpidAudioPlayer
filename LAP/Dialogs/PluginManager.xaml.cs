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
using System.Windows.Shapes;

namespace LAP.Dialogs
{
    /// <summary>
    /// PluginManager.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginManager : Window
    {
        public List<bool> EnabledList = new List<bool>();

        public PluginManager()
        {
            InitializeComponent();

            for(int i = 0;Utils.PluginManager.InitializedPlugin.Count > i; i++)
            {
                Utils.PluginManager.Plugin plg = Utils.PluginManager.InitializedPlugin[i];

                ClearUC.ListViewItems.ListToggleItem lti = new ClearUC.ListViewItems.ListToggleItem();
                lti.ListItem.MainLabelText = plg.Instance.Title;
                lti.ListItem.SubLabelText = plg.Asm.Location;
                lti.ToggleButton.State = plg.Enabled;
                lti.Data = plg;
                lti.DataType = plg.GetType();
                lti.ToggleButton.ToggleStateChanged += ToggleButton_ToggleStateChanged;

                PV.Items.Add(lti);

                EnabledList.Add(plg.Enabled);
            }
        }

        private void ToggleButton_ToggleStateChanged(object sender, EventArgs e)
        {

        }

        ~PluginManager()
        {
            for(int i = 0;PV.Items.Count > i; i++)
            {
                if (PV.Items[i].Data != null) PV.Items[i].Data = null;
                ClearUC.ListViewItems.ListToggleItem lti = PV.Items[i] as ClearUC.ListViewItems.ListToggleItem;
                if(lti != null)
                {
                    lti.ToggleButton.ToggleStateChanged -= ToggleButton_ToggleStateChanged;
                }
            }
        }

        private void PV_ItemClicked(object sender, ClearUC.ListViewItems.ListItem.ItemClickedEventArgs e)
        {

        }
    }
}
