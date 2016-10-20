using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearUC.ListViewItems;

namespace TestPlugin
{
    public class PagePlugin : LAPP.Page.Plugin
    {
        private List<ListItem> Items = new List<ListItem>();

        private List<LAPP.MTag.File> Files = new List<LAPP.MTag.File>();

        public override System.Windows.Controls.Border Border { get; set; }

        public override string Title { get; set; } = "TestPluginPage";

        public PagePlugin()
        {
            
        }

        public override void Dispose()
        {
        }

        public override ListItem[] GetPageItems()
        {
            return Items.ToArray();
        }

        public override ListItem[] GetTopPageItems()
        {
            return Items.ToArray();
        }

        public override void PlayAnyFile()
        {
            OnPlayFile(Files.ToArray(), 0);
        }

        public override void ItemClicked(int Index, ListItem Item)
        {
            OnPlayFile(Files.ToArray(), Index);
        }

        public override void Update()
        {
            GetItems();
        }

        private void GetItems()
        {
            Items.Clear();
            string[] files
                = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
                + @"\Echosmith\Talking Dreams");
            foreach(string f in files)
            {
                LAPP.MTag.File file = new LAPP.MTag.File(f, GetTag(f));
                file.Artwork = file.Tag.GetArtwork();
                Items.Add(new ListSubItem() { MainLabelText = file.Tag.Title, SubLabelVisibility = System.Windows.Visibility.Hidden });
                Files.Add(file);
            }
        }
    }
}
