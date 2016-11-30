using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// ContainerItem.xaml の相互作用ロジック
    /// </summary>
    public partial class ContainerItem : ListItem
    {
        private Grid ParentGrid;

        public ContainerItem(bool IncludeSearchTarget, bool ExcludeResult, bool Fill) : base(IncludeSearchTarget, ExcludeResult)
        {
            InitializeComponent();
            this.Fill = Fill;
            Loaded += ContainerItem_Loaded;
            Unloaded += ContainerItem_Unloaded;
        }

        private void ContainerItem_Unloaded(object sender, RoutedEventArgs e)
        {
            if(ParentGrid != null)
            {
                ParentGrid = null;
            }
        }

        private void ContainerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if(Parent != null)
            {
                ParentGrid = (Grid)Parent;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Height = ParentGrid.ActualHeight;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                }));
            }
        }

        public ContainerItem(bool IncludeSearchTarget, bool ExcludeResult, double Height) : this(IncludeSearchTarget, ExcludeResult, false)
        {
            this.Height = Height;
        }

        protected override void OnAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Parent != null)
            {
                ParentGrid = (Grid)Parent;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Height = ParentGrid.ActualHeight;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                }));
            }
            base.OnAlignmentChanged(e);
        }

        public bool Fill { get; set; } = false;
    }
}
