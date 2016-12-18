using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// ListItem.xaml の相互作用ロジック
    /// </summary>
    public abstract partial class ListItem : Grid
    {
        public event EventHandler IndexChanged;

        public event EventHandler<ItemClickedEventArgs> ItemClicked;

        private bool downf = false;

        private int ii = -1;

        private int ind = -1;

        public ListItem(bool IncludeSearchTarget, bool ExcludeResult)
        {
            this.IncludeSearchTarget = IncludeSearchTarget;
            this.ExcludeResult = ExcludeResult;
            Init();
        }

        public ListItem()
        {
            Init();
        }

        public enum State
        {
            Adding, Added, Removing, Removed, Hide, Instance
        }

        public bool ExcludeResult { get; set; } = true;

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

        protected virtual void OnIndexChanged(EventArgs e)
        {
            IndexChanged?.Invoke(this, e);
        }

        protected virtual void OnItemClicked(ItemClickedEventArgs e)
        {
            ItemClicked?.Invoke(this, e);
        }

        protected virtual void OnAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == VerticalAlignmentProperty)
                OnAlignmentChanged(e);
            if (e.Property == HorizontalAlignmentProperty)
                OnAlignmentChanged(e);
            base.OnPropertyChanged(e);
        }

        private void Init()
        {
            MouseLeave += ListItem_MouseLeave;
            MouseDown += ListItem_MouseDown;
            MouseUp += ListItem_MouseUp;
            ItemStatus = State.Instance;
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
    }
}