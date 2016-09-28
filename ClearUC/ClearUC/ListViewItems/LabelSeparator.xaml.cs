using System.Windows.Media;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// LabelSeparator.xaml の相互作用ロジック
    /// </summary>
    public partial class LabelSeparator : ListItem
    {
        public LabelSeparator() : base(false, false)
        {
            InitializeComponent();
        }

        public string Label
        {
            get { return (string)label.Content; }
            set
            {
                label.Content = value;
                SearchText = value;
            }
        }

        public Brush Sticky
        {
            get { return sticky.Fill; }
            set { sticky.Fill = value; }
        }
    }
}