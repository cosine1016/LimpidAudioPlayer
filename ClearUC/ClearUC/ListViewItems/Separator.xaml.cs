using System.Windows.Media;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// Separater.xaml の相互作用ロジック
    /// </summary>
    public partial class Separator : ListItem
    {
        public Separator()
        {
            InitializeComponent();
        }

        public Brush SeparateBrush
        {
            get { return line.Fill; }
            set { line.Fill = value; }
        }

        public Brush SeparateStrokeBrush
        {
            get { return line.Stroke; }
            set { line.Stroke = value; }
        }

        public double SeparateStrokeThickness
        {
            get { return line.StrokeThickness; }
            set { line.StrokeThickness = value; }
        }
    }
}