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
    public partial class EditString : Form
    {
        internal event EventHandler Accepted;
        internal string String { get; set; }
        internal string ID { get; set; }
        internal bool SameIDAvailable { get; set; } = false;

        public EditString()
        {
            InitializeComponent();
        }

        private void OKB_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked) textBox1.Text = textBox1.Text.Replace("-", "_");
            Accepted?.Invoke(this, new EventArgs());

            if (SameIDAvailable)
            {

                return;
            }
            else
                Close();
        }

        private void CancelB_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ID = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            String = textBox2.Text;
        }

        private void EditString_Shown(object sender, EventArgs e)
        {
            bool Normalize = false;
            for (int i = 0; ID.Length > i; i++)
            {
                char c = ID[i];
                if (('a' <= c && c <= 'z') || ('ａ' <= c && c <= 'ｚ'))
                {
                    Normalize = true;
                    break;
                }
            }

            checkBox1.Checked = !Normalize;

            textBox1.Text = ID;
            textBox2.Text = String;

            textBox1.TextChanged += textBox1_TextChanged;
            textBox2.TextChanged += textBox2_TextChanged;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox1.CharacterCasing = CharacterCasing.Upper;
            else
                textBox1.CharacterCasing = CharacterCasing.Normal;
        }
    }
}
