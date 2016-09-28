using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LAP.Utils
{
    public class Animation
    {
        public class SwitchVisibility
        {
            private FrameworkElement VI;
            private FrameworkElement HI;

            public void Animate(double Duration, FrameworkElement VisibleItem, FrameworkElement HideItem)
            {
                VI = VisibleItem;
                HI = HideItem;

                VisibleItem.Opacity = 0;
                VisibleItem.Visibility = Visibility.Visible;
                ClearUC.Utils.AnimationHelper.Double dav = new ClearUC.Utils.AnimationHelper.Double();
                dav.AnimationCompleted += Dav_AnimationCompleted;
                dav.Animate(VisibleItem.Opacity, 1, Duration, null, UIElement.OpacityProperty, VisibleItem);

                if (HI.Opacity != 0)
                {
                    ClearUC.Utils.AnimationHelper.Double dah = new ClearUC.Utils.AnimationHelper.Double();
                    dah.AnimationCompleted += Dah_AnimationCompleted;
                    dah.Animate(HideItem.Opacity, 0, Duration, null, UIElement.OpacityProperty, HideItem);
                }
            }

            private void Dah_AnimationCompleted(object sender, ClearUC.Utils.AnimationHelper.AnimationEventArgs e)
            {
                ((ClearUC.Utils.AnimationHelper.Double)sender).AnimationCompleted -= Dah_AnimationCompleted;
                HI.Opacity = 0;
                HI.Visibility = Visibility.Hidden;
            }

            private void Dav_AnimationCompleted(object sender, ClearUC.Utils.AnimationHelper.AnimationEventArgs e)
            {
                ((ClearUC.Utils.AnimationHelper.Double)sender).AnimationCompleted -= Dav_AnimationCompleted;
                VI.Opacity = 1;
                VI.Visibility = Visibility.Visible;
            }
        }

        public class Visible
        {
            public event EventHandler AnimationCompleted;

            private Visibility V;
            private FrameworkElement E;

            public void Animate(double Duration, FrameworkElement Item, Visibility Visible)
            {
                V = Visible;
                E = Item;

                ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
                da.AnimationCompleted += Da_AnimationCompleted;
                switch (Visible)
                {
                    case Visibility.Visible:
                        Item.Opacity = 0;
                        Item.Visibility = Visibility.Visible;
                        da.Animate(Item.Opacity, 1, Duration, null, UIElement.OpacityProperty, Item);
                        break;

                    case Visibility.Hidden:
                        da.Animate(Item.Opacity, 0, Duration, null, UIElement.OpacityProperty, Item);
                        break;
                }
            }

            public void Animate(double Duration, FrameworkElement Item, double Opacity)
            {
                Item.Visibility = Visibility.Visible;
                ClearUC.Utils.AnimationHelper.Double da = new ClearUC.Utils.AnimationHelper.Double();
                da.AnimationCompleted += Da_AnimationCompleted;
                da.Animate(Item.Opacity, Opacity, Duration, null, UIElement.OpacityProperty, Item);
            }

            private void Da_AnimationCompleted(object sender, ClearUC.Utils.AnimationHelper.AnimationEventArgs e)
            {
                ((ClearUC.Utils.AnimationHelper.Double)sender).AnimationCompleted -= Da_AnimationCompleted;
                switch (V)
                {
                    case Visibility.Hidden:
                        E.Visibility = Visibility.Hidden;
                        break;
                }

                AnimationCompleted?.Invoke(this, new EventArgs());
            }
        }
    }
}