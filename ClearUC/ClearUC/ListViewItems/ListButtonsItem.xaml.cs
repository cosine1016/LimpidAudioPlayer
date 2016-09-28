using System.Windows;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// ListButtonsItem.xaml の相互作用ロジック
    /// </summary>
    public partial class ListButtonsItem : ListItem
    {
        public ListButtonsItem()
        {
            InitializeComponent();
        }

        private void calcsize()
        {
            double W = background.ActualWidth;

            double Sep = W / Parent.Children.Count;

            for (int i = 0; Parent.Children.Count > i; i++)
            {
                ListButton Item = Parent.Children[i] as ListButton;
                if (Item != null)
                {
                    Item.HorizontalAlignment = HorizontalAlignment.Left;
                    Item.VerticalAlignment = VerticalAlignment.Top;
                    Item.Width = Sep;
                    Item.Height = background.ActualHeight;
                    Item.Margin = new Thickness(Sep * i, 0, 0, 0);
                }
            }
        }

        public void Add(ListButton Button)
        {
            Parent.Children.Add(Button);
            calcsize();
        }

        public void Remove(ListButton Button)
        {
            Parent.Children.Remove(Button);
            calcsize();
        }

        public void RemoveAt(int Index)
        {
            Parent.Children.RemoveAt(Index);
            calcsize();
        }

        public void Insert(int Index, ListButton Button)
        {
            Parent.Children.Insert(Index, Button);
            calcsize();
        }

        private void ListItem_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            calcsize();
        }

        public class ListButton : ClearUC.Button
        {
            public ListButton(ListItem ParentItem)
            {
                this.ParentItem = ParentItem;
            }

            public ListItem ParentItem { get; set; } = null;
        }
    }
}