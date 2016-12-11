using System.Windows;
using System.Windows.Input;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// ListAnimativeItem.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAnimativeItem : ListItem
    {
        private const int dur = 200;
        public ListAnimativeItem(bool IncludeSearchTarget) : base(IncludeSearchTarget, true)
        {
            InitializeComponent();
            ItemClicked += ListAnimativeItem_ItemClicked;
            SwitchItem();
        }

        private void ListAnimativeItem_ItemClicked(object sender, ItemClickedEventArgs e)
        {
            if (e.MouseButtonEventArgs.ChangedButton == SwitchButton)
            {
                SwitchFrontItem();
            }
        }

        public enum Item { First, Second }

        private Item item = Item.First;
        private ListItem fir;
        private ListItem sec;

        public ListItem FirstItem
        {
            get { return fir; }
            set
            {
                fir = value;
                front.Children.Clear();

                fir.HorizontalAlignment = HorizontalAlignment.Stretch;
                fir.VerticalAlignment = VerticalAlignment.Stretch;

                fir.Height = ItemsHeight;
                fir.Width = grid.Width;

                fir.Opacity = 1;

                fir.Margin = new Thickness(0);

                front.Children.Add(fir);

                SwitchItem();
            }
        }

        public ListItem SecondItem
        {
            get { return sec; }
            set
            {
                sec = value;
                back.Children.Clear();

                sec.HorizontalAlignment = HorizontalAlignment.Stretch;
                sec.VerticalAlignment = VerticalAlignment.Stretch;

                sec.Height = ItemsHeight;
                sec.Width = grid.Width;

                sec.Opacity = 1;

                sec.Margin = new Thickness(0);

                back.Children.Add(sec);

                SwitchItem();
            }
        }

        public double ItemsHeight
        {
            get { return Height; }
            set
            {
                Height = value;

                grid.Height = value;

                if (fir != null)
                {
                    fir.Height = value;
                }

                if (sec != null)
                {
                    sec.Height = value;
                }
            }
        }

        public double ItemsOpacity
        {
            get { return Opacity; }
            set { Opacity = value; }
        }

        public Item FrontItem
        {
            get { return item; }
            set
            {
                item = value;
                ShowItem();
            }
        }

        public void SwitchFrontItem()
        {
            if (FrontItem == Item.First)
            {
                FrontItem = Item.Second;
            }
            else if (FrontItem == Item.Second)
            {
                FrontItem = Item.First;
            }

            ShowItem();
        }

        public MouseButton SwitchButton { get; set; } = MouseButton.Right;

        private void SwitchItem()
        {
            ShowItem();
        }

        private void ShowItem()
        {
            Utils.AnimationHelper.Thickness ta = new Utils.AnimationHelper.Thickness();
            Utils.AnimationHelper.Double da = new Utils.AnimationHelper.Double();
            if (FrontItem == Item.First)
            {
                back.Visibility = Visibility.Visible;
                front.Visibility = Visibility.Visible;
                ta.Animate(front.Margin, new Thickness(0, 0, 0, 0), dur, null, new PropertyPath(MarginProperty), fir);
                da.Animate(back.Opacity, 0, dur, null, OpacityProperty, sec);
            }
            else if (FrontItem == Item.Second)
            {
                back.Visibility = Visibility.Visible;
                front.Visibility = Visibility.Visible;
                ta.Animate(front.Margin, new Thickness(0, sec.Height, 0, 0), dur, null, new PropertyPath(MarginProperty), fir);
                da.Animate(back.Opacity, 1, dur, null, OpacityProperty, sec);
            }
        }
    }
}