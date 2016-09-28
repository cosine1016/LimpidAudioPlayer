using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClearUC
{
    /// <summary>
    /// ToggleButton.xaml の相互作用ロジック
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        public event EventHandler ToggleStateChanged;

        public ToggleButton()
        {
            InitializeComponent();
        }

        public bool Animate { get; set; } = false;

        public double AnimationDuration { get; set; } = 100;

        private bool st = false;

        public bool State
        {
            get { return st; }
            set
            {
                switch (value)
                {
                    case true:
                        ToOn();
                        break;

                    case false:
                        ToOff();
                        break;
                }
            }
        }

        private void SeekBar_ValueChanged(object sender, SeekBar.ValueChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case SeekBar.ValueChangedEventArgs.ChangedType.Manual:
                    break;

                case SeekBar.ValueChangedEventArgs.ChangedType.Code:
                    break;
            }
        }

        private bool downf = false;
        private Point def;
        private Point moves;

        private void bar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            downf = true;
            def = e.GetPosition(this);
            moves = new Point(0, 0);
        }

        private void bar_MouseMove(object sender, MouseEventArgs e)
        {
            if (downf == true)
            {
                Point pos = e.GetPosition(this);
                moves.X = def.X - pos.X;
                moves.Y = def.Y - pos.Y;
            }
        }

        private void bar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (downf == true)
            {
                if (moves.X <= 10 && moves.X >= -10)
                {
                    if (moves.Y <= 10 && moves.Y >= -10)
                    {
                        switch (State)
                        {
                            case false:
                                ToOn();
                                break;

                            case true:
                                ToOff();
                                break;
                        }
                    }
                }
                else
                {
                    double sep = (bar.Maximum - bar.Minimum) / 2;
                    if (bar.Value >= sep)
                    {
                        ToOn();
                    }
                    else
                    {
                        ToOff();
                    }
                }
            }

            downf = false;
        }

        public void Switch()
        {
            switch (State)
            {
                case false:
                    ToOn();
                    break;

                case true:
                    ToOff();
                    break;
            }
        }

        private void ToOn()
        {
            if (bar.Value == bar.Maximum)
            {
                st = true;
                if (ToggleStateChanged != null) ToggleStateChanged(this, new EventArgs());
                return;
            }

            if (Animate == true)
            {
                Utils.AnimationHelper.Double da = new Utils.AnimationHelper.Double();
                da.Animate(bar.Value, bar.Maximum, AnimationDuration, null, SeekBar.ValueProperty, bar);
                st = true;
            }
            else
            {
                bar.Value = bar.Maximum;
                st = true;
            }
            if (ToggleStateChanged != null) ToggleStateChanged(this, new EventArgs());
        }

        private void ToOff()
        {
            if (bar.Value == bar.Minimum)
            {
                st = false;
                if (ToggleStateChanged != null) ToggleStateChanged(this, new EventArgs());
                return;
            }

            if (Animate == true)
            {
                Utils.AnimationHelper.Double da = new Utils.AnimationHelper.Double();
                da.Animate(bar.Value, bar.Minimum, AnimationDuration, null, SeekBar.ValueProperty, bar);
                st = false;
            }
            else
            {
                bar.Value = bar.Minimum;
                st = false;
            }
            if (ToggleStateChanged != null) ToggleStateChanged(this, new EventArgs());
        }
    }
}