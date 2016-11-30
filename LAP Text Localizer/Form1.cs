using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LAP_Text_Localizer
{
    public partial class Form1 : Form
    {
        IDSorter Sorter = new IDSorter();
        public Form1()
        {
            InitializeComponent();
            
            if (checkBox1.Checked)
                textBox1.CharacterCasing = CharacterCasing.Upper;
            else
                textBox1.CharacterCasing = CharacterCasing.Normal;

            MainView.ListViewItemSorter = Sorter;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainView_SizeChanged(object sender, EventArgs e)
        {
        }

        private void languageSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LangSetting().ShowDialog();
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            if (IsEditMode)
            {
                string culture = "";
                try
                {
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(Language.CurrentLanguage.LCID);
                    culture = ci.Name;
                }
                catch (System.Globalization.CultureNotFoundException) { culture = "Unknown"; }

                Text = "LAP Localizer [" + Language.CurrentLanguage.LCID + "(" + culture + ")" +
                    " - LAP:" + Language.CurrentLanguage.SupportVersion.Replace(" ", "") + "]";

                if (file)
                    Text += " " + path;
            }
            else
                Text = "LAP Localizer";
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitEditMode(false, null);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (edited)
            {
                if (MessageBox.Show("Some Progresses Are Not Saved.\nAre You Sure You Want to Close File?",
                    "Localizer", MessageBoxButtons.YesNo)
                    == DialogResult.No)
                    return;
            }

            EndEditMode();
        }

        #region EditMode

        bool edited = false, file = false;
        string path = null;
        bool IsEditMode = false;

        internal void InitEditMode(bool IsFile, string Path)
        {
            IsEditMode = true;
            MainView.Items.Clear();

            if (IsFile)
            {
                file = true;
                path = Path;
                Language.Load(Path);

                KeyValuePair<string, string>[] strs = Language.CurrentLanguage.Strings.ToArray();
                for (int i = 0; strs.Length > i; i++)
                    MainView.Items.Add(new ListViewItem(new string[] { strs[i].Value, strs[i].Key }));
            }
            else
            {
                file = false;
                LangSetting setd = new LangSetting();
                setd.ShowDialog();
                if (!setd.Created)
                {
                    IsEditMode = false;
                    return;
                }

                edited = true;
            }

            MainView.Columns[0].Width = -2;
            openToolStripMenuItem.Enabled = false;
            createToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            button1.Enabled = true;
            UpdateTitle();
        }

        internal void EndEditMode()
        {
            Language.CurrentLanguage = null;
            IsEditMode = false;
            edited = false;
            UpdateTitle();
            MainView.Items.Clear();

            openToolStripMenuItem.Enabled = true;
            createToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            button1.Enabled = false;
        }

        #endregion
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked) textBox1.Text = textBox1.Text.Replace("-", "_");
            for(int i = 0;MainView.Items.Count > i; i++)
            {
                if(MainView.Items[i].SubItems[1].Text == textBox1.Text)
                {
                    MainView.Items[i].Focused = true;
                    MainView.FocusedItem.BeginEdit();

                    textBox1.Text = null;
                    return;
                }
            }

            ListViewItem lvi = new ListViewItem(new string[] { "<Text>", textBox1.Text });
            MainView.Items.Add(lvi);
            lvi.Focused = true;
            MainView.FocusedItem.BeginEdit();
            textBox1.Text = null;
            edited = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox1.CharacterCasing = CharacterCasing.Upper;
            else
                textBox1.CharacterCasing = CharacterCasing.Normal;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (MainView.SelectedIndices.Count == 1)
                contextMenuStrip1.Items[0].Enabled = true;
            else
                contextMenuStrip1.Items[0].Enabled = false;
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = MainView.SelectedItems[0];
            EditString dlg = new EditString();

            dlg.Accepted += (obj, ea) =>
            {
                if(lvi.SubItems[1].Text != dlg.ID)
                {
                    for (int i = 0; MainView.Items.Count > i; i++)
                    {
                        if (MainView.Items[i].SubItems[1].Text == textBox1.Text)
                        {
                            dlg.SameIDAvailable = true;
                            return;
                        }
                    }
                }

                lvi.Text = dlg.String;
                lvi.SubItems[1].Text = dlg.ID;
            };

            dlg.String = lvi.Text;
            dlg.ID = lvi.SubItems[1].Text;
            dlg.ShowDialog();
        }

        private void MainView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            switch (Sorter.Order)
            {
                case SortOrder.Ascending:
                    Sorter.Order = SortOrder.Descending;
                    break;
                case SortOrder.Descending:
                    Sorter.Order = SortOrder.Ascending;
                    break;
                case SortOrder.None:
                    Sorter.Order = SortOrder.Descending;
                    break;
            }

            MainView.Sort();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Language.CurrentLanguage.Strings.Clear();

            for (int i = 0; MainView.Items.Count > i; i++)
            {
                Language.CurrentLanguage.Strings.Add(MainView.Items[i].SubItems[1].Text, MainView.Items[i].Text);
            }

            if (file)
            {
                Language.Save(path);
                edited = false;
            }
            else
                saveAsToolStripMenuItem_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                InitEditMode(true, openFileDialog1.FileName);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = MainView.SelectedItems[0];
            MainView.Items.Remove(lvi);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Language.Save(saveFileDialog1.FileName);
                file = true;
                path = saveFileDialog1.FileName;
                UpdateTitle();
                edited = false;
            }
        }
    }

    public class IDSorter : IComparer<string>, System.Collections.IComparer
    {
        public int Compare(string x, string y)
        {
            int xi = GetNumberFromStr(x), yi = GetNumberFromStr(y);
            if (xi > -1 && yi > -1)
                return xi - yi;
            else if (xi > -1) return -1;
            else if (yi > -1) return 1;

            switch (Order)
            {
                case SortOrder.Ascending:
                    return CompareStr(x, y);
                case SortOrder.Descending:
                    return -CompareStr(x, y);
                case SortOrder.None:
                    return 0;
            }

            return 0;
        }

        private int GetNumberFromStr(string Str)
        {
            int i = Str.IndexOf('_');

            if(i > -1)
            {
                int strInd = -1;
                if (!int.TryParse(Str.Substring(0, i), out strInd)) return -1;
                else return strInd;
            }

            return -1;
        }

        private int CompareStr(string x, string y)
        {
            return string.Compare(x, y);
        }

        public int Compare(object x, object y)
        {
            ListViewItem lvix = (ListViewItem)x, lviy = (ListViewItem)y;
            return Compare(lvix.SubItems[1].Text, lviy.SubItems[1].Text);
        }

        public SortOrder Order { get; set; }
    }
}
