using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// ListItem.xaml の相互作用ロジック
    /// </summary>
    public partial class ListItem : UserControl
    {
        public class ItemClickedEventArgs : EventArgs
        {
            public ItemClickedEventArgs(ListItem Item, int Index, MouseButtonEventArgs MouseButtonEventArgs)
            {
                this.Item = Item;
                this.Index = Index;
                this.MouseButtonEventArgs = MouseButtonEventArgs;
            }

            public int Index { get; private set; }

            public ListItem Item { get; private set; }

            public MouseButtonEventArgs MouseButtonEventArgs { get; set; }
        }

        public class ImageSourceList : List<ImageSource> { }

        internal event EventHandler<ItemClickedEventArgs> ItemClicked;

        protected virtual void OnItemClicked(ItemClickedEventArgs e)
        {
            ItemClicked?.Invoke(this, e);
        }

        public event EventHandler IndexChanged;

        protected virtual void OnIndexChanged(EventArgs e)
        {
            IndexChanged?.Invoke(this, e);
        }

        public event EventHandler ImageIndexChanged;

        protected virtual void OnImageIndexChanged(EventArgs e)
        {
            ImageIndexChanged?.Invoke(this, e);
        }

        public ListItem(bool IncludeSearchTarget, bool ExcludeResult)
        {
            this.IncludeSearchTarget = IncludeSearchTarget;
            this.ExcludeResult = ExcludeResult;
            Init(null);
        }

        public ListItem()
        {
            Init(null);
        }

        public ListItem(ImageSourceList ImageSources)
        {
            Init(ImageSources);
        }

        private void Init(ImageSourceList ImageSources)
        {
            MouseLeave += ListItem_MouseLeave;
            MouseDown += ListItem_MouseDown;
            MouseUp += ListItem_MouseUp;
            ItemStatus = State.Instance;
            this.ImageSources = ImageSources;
        }

        private void ListItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (downf == true)
            {
                OnItemClicked(new ItemClickedEventArgs(this, Index, e));
            }
        }

        private void ListItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            downf = true;
        }

        private void ListItem_MouseLeave(object sender, MouseEventArgs e)
        {
            downf = false;
        }

        private int ind = -1;
        private int ii = -1;
        private bool downf = false;

        public int Index
        {
            get { return ind; }
            internal set
            {
                ind = value;
                OnIndexChanged(new EventArgs());
            }
        }

        public ImageSourceList ImageSources { get; set; } = null;

        public int ImageIndex
        {
            get { return ii; }
            set
            {
                ii = value;
                OnImageIndexChanged(new EventArgs());
            }
        }

        public string SearchText { get; set; } = "";

        public bool IncludeSearchTarget { get; set; } = false;

        public bool ExcludeResult { get; set; } = true;

        public object Data { get; set; }

        public Type DataType { get; set; }

        public enum State
        {
            Adding, Added, Removing, Removed, Hide, Instance
        }

        public State ItemStatus { get; set; } = State.Instance;
    }
}