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
    /// ListItem.xaml の相互作用ロジック
    /// </summary>
    public partial class ListSubItem : ListItem
    {
        public event EventHandler MainLabelTextChanged;
        public event EventHandler SubLabelTextChanged;
        public event EventHandler StatusLabelTextChanged;

        public class Config
        {
            public Brush Background { get; set; } = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            public Brush BackgroundMouseEnter { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
            public Brush BackgroundMouseClick { get; set; } = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
            public double AnimationDuration { get; set; } = 50;
        }

        private Config cnf = new Config();
        private Shape lefts = null;

        public ListSubItem() : base(true, true)
        {
            InitializeComponent();

            mainLsubH.Content = null;
            mainL.Content = null;
            subL.Content = null;
            staL.Content = null;
            LeftItem = litem;
            ImageIndexChanged += ListSubItem_ImageIndexChanged;
        }

        public ListSubItem(bool IncludeSearchTarget, bool ExcludeResult) : base(IncludeSearchTarget, ExcludeResult)
        {
            InitializeComponent();

            mainLsubH.Content = null;
            mainL.Content = null;
            subL.Content = null;
            staL.Content = null;
            LeftItem = litem;
            ImageIndexChanged += ListSubItem_ImageIndexChanged;
        }

        private void ListSubItem_ImageIndexChanged(object sender, EventArgs e)
        {
            if (ImageIndex > -1) image.Source = ImageSources[ImageIndex];
            else image.Source = null;
        }

        private bool tl = false;

        public bool TitleLabelVisible
        {
            get { return tl; }
            set
            {
                tl = value;
                ChangeMainLabel();
            }
        }

        public string MainLabelText
        {
            get { return (string)mainL.Content; }
            set
            {
                mainL.Content = value;
                mainLsubH.Content = value;
                TitleL.Content = value;
                SearchText = value;
                ChangeMainLabel();
                MainLabelTextChanged?.Invoke(this, new EventArgs());
            }
        }

        public string SubLabelText
        {
            get { return (string)subL.Content; }
            set
            {
                subL.Content = value;
                SubLabelTextChanged?.Invoke(this, new EventArgs());
            }
        }

        public string StatusLabelText
        {
            get { return (string)staL.Content; }
            set
            {
                staL.Content = value;
                if (StatusLabelTextChanged != null) StatusLabelTextChanged(this, new EventArgs());
            }
        }

        public string NumberLabelText
        {
            get { return (string)numL.Content; }
            set { numL.Content = value; }
        }

        public Brush MainLabelBrush
        {
            get { return mainL.Foreground; }
            set
            {
                mainL.Foreground = value;
                mainLsubH.Foreground = value;
                TitleL.Foreground = value;
            }
        }

        public Brush SubLabelBrush
        {
            get { return subL.Foreground; }
            set { subL.Foreground = value; }
        }

        public Brush StatusLabelBrush
        {
            get { return staL.Foreground; }
            set { staL.Foreground = value; }
        }

        public Brush NumberLabelBrush
        {
            get { return numL.Foreground; }
            set { numL.Foreground = value; }
        }

        public Visibility SubLabelVisibility
        {
            get { return subL.Visibility; }
            set
            {
                subL.Visibility = value;
                ChangeMainLabel();
            }
        }

        public Visibility StatusLabelVisibility
        {
            get { return staL.Visibility; }
            set { staL.Visibility = value; }
        }

        public enum LeftItems { Image, Number, Shape, Nothing }

        private LeftItems litem = LeftItems.Nothing;

        public LeftItems LeftItem
        {
            get { return litem; }
            set
            {
                litem = value;

                if (litem == LeftItems.Image || litem == LeftItems.Number || litem == LeftItems.Shape)
                {
                    mainLsubH.Margin = new Thickness(55, mainLsubH.Margin.Top, 0, 0);
                    mainL.Margin = new Thickness(55, mainL.Margin.Top, 0, 0);
                    subL.Margin = new Thickness(55, subL.Margin.Top, 0, 0);
                }
                else
                {
                    mainLsubH.Margin = new Thickness(1, mainLsubH.Margin.Top, 0, 0);
                    mainL.Margin = new Thickness(1, mainL.Margin.Top, 0, 0);
                    subL.Margin = new Thickness(1, subL.Margin.Top, 0, 0);
                }

                ChangeMainLabel();
            }
        }

        public double LeftItemHeight { get { return shapecontainer.Height; } }

        public double LeftItemWidth { get { return shapecontainer.Width; } }

        public Shape ShapeItem
        {
            get { return lefts; }
            set
            {
                lefts = value;
                shapecontainer.Children.Clear();
                if (value != null) { shapecontainer.Children.Add(lefts); }
            }
        }

        public double BackgroundFillOpacity
        {
            get { return background.Fill.Opacity; }
            set { background.Fill.Opacity = value; }
        }

        public double BackgroundStrokeOpacity
        {
            get { return background.Stroke.Opacity; }
            set { background.Stroke.Opacity = value; }
        }

        public Brush Stroke
        {
            get { return background.Stroke; }
            set { background.Stroke = value; }
        }

        public Stretch Stretch
        {
            get { return image.Stretch; }
            set { image.Stretch = value; }
        }

        public StretchDirection StretchDirection
        {
            get { return image.StretchDirection; }
            set { image.StretchDirection = value; }
        }

        public void ApplyConfig(Config Config)
        {
            cnf = Config;
            background.Fill = Config.Background;
        }

        public bool ChangeStroke { get; set; } = true;

        private void ChangeMainLabel()
        {
            if (TitleLabelVisible == true)
            {
                mainL.Visibility = Visibility.Hidden;
                mainLsubH.Visibility = Visibility.Hidden;
                TitleL.Visibility = Visibility.Visible;
            }
            else
            {
                TitleL.Visibility = Visibility.Hidden;
                if (subL.Visibility == Visibility.Visible)
                {
                    mainLsubH.Visibility = Visibility.Hidden;
                    mainL.Visibility = Visibility.Visible;
                }
                else
                {
                    mainL.Visibility = Visibility.Hidden;
                    mainLsubH.Visibility = Visibility.Visible;
                }

                switch (litem)
                {
                    case LeftItems.Image:
                        numL.Visibility = Visibility.Hidden;
                        image.Visibility = Visibility.Visible;
                        break;

                    case LeftItems.Number:
                        image.Visibility = Visibility.Hidden;
                        numL.Visibility = Visibility.Visible;
                        break;

                    case LeftItems.Shape:
                        image.Visibility = Visibility.Hidden;
                        numL.Visibility = Visibility.Hidden;
                        break;

                    case LeftItems.Nothing:
                        numL.Visibility = Visibility.Hidden;
                        image.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        private Brush defs;
        private bool flag = false;

        private void background_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateBackground(background.Fill, cnf.BackgroundMouseEnter, cnf.AnimationDuration);

            defs = background.Stroke;
            if (ChangeStroke == true) background.Stroke = cnf.BackgroundMouseEnter;
        }

        private void background_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateBackground(background.Fill, cnf.Background, cnf.AnimationDuration);

            background.Stroke = defs;
        }

        private void background_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                AnimateBackground(background.Fill, cnf.BackgroundMouseClick, cnf.AnimationDuration);

                if (ChangeStroke == true) background.Stroke = cnf.BackgroundMouseClick;
            }
            flag = true;
        }

        private void background_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (flag == true)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    AnimateBackground(background.Fill, cnf.BackgroundMouseEnter, cnf.AnimationDuration);

                    if (ChangeStroke == true) background.Stroke = cnf.BackgroundMouseEnter;
                }

                flag = false;
            }
        }

        private Brush af;

        private void AnimateBackground(Brush Before, Brush After, double Duration)
        {
            if (Before == null | After == null) return;
            this.af = After;

            Storyboard s = new Storyboard();
            ColorAnimation ca = new ColorAnimation();

            Color be = ((SolidColorBrush)Before).Color;
            Color af = ((SolidColorBrush)After).Color;

            ca.From = be;
            ca.To = af;
            ca.Duration = TimeSpan.FromMilliseconds(Duration);

            PropertyPath pp = new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty);
            Storyboard.SetTargetProperty(ca, pp);
            s.Children.Add(ca);

            background.BeginStoryboard(s);
            s.Completed += S_Completed;
        }

        private void S_Completed(object sender, EventArgs e)
        {
            background.Fill = af;
        }
    }
}