using ClearUC.ListViewItems;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ClearUC
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

    public class ListItemCollection : System.Collections.ObjectModel.ObservableCollection<ListItem>
    {
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChangeNotice;

        public new void Add(ListItem item)
        {
            CollectionChangeNotice?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            base.Add(item);
        }

        public void AddRange(ListItem[] items)
        {
            for (int i = 0; items.Length > i; i++) Add(items[i]);
        }

        public new void Clear()
        {
            CollectionChangeNotice?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            base.Clear();
        }

        public new void Remove(ListItem item)
        {
            CollectionChangeNotice?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            base.Remove(item);
        }
    }

    /// <summary>
    /// ListView.xaml の相互作用ロジック
    /// </summary>
    public partial class ListView : UserControl
    {
        public event EventHandler<ItemClickedEventArgs> ItemClicked;

        public static readonly DependencyProperty SearchBoxVisibleProperty = DependencyProperty.Register("SearchBoxVisible", typeof(bool), typeof(ListView));

        public static readonly DependencyProperty SearchStyleProperty = DependencyProperty.Register("SearchStyle", typeof(SearchBox.SearchStyle), typeof(ListView), new PropertyMetadata(SearchBox.SearchStyle.Partly));

        public EventHandler<RoutedEventArgs> SearchBoxLostFocus;

        private Brush ibf;

        private Brush ibs;

        private ListViewItems.SearchBox SB = new ListViewItems.SearchBox();

        private bool sbadded = false;

        public ListView()
        {
            InitializeComponent();

            Items.CollectionChanged += Items_CollectionChanged;
            Items.CollectionChangeNotice += Items_CollectionChangeNotice;

            Color cl = new Color();
            cl.A = 1;
            cl.B = 0;
            cl.G = 0;
            cl.R = 0;
            ibf = new SolidColorBrush(cl);
            ibs = new SolidColorBrush(cl);
        }

        public double BackgroundFillOpacity
        {
            get { return bg.Fill.Opacity; }
            set { bg.Fill.Opacity = value; }
        }

        public double BackgroundStrokeOpacity
        {
            get { return bg.Stroke.Opacity; }
            set { bg.Stroke.Opacity = value; }
        }

        public IEasingFunction EasingFunction { get; set; } = new CircleEase();

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return sc.HorizontalScrollBarVisibility; }
            set { sc.HorizontalScrollBarVisibility = value; }
        }

        public ListItemCollection Items { get; set; } = new ListItemCollection();

        public double SearchBoxAnimationDuration { get; set; } = 100;

        public bool SearchBoxVisible
        {
            get
            {
                bool val = (bool)GetValue(SearchBoxVisibleProperty);
                return val;
            }
            set
            {
                bool ssb = SearchBoxVisible;
                SetValue(SearchBoxVisibleProperty, value);
                if (value == true)
                {
                    if (sbadded == false)
                    {
                        addsearchbox();
                    }
                }
                else
                {
                    sbadded = false;
                    SB.ClearText();
                    SB.TextChanged -= SB_TextChanged;
                    parent.Children.Remove(SB);
                    RefreshItems();
                }
            }
        }

        public SearchBox.SearchStyle SearchStyle
        {
            get { return (SearchBox.SearchStyle)GetValue(SearchStyleProperty); }
            set { SetValue(SearchStyleProperty, value); }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return sc.VerticalScrollBarVisibility; }
            set { sc.VerticalScrollBarVisibility = value; }
        }

        protected virtual void OnItemClicked(ItemClickedEventArgs e)
        {
            ItemClicked?.Invoke(this, e);
        }

        protected virtual void OnTextBoxLostFocus(RoutedEventArgs e)
        {
            SearchBoxLostFocus?.Invoke(this, e);
        }

        private void additem(ListItem Item)
        {
            if (Item.ItemStatus == ListItem.State.Removing)
                throw new Exception("既に利用されている項目です");
            Item.Width = parent.ActualWidth;
            Item.HorizontalAlignment = HorizontalAlignment.Stretch;
            Item.VerticalAlignment = VerticalAlignment.Top;
            Item.Width = double.NaN;

            if (Items.Count > 5)
            {
                Item.Margin = CalcMargin(Items.Count);
            }
            else
            {
                Thickness tk = CalcMargin(Items.Count);
                Item.Margin = new Thickness(parent.ActualWidth / Items.Count, tk.Top, tk.Right, tk.Bottom);
            }

            Item.ItemStatus = ListItem.State.Adding;
            Item.ItemClicked += Item_ItemClicked;
        }

        private void addsearchbox()
        {
            SB.Width = parent.ActualWidth;
            SB.Margin = CalcMargin(-1);

            SB.HorizontalAlignment = HorizontalAlignment.Stretch;
            SB.VerticalAlignment = VerticalAlignment.Top;
            SB.Width = double.NaN;

            SB.Margin = new Thickness(0);
            parent.Children.Add(SB);

            RefreshItems();
            sbadded = true;
            SB.TextChanged += SB_TextChanged;
        }

        private Thickness CalcMargin(int Index, bool CheckVisibility)
        {
            if (SearchBoxVisible == true)
            {
                if (Index > -1)
                {
                    double t = CommonItems.SearchBoxHeight;
                    for (int i = 0; Index - 1 >= i; i++)
                    {
                        if (Items[i].ItemStatus != ListItem.State.Removing &&
                            Items[i].ItemStatus != ListItem.State.Removed)
                        {
                            if (CheckVisibility == true)
                            {
                                if (Items[i].Visibility == Visibility.Visible && !double.IsNaN(Items[i].Height))
                                {
                                    t += Items[i].Height;
                                }
                            }
                            else
                            {
                                if(!double.IsNaN(Items[i].Height))
                                    t += Items[i].Height;
                            }
                        }
                    }

                    return new Thickness(0, t, 0, 0);
                }
                else if (Index == -1)
                {
                    return new Thickness(0, 0, 0, 0);
                }
                else
                {
                    return new Thickness();
                }
            }
            else
            {
                double t = 0;
                for (int i = 0; Index - 1 >= i; i++)
                {
                    if (Items[i].ItemStatus != ListItem.State.Removing &&
                        Items[i].ItemStatus != ListItem.State.Removed)
                    {
                        if (CheckVisibility == true)
                        {
                            if (Items[i].Visibility == Visibility.Visible && !double.IsNaN(Items[i].Height))
                            {
                                t += Items[i].Height;
                            }
                        }
                        else
                        {
                            if (!double.IsNaN(Items[i].Height))
                                t += Items[i].Height;
                        }
                    }
                }
                return new Thickness(0, t, 0, 0);
            }
        }

        private Thickness CalcMargin(int Index)
        {
            return CalcMargin(Index, false);
        }

        private void cleartitem()
        {
            if (Items == null) return;

            for (int i = 0; parent.Children.Count > i; i++)
            {
                ListItem Item = parent.Children[i] as ListItem;
                if (Item != null)
                {
                    Item.ItemStatus = ListItem.State.Removing;
                    Item.Index = -1;
                    Item.ItemClicked -= Item_ItemClicked;
                    parent.Children.Remove(Item);
                    Item.ItemStatus = ListItem.State.Removed;
                    i--;
                }
            }

            sc.ScrollToTop();
        }

        private void insertitem(int Index, ListItem Item)
        {
            Item.Width = parent.ActualWidth;

            Item.HorizontalAlignment = HorizontalAlignment.Stretch;
            Item.VerticalAlignment = VerticalAlignment.Top;
            
            Item.Width = double.NaN;
            Item.Index = Items.Count;

            parent.Children.Insert(Index, Item);
            Items.Insert(Index, Item);
            Item.ItemClicked += Item_ItemClicked;
        }

        private void Item_ItemClicked(object sender, ItemClickedEventArgs e)
        {
            if (e.MouseButtonEventArgs.ChangedButton == MouseButton.Left)
            {
                OnItemClicked(e);
                if (string.IsNullOrEmpty(SB.Text)) SB.ClearText();
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; e.NewItems.Count > i; i++)
                        additem((ListItem)e.NewItems[i]);
                    break;
            }

            RefreshItems();
        }

        private void Items_CollectionChangeNotice(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    removeitem((ListItem)e.OldItems[0]);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    cleartitem();
                    return;
            }
        }

        private void RefreshIndexes()
        {
            int ind = 0;
            for (int i = 0; Items.Count > i; i++)
            {
                if (Items[i].ItemStatus != ListItem.State.Removed && Items[i].ItemStatus != ListItem.State.Removing)
                {
                    Items[i].Index = ind;
                    ind++;
                }
            }
        }

        private void RefreshItems()
        {
            double h = 0;

            if (SearchBoxVisible == true)
            {
                SB.Margin = CalcMargin(-1);
                h = SB.Height;
            }

            int ind = 0;
            for (int i = 0; Items.Count > i; i++)
            {
                if (Items[i].ItemStatus != ListItem.State.Removed && Items[i].ItemStatus != ListItem.State.Removing)
                {
                    if (Items[i].ItemStatus == ListItem.State.Adding)
                    {
                        parent.Children.Add(Items[i]);
                        Items[i].ItemStatus = ListItem.State.Added;

                        Items[i].Margin = new Thickness(0, h, 0, 0);
                    }
                    else
                    {
                        Items[i].Margin = new Thickness(0, h, 0, 0);
                    }

                    h += Items[ind].Height;
                    Items[i].Index = ind;
                    ind++;
                }
                else
                {
                    if (Items[i].ItemStatus == ListItem.State.Removing)
                    {
                        Items[i].Index = -1;
                        parent.Children.Remove(Items[i]);
                        Items[i].ItemStatus = ListItem.State.Removed;
                    }
                }
            }

            if (SearchBoxVisible)
            {
                SB.ClearText();
            }
        }

        private void RemoveAnimationCompleted(object sender, Utils.AnimationHelper.AnimationEventArgs e)
        {
            ListItem li = e.AnimatedElement as ListItem;
            if (li != null)
            {
                parent.Children.Remove(li);
                li.Opacity = ((Utils.AnimationHelper.Double)sender).Before;
                sc.ScrollToTop();
            }
        }

        private void removeatitem(int Index)
        {
            Items[Index].ItemClicked -= Item_ItemClicked;
            Items[Index].ItemStatus = ListItem.State.Removing;
            parent.Children.Remove(Items[Index]);
            Items[Index].ItemStatus = ListItem.State.Removed;
        }

        private void removeitem(ListItem Item)
        {
            Item.ItemClicked -= Item_ItemClicked;
            Item.ItemStatus = ListItem.State.Removing;
            parent.Children.Remove(Item);
            Item.ItemStatus = ListItem.State.Removed;
        }

        private void RevisibleItems()
        {
            for (int i = 0; Items.Count > i; i++)
            {
                Items[i].Visibility = Visibility.Visible;
            }
        }

        private void SB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SB.Text.Length > 0)
            {
                RevisibleItems();
                search(SB.Text.ToLower());

                for (int i = 0; Items.Count > i; i++)
                {
                    Items[i].Margin = CalcMargin(i, true);
                }
            }
            else
            {
                RevisibleItems();
                for (int i = 0; Items.Count > i; i++)
                {
                    Items[i].Margin = CalcMargin(i);
                }
            }
        }

        private void SBRemove_AnimationCompleted(object sender, Utils.AnimationHelper.AnimationEventArgs e)
        {
            parent.Children.Remove(SB);
            RefreshItems();
        }

        private void search(string Text)
        {
            for (int i = 0; Items.Count > i; i++)
            {
                ListItem li = Items[i];
                if (li.IncludeSearchTarget == true)
                {
                    string tex = li.SearchText.ToLower();

                    switch (SearchStyle)
                    {
                        case SearchBox.SearchStyle.FromBegin:
                            if (tex.StartsWith(Text) == false) Items[i].Visibility = Visibility.Hidden;
                            break;

                        case SearchBox.SearchStyle.Partly:
                            if (tex.Contains(Text) == false) Items[i].Visibility = Visibility.Hidden;
                            break;
                    }
                }
                else
                {
                    if (li.ExcludeResult == true)
                    {
                        li.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void Ta_AnimationCompleted(object sender, Utils.AnimationHelper.AnimationEventArgs e)
        {
            RefreshItems();
        }

        public class ItemChangedEventArgs : EventArgs
        {
            public bool Added = true;

            public ListItem ChangedItem;

            public ItemChangedEventArgs(ListItem ChangedItem, bool Added)
            {
                this.ChangedItem = ChangedItem;
                this.Added = Added;
            }
        }

        public class SearchBox
        {
            public enum SearchStyle
            {
                FromBegin, Partly
            }
        }
    }
}