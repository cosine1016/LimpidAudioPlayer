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

namespace MVPUC.Toggles
{
    /// <summary>
    /// Shuffle.xaml の相互作用ロジック
    /// </summary>
    public partial class Repeat : UserControl
    {
        public event EventHandler<MouseButtonEventArgs> Click;
        public event EventHandler StateChanged;

        public class Config
        {
            internal static Brush Disabled { get; set; } = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

            internal static Brush Enabled { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 150, 0));
            internal static Brush EnabledIcon { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 160, 0));
            internal static Brush EnabledText { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));

            public static bool Animate { get; set; } = true;
            public static int AnimationDuration { get; set; } = 100;
        }

        public enum State { Disable, Repeat, SingleRepeat };
        State ts = State.Disable;
        public State ToggleState
        {
            get { return ts; }
            set
            {
                ts = value;
                if (StateChanged != null) StateChanged(this, new EventArgs());
            }
        }
        
        public Repeat()
        {
            InitializeComponent();
            Click += Shuffle_Click;
            StateChanged += Repeat_StateChanged;
        }

        private void Repeat_StateChanged(object sender, EventArgs e)
        {
            switch (ToggleState)
            {
                case State.Disable:
                    Disable();
                    break;
                case State.Repeat:
                    EnableRepeat();
                    break;
                case State.SingleRepeat:
                    EnableSingleRepeat();
                    break;
            }
        }

        private void Shuffle_Click(object sender, MouseButtonEventArgs e)
        {
            if(ToggleOnClick == true)
            {
                Switch();
            }
        }
        
        public Brush DisabledShape
        {
            get { return Config.Disabled; }
            set
            {
                Config.Disabled = value;
                if (ToggleState == State.Disable) repeat.Fill = value;
            }
        }
        
        public Brush EnabledShape
        {
            get { return Config.Enabled; }
            set
            {
                Config.Enabled = value;
                if (ToggleState != State.Disable) repeat.Fill = value;
            }
        }

        public Brush EnabledIcon
        {
            get { return Config.EnabledIcon; }
            set
            {
                Config.EnabledIcon = value;
                single_Circle.Fill = value;
            }
        }

        public Brush EnabledText
        {
            get { return Config.EnabledText; }
            set
            {
                Config.EnabledText = value;
                single_Text.Fill = value;
            }
        }

        public bool ToggleOnClick { get; set; } = true;

        public void Switch()
        {
            switch (ToggleState)
            {
                case State.SingleRepeat:
                    ToggleState = State.Disable;
                    break;
                case State.Disable:
                    ToggleState = State.Repeat;
                    break;
                case State.Repeat:
                    ToggleState = State.SingleRepeat;
                    break;
            }
        }

        private void EnableSingleRepeat()
        {
            if (Config.Animate == true)
            {
                single_Circle.Opacity = 0;
                single_Circle.Opacity = 0;
                single_Circle.Visibility = Visibility.Visible;
                single_Text.Visibility = Visibility.Visible;

                ClearUC.Utils.AnimationHelper.Brush ba = new ClearUC.Utils.AnimationHelper.Brush();
                PropertyPath pp = new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty);
                ba.Animate(repeat.Fill, Config.Enabled, Config.AnimationDuration, repeat, pp);

                ClearUC.Utils.AnimationHelper.Double di = new ClearUC.Utils.AnimationHelper.Double();
                di.Animate(single_Circle.Opacity, 1, Config.AnimationDuration, null, OpacityProperty, single_Circle);

                ClearUC.Utils.AnimationHelper.Double dt = new ClearUC.Utils.AnimationHelper.Double();
                dt.Animate(single_Text.Opacity, 1, Config.AnimationDuration, null, OpacityProperty, single_Text);
            }
            else
            {
                repeat.Fill = Config.Enabled;
                single_Circle.Visibility = Visibility.Visible;
                single_Text.Visibility = Visibility.Visible;
            }
        }

        private void EnableRepeat()
        {
            if (Config.Animate == true)
            {
                ClearUC.Utils.AnimationHelper.Brush ba = new ClearUC.Utils.AnimationHelper.Brush();
                PropertyPath pp = new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty);
                ba.Animate(repeat.Fill, Config.Enabled, Config.AnimationDuration, repeat, pp);

                ClearUC.Utils.AnimationHelper.Double di = new ClearUC.Utils.AnimationHelper.Double();
                di.AnimationCompleted += Di_AnimationCompleted;
                di.Animate(single_Circle.Opacity, 0, Config.AnimationDuration, null, OpacityProperty, single_Circle);

                ClearUC.Utils.AnimationHelper.Double dt = new ClearUC.Utils.AnimationHelper.Double();
                dt.AnimationCompleted += Di_AnimationCompleted;
                dt.Animate(single_Text.Opacity, 0, Config.AnimationDuration, null, OpacityProperty, single_Text);
            }
            else
            {
                repeat.Fill = Config.Enabled;
                single_Circle.Visibility = Visibility.Hidden;
                single_Text.Visibility = Visibility.Hidden;
            }
        }

        private void Di_AnimationCompleted(object sender, ClearUC.Utils.AnimationHelper.AnimationEventArgs e)
        {
            single_Circle.Visibility = Visibility.Hidden;
            single_Text.Visibility = Visibility.Hidden;
        }

        private void Disable()
        {
            if (Config.Animate == true)
            {
                ClearUC.Utils.AnimationHelper.Brush ba = new ClearUC.Utils.AnimationHelper.Brush();
                PropertyPath pp = new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty);
                ba.Animate(repeat.Fill, Config.Disabled, Config.AnimationDuration, repeat, pp);

                ClearUC.Utils.AnimationHelper.Double di = new ClearUC.Utils.AnimationHelper.Double();
                di.AnimationCompleted += Di_AnimationCompleted;
                di.Animate(single_Circle.Opacity, 0, Config.AnimationDuration, null, OpacityProperty, single_Circle);

                ClearUC.Utils.AnimationHelper.Double dt = new ClearUC.Utils.AnimationHelper.Double();
                dt.AnimationCompleted += Di_AnimationCompleted;
                dt.Animate(single_Text.Opacity, 0, Config.AnimationDuration, null, OpacityProperty, single_Text);
            }
            else
            {
                repeat.Fill = Config.Disabled;
                single_Circle.Visibility = Visibility.Hidden;
                single_Text.Visibility = Visibility.Hidden;
            }
        }

        private void drain_MouseEnter(object sender, MouseEventArgs e)
        {
            downf = false;
        }

        private void drain_MouseLeave(object sender, MouseEventArgs e)
        {
            downf = false;
        }

        bool downf = false;
        private void drain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            downf = true;
        }

        private void drain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(downf == true)
            {
                if (Click != null) Click(this, e);
                downf = false;
            }
        }
    }
}
