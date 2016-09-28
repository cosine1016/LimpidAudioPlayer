using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ClearUC.ListViewItems;
using LAPP.Page;

namespace LAP.Page.Plugin
{
    class Page : ListViewPage
    {
        private LAPP.Page.Plugin Plg;

        public Page(LAPP.Page.Plugin PluginInstance)
        {
            Plg = PluginInstance;

            Plg.OnPlayFileEvent += Plg_OnPlayFileEvent;
            Plg.PlayFile += Plg_PlayFile;
            Plg.RendererDisposeRequest += Plg_RendererDisposeRequest;
            Plg.OrderEnded += Plg_OrderEnded;
        }

        private void Plg_OnPlayFileEvent(object sender, OnPlayFileEventArgs e)
        {
            Utils.Classes.File[] files = new Utils.Classes.File[e.Files.Length];
            for (int i = 0; files.Length > i; i++)
                files[i] = Utils.Converter.PluginFileToFile(e.Files[i]);
            OnPlayFile(files, e.Index);
        }

        private void Plg_OrderEnded(object sender, EventArgs e)
        {
            OnOrderEnded();
        }

        private void Plg_RendererDisposeRequest(object sender, EventArgs e)
        {
            OnRendererDisposeRequest();
        }

        private void Plg_PlayFile(object sender, LAPP.Page.PlayFileEventArgs e)
        {
            OnPlayFile(new PlayFileEventArgs(Utils.Converter.PluginFileToFile(e.File)));
        }

        public override Border Border
        {
            get
            {
                return Plg.Border;
            }

            protected set
            {
                Plg.Border = value;
            }
        }

        public override string Title
        {
            get
            {
                return Plg.Title;
            }

            protected set
            {
                Plg.Title = value;
            }
        }

        public override void Dispose()
        {
            Plg.OnPlayFileEvent -= Plg_OnPlayFileEvent;
            Plg.PlayFile -= Plg_PlayFile;
            Plg.RendererDisposeRequest -= Plg_RendererDisposeRequest;
            Plg.OrderEnded -= Plg_OrderEnded;
            Plg.Dispose();
        }

        public override ListItem[] GetPageItems()
        {
            LAPP.ListItems.ListItem[] lis = Plg.GetPageItems();
            if (lis == null) return null;
            ListItem[] olis = new ListItem[lis.Length];
            for (int i = 0; olis.Length > i; i++) olis[i] = GetItemFromPlugin(lis[i]);
            return olis;
        }

        public override ListItem[] GetTopPageItems()
        {
            LAPP.ListItems.ListItem[] lis = Plg.GetTopPageItems();
            if (lis == null) return null;
            ListItem[] olis = new ListItem[lis.Length];
            for (int i = 0; olis.Length > i; i++) olis[i] = GetItemFromPlugin(lis[i]);
            return olis;
        }

        public override void PlayAnyFile()
        {
            Plg.PlayAnyFile();
        }

        public override void Update()
        {
            Plg.Update();
        }

        public ListItem GetItemFromPlugin(LAPP.ListItems.ListItem Item)
        {
            if(Item is LAPP.ListItems.ListSubItem)
            {
                LAPP.ListItems.ListSubItem blsi = Item as LAPP.ListItems.ListSubItem;
                ListSubItem lsi = new ListSubItem(Item.IncludeSearchTarget, Item.ExcludeResult);
                lsi.TitleLabelVisible = blsi.TitleLabelVisible;
                lsi.MainLabelText = blsi.MainLabelText;
                lsi.SubLabelText = blsi.SubLabelText;
                lsi.StatusLabelText = blsi.StatusLabelText;
                lsi.NumberLabelText = blsi.NumberLabelText;
                lsi.SubLabelVisibility = blsi.SubLabelVisibility;
                lsi.StatusLabelVisibility = blsi.StatusLabelVisibility;
                lsi.LeftItem = (ListSubItem.LeftItems)blsi.LeftItem;
                lsi.ShapeItem = blsi.ShapeItem;
                lsi.Stretch = blsi.Stretch;
                lsi.StretchDirection = blsi.StretchDirection;

                return lsi;
            }

            return null;
        }
    }
}
