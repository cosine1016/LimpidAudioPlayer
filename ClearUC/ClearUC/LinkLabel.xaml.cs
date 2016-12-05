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

namespace ClearUC
{
    /// <summary>
    /// LinkLabel.xaml の相互作用ロジック
    /// </summary>
    public partial class LinkLabel : Label
    {
        private Hyperlink link = new Hyperlink();
        private object inlinev = null;

        public LinkLabel()
        {
            InitializeComponent();
            Content = base.Content;
            base.Content = link;
        }

        public new object Content
        {
            get { return inlinev; }
            set
            {
                link.Inlines.Clear();
                if(value != null)
                    link.Inlines.Add(value.ToString());
                inlinev = value;
            }
        }
    }
}
