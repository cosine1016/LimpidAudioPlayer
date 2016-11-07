using ClearUC.ListViewItems;
using LAPP.IO;
using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;

namespace LAPP
{
    public class PageItemCollection : ObservableCollection<PageItem>
    {
        public class ClickedEventArgs : EventArgs
        {
            public ClickedEventArgs(PageItem Item, PageItemClickedEventArgs ParentEventArgs)
            {
                ClickedItem = Item;
                this.ParentEventArgs = ParentEventArgs;
            }

            public PageItem ClickedItem { get; set; }

            public PageItemClickedEventArgs ParentEventArgs { get; set; }
        }

        public event EventHandler<ClickedEventArgs> ItemClicked;

        public PageItemCollection()
        {
            CollectionChanged += PageItemCollection_CollectionChanged;
        }

        private void PageItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PageItem item;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    item = (PageItem)e.NewItems[0];
                    item.ItemClicked += Item_ItemClicked;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    item = (PageItem)e.OldItems[0];
                    item.ItemClicked -= Item_ItemClicked;
                    break;
            }
        }

        private void Item_ItemClicked(object sender, PageItemClickedEventArgs e)
        {
            ItemClicked?.Invoke(this, new ClickedEventArgs((PageItem)sender, e));
        }

        public void AddRange(PageItem[] items)
        {
            for (int i = 0; items.Length > i; i++)
                Add(items[i]);
        }

        public new void Clear()
        {
            for(int i = 0;Count > i; i++)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this[i]));
            }

            base.Clear();
        }

        public int[] GetFileItemIndexes()
        {
            List<int> indexes = new List<int>(Count);
            for (int i = 0; Count > i; i++)
            {
                FileItem item = Items[i] as FileItem;
                if (item != null && item.File != null)
                    indexes.Add(i);
            }

            return indexes.ToArray();
        }

        public ListItem[] GetListItems()
        {
            ListItem[] li = new ListItem[Count];
            for (int i = 0; Count > i; i++)
                li[i] = this[i].ListItem;

            return li;
        }
    }

    public class PageItem
    {
        protected PageItem() { }
        public PageItem(ListItem ListItem)
        {
            this.ListItem = ListItem;
            ListItem.ItemClicked += ListItem_ItemClicked;
        }

        private void ListItem_ItemClicked(object sender, ClearUC.ItemClickedEventArgs e)
        {
            ItemClicked?.Invoke(this, new PageItemClickedEventArgs(e));
        }

        public ListItem ListItem { get; set; }

        public event EventHandler<PageItemClickedEventArgs> ItemClicked;

        public static implicit operator PageItem(ListItem Item)
        {
            return new PageItem(Item);
        }
    }

    public class PageItemClickedEventArgs : EventArgs
    {
        public PageItemClickedEventArgs(ClearUC.ItemClickedEventArgs e)
        {
            ParentEventArgs = e;
        }

        public ClearUC.ItemClickedEventArgs ParentEventArgs;
    }

    public class ItemSelectedEventArgs : EventArgs
    {
        public ItemSelectedEventArgs(int Index, FileItem FileItem)
        {
            this.Index = Index;
            this.FileItem = FileItem;
        }

        public FileItem FileItem { get; set; } = null;
        public int Index { get; set; } = -1;
    }

    public abstract class BasePage<T, TSelected, TCollection> : IDisposable
    {
        public event EventHandler ClearPageRequested;

        public event EventHandler<TSelected> ItemSelected;

        public event EventHandler<PageItemEventArgs> PageItemChanged;

        public abstract Border Border { get; protected set; }

        public abstract string Title { get; protected set; }

        public bool Visible { get; set; } = true;

        public abstract void Dispose();

        public abstract PageItemCollection GetItems(Level PageLevel);

        /// <summary>
        /// ページがアクティブ化される前に呼び出されます
        /// </summary>
        public abstract void Update();

        internal virtual void OnItemSelected(TSelected SelectedItem)
        {
            ItemSelected?.Invoke(this, SelectedItem);
        }

        /// <summary>
        /// ページにアイテムを追加します
        /// </summary>
        /// <param name="Item"></param>
        protected virtual void Add(T Item)
        {
            PageItemChanged?.Invoke(this, new PageItemEventArgs(PageItemEventArgs.Action.Add, Item));
        }

        /// <summary>
        /// ページにアイテムを挿入します
        /// </summary>
        /// <param name="Item">挿入するアイテム</param>
        /// <param name="Index">挿入するインデックス</param>
        protected virtual void Insert(int Index, T Item)
        {
            PageItemChanged?.Invoke(this, new PageItemEventArgs(PageItemEventArgs.Action.Add, Item, Index));
        }

        /// <summary>
        /// ページからアイテムを削除します
        /// </summary>
        /// <param name="Item">削除するアイテム</param>
        protected virtual void Remove(T Item)
        {
            PageItemChanged?.Invoke(this, new PageItemEventArgs(PageItemEventArgs.Action.Remove, Item));
        }

        /// <summary>
        /// 指定されたインデックスのアイテムを削除します
        /// </summary>
        protected virtual void RemoveAt(int Index)
        {
            PageItemChanged?.Invoke(this, new PageItemEventArgs(PageItemEventArgs.Action.Remove, Index));
        }

        /// <summary>
        /// ページの初期化を要求します
        /// </summary>
        protected virtual void RequestClearPage()
        {
            ClearPageRequested?.Invoke(this, new EventArgs());
        }

        public class PageItemEventArgs
        {
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

            internal PageItemEventArgs(Action ItemAction, int Index)
            {
                this.ItemAction = ItemAction;
                this.Index = Index;
            }

            internal PageItemEventArgs(Action ItemAction)
            {
                this.ItemAction = ItemAction;
                this.Item = Item;
            }

            public enum Action { Add, Remove, Clear }

            public int Index { get; set; }
            public T Item { get; set; }
            public Action ItemAction { get; set; }
        }
    }

    public enum Level { Top, Current }

    public class PageCollection : ICollection<Page>, INotifyCollectionChanged, IList<Page>
    {
        public PageCollection(bool LeaveOpen)
        {
            this.LeaveOpen = LeaveOpen;
        }

        public Page this[int i]
        {
            get { return pages[i]; }
            set { pages[i] = value; }
        }

        public bool LeaveOpen { get; private set; }

        List<Page> pages = new List<Page>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count { get { return pages.Count; } }

        public bool IsReadOnly { get; } = false;

        public void Add(Page item)
        {
            pages.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void AddRange(IList<Page> items)
        {
            for(int i = 0;items.Count > i; i++)
            {
                Add(items[i]);
            }
        }

        public void Clear()
        {
            if(!LeaveOpen)
                for (int i = 0; pages.Count > i; i++)
                {
                    pages[i].Dispose();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pages[i]));
                }
            
            pages.Clear();
        }

        public bool Contains(Page item)
        {
            return pages.Contains(item);
        }

        public void CopyTo(Page[] array, int arrayIndex)
        {
            pages.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Page> GetEnumerator()
        {
            return pages.GetEnumerator();
        }

        public bool Remove(Page item)
        {
            bool s = pages.Remove(item);
            if (s && !LeaveOpen)
                item.Dispose();

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

            return s;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pages.GetEnumerator();
        }

        public int IndexOf(Page item)
        {
            for(int i = 0;pages.Count > i; i++)
            {
                if (item == pages[i]) return i;
            }
            return -1;
        }

        public void Insert(int index, Page item)
        {
            pages.Insert(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void RemoveAt(int index)
        {
            pages.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
        }
    }

    public class RunFileEventArgs : EventArgs
    {
        public FileItem Item;

        public RunFileEventArgs(FileItem Item)
        {
            this.Item = Item;
        }
    }

    public abstract class Page : BasePage<PageItem, ItemSelectedEventArgs, PageItemCollection>
    {
        public event EventHandler<RunFileEventArgs> RunFile;
        public event EventHandler OrderEnded;

        private bool loop, shuffle;
        public bool Loop
        {
            get { return loop; }
            set
            {
                loop = value;
                if (man != null) man.Loop = value;
            }
        }
        public bool Shuffle
        {
            get { return shuffle; }
            set
            {
                shuffle = value;
                if (man != null) man.Shuffle = value;
            }
        }

        public bool Search { get; protected set; } = true;

        public void PlayAnyFile()
        {
            man.First();
            PlayFile();
        }
        public void PlayLast()
        {
            man.Last();
            PlayFile();
        }
        public void PlayNext()
        {
            man.Next();
            PlayFile();
        }

        public abstract void PlaybackStateChanged(NAudio.Wave.PlaybackState PlaybackState);

        private OrderManager man = null;
        protected void UpdateOrder(OrderManager Manager)
        {
            if(man != null)
            {
                Manager.OrderEnded -= Manager_OrderEnded;
                Manager.IndexChanged -= Manager_IndexChanged;
            }

            Manager.OrderEnded += Manager_OrderEnded;
            Manager.IndexChanged += Manager_IndexChanged;
            Manager.Loop = Loop;
            Manager.Shuffle = Shuffle;

            man = Manager;
        }

        protected FileItem GetPlayingItem()
        {
            if (man == null) return null;
            if (man.Index > -1)
                return man.GetFile();
            else
                return null;
        }

        private void Manager_IndexChanged(object sender, EventArgs e)
        {
        }

        private void Manager_OrderEnded(object sender, EventArgs e)
        {
            OrderEnded?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// アイテムがクリックされた際に呼び出されます
        /// </summary>
        public abstract void ItemClicked(PageItem Item, PageItemClickedEventArgs e);

        /// <summary>
        /// ファイルが再生される直前に呼び出されます。
        /// 必要な処理をこの関数内で実行してください
        /// </summary>
        /// <param name="FileItem">再生されるファイル</param>
        protected abstract void Initialize(FileItem FileItem);

        /// <summary>
        /// OrderManagerの現在位置からファイルを再生します。
        /// </summary>
        public void PlayFile()
        {
            FileItem File = man.GetFile();

            if(File != null)
            {
                Initialize(File);
                RunFile?.Invoke(this, new RunFileEventArgs(File));
            }
        }

        /// <summary>
        /// ファイルを再生します。
        /// </summary>
        public void PlayFile(FileItem File)
        {
            man.SetFile(File);
            if(man.Index > -1)
            {
                Initialize(File);
                RunFile?.Invoke(this, new RunFileEventArgs(File));
            }
        }
    }

    public abstract class ManageablePage : Page
    {
        public ManageablePage()
        {
            InitializeTopItems();
            Items = GetItems(Level.Top);
        }

        protected virtual void UpdatePage(Level Level)
        {
            RequestClearPage();

            PageItemCollection Items;
            if (Level == Level.Current) Items = this.Items;
            else Items = GetTopItems();

            for(int i = 0;Items.Count > i; i++)
            {
                Add(Items[i]);
            }
        }

        public ManageablePage(Border Border, string Title) : this()
        {
            this.Border = Border;
            this.Title = Title;
        }

        private int PlayingNumber { get; set; } = -1;

        protected virtual PageItemCollection Items { get; set; } = new PageItemCollection();

        public override void ItemClicked(PageItem Item, PageItemClickedEventArgs e)
        {
            if(e.ParentEventArgs.MouseButtonEventArgs.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                FileItem FItem = Item as FileItem;
                if (FItem != null)
                    PlayFile(FItem);
            }
        }

        protected override void Initialize(FileItem FileItem)
        {
            int index = Items.IndexOf(FileItem);
            PlayingNumber = index;
        }

        public override PageItemCollection GetItems(Level PageLevel)
        {
            if (PageLevel == Level.Top)
            {
                Items.Clear();
                Items.AddRange(GetTopItems().ToArray());
            }

            return Items;
        }

        protected virtual PageItem FindPageItem(PageItemCollection Items, ListItem ListItem)
        {
            for(int i = 0;Items.Count > i; i++)
            {
                if (Items[i].ListItem == ListItem)
                    return Items[i];
            }

            return null;
        }

        protected virtual PageItem FindPageItem(ListItem ListItem)
        {
            return FindPageItem(Items, ListItem);
        }
        
        protected abstract PageItemCollection GetTopItems();
        protected abstract void InitializeTopItems();
    }

    public class OrderManager
    {
        public event EventHandler IndexChanged;
        public event EventHandler OrderEnded;

        private int currentIndex = -1;

        public OrderManager() { }
        public OrderManager(PageItemCollection Items) { Scan(Items); }
        
        public void Scan(PageItemCollection Items)
        {
            ActualIndexes.Clear();
            Files.Clear();

            for (int i = 0; Items.Count > i; i++)
            {
                FileItem fi = Items[i] as FileItem;
                if (fi != null)
                {
                    if (fi.Playable && System.IO.File.Exists(fi.File.Path))
                    {
                        ActualIndexes.Add(i);
                        Files.Add(fi);
                    }
                }
            }

            ShuffledIndexes = Enumerable.Range(0, Files.Count).ToArray();

            Random rng = new Random();
            int n = Files.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int tmp = ShuffledIndexes[k];
                ShuffledIndexes[k] = ShuffledIndexes[n];
                ShuffledIndexes[n] = tmp;
            }

            Index = -1;
        }

        public int Index
        {
            get { return currentIndex; }
            set
            {
                if(Files.Count == 0)
                {
                    currentIndex = -1;
                    return;
                }

                if(currentIndex != value)
                {
                    currentIndex = value;

                    if (Files.Count - 1 < currentIndex)
                    {
                        if (Loop)
                            currentIndex = 0;
                        else
                            EndOfOrder();
                    }
                    else if(currentIndex < 0)
                    {
                        if (Loop)
                            currentIndex = Files.Count - 1;
                        else
                            EndOfOrder();
                    }

                    IndexChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public int First()
        {
            Index = 0;
            return Index;
        }

        public int Next()
        {
            Index++;
            return Index;
        }

        public int Last()
        {
            Index--;
            return Index;
        }

        private void EndOfOrder()
        {
            currentIndex = -1;
            OrderEnded?.Invoke(this, new EventArgs());
        }

        public bool Loop { get; set; }
        public bool Shuffle { get; set; }

        public FileItem GetFile()
        {
            if (Index < 0) return null;
            if (Shuffle)
                return Files[ShuffledIndexes[Index]];
            else
                return Files[Index];
        }

        public void SetFile(FileItem File)
        {
            for(int i = 0;Files.Count > i; i++)
            {
                if(File == Files[i])
                {
                    Index = i;
                    return;
                }
            }

            Index = -1;
        }

        /// <summary>
        /// コンストラクタで使用したコレクションのインデックスからファイルアイテムを取得します
        /// </summary>
        public FileItem FindItemFromIndex(int Index)
        {
            int ind = ActualIndexes[ActualIndexes.IndexOf(Index)];

            if (ind > -1)
                return Files[ind];
            else
                return null;
        }

        private List<FileItem> Files = new List<FileItem>();
        private List<int> ActualIndexes = new List<int>();
        private int[] ShuffledIndexes;
    }
}