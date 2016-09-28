using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.Page
{
    public class ItemSelectedEventArgs : EventArgs
    {
        public ItemSelectedEventArgs(int Index, ListItems.ListItem Item)
        {
            this.Index = Index;
            this.Item = Item;
        }

        public int Index { get; set; } = -1;

        public ListItems.ListItem Item { get; set; } = null;
    }

    public class OnPlayFileEventArgs : EventArgs
    {
        public Utils.File[] Files;

        public int Index;
    }

    public abstract class Plugin : Page<ListItems.ListItem, ItemSelectedEventArgs>
    {
        public event EventHandler OrderEnded;

        public event EventHandler<PlayFileEventArgs> PlayFile;

        public event EventHandler RendererDisposeRequest;

        public event EventHandler<OnPlayFileEventArgs> OnPlayFileEvent;

        public bool Loop { get; set; } = false;
        public bool Opened { get; set; } = false;
        public List<Utils.File> Order { get; protected set; } = new List<Utils.File>();

        public bool Search { get; protected set; } = true;
        public bool Playing { get; private set; } = false;
        public int PlayingIndex { get; private set; } = -1;
        public bool Shuffle { get; set; } = false;
        public List<Utils.File> ShuffledOrder { get; private set; } = new List<Utils.File>();

        /// <summary>
        /// オーダーをすべて初期化した後
        /// ファイルを再生します。
        /// </summary>
        /// <param name="File">ファイル</param>
        public void OnPlayFile(Utils.File[] Files, int Index)
        {
            OnPlayFileEvent?.Invoke(this, new OnPlayFileEventArgs() { Files = Files, Index = Index });
            MakeOrder(Files, Index);

            RendererDisposeRequest?.Invoke(this, new EventArgs());

            Playing = true;

            if (Shuffle) PlayFile?.Invoke(this, new PlayFileEventArgs(ShuffledOrder[Index]));
            else PlayFile?.Invoke(this, new PlayFileEventArgs(Order[Index]));
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

        protected void MakeOrder(Utils.File[] Files, int Index)
        {
            Order.Clear();
            Order.AddRange(Files);

            PlayingIndex = Index;

            ShuffledOrder.Clear();

            Utils.File[] sorder = Order.OrderBy(i => Guid.NewGuid()).ToArray();
            {
                for (int i = 0; sorder.Length > i; i++)
                    if (sorder[i] == Order[PlayingIndex])
                    {
                        Utils.File bk = sorder[0];
                        sorder[0] = Order[PlayingIndex];
                        sorder[i] = bk;

                        ShuffledOrder.AddRange(sorder);
                        return;
                    }
            }

            throw new Exception("シャッフルオーダーの作成に失敗しました。");
        }
    }

    public abstract class Page<T, TSelected> : IDisposable
    {
        public enum PlayingState { Play, Pause };

        internal event EventHandler ClearPageRequested;

        internal event EventHandler<TSelected> ItemSelected;

        internal event EventHandler<PageItemEventArgs> PageItemChanged;

        public abstract System.Windows.Controls.Border Border { get; set; }

        public abstract string Title { get; set; }

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

    public class PageCollection : ObservableCollection<Plugin>
    {
        public void AddRange(Plugin[] items)
        {
            for (int i = 0; items.Length > i; i++)
            {
                Add(items[i]);
            }
        }
    }

    public class PlayFileEventArgs : EventArgs
    {
        public Utils.File File;

        public PlayFileEventArgs(Utils.File File)
        {
            this.File = File;
        }
    }
}
