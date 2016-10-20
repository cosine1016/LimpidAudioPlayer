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
            Plg.GetTagEvent += Plg_GetTagEvent;

            ItemSelected += Page_ItemSelected;
        }

        private void Page_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            Plg.ItemClicked(e.Index, e.Item);
        }

        private void Plg_GetTagEvent(object sender, LAPP.Utils.ReturnableEventArgs<string, LAPP.MTag.TagEx> e)
        {
            e.Return = GetTag(e.Value);
        }

        private void Plg_OnPlayFileEvent(object sender, OnPlayFileEventArgs e)
        {
            OnPlayFile(e.Files, e.Index);
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
            OnPlayFile(new PlayFileEventArgs(e.File));
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
            Plg.GetTagEvent -= Plg_GetTagEvent;
            Plg.Dispose();
        }

        public override ListItem[] GetPageItems()
        {
            return Plg.GetPageItems();
        }

        public override ListItem[] GetTopPageItems()
        {
            return Plg.GetTopPageItems();
        }

        public override void PlayAnyFile()
        {
            Plg.PlayAnyFile();
        }

        public override void Update()
        {
            Plg.Update();
        }
    }
}
