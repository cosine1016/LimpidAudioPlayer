using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ClearUC
{
    /// <summary>
    /// SeekBar.xaml の相互作用ロジック
    /// </summary>
    public partial class SeekBar : UserControl
    {
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public static readonly DependencyProperty BarDirectionProperty = DependencyProperty.Register("BarDirection", typeof(Direction), typeof(SeekBar),
            new PropertyMetadata(Direction.Horizonal));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(long), typeof(SeekBar));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(long), typeof(SeekBar));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(long), typeof(SeekBar));
        private Brush af;

        private Utils.AnimationHelper.Brush ba = null;

        private Config cnf = new Config();

        private bool downf = false;

        private Rectangle item;

        private long max = 100;

        private long min = 0;

        public SeekBar()
        {
            InitializeComponent();
            ApplyConfig(cnf);
        }

        public enum Direction
        {
            Vertical, Horizonal
        }

        public Direction BarDirection
        {
            get
            {
                Direction val = (Direction)GetValue(BarDirectionProperty);
                return val;
            }
            set
            {
                SetValue(BarDirectionProperty, value);
            }
        }

        public int EventInterval { get; set; } = 100;

        public long Maximum
        {
            get
            {
                long val = (long)GetValue(MaximumProperty);
                return val;
            }
            set
            {
                SetValue(MaximumProperty, value);
                CalcMargin();
            }
        }

        public long Minimum
        {
            get
            {
                long val = (long)GetValue(MinimumProperty);
                return val;
            }
            set
            {
                SetValue(MinimumProperty, value);
                CalcMargin();
            }
        }

        private void ThrowValueError()
        {
            if (Maximum < Value)
                throw new Exception();
            else if (Minimum > Value)
                throw new Exception();
        }

        public Config SeekBarConfig
        {
            get { return cnf; }
            set
            {
                cnf = value;
                ApplyConfig(cnf);
            }
        }

        public long Value
        {
            get { return (long)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                OnValueChanged(new ValueChangedEventArgs(ValueChangedEventArgs.ChangedType.Code, value));
                if (downf == false) CalcMargin();
            }
        }

        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        private void AnimateRectangleColor(Brush Before, Brush After,
            double Duration, Rectangle Item, DependencyProperty Property)
        {
            if (ba != null)
            {
                item.BeginAnimation(Shape.FillProperty, null);
                item.Fill = af;
                ba = null;
            }

            ba = new Utils.AnimationHelper.Brush();

            item = Item;
            af = After;
            Item.Fill = Before;

            ba.Storyboard.Completed += Storyboard_Completed;
            ba.Animation.FillBehavior = FillBehavior.Stop;
            PropertyPath pp = new PropertyPath("(0).(1)", Property, SolidColorBrush.ColorProperty);
            ba.Animate(Before, After, Duration, Item, pp);
        }

        private void ApplyConfig(Config Config)
        {
            bg.Fill = Config.BackBar;
            bg.Opacity = Config.BackBarOpacity;

            front.Fill = Config.FrontBar;
            front.Opacity = Config.FrontBarOpacity;

            thumb.Fill = Config.Thumb;
            thumb.Opacity = Config.ThumbOpacity;

            thumb.Stroke = Config.ThumbStroke;
            thumb.Width = cnf.ThumbSize;
            thumb.StrokeThickness = Config.ThumbStrokeThickness;
        }

        private void Bar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            downf = true;
            thumb_MouseDown(sender, e);
        }

        private void Bar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            downf = false;
            thumb_MouseUp(sender, e);
        }

        private void Bar_MouseMove(object sender, MouseEventArgs e)
        {
            if (downf == true)
            {
                CalcValue(e.GetPosition(this));
            }
        }

        private void bg_MouseEnter(object sender, MouseEventArgs e)
        {
            if (downf == true) return;
            AnimateRectangleColor(bg.Fill, cnf.BackBarMouseEnter, cnf.BackBarAnimationSpeed, bg, Shape.FillProperty);
        }

        private void bg_MouseLeave(object sender, MouseEventArgs e)
        {
            if (downf == true) return;
            AnimateRectangleColor(bg.Fill, cnf.BackBar, cnf.BackBarAnimationSpeed, bg, Shape.FillProperty);
        }

        private void bg_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalcMargin();
        }

        private void CalcMargin()
        {
            switch (BarDirection)
            {
                case Direction.Horizonal:
                    thumb.Width = cnf.ThumbSize;
                    thumb.Height = drain.ActualHeight;
                    if (Value == Minimum)
                    {
                        front.Width = 0;
                        bg.Width = drain.ActualWidth;
                        thumb.Margin = new Thickness(0, 0, 0, 0);
                        bg.Margin = new Thickness(0, 0, 0, 0);
                        front.Margin = new Thickness(0, 0, 0, 0);
                    }
                    else if (Value == Maximum)
                    {
                        bg.Width = 0;
                        front.Width = drain.ActualWidth;
                        thumb.Margin = new Thickness(drain.ActualWidth - thumb.ActualWidth, 0, 0, 0);
                        bg.Margin = new Thickness(0, 0, 0, 0);
                        front.Margin = new Thickness(0, 0, 0, 0);
                    }
                    else
                    {
                        double aw = (drain.ActualWidth - thumb.ActualWidth) / (Maximum - Minimum);

                        double av = Value - Minimum;
                        double loc = aw * av;
                        if (loc > drain.ActualWidth) loc = drain.ActualWidth - aw;

                        thumb.Margin = new Thickness(loc, 0, 0, 0);

                        bg.Width = (drain.ActualWidth - loc);
                        bg.Margin = new Thickness(loc + thumb.ActualWidth, 0, 0, 0);

                        front.Width = loc;
                        front.Margin = new Thickness(0, 0, drain.ActualWidth - loc, 0);
                    }
                    break;

                case Direction.Vertical:
                    thumb.Height = cnf.ThumbSize;
                    thumb.Width = drain.ActualWidth;
                    if (Value == Minimum)
                    {
                        front.Height = 0;
                        bg.Height = drain.ActualHeight;
                        thumb.Margin = new Thickness(0, drain.ActualHeight - thumb.ActualHeight, 0, 0);
                        bg.Margin = new Thickness(0, 0, 0, 0);
                        front.Margin = new Thickness(0, 0, 0, 0);
                    }
                    else if (Value == Maximum)
                    {
                        bg.Height = 0;
                        front.Height = drain.ActualHeight;
                        thumb.Margin = new Thickness(0, 0, 0, 0);
                        bg.Margin = new Thickness(0, 0, 0, 0);
                        front.Margin = new Thickness(0, 0, 0, 0);
                    }
                    else
                    {
                        double ah = (drain.ActualHeight - thumb.ActualHeight) / (Maximum - Minimum);

                        double av = Value - Minimum;
                        double loc = ah * av;
                        if (loc > drain.ActualHeight) loc = drain.ActualHeight - ah;

                        thumb.Margin = new Thickness(0, loc, 0, 0);

                        bg.Height = (drain.ActualHeight - loc);
                        bg.Margin = new Thickness(0, 0, 0, 0);

                        front.Height = loc;
                        front.Margin = new Thickness(0, drain.ActualHeight - loc, 0, 0);
                    }
                    break;
            }
        }

        private void CalcValue(Point Point)
        {
            switch (BarDirection)
            {
                case Direction.Horizonal:
                    double aw = (drain.ActualWidth - thumb.ActualWidth) / (Maximum - Minimum);
                    long valu = (long)(Point.X / aw);

                    if (valu > Maximum)
                    {
                        SetValue(ValueProperty, Maximum);
                        OnValueChanged(new ValueChangedEventArgs(ValueChangedEventArgs.ChangedType.Manual, Maximum));

                        CalcMargin();
                    }
                    else
                    {
                        SetValue(ValueProperty, valu);
                        OnValueChanged(new ValueChangedEventArgs(ValueChangedEventArgs.ChangedType.Manual, valu));

                        CalcMargin();
                    }
                    break;

                case Direction.Vertical:
                    double ah = (drain.ActualHeight - thumb.ActualHeight) / (Maximum - Minimum);
                    double valuv = Point.Y / ah;

                    if (valuv > Maximum)
                    {
                        SetValue(ValueProperty, Maximum);
                        OnValueChanged(new ValueChangedEventArgs(ValueChangedEventArgs.ChangedType.Manual, Maximum));

                        CalcMargin();
                    }
                    else
                    {
                        SetValue(ValueProperty, valuv);
                        OnValueChanged(new ValueChangedEventArgs(ValueChangedEventArgs.ChangedType.Manual, valuv));

                        CalcMargin();
                    }
                    break;
            }
        }

        private void front_MouseEnter(object sender, MouseEventArgs e)
        {
            if (downf == true) return;
            AnimateRectangleColor(front.Fill, cnf.FrontBarMouseEnter, cnf.BackBarAnimationSpeed, front, Shape.FillProperty);
        }

        private void front_MouseLeave(object sender, MouseEventArgs e)
        {
            if (downf == true) return;
            AnimateRectangleColor(front.Fill, cnf.FrontBar, cnf.BackBarAnimationSpeed, front, Shape.FillProperty);
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            downf = true;
        }

        private void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            downf = false;
            AnimateRectangleColor(thumb.Fill, cnf.Thumb, cnf.ThumbAnimationSpeed, thumb, Shape.FillProperty);
        }

        private void grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            downf = false;
        }

        private void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalcMargin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            item.Fill = af;
        }

        private void thumb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AnimateRectangleColor(bg.Fill, cnf.BackBar, cnf.BackBarAnimationSpeed, bg, Shape.FillProperty);
            AnimateRectangleColor(front.Fill, cnf.FrontBar, cnf.BackBarAnimationSpeed, front, Shape.FillProperty);

            CalcValue(e.GetPosition(this));
            AnimateRectangleColor(thumb.Fill, cnf.ThumbMouseClick, cnf.ThumbAnimationSpeed, thumb, Shape.FillProperty);
        }

        private void thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (downf == true) return;
            AnimateRectangleColor(thumb.Fill, cnf.ThumbMouseEnter, cnf.ThumbAnimationSpeed, thumb, Shape.FillProperty);
        }

        private void thumb_MouseLeave(object sender, MouseEventArgs e)
        {
            if (downf == true) return;
            AnimateRectangleColor(thumb.Fill, cnf.Thumb, cnf.ThumbAnimationSpeed, thumb, Shape.FillProperty);
        }

        private void thumb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CalcValue(e.GetPosition(this));
            OnValueChanged(new ValueChangedEventArgs(ValueChangedEventArgs.ChangedType.ManualEnd, Value));
            AnimateRectangleColor(thumb.Fill, cnf.ThumbMouseEnter, cnf.ThumbAnimationSpeed, thumb, Shape.FillProperty);
        }

        public class Config
        {
            //BackBar
            public Brush BackBar { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));

            public double BackBarAnimationSpeed { get; set; } = 50;

            public Brush BackBarMouseClick { get; set; } = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

            public Brush BackBarMouseEnter { get; set; } = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));

            public double BackBarOpacity { get; set; } = 0.5;

            //FrontBar
            public Brush FrontBar { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 150, 0));

            public double FrontBarAnimationSpeed { get; set; } = 50;
            public Brush FrontBarMouseClick { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 250, 100));
            public Brush FrontBarMouseEnter { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 200, 50));
            public double FrontBarOpacity { get; set; } = 0.8;

            //Thumb
            public Brush Thumb { get; set; } = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));

            public double ThumbAnimationSpeed { get; set; } = 50;
            public Brush ThumbMouseClick { get; set; } = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
            public Brush ThumbMouseEnter { get; set; } = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
            public double ThumbOpacity { get; set; } = 1;
            public double ThumbSize { get; set; } = 10;
            public Brush ThumbStroke { get; set; } = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
            public double ThumbStrokeThickness { get; set; } = 0;
        }

        public class ValueChangedEventArgs : EventArgs
        {
            public ValueChangedEventArgs(ChangedType ChangeType, double Value)
            {
                this.ChangeType = ChangeType;
                this.Value = Value;
            }

            public enum ChangedType
            {
                Manual, Code, ManualEnd
            }

            public ChangedType ChangeType { get; private set; }

            public double Value { get; private set; }
        }
    }
}