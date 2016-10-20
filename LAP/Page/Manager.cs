using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ClearUC.ListViewItems;

namespace LAP.Page
{
    public class ItemSelectedEventArgs : EventArgs
    {
        public ItemSelectedEventArgs(int Index, ClearUC.ListViewItems.ListItem Item)
        {
            this.Index = Index;
            this.Item = Item;
        }

        public int Index { get; set; } = -1;

        public ClearUC.ListViewItems.ListItem Item { get; set; } = null;
    }

    public abstract class ListViewPage : Page<ListItem, ItemSelectedEventArgs>
    {
        public event EventHandler OrderEnded;

        public event EventHandler<PlayFileEventArgs> PlayFile;

        public event EventHandler RendererDisposeRequest;

        public event EventHandler<LAPP.Utils.ReturnableEventArgs<string, LAPP.MTag.TagEx>> GetTagEvent;

        public bool Loop { get; set; } = false;
        public bool Opened { get; set; } = false;
        public List<LAPP.MTag.File> Order { get; protected set; } = new List<LAPP.MTag.File>();

        public bool Search { get; protected set; } = true;
        public bool Playing { get; private set; } = false;
        public int PlayingIndex { get; private set; } = -1;
        public bool Shuffle { get; set; } = false;
        public List<LAPP.MTag.File> ShuffledOrder { get; private set; } = new List<LAPP.MTag.File>();

        /// <summary>
        /// オーダーをすべて初期化した後
        /// ファイルを再生します。
        /// </summary>
        /// <param name="File">ファイル</param>
        public void OnPlayFile(LAPP.MTag.File[] Files, int Index)
        {
            MakeOrder(Files, Index);

            RendererDisposeRequest?.Invoke(this, new EventArgs());

            Playing = true;
            
            PlayFile?.Invoke(this, new PlayFileEventArgs(Order[Index]));
        }

        public void OnStopFile()
        {
            Playing = false;
        }

        public void OnPlayLast()
        {
            if (PlayingIndex == 0)
            {
                if (Loop)
                {
                    PlayingIndex = Order.Count - 1;
                    OrderEnded?.Invoke(this, new EventArgs());
                }
                else
                {
                    OrderEnded?.Invoke(this, new EventArgs());
                    return;
                }
            }
            else
                PlayingIndex--;

            RendererDisposeRequest?.Invoke(this, new EventArgs());

            Playing = true;

            if (Shuffle) PlayFile?.Invoke(this, new PlayFileEventArgs(ShuffledOrder[PlayingIndex]));
            else PlayFile?.Invoke(this, new PlayFileEventArgs(Order[PlayingIndex]));
        }

        protected LAPP.MTag.TagEx GetTag(string FilePath)
        {
            LAPP.Utils.ReturnableEventArgs<string, LAPP.MTag.TagEx> args
                = new LAPP.Utils.ReturnableEventArgs<string, LAPP.MTag.TagEx>(FilePath);
            GetTagEvent?.Invoke(this, args);
            return args.Return;
        }

        protected void OnRendererDisposeRequest()
        {
            RendererDisposeRequest?.Invoke(this, new EventArgs());
        }

        protected void OnOrderEnded()
        {
            OrderEnded?.Invoke(this, new EventArgs());
        }

        protected void OnPlayFile(PlayFileEventArgs e)
        {
            PlayFile?.Invoke(this, e);
        }

        public void OnPlayNext()
        {
            if (PlayingIndex == Order.Count - 1)
            {
                if (Loop)
                {
                    PlayingIndex = 0;
                    OrderEnded?.Invoke(this, new EventArgs());
                }
                else
                {
                    OrderEnded?.Invoke(this, new EventArgs());
                    return;
                }
            }
            else
                PlayingIndex++;

            RendererDisposeRequest?.Invoke(this, new EventArgs());

            Playing = true;

            if (Shuffle) PlayFile?.Invoke(this, new PlayFileEventArgs(ShuffledOrder[PlayingIndex]));
            else PlayFile?.Invoke(this, new PlayFileEventArgs(Order[PlayingIndex]));
        }

        public abstract void PlayAnyFile();

        protected void MakeOrder(LAPP.MTag.File[] Files, int Index)
        {
            Order.Clear();
            Order.AddRange(Files);

            PlayingIndex = Index;

            ShuffledOrder.Clear();

            LAPP.MTag.File[] sorder = Order.OrderBy(i => Guid.NewGuid()).ToArray();
            {
                for (int i = 0; sorder.Length > i; i++)
                    if (sorder[i] == Order[PlayingIndex])
                    {
                        LAPP.MTag.File bk = sorder[0];
                        sorder[0] = Order[PlayingIndex];
                        sorder[i] = bk;

                        ShuffledOrder.AddRange(sorder);
                        return;
                    }
            }

            throw new Exception("シャッフルオーダーの作成に失敗しました。");
        }
    }

    public class Manager : IDisposable
    {
        private ClearUC.ListView LV;

        private ClearUC.ListView QV;

        private ClearUC.Tab Tab;

        private Utils.BackgroundScanner bs = new Utils.BackgroundScanner();

        public Manager(ClearUC.ListView ListView, ClearUC.ListView QueueView, ClearUC.Tab Tab)
        {
            LV = ListView;
            QV = QueueView;
            this.Tab = Tab;
            LV.ItemClicked += LV_ItemClicked;
            Pages.CollectionChanged += Pages_CollectionChanged;
            this.Tab.ActiveItemChanged += Tab_ActiveItemChanged;
        }

        private void Tab_ActiveItemChanged(object sender, EventArgs e)
        {
            if (Pages[Tab.ActiveIndex].Search)
                LV.SearchBoxVisible = true;
            else
                LV.SearchBoxVisible = false;
        }

        public event EventHandler<PlayFileEventArgs> PlayFile;

        public event EventHandler RendererDisposeRequest;

        public event EventHandler OrderEnded;

        public void OnPlayStateChanged(NWrapper.Audio.Status Status, LAPP.MTag.File File)
        {
            int Index = -1;
            ListViewPage lvp = GetPlayingPage(out Index);
            switch (Status)
            {
                case NWrapper.Audio.Status.Stopped:
                    for (int i = 0; Pages.Count > i; i++) Pages[i].OnStopFile();
                    UpdateQueue(true, File, Status);
                    break;
                default:
                    if (Tab.ActiveIndex == Index && QV.Items.Count > 0)
                        UpdateQueue(false, File, Status);
                    else
                        UpdateQueue(true, File, Status);
                    break;
            }
        }

        private void UpdateQueue(bool UpdateOrder, LAPP.MTag.File File, NWrapper.Audio.Status Status)
        {
            if (UpdateOrder)
            {
                QV.Items.Clear();
                LAPP.MTag.File[] Files = GetOrder();
                if(Files != null)
                {
                    for(int i = 0;Files.Length > i; i++)
                    {
                        QV.Items.Add(GetItemFromFile(Files[i]));
                    }
                }
            }

            if (File == null) return;

            for(int i = 0;QV.Items.Count > i; i++)
            {
                ListSubItem lsi = QV.Items[i] as ListSubItem;
                if(lsi != null)
                {
                    lsi.StatusLabelText = "";
                    LAPP.MTag.File itemf = lsi.Data as LAPP.MTag.File;
                    if(itemf != null)
                    {
                        if(itemf.Path == File.Path)
                        {
                            switch (Status)
                            {
                                case NWrapper.Audio.Status.Playing:
                                    lsi.StatusLabelText = Utils.Config.Language.Strings.Status.Playing;
                                    break;
                                case NWrapper.Audio.Status.Paused:
                                    lsi.StatusLabelText = Utils.Config.Language.Strings.Status.Pause;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private ListSubItem GetItemFromFile(LAPP.MTag.File File)
        {
            ListSubItem lsi = new ListSubItem();

            lsi.StatusLabelVisibility = System.Windows.Visibility.Visible;

            lsi.Data = File;
            lsi.DataType = File.GetType();

            if (string.IsNullOrEmpty(File.Tag.Title))
                lsi.MainLabelText = System.IO.Path.GetFileNameWithoutExtension(File.Path);
            else
                lsi.MainLabelText = File.Tag.Title;

            lsi.SubLabelVisibility = System.Windows.Visibility.Collapsed;

            return lsi;
        }

        public bool Loop { get; set; } = false;

        public PageCollection Pages { get; set; }
            = new PageCollection();

        public bool Shuffle { get; set; } = false;

        public void Dispose()
        {
            for (int i = 0; Pages.Count > i; i++) Pages[i].Dispose();
        }

        public LAPP.MTag.File[] GetOrder()
        {
            ListViewPage lvp = GetPlayingPage();
            if (lvp != null) return lvp.Order.ToArray();

            return null;
        }

        private ListViewPage GetPlayingPage(out int Index)
        {
            Index = -1;
            for (int i = 0; Pages.Count > i; i++)
            {
                if (Pages[i].Playing)
                {
                    Index = i;
                    return Pages[i];
                }
            }

            return null;
        }

        private ListViewPage GetPlayingPage()
        {
            int index = 0;
            return GetPlayingPage(out index);
        }

        public void PlayLastFile()
        {
            for (int i = 0; Pages.Count > i; i++)
            {
                if (Pages[i].Playing)
                {
                    Pages[i].Shuffle = Shuffle;
                    Pages[i].Loop = Loop;
                    Pages[i].OnPlayLast();
                    return;
                }
            }

            Pages[Tab.ActiveIndex].PlayAnyFile();
        }

        public void PlayNextFile()
        {
            for (int i = 0; Pages.Count > i; i++)
            {
                if (Pages[i].Playing)
                {
                    Pages[i].Shuffle = Shuffle;
                    Pages[i].Loop = Loop;
                    Pages[i].OnPlayNext();
                    return;
                }
            }

            Pages[Tab.ActiveIndex].PlayAnyFile();
        }

        private void ClearPageRequested(object sender, EventArgs e)
        {
            LV.Items.Clear();
        }

        private void LV_ItemClicked(object sender, ClearUC.ListViewItems.ListItem.ItemClickedEventArgs e)
        {
            Pages[Tab.ActiveIndex].OnItemSelected(new ItemSelectedEventArgs(e.Index, e.Item));
        }

        private void NewItem_PlayFile(object sender, PlayFileEventArgs e)
        {
            PlayFile?.Invoke(this, e);
        }

        private void PageItemChanged(object sender, ListViewPage.PageItemEventArgs e)
        {
            switch (e.ItemAction)
            {
                case ListViewPage.PageItemEventArgs.Action.Add:
                    if (e.Index > -1) LV.Items.Insert(e.Index, e.Item);
                    else LV.Items.Add(e.Item);
                    break;

                case ListViewPage.PageItemEventArgs.Action.Remove:
                    LV.Items.Remove(e.Item);
                    break;

                case ListViewPage.PageItemEventArgs.Action.Clear:
                    for (int i = 0; e.Items.Length > i; i++) LV.Items.Remove(e.Items[i]);
                    break;
            }
        }

        private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ListViewPage NewItem = (ListViewPage)e.NewItems[0];
                    NewItem.PageItemChanged += PageItemChanged;
                    NewItem.ClearPageRequested += ClearPageRequested;
                    NewItem.PlayFile += NewItem_PlayFile;
                    NewItem.RendererDisposeRequest += NewItem_RendererDisposeRequest;
                    NewItem.OrderEnded += NewItem_OrderEnded;
                    NewItem.GetTagEvent += NewItem_GetTagEvent;

                    Tab.Items.Add(new ClearUC.Tab.TabItem(NewItem.Title, NewItem.Border));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    ListViewPage OldItem = (ListViewPage)e.OldItems[0];
                    OldItem.PageItemChanged -= PageItemChanged;
                    OldItem.ClearPageRequested -= ClearPageRequested;
                    OldItem.PlayFile -= NewItem_PlayFile;
                    OldItem.RendererDisposeRequest -= NewItem_RendererDisposeRequest;
                    OldItem.OrderEnded -= NewItem_OrderEnded;
                    OldItem.GetTagEvent -= NewItem_GetTagEvent;
                    OldItem.Dispose();
                    Tab.Items.RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Tab.Items.Clear();
                    if(e.OldItems != null)
                    {
                        ListViewPage[] OldItems = (ListViewPage[])e.OldItems;
                        for (int i = 0; OldItems.Length > i; i++)
                        {
                            OldItems[i].PageItemChanged -= PageItemChanged;
                            OldItems[i].ClearPageRequested -= ClearPageRequested;
                            OldItems[i].PlayFile -= NewItem_PlayFile;
                            OldItems[i].RendererDisposeRequest -= NewItem_RendererDisposeRequest;
                            OldItems[i].OrderEnded -= NewItem_OrderEnded;
                            OldItems[i].GetTagEvent -= NewItem_GetTagEvent;
                            OldItems[i].Dispose();
                        }
                    }
                    break;
            }
        }

        private void NewItem_GetTagEvent(object sender, LAPP.Utils.ReturnableEventArgs<string, LAPP.MTag.TagEx> e)
        {
            e.Return = bs.GetTag(e.Value);
        }

        private void NewItem_OrderEnded(object sender, EventArgs e)
        {
            OrderEnded?.Invoke(sender, e);
        }

        private void NewItem_RendererDisposeRequest(object sender, EventArgs e)
        {
            RendererDisposeRequest?.Invoke(sender, e);
        }

        public LAPP.MTag.TagEx GetTag(string FilePath)
        {
            return bs.GetTag(FilePath);
        }
    }

    public abstract class Page<T, TSelected> : IDisposable
    {
        public enum PlayingState { Play, Pause };

        internal event EventHandler ClearPageRequested;

        internal event EventHandler<TSelected> ItemSelected;

        internal event EventHandler<PageItemEventArgs> PageItemChanged;

        public abstract System.Windows.Controls.Border Border { get; protected set; }

        public abstract string Title { get; protected set; }

        public bool Visible { get; set; } = true;

        public abstract void Dispose();

        public abstract T[] GetPageItems();

        public abstract T[] GetTopPageItems();

        public abstract void Update();

        internal virtual void OnItemSelected(TSelected SelectedItem)
        {
            ItemSelected?.Invoke(this, SelectedItem);
        }

        protected virtual void Add(T Item)
        {
            PageItemChanged?.Invoke(this, new PageItemEventArgs(PageItemEventArgs.Action.Add, Item));
        }

        protected virtual void Remove(T Item)
        {
            PageItemChanged?.Invoke(this, new PageItemEventArgs(PageItemEventArgs.Action.Remove, Item));
        }

        protected virtual void Insert(T Item, int Index)
        {
            PageItemChanged?.Invoke(this, new PageItemEventArgs(PageItemEventArgs.Action.Add, Item, Index));
        }

        protected virtual void RequestClearPage()
        {
            ClearPageRequested?.Invoke(this, new EventArgs());
        }

        public class PageItemEventArgs
        {
            internal PageItemEventArgs(Action ItemAction, T[] Items)
            {
                this.ItemAction = ItemAction;
                this.Items = Items;
                Index = -1;
            }

            internal PageItemEventArgs(Action ItemAction, T Item)
            {
                this.ItemAction = ItemAction;
                this.Item = Item;
                Index = -1;
            }

            internal PageItemEventArgs(Action ItemAction, T Item, int Index)
            {
                this.ItemAction = ItemAction;
                this.Item = Item;
                this.Index = Index;
            }

            internal PageItemEventArgs(Action ItemAction)
            {
                this.ItemAction = ItemAction;
                this.Item = Item;
            }

            public enum Action { Add, Remove, Clear }

            public T Item { get; set; }
            public Action ItemAction { get; set; }
            public T[] Items { get; set; }
            public int Index { get; set; }
        }
    }

    public class PageCollection : ObservableCollection<ListViewPage>
    {
        public void AddRange(ListViewPage[] items)
        {
            for (int i = 0; items.Length > i; i++)
            {
                Add(items[i]);
            }
        }
    }

    public class PlayFileEventArgs : EventArgs
    {
        public LAPP.MTag.File File;

        public PlayFileEventArgs(LAPP.MTag.File File)
        {
            this.File = File;
        }
    }
}