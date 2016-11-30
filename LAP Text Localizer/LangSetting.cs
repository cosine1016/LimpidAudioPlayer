using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAP_Text_Localizer
{
    public partial class LangSetting : Form
    {
        internal bool Created = false;
        public LangSetting()
        {
            InitializeComponent();

            if(Language.CurrentLanguage != null)
            {
                LCIDNum.Value = Language.CurrentLanguage.LCID;
                LAPVersionMT.Text = Language.CurrentLanguage.SupportVersion;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Language.CurrentLanguage == null)
                Language.CurrentLanguage = new Language();

            Language.CurrentLanguage.LCID = (int)LCIDNum.Value;
            Language.CurrentLanguage.SupportVersion = LAPVersionMT.Text;

            Created = true;
            Close();
        }

        private void LangSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
