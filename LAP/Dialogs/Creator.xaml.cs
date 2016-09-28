using System.Windows;

namespace LAP.Dialogs
{
    /// <summary>
    /// Creator.xaml の相互作用ロジック
    /// </summary>
    public partial class Creator : Window
    {
        public Creator()
        {
            InitializeComponent();

            Caption.Title = Utils.Config.Language.Strings.Creator;

            LicenseTab.ActiveItemChanged += LicenseTab_ActiveItemChanged;

            LicenseTab.Items.Add(new ClearUC.Tab.TabItem("Ms-PL"));
            LicenseTab.Items.Add(new ClearUC.Tab.TabItem("Icons"));

            LicenseTab.ActiveIndex = 0;
        }

        private void LicenseTab_ActiveItemChanged(object sender, System.EventArgs e)
        {
            switch (LicenseTab.ActiveIndex)
            {
                case 0:
                    License.Text = Properties.Resources.Ms_PL;
                    break;

                case 1:
                    License.Text = Properties.Resources.Icon;
                    break;
            }
        }
    }
}