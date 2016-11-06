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
        public event EventHandler ImageIndexChanged;

        public event EventHandler IndexChanged;

        public event EventHandler<ItemClickedEventArgs> ItemClicked;

        private bool downf = false;

        private int ii = -1;

        private int ind = -1;

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

        public enum State
        {
            Adding, Added, Removing, Removed, Hide, Instance
        }

        public bool ExcludeResult { get; set; } = true;

        public int ImageIndex
        {
            get { return ii; }
            set
            {
                ii = value;
                OnImageIndexChanged(new EventArgs());
            }
        }

        public ImageSourceList ImageSources { get; set; } = null;

        public bool IncludeSearchTarget { get; set; } = false;

        public int Index
        {
            get { return ind; }
            internal set
            {
                ind = value;
                OnIndexChanged(new EventArgs());
            }
        }

        public State ItemStatus { get; set; } = State.Instance;

        public string SearchText { get; set; } = "";

        protected virtual void OnImageIndexChanged(EventArgs e)
        {
            ImageIndexChanged?.Invoke(this, e);
        }

        protected virtual void OnIndexChanged(EventArgs e)
        {
            IndexChanged?.Invoke(this, e);
        }

        protected virtual void OnItemClicked(ItemClickedEventArgs e)
        {
            ItemClicked?.Invoke(this, e);
        }

        private void Init(ImageSourceList ImageSources)
        {
            MouseLeave += ListItem_MouseLeave;
            MouseDown += ListItem_MouseDown;
            MouseUp += ListItem_MouseUp;
            ItemStatus = State.Instance;
            this.ImageSources = ImageSources;
        }

        private void ListItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            downf = true;
        }

        private void ListItem_MouseLeave(object sender, MouseEventArgs e)
        {
            downf = false;
        }

        private void ListItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (downf == true)
            {
                OnItemClicked(new ItemClickedEventArgs(this, Index, e));
                downf = false;
            }
        }

        public class ImageSourceList : List<ImageSource> { }
    }
}