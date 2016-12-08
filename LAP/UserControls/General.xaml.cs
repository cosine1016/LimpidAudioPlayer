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
using System.IO;

namespace LAP.UserControls
{
    /// <summary>
    /// General.xaml の相互作用ロジック
    /// </summary>
    public partial class General : UserControl
    {
        LAPP.Localize[] Langs;
        string[] LangFiles;

        public General()
        {
            InitializeComponent();

            Localize_LanguageChanged(null, null);
            Localize.LanguageChanged += Localize_LanguageChanged;
        }

        private void Localize_LanguageChanged(object sender, EventArgs e)
        {
            InstallLangB.Content = Localize.Get("INSTALL_LANG");
            UpdateLang();
        }

        private void UpdateLang()
        {
            InstallLangC.Items.Clear();

            string[] files = Directory.GetFiles(Config.Current.Path[Enums.Path.LanguageDirectory],
                "*.loc");
            Langs = new LAPP.Localize[files.Length];

            for(int i = 0;files.Length > i; i++)
            {
                string name = Path.GetFileName(files[i]);
                LAPP.Localize loc = LAPP.Localize.Load(files[i]);
                Langs[i] = loc;

                try
                {
                    System.Globalization.CultureInfo ci
                        = new System.Globalization.CultureInfo(Langs[i].Info["LCID"]);

                    InstallLangC.Items.Add(ci.Name + " : " + name);
                }
                catch (Exception)
                { InstallLangC.Items.Add(Localize.Get("0_UNKNOWN") + " : " + name); }
            }

            LangFiles = files;
        }

        public LAPP.Setting.ApplyInfo Apply()
        {
            if(InstallLangC.SelectedIndex > -1)
            {
                Config.Current.Path[Enums.Path.LanguageFile] = LangFiles[InstallLangC.SelectedIndex];
                Localize.Load(Config.Current.Path[Enums.Path.LanguageFile]);
            }

            return new LAPP.Setting.ApplyInfo(true, false, true, false);
        }
    }
}
