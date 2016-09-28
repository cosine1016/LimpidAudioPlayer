using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP.ListItems;

namespace TestPlugin
{
    public class PagePlugin : LAPP.Page.Plugin
    {
        LAPP.Utils.TagReader reader = new LAPP.Utils.TagReader();

        private List<ListItem> Items = new List<ListItem>();

        private List<LAPP.Utils.File> Files = new List<LAPP.Utils.File>();

        public override System.Windows.Controls.Border Border { get; set; }

        public override string Title { get; set; } = "TestPluginPage";

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

        public override void Update()
        {
            GetItems();
        }

        private void GetItems()
        {
            Items.Clear();
            string[] files = System.IO.Directory.GetFiles(@"C:\Users\skkby\Music\iTunes\iTunes Media\Music\Echosmith\Talking Dreams");
            foreach(string f in files)
            {
                LAPP.Utils.File file = new LAPP.Utils.File(f, reader.GetTag(f));
                Items.Add(new ListSubItem() { MainLabelText = f, SubLabelVisibility = System.Windows.Visibility.Hidden });
                Files.Add(file);
            }
        }
    }
}
