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
    public partial class Shuffle : UserControl
    {
        public event EventHandler<MouseButtonEventArgs> Click;
        public event EventHandler StateChanged;

        public class Config
        {
            internal static Brush Disabled { get; set; } = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

            internal static Brush Enabled { get; set; } = new SolidColorBrush(Color.FromArgb(255, 255, 150, 0));

            public static bool Animate { get; set; } = true;
            public static int AnimationDuration { get; set; } = 100;
        }

        bool ts = false;
        public bool ToggleState
        {
            get { return ts; }
            set
            {
                ts = value;
                if (StateChanged != null) StateChanged(this, new EventArgs());
            }
        }
        
        public Shuffle()
        {
            InitializeComponent();
            Click += Shuffle_Click;
        }

        private void Shuffle_Click(object sender, MouseButtonEventArgs e)
        {
            if(ToggleOnClick == true)
            {
                Switch();
            }
        }
        
        public Brush DisabledBackShape
        {
            get { return Config.Disabled; }
            set
            {
                Config.Disabled = value;
                if (ToggleState == false) shuffle.Fill = value;
            }
        }
        
        public Brush EnabledBackShape
        {
            get { return Config.Enabled; }
            set
            {
                Config.Enabled = value;
                if (ToggleState == true) shuffle.Fill = value;
            }
        }

        public bool ToggleOnClick { get; set; } = true;

        public void Switch()
        {
            switch (ToggleState)
            {
                case true:
                    Disable();
                    break;
                case false:
                    Enable();
                    break;
            }
        }

        private void Enable()
        {
            ToggleState = true;
            if (Config.Animate == true)
            {
                ClearUC.Utils.AnimationHelper.Brush ba = new ClearUC.Utils.AnimationHelper.Brush();
                PropertyPath pp = new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty);
                ba.Animate(shuffle.Fill, Config.Enabled, Config.AnimationDuration, shuffle, pp);
            }
            else
            {
                shuffle.Fill = Config.Enabled;
            }
        }

        private void Disable()
        {
            ToggleState = false;
            if (Config.Animate == true)
            {
                ClearUC.Utils.AnimationHelper.Brush ba = new ClearUC.Utils.AnimationHelper.Brush();
                PropertyPath pp = new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty);
                ba.Animate(shuffle.Fill, Config.Disabled, Config.AnimationDuration, shuffle, pp);
            }
            else
            {
                shuffle.Fill = Config.Disabled;
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
                if (ToggleOnClick == true) return;
                
                ClearUC.Utils.AnimationHelper.Brush ba = new ClearUC.Utils.AnimationHelper.Brush();
                PropertyPath pp = new PropertyPath("(0).(1)", Shape.FillProperty, SolidColorBrush.ColorProperty);
                switch (ToggleState)
                {

                    case true:
                        if (Config.Animate == true)
                        {
                            ba.Animate(shuffle.Fill, Config.Enabled, Config.AnimationDuration, shuffle, pp);
                        }
                        else
                        {
                            shuffle.Fill = Config.Enabled;
                        }
                        break;
                    case false:
                        if (Config.Animate == true)
                        {
                            ba.Animate(shuffle.Fill, Config.Disabled, Config.AnimationDuration, shuffle, pp);
                        }
                        else
                        {
                            shuffle.Fill = Config.Disabled;
                        }
                        break;
                }
            }
        }
    }
}
