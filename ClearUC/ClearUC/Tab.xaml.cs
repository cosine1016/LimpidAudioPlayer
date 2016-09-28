using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Tab.xaml の相互作用ロジック
    /// </summary>
    public partial class Tab : UserControl
    {
        public class TabItem
        {
            public event EventHandler MouseClick;

            internal event EventHandler VisibleChanged;

            public TabItem(string Title, Border Border)
            {
                this.Title = Title;
                this.Border = Border;
                Init();
            }

            public TabItem(string Title)
            {
                this.Title = Title;
                Init();
            }

            private void Init()
            {
                BG.Background = Background;
                BG.VerticalAlignment = VerticalAlignment.Stretch;
                BG.HorizontalAlignment = HorizontalAlignment.Left;

                TitleLabel.VerticalAlignment = VerticalAlignment.Stretch;
                TitleLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
                TitleLabel.Margin = new Thickness(0);
                TitleLabel.Foreground = Foreground;
                TitleLabel.FontSize = 13;

                CF.DefaultColor += CF_DefaultColor;
                CF.DownColor += CF_DownColor;
                CF.EnterColor += CF_EnterColor;
                CF.MouseClick += CF_MouseClick;

                if (Border != null)
                {
                    BG.Children.Add(Border);
                }

                BG.Children.Add(TitleLabel);
                BG.Children.Add(CF.Filter);

                TitleLabel.HorizontalContentAlignment = HorizontalAlignment.Center;

                if (Border != null)
                    TitleLabel.VerticalContentAlignment = VerticalAlignment.Bottom;
                else
                    TitleLabel.VerticalContentAlignment = VerticalAlignment.Center;
            }

            private void CF_MouseClick(object sender, MouseButtonEventArgs e)
            {
                MouseClick?.Invoke(this, new EventArgs());
            }

            private Utils.AnimationHelper.Brush bas = new Utils.AnimationHelper.Brush();
            private Utils.AnimationHelper.Brush bat = new Utils.AnimationHelper.Brush();

            private void CF_EnterColor(object sender, EventArgs e)
            {
                if (!IsActive)
                {
                    bat.Animate(TitleLabel.Foreground, MouseEnter, AnimationDuration, TitleLabel,
                        new PropertyPath("(0).(1)", ForegroundProperty, SolidColorBrush.ColorProperty));
                }
            }

            private void CF_DownColor(object sender, EventArgs e)
            {
                if (!IsActive)
                {
                    TitleLabel.Foreground = MouseDown;
                }
            }

            private void CF_DefaultColor(object sender, EventArgs e)
            {
                if (!IsActive)
                {
                    bat.Animate(TitleLabel.Foreground, Foreground, AnimationDuration, TitleLabel,
                        new PropertyPath("(0).(1)", ForegroundProperty, SolidColorBrush.ColorProperty));
                }
            }

            public object Title
            {
                get { return TitleLabel.Content; }
                set { TitleLabel.Content = value; }
            }

            private Border s = null;

            public Border Border
            {
                get { return s; }
                set
                {
                    s = value;
                    if (value != null)
                    {
                        s.HorizontalAlignment = HorizontalAlignment.Center;
                        s.VerticalAlignment = VerticalAlignment.Top;
                    }
                }
            }

            public Brush Background { get; set; } = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            public Brush Foreground { get; set; } = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));

            public Brush MouseEnter { get; set; } = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180));

            public Brush MouseDown { get; set; } = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

            public Brush ActiveBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 200, 200, 255));

            public double AnimationDuration { get; set; } = 200;

            bool vis = true;
            public bool Visible
            {
                get { return vis; }
                set
                {
                    if (value == vis) return;
                    vis = value;
                    VisibleChanged?.Invoke(this, new EventArgs());
                }
            }

            private ClickFilter CF = new ClickFilter();

            internal Grid BG { get; set; } = new Grid();

            internal Label TitleLabel { get; set; } = new Label();

            private bool ia = false;

            internal bool IsActive
            {
                get { return ia; }
                set
                {
                    if (value != ia)
                    {
                        ia = value;
                        if (value)
                        {
                            bat.Animate(TitleLabel.Foreground, ActiveBrush, AnimationDuration, TitleLabel,
                                new PropertyPath("(0).(1)", ForegroundProperty, SolidColorBrush.ColorProperty));
                        }
                        else
                        {
                            bat.Animate(TitleLabel.Foreground, Foreground, AnimationDuration, TitleLabel,
                                new PropertyPath("(0).(1)", ForegroundProperty, SolidColorBrush.ColorProperty));
                        }
                    }
                }
            }
        }

        public class TabItemCollection : System.Collections.ObjectModel.ObservableCollection<TabItem>
        {
            public void AddRange(TabItem[] items)
            {
                for (int i = 0; items.Length > i; i++)
                {
                    Add(items[i]);
                }
            }
        }

        public event EventHandler ActiveItemChanged;

        public Tab()
        {
            InitializeComponent();
            Items.CollectionChanged += Items_CollectionChanged;
            SizeChanged += Tab_SizeChanged;
            ActiveItemChanged += Tab_ActiveItemChanged;
        }

        private void Tab_ActiveItemChanged(object sender, EventArgs e)
        {
            for (int i = 0; Items.Count > i; i++) Items[i].IsActive = false;
            if (ActiveIndex > -1) Items[ActiveIndex].IsActive = true;
        }

        private void Tab_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateItems();
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    Items[e.NewStartingIndex].BG.Visibility = Visibility.Hidden;
                    Items[e.NewStartingIndex].MouseClick += Tab_MouseClick;
                    Items[e.NewStartingIndex].VisibleChanged += Tab_VisibleChanged;
                    Base.Children.Add(Items[e.NewStartingIndex].BG);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    ((TabItem)e.OldItems[0]).MouseClick -= Tab_MouseClick;
                    ((TabItem)e.OldItems[0]).VisibleChanged -= Tab_VisibleChanged;
                    if (((TabItem)e.OldItems[0]).IsActive) ActiveIndex = -1;
                    Base.Children.Remove(((TabItem)e.OldItems[0]).BG);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                    {
                        for (int i = 0; e.OldItems.Count > i; i++)
                        {
                            ((TabItem)e.OldItems[i]).MouseClick -= Tab_MouseClick;
                            ((TabItem)e.OldItems[i]).VisibleChanged -= Tab_VisibleChanged;
                        }
                        Base.Children.Clear();
                    }
                    break;
            }

            UpdateItems();
        }

        private void Tab_VisibleChanged(object sender, EventArgs e)
        {
            UpdateItems();
        }

        private void Tab_MouseClick(object sender, EventArgs e)
        {
            ActiveIndex = Items.IndexOf((TabItem)sender);
        }

        private void UpdateItems()
        {
            int count = Items.Count;

            Dispatcher.Invoke(() =>
            {
                for (int i = 0; Items.Count > i; i++)
                {
                    if(Items[i].Visible == false)
                    {
                        count -= 1;
                        Items[i].TitleLabel.Visibility = Visibility.Hidden;
                        if (Items[i].Border != null) Items[i].Border.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        Items[i].TitleLabel.Visibility = Visibility.Visible;
                        if (Items[i].Border != null) Items[i].Border.Visibility = Visibility.Visible;
                    }
                }

                if (ActualWidth <= 0) return;
                double perW = ActualWidth / count;
                if (perW > 200) perW = 200;
                double lef = (ActualWidth - perW * count) / 2;
                if (lef < 0) lef = 0;

                for (int i = 0; Items.Count > i; i++)
                {
                    Items[i].BG.Margin = new Thickness(lef + perW * i, 0, 0, 0);
                    Items[i].BG.Width = perW - 1;
                    Items[i].BG.Visibility = Visibility.Visible;
                }
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private int ai = -1;

        public int ActiveIndex
        {
            get { return ai; }
            set
            {
                if (ai == value) return;
                ai = value;
                ActiveItemChanged?.Invoke(this, new EventArgs());
            }
        }

        public TabItemCollection Items { get; set; } = new TabItemCollection();
    }
}