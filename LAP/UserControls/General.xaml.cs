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
            LangL.Content = Localize.Get("LANGUAGE");
            UpdateLang();
        }

        private void UpdateLang()
        {
            LangC.Items.Clear();
            Langs = null;
            LangFiles = null;

            if (Directory.Exists(Config.Current.Path[Enums.Path.LanguageDirectory]))
            {
                string[] files = Directory.GetFiles(Config.Current.Path[Enums.Path.LanguageDirectory],
                    "*.loc").Where(item => Path.GetExtension(item).ToLower() == ".loc").ToArray();
                Langs = new LAPP.Localize[files.Length];

                for (int i = 0; files.Length > i; i++)
                {
                    string name = Path.GetFileName(files[i]);
                    LAPP.Localize loc = LAPP.Localize.Load(files[i]);
                    Langs[i] = loc;

                    if (loc.Info.ContainsKey("Language"))
                    {
                        LangC.Items.Add(loc.Info["Language"]);
                    }
                    else
                    {
                        LangC.Items.Add(Localize.Get(Strings.Unknown) + "(" + name + ")");
                    }

                    if (!string.IsNullOrEmpty(Localize.CurrentFilePath))
                    {
                        if (Localize.CurrentFilePath == files[i])
                            LangC.SelectedIndex = i;
                    }
                }

                LangFiles = files;
            }
        }

        public void Apply()
        {
            if(LangC.SelectedIndex > -1)
            {
                Config.Current.Path[Enums.Path.LanguageFile] = LangFiles[LangC.SelectedIndex];
                Localize.Load(Config.Current.Path[Enums.Path.LanguageFile]);
            }
        }

        private void InstallLangB_Click(object sender, RoutedEventArgs e)
        {
            string dir = Config.Current.Path[Enums.Path.LanguageDirectory];
            
            if (File.Exists(dir))
            {
                Config.Current.Path.Reset(Enums.Path.LanguageDirectory);
                dir = Config.Current.Path[Enums.Path.LanguageDirectory];
            }

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = Localize.Get("LOCALIZE_FILE") + "|*.loc";
            ofd.FileName = null;
            ofd.Title = Localize.Get(Strings.Open);
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string Dest = dir + Path.GetFileName(ofd.FileName);

                if (File.Exists(Dest))
                {
                    if (ClearUC.Dialogs.Dialog.ShowMessageBox(ClearUC.Dialogs.Dialog.Buttons.YesNo,
                        Localize.Get("OVERWRITE_T"), Localize.Get("OVERWRITE_M")) == ClearUC.Dialogs.Dialog.ClickedButton.No)
                    {
                        return;
                    }
                }

                File.Copy(ofd.FileName, Dest, true);
            }

            UpdateLang();
        }
    }
}
