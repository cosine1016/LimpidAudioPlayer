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

namespace LAP.UserControls
{
    /// <summary>
    /// Caption.xaml の相互作用ロジック
    /// </summary>
    public partial class Caption : UserControl
    {
        public event EventHandler OptionalButtonClick;

        private Window parent;
        private Thickness First, Second, Third;

        public enum Buttons
        {
            Close, Maximize, Minimize
        }

        public Caption()
        {
            InitializeComponent();
        }

        public bool MaximizeButtonVisible
        {
            get { return maxv; }
            set { MaximizeButtonV(value); }
        }

        public bool MinimizeButtonVisible
        {
            get { return minv; }
            set { MinimizeButtonV(value); }
        }

        public bool CloseButtonVisible
        {
            get { return clov; }
            set { CloseButtonV(value); }
        }

        public bool OptionalButtonVisible
        {
            get { return GetOptionalButtonVisible(); }
            set
            {
                switch (value)
                {
                    case true:
                        Optional.Visibility = Visibility.Visible;
                        TitleL.Margin = new Thickness(Optional.Width, 0, Close.Width + Maximize.Width + Minimize.Width, 0);
                        break;
                    case false:
                        Optional.Visibility = Visibility.Hidden;
                        TitleL.Margin = new Thickness(0, 0, Close.Width + Maximize.Width + Minimize.Width, 0);
                        break;
                }
            }
        }

        public object Title
        {
            get { return TitleL.Content; }
            set { TitleL.Content = value; }
        }

        public bool GetOptionalButtonVisible()
        {
            switch (Optional.Visibility)
            {
                case Visibility.Visible:
                    return true;
                case Visibility.Hidden:
                    return false;
            }
            return false;
        }

        bool minv = true;
        public void MinimizeButtonV(bool Visible)
        {
            minv = Visible;
            switch (minv)
            {
                case true:
                    Minimize.Visibility = Visibility.Visible;
                    break;
                case false:
                    Minimize.Visibility = Visibility.Hidden;
                    break;
            }
        }

        bool maxv = true;
        public void MaximizeButtonV(bool Visible)
        {
            maxv = Visible;
            switch (maxv)
            {
                case true:
                    Maximize.Visibility = Visibility.Visible;
                    break;
                case false:
                    Maximize.Visibility = Visibility.Hidden;
                    break;
            }
        }

        bool clov = true;

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            parent.Close();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            switch (parent.WindowState)
            {
                case WindowState.Normal:
                    parent.WindowState = WindowState.Maximized;
                    break;
                case WindowState.Maximized:
                    parent.WindowState = WindowState.Normal;
                    break;
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            parent.WindowState = WindowState.Minimized;
        }

        private void Optional_Click(object sender, RoutedEventArgs e)
        {
            if (OptionalButtonClick != null) OptionalButtonClick(this, new EventArgs());
        }

        public void CloseButtonV(bool Visible)
        {
            clov = Visible;
            switch (clov)
            {
                case true:
                    Close.Visibility = Visibility.Visible;
                    break;
                case false:
                    Close.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == false)
            {
                parent = Window.GetWindow(this);
                parent.StateChanged += Parent_StateChanged;

                First = new Thickness(0, 0, 0, 0);
                Second = new Thickness(0, 0, Maximize.Width, 0);
                Third = new Thickness(0, 0, Minimize.Width + Maximize.Width, 0);

                switch (parent.WindowState)
                {
                    case WindowState.Maximized:
                        Maximize.Content = 2;
                        break;
                    case WindowState.Normal:
                        Maximize.Content = 1;
                        break;
                }
            }
        }

        private void Parent_StateChanged(object sender, EventArgs e)
        {
            switch (parent.WindowState)
            {
                case WindowState.Maximized:
                    Maximize.Content = 2;
                    break;
                case WindowState.Normal:
                    Maximize.Content = 1;
                    break;
            }
        }
    }
}
