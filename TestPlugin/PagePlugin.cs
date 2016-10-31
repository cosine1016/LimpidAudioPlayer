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
        protected List<ListItem> Items = new List<ListItem>();

        protected List<LAPP.MediaFile> Files = new List<LAPP.MediaFile>();

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

        protected virtual void GetItems()
        {
        }
    }

    public class FastPage : PagePlugin
    {
        protected override void GetItems()
        {
            Items.Clear();
            string[] files
                = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
                + @"\Echosmith\Talking Dreams");

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            foreach (string f in files)
            {
                LAPP.MediaFile tag = new LAPP.MediaFile(f);
                Items.Add(new ListSubItem() { MainLabelText = tag.Title,
                    SubLabelVisibility = System.Windows.Visibility.Hidden });
                Files.Add(tag);
                
            }
            sw.Stop();
            Items.Add(new ListSubItem() { MainLabelText = sw.ElapsedMilliseconds.ToString() });
        }
    }
}
