using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Animation;
using System.Collections.Specialized;

namespace LAP.UserControls
{
    /// <summary>
    /// MediaPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class MediaPanel : UserControl
    {
        public MediaPanel()
        {
            InitializeComponent();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (Index < 0) Index = 0;
                    break;
            }

            if (Children.Count == 0)
                Index = -1;

            if(Children.Count <= 1)
            {
                Back.Visibility = Visibility.Hidden;
                Next.Visibility = Visibility.Hidden;
            }
            else
            {
                Back.Visibility = Visibility.Visible;
                Next.Visibility = Visibility.Visible;
            }
        }

        public ObservableCollection<FrameworkElement> Children { get; } = new ObservableCollection<FrameworkElement>();

        private int _ind = -1;
        public int Index
        {
            get { return _ind; }
            set
            {
                Parent.Children.Clear();

                if(value > -1)
                    AddControl(value, new Thickness(0));

                _ind = value;
            }
        }

        private void AnimateToLeft()
        {
            if (!_animating)
            {
                int Last = Index;
                int Next = Index + 1;

                if (Next >= Children.Count)
                    Next = 0;

                AddControl(Next, new Thickness(ActualWidth, 0, -ActualWidth, 0));
                AnimateItem(Last, -ActualWidth, true);
                AnimateItem(Next, 0);

                _ind = Next;
            }
        }

        private void AnimateToRight()
        {
            if (!_animating)
            {
                int Last = Index;
                int Next = Index - 1;

                if (Index <= 0)
                    Next = Children.Count - 1;

                AddControl(Next, new Thickness(-ActualWidth, 0, ActualWidth, 0));
                AnimateItem(Last, ActualWidth, true);
                AnimateItem(Next, 0);

                _ind = Next;
            }
        }

        private bool _animating = false;
        private void AnimateItem(int Index, double ToLeft, bool RemoveWhenComplete = false)
        {
            _animating = true;
            FrameworkElement Element = Children[Index];
            ClearUC.Utils.AnimationHelper.Thickness ta = new ClearUC.Utils.AnimationHelper.Thickness();

            EventHandler<ClearUC.Utils.AnimationHelper.AnimationEventArgs> ev = null;
            ev = (sender, e) =>
            {
                ta.AnimationCompleted -= ev;
                if (RemoveWhenComplete)
                    Parent.Children.Remove(Element);

                _animating = false;
            };

            ta.AnimationCompleted += ev;

            ta.Animate(Element.Margin, new Thickness(ToLeft, 0, -ToLeft, 0),
                Config.Current.Animation[Enums.Animation.MediaPanel], null, new PropertyPath(MarginProperty), Element);
        }

        private Duration GetDuration()
        {
            return new Duration(new TimeSpan(Config.Current.Animation[Enums.Animation.Default]));
        }

        private void AddControl(int Index, Thickness Margin)
        {
            Children[Index].Margin = Margin;
            Parent.Children.Add(Children[Index]);
        }

        private void Next_MouseEnter(object sender, MouseEventArgs e)
        {
            FadeIn(Next);
        }

        private void Next_MouseLeave(object sender, MouseEventArgs e)
        {
            FadeOut(Next);
        }

        private void Back_MouseEnter(object sender, MouseEventArgs e)
        {
            FadeIn(Back);
        }

        private void Back_MouseLeave(object sender, MouseEventArgs e)
        {
            FadeOut(Back);
        }

        private void FadeOut(FrameworkElement Element)
        {
            ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
            da.Animate(Element.Opacity, 0.01, Config.Current.Animation[Enums.Animation.Default], null, OpacityProperty, Element);
        }

        private void FadeIn(FrameworkElement Element)
        {
            ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
            da.Animate(Element.Opacity, 1.0, Config.Current.Animation[Enums.Animation.Default], null, OpacityProperty, Element);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            AnimateToLeft();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            AnimateToRight();
        }
    }
}
