using System;
using System.Windows;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// ListToggleItem.xaml の相互作用ロジック
    /// </summary>
    public partial class ListToggleItem : ListItem
    {
        public ListToggleItem() : base(true, true)
        {
            InitializeComponent();
            ListItem.SubLabelVisibility = Visibility.Hidden;
            ListItem.MainLabelTextChanged += ListItem_MainLabelTextChanged;
        }

        private void ListItem_MainLabelTextChanged(object sender, EventArgs e)
        {
            SearchText = ListItem.MainLabelText;
        }

        private void ListSubItem_ItemClicked(object sender, ItemClickedEventArgs e)
        {
            if (ToggleOnClick) Toggle.Switch();
        }

        public bool ToggleOnClick { get; set; } = true;

        public ListSubItem ListItem
        {
            get { return ListSubItem; }
            set { ListSubItem = value; }
        }

        public ToggleButton ToggleButton
        {
            get { return Toggle; }
            set { Toggle = value; }
        }
    }
}