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

namespace MVPUC.Buttons
{
    /// <summary>
    /// LibraryButton.xaml の相互作用ロジック
    /// </summary>
    public partial class LibraryButton : ButtonBase
    {
        public LibraryButton()
        {
            InitializeComponent();

            AppliedPropertyChanges += LibraryButton_AppliedPropertyChanges;
        }

        private void LibraryButton_AppliedPropertyChanges(object sender, EventArgs e)
        {
            path.Fill = ButtonBrush;
            path.Stroke = ButtonStroke;
            path.StrokeThickness = StrokeThickness;
        }

        private void path_MouseEnter(object sender, MouseEventArgs e)
        {
            Animate(path.Fill, MouseEnterBrush, path);
        }

        private void path_MouseLeave(object sender, MouseEventArgs e)
        {
            Animate(path.Fill, ButtonBrush, path);
        }

        private void path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Animate(path.Fill, MouseClickBrush, path);
        }

        private void path_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Animate(path.Fill, MouseEnterBrush, path);
        }
    }
}
