using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using LAPP;
using LAPP.IO;

namespace LAP.Page
{
    class Manager : IDisposable
    {
        private LAPP.Page PlayingPage;

        public event EventHandler<RunFileEventArgs> RunFile;
        public event EventHandler Stop;

        public Manager(ClearUC.ListView LV, ClearUC.Tab Tab)
        {
            this.LV = LV;
            this.Tab = Tab;

            Tab.ActiveItemChanged += Tab_ActiveItemChanged;
            Pages.CollectionChanged += Pages_CollectionChanged;
        }

        private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LAPP.Page page;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    page = (LAPP.Page)e.NewItems[0];
                    page.RunFile += Page_RunFile;
                    page.ClearPageRequested += Page_ClearPageRequested;
                    page.PageItemChanged += Page_PageItemChanged;
                    page.OrderEnded += Page_OrderEnded;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    page = (LAPP.Page)e.OldItems[0];
                    page.RunFile -= Page_RunFile;
                    page.ClearPageRequested -= Page_ClearPageRequested;
                    page.PageItemChanged -= Page_PageItemChanged;
                    page.OrderEnded -= Page_OrderEnded;
                    break;
            }

            UpdateTab();
        }

        private void Page_OrderEnded(object sender, EventArgs e)
        {
            Stop?.Invoke(this, new EventArgs());
        }

        private void Page_PageItemChanged(object sender, BasePage<PageItem, ItemSelectedEventArgs, PageItemCollection>.PageItemEventArgs e)
        {
            switch (e.ItemAction)
            {
                case BasePage<PageItem, ItemSelectedEventArgs, PageItemCollection>.PageItemEventArgs.Action.Add:
                    if (e.Index > -1)
                        LV.Items.Insert(e.Index, e.Item.ListItem);
                    else
                        LV.Items.Add(e.Item.ListItem);
                    break;
                case BasePage<PageItem, ItemSelectedEventArgs, PageItemCollection>.PageItemEventArgs.Action.Remove:
                    if (e.Index > -1)
                        LV.Items.RemoveAt(e.Index);
                    else
                        LV.Items.Remove(e.Item.ListItem);
                    break;
                case BasePage<PageItem, ItemSelectedEventArgs, PageItemCollection>.PageItemEventArgs.Action.Clear:
                    LV.Items.Clear();
                    break;
            }
        }

        private void Page_ClearPageRequested(object sender, EventArgs e)
        {
            LV.Items.Clear();
        }

        private void Page_RunFile(object sender, RunFileEventArgs e)
        {
            for(int i = 0;Pages.Count > i; i++)
            {
                Pages[i].PlaybackStateChanged(NAudio.Wave.PlaybackState.Stopped);
            }

            RunFile?.Invoke(sender, e);
            if (e.Success)
                PlayingPage = (LAPP.Page)sender;
            else
                PlayingPage = null;
        }

        private int LastActiveIndex { get; set; } = -1;
        private void Tab_ActiveItemChanged(object sender, EventArgs e)
        {
            if (Tab.ActiveIndex > -1)
            {
                if (Tab.ActiveIndex != LastActiveIndex)
                {
                    Pages[Tab.ActiveIndex].Update();
                    LastActiveIndex = Tab.ActiveIndex;
                }

                PageItemCollection items;
                LV.Items.Clear();

                items = Pages[Tab.ActiveIndex].GetItems(Level.Current);
                if (items != null)
                {
                    LV.Items.AddRange(items.GetListItems());
                    items.ItemClicked += (obj, args) =>
                    {
                        LAPP.Page cur = GetCurrentPage();
                        cur?.ItemClicked(args.ClickedItem, args.ParentEventArgs);
                    };
                }

                Player.RaiseReceivedEvent(new Player.EventReceiveArgs(Player.Action.TabIndexChanged, Tab.ActiveIndex));
            }
            else
                LV.Items.Clear();
        }

        private ClearUC.ListView LV { get; set; }
        private ClearUC.Tab Tab { get; set; }

        public void SetTopPage()
        {
            LV.Items.Clear();
            LAPP.Page page = GetCurrentPage();
            if (page != null)
                LV.Items.AddRange(page.GetItems(Level.Top).GetListItems());
        }

        public LAPP.Page GetCurrentPage()
        {
            if (Tab.Items.Count > 0 && Tab.ActiveIndex > -1)
                return Pages[Tab.ActiveIndex];
            else
                return null;
        }

        PageCollection pc = new PageCollection(false);
        public PageCollection Pages
        {
            get { return pc; }
        }

        internal void UpdateTab()
        {
            LastActiveIndex = -1;
            Tab.Items.Clear();
            for(int i = 0;Pages.Count > i; i++)
            {
                Tab.Items.Add(new ClearUC.Tab.TabItem(Pages[i].Title, Pages[i].Border));
            }
        }

        public void PlaybackStateChanged(NAudio.Wave.PlaybackState PlaybackState)
        {
            PlayingPage?.PlaybackStateChanged(PlaybackState);
        }

        public void PlayNext()
        {
            PlayingPage?.PlayNext();
        }

        public void PlayLast()
        {
            PlayingPage?.PlayLast();
        }

        public void Dispose()
        {
            Tab.ActiveItemChanged -= Tab_ActiveItemChanged;
            Pages.Clear();
        }

        private bool shuffle = false, loop = false;
        public bool Shuffle
        {
            get { return shuffle; }
            set
            {
                shuffle = value;
                SetCommonProperties(value, Loop);
            }
        }

        public bool Loop
        {
            get { return loop; }
            set
            {
                loop = value;
                SetCommonProperties(Shuffle, value);
            }
        }

        private void SetCommonProperties(bool Shuffle, bool Loop)
        {
            for(int i = 0;Pages.Count > i; i++)
            {
                Pages[i].Shuffle = Shuffle;
                Pages[i].Loop = Loop;
            }
        }
    }
}
