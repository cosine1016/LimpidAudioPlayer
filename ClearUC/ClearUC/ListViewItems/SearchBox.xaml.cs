using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ClearUC.ListViewItems
{
    /// <summary>
    /// SearchBox.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchBox : UserControl
    {
        public class Config
        {
            public Brush SearchIcon { get; set; } = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));
            public Brush SearchIconStroke { get; set; } = new SolidColorBrush(Color.FromArgb(255, 105, 105, 105));
            public double SearchIconStrokeThickness { get; set; } = 1;

            public Brush ClearIcon { get; set; } = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));
            public Brush ClearIconMouseEnter { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            public Brush ClearIconMouseClick { get; set; } = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            public Brush ClearIconStroke { get; set; } = new SolidColorBrush(Color.FromArgb(255, 105, 105, 105));
            public double ClearIconStrokeThickness { get; set; } = 1;
            public double ClearIconAnimationSpeed { get; set; } = 50;

            public Brush TextBox { get; set; } = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            public Brush TextBoxMouseEnter { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            public Brush TextBoxSelection { get; set; } = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
            public double TextBoxOpacity { get; set; } = 0.7;
            public double TextBoxAnimationSpeed { get; set; } = 50;

            public double BackgroundOpacity { get; set; } = 0.5;
        }

        public event EventHandler<TextChangedEventArgs> TextChanged;

        protected virtual void OnTextChanged(TextChangedEventArgs e)
        {
            if (TextChanged != null) TextChanged(this, e);
        }

        private bool hiding = false;
        private Config cnf = new Config();

        public SearchBox()
        {
            InitializeComponent();
            ApplyConfig(cnf);
        }

        public string Text
        {
            get { return textBox.Text; }
            set
            {
                textBox.Text = value;

                front_MouseDown(null, null);
                textBox_MouseUp(null, null);
            }
        }

        public double BackgroundOpacity
        {
            get { return textBox.Opacity; }
            set { textBox.Opacity = value; }
        }

        public void ClearText()
        {
            hiding = false;
            textBox.Clear();
            textBox.Visibility = Visibility.Hidden;
            search.Visibility = Visibility.Visible;
            front.Visibility = Visibility.Visible;
            clear.Visibility = Visibility.Hidden;
            clearf.Visibility = Visibility.Hidden;
            drain.Visibility = Visibility.Visible;
            drain.Opacity = cnf.BackgroundOpacity;
            textBox.Opacity = 0;
        }

        public Config SearchBoxConfig
        {
            get { return cnf; }
            set
            {
                cnf = value;
                ApplyConfig(value);
            }
        }

        private void ApplyConfig(Config Config)
        {
            search.Fill = Config.SearchIcon;
            search.Stroke = Config.SearchIconStroke;
            search.StrokeThickness = Config.SearchIconStrokeThickness;

            clear.Fill = Config.ClearIcon;
            clear.Stroke = Config.ClearIconStroke;
            clear.StrokeThickness = Config.ClearIconStrokeThickness;

            textBox.Background = Config.TextBox;
            textBox.SelectionBrush = Config.TextBoxSelection;
            textBox.Opacity = Config.TextBoxOpacity;
        }

        private void front_MouseDown(object sender, MouseButtonEventArgs e)
        {
            search.Visibility = Visibility.Hidden;
            front.Visibility = Visibility.Hidden;
            textBox.Visibility = Visibility.Visible;
            clear.Visibility = Visibility.Visible;
            clearf.Visibility = Visibility.Visible;
            drain.Visibility = Visibility.Hidden;
            textBox.Opacity = cnf.TextBoxOpacity;
            hiding = true;
        }

        private void front_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateShape(drain.Fill, cnf.TextBoxMouseEnter, cnf.TextBoxAnimationSpeed, drain);
        }

        private void front_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateShape(drain.Fill, cnf.TextBox, cnf.TextBoxAnimationSpeed, drain);
        }

        private void textBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (hiding == true)
            {
                textBox.Select(0, 0);
                textBox.Focus();

                hiding = false;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnTextChanged(e);
        }

        private void clearf_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (hiding == false)
            {
                ClearText();
            }
            else
            {
                textBox_MouseUp(null, null);
            }
        }

        private void clearf_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateShape(clear.Fill, cnf.ClearIconMouseEnter, cnf.ClearIconAnimationSpeed, clear);
        }

        private void clearf_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateShape(clear.Fill, cnf.ClearIcon, cnf.ClearIconAnimationSpeed, clear);
        }

        private Brush af;
        private Shape shItem;

        private void AnimateShape(Brush Before, Brush After, double Duration, Shape Item)
        {
            Item.Fill = Before;

            if (shItem != null)
            {
                shItem.Fill = this.af;
                shItem = null;
            }

            this.af = After;
            shItem = Item;

            Storyboard s = new Storyboard();
            ColorAnimation ca = new ColorAnimation();

            Color be = ((SolidColorBrush)Before).Color;
            Color af = ((SolidColorBrush)After).Color;

            ca.From = be;
            ca.To = af;
            ca.Duration = TimeSpan.FromMilliseconds(Duration);
            ca.FillBehavior = FillBehavior.Stop;

            PropertyPath pp = new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty);
            Storyboard.SetTargetProperty(ca, pp);
            s.Children.Add(ca);

            s.Completed += S_Completed;
            Item.BeginStoryboard(s);
        }

        private void S_Completed(object sender, EventArgs e)
        {
            if (shItem != null)
            {
                shItem.Fill = af;
                shItem = null;
            }
        }

        private void clearf_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateShape(clear.Fill, cnf.ClearIconMouseClick, cnf.ClearIconAnimationSpeed, clear);
        }
    }
}