using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ClearUC
{
    public class Utils
    {
        /// <summary>
        /// HSL (HLS) カラーを表す
        /// </summary>
        public class HslColor
        {
            private float _h;
            /// <summary>
            /// 色相 (Hue)
            /// </summary>
            public float H
            {
                get { return this._h; }
            }

            private float _s;
            /// <summary>
            /// 彩度 (Saturation)
            /// </summary>
            public float S
            {
                get { return this._s; }
            }

            private float _l;
            /// <summary>
            /// 輝度 (Lightness)
            /// </summary>
            public float L
            {
                get { return this._l; }
            }

            public HslColor(float hue, float saturation, float lightness)
            {
                if (hue == 360f) hue = 0f;
                if (hue < 0f || 360f < hue)
                {
                    throw new ArgumentException(
                        "hueは0以上360未満の値です。", "hue");
                }
                if (saturation < 0f || 1f < saturation)
                {
                    throw new ArgumentException(
                        "saturationは0以上1以下の値です。", "saturation");
                }
                if (lightness < 0f || 1f < lightness)
                {
                    throw new ArgumentException(
                        "lightnessは0以上1以下の値です。", "lightness");
                }

                this._h = hue;
                this._s = saturation;
                this._l = lightness;
            }

            /// <summary>
            /// 指定したColorからHslColorを作成する
            /// </summary>
            /// <param name="rgb">Color</param>
            /// <returns>HslColor</returns>
            public static HslColor FromRgb(Color rgb)
            {
                float r = (float)rgb.R / 255f;
                float g = (float)rgb.G / 255f;
                float b = (float)rgb.B / 255f;

                float max = Math.Max(r, Math.Max(g, b));
                float min = Math.Min(r, Math.Min(g, b));

                float lightness = (max + min) / 2f;

                float hue, saturation;
                if (max == min)
                {
                    //undefined
                    hue = 0f;
                    saturation = 0f;
                }
                else
                {
                    float c = max - min;

                    if (max == r)
                    {
                        hue = (g - b) / c;
                    }
                    else if (max == g)
                    {
                        hue = (b - r) / c + 2f;
                    }
                    else
                    {
                        hue = (r - g) / c + 4f;
                    }
                    hue *= 60f;
                    if (hue < 0f)
                    {
                        hue += 360f;
                    }

                    //saturation = c / (1f - Math.Abs(2f * lightness - 1f));
                    if (lightness < 0.5f)
                    {
                        saturation = c / (max + min);
                    }
                    else
                    {
                        saturation = c / (2f - max - min);
                    }
                }

                return new HslColor(hue, saturation, lightness);
            }

            /// <summary>
            /// 指定したHslColorからColorを作成する
            /// </summary>
            /// <param name="hsl">HslColor</param>
            /// <returns>Color</returns>
            public static Color ToRgb(HslColor hsl)
            {
                float s = hsl.S;
                float l = hsl.L;

                float r1, g1, b1;
                if (s == 0)
                {
                    r1 = l;
                    g1 = l;
                    b1 = l;
                }
                else
                {
                    float h = hsl.H / 60f;
                    int i = (int)Math.Floor(h);
                    float f = h - i;
                    //float c = (1f - Math.Abs(2f * l - 1f)) * s;
                    float c;
                    if (l < 0.5f)
                    {
                        c = 2f * s * l;
                    }
                    else
                    {
                        c = 2f * s * (1f - l);
                    }
                    float m = l - c / 2f;
                    float p = c + m;
                    //float x = c * (1f - Math.Abs(h % 2f - 1f));
                    float q; // q = x + m
                    if (i % 2 == 0)
                    {
                        q = l + c * (f - 0.5f);
                    }
                    else
                    {
                        q = l - c * (f - 0.5f);
                    }

                    switch (i)
                    {
                        case 0:
                            r1 = p;
                            g1 = q;
                            b1 = m;
                            break;
                        case 1:
                            r1 = q;
                            g1 = p;
                            b1 = m;
                            break;
                        case 2:
                            r1 = m;
                            g1 = p;
                            b1 = q;
                            break;
                        case 3:
                            r1 = m;
                            g1 = q;
                            b1 = p;
                            break;
                        case 4:
                            r1 = q;
                            g1 = m;
                            b1 = p;
                            break;
                        case 5:
                            r1 = p;
                            g1 = m;
                            b1 = q;
                            break;
                        default:
                            throw new ArgumentException(
                                "色相の値が不正です。", "hsl");
                    }
                }

                return Color.FromArgb(255,
                    (byte)Math.Round(r1 * 255f),
                    (byte)Math.Round(g1 * 255f),
                    (byte)Math.Round(b1 * 255f));
            }
        }

        public class AnimationHelper
        {
            public class SwitchVisibility
            {
                FrameworkElement VI;
                FrameworkElement HI;

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
                    HI.Opacity = 0;
                    HI.Visibility = Visibility.Hidden;
                }

                private void Dav_AnimationCompleted(object sender, ClearUC.Utils.AnimationHelper.AnimationEventArgs e)
                {
                    VI.Opacity = 1;
                    VI.Visibility = Visibility.Visible;
                }
            }

            public class Visible
            {
                public event EventHandler AnimationCompleted;

                Visibility V;
                FrameworkElement E;

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

                private void Da_AnimationCompleted(object sender, ClearUC.Utils.AnimationHelper.AnimationEventArgs e)
                {
                    switch (V)
                    {
                        case Visibility.Hidden:
                            E.Visibility = Visibility.Hidden;
                            break;
                    }

                    AnimationCompleted?.Invoke(this, new EventArgs());
                }
            }

            public class AnimationEventArgs : EventArgs
            {
                public AnimationEventArgs(object AnimatedObject)
                {
                    this.AnimatedObject = AnimatedObject;
                }

                public object AnimatedObject;
            }

            public class Brush
            {
                private System.Windows.Media.Brush af;
                private FrameworkElement item;
                private PropertyPath Property;

                public event EventHandler Completed;

                protected virtual void OnCompleted(EventArgs e)
                {
                    Completed?.Invoke(this, e);
                }

                public Brush()
                {
                    Storyboard.Completed += Storyboard_Completed;
                }

                private void Storyboard_Completed(object sender, EventArgs e)
                {
                    if (item != null)
                    {
                        DependencyProperty dp = (DependencyProperty)Property.PathParameters[0];
                        item.SetValue(dp, af);
                    }
                    OnCompleted(e);
                }

                public Storyboard Storyboard { get; private set; } = new Storyboard();

                public ColorAnimation Animation { get; private set; } = new ColorAnimation();

                public void Animate(System.Windows.Media.Brush Before, System.Windows.Media.Brush After,
                double Duration, FrameworkElement Item, PropertyPath Property)
                {
                    this.af = After;
                    this.Property = Property;
                    item = Item;

                    Storyboard.Children.Clear();
                    if (Before.GetType() == typeof(SolidColorBrush))
                    {
                        Color be = ((SolidColorBrush)Before).Color;
                        Color af = ((SolidColorBrush)After).Color;

                        Animation.From = be;
                        Animation.To = af;
                        Animation.Duration = TimeSpan.FromMilliseconds(Duration);

                        Animation.FillBehavior = FillBehavior.Stop;

                        Storyboard.SetTargetProperty(Animation, Property);
                        Storyboard.Children.Add(Animation);

                        Storyboard.Begin(Item, true);
                    }

                    if (Before.GetType() == typeof(LinearGradientBrush))
                    {
                        LinearGradientBrush be = (LinearGradientBrush)Before;
                        LinearGradientBrush af = (LinearGradientBrush)After;

                        for (int i = 0; be.GradientStops.Count > i; i++)
                        {
                            ColorAnimation CA = new ColorAnimation();
                            CA.From = be.GradientStops[i].Color;
                            CA.To = af.GradientStops[i].Color;
                            CA.Duration = TimeSpan.FromMilliseconds(Duration);
                            CA.FillBehavior = FillBehavior.Stop;
                            Storyboard.SetTargetProperty(CA, Property);
                            Storyboard.Children.Add(CA);
                        }

                        Storyboard.Begin();
                    }
                }
            }

            public class Thickness
            {
                public event EventHandler<AnimationEventArgs> AnimationCompleted;

                protected virtual void OnAnimationCompleted(AnimationEventArgs e)
                {
                    if (AnimationCompleted != null) AnimationCompleted(this, e);
                }

                private System.Windows.Thickness af;
                private FrameworkElement cnr;

                public void Animate(System.Windows.Thickness Before, System.Windows.Thickness After, double Duration, IEasingFunction Easing, PropertyPath Property, FrameworkElement Item)
                {
                    if (Before == null || After == null) return;
                    if (Duration == 0 || Before == After)
                    {
                        Item.Margin = After;
                        return;
                    }

                    Item.Margin = Before;
                    af = After;
                    cnr = Item;

                    Storyboard sb = new Storyboard();
                    ThicknessAnimation ta = new ThicknessAnimation()
                    {
                        From = Before,
                        To = After,
                        Duration = TimeSpan.FromMilliseconds(Duration),
                        FillBehavior = FillBehavior.Stop,
                        EasingFunction = Easing
                    };

                    Storyboard.SetTargetProperty(ta, Property);
                    sb.Children.Add(ta);

                    sb.Completed += Sb_Completed;
                    Item.BeginStoryboard(sb);
                }

                private void Sb_Completed(object sender, EventArgs e)
                {
                    cnr.Margin = af;
                    OnAnimationCompleted(new AnimationEventArgs(cnr));
                }
            }

            public class Double
            {
                public event EventHandler<AnimationEventArgs> AnimationCompleted;

                protected virtual void OnAnimationCompleted(AnimationEventArgs e)
                {
                    if (AnimationCompleted != null) AnimationCompleted(this, e);
                }

                private DependencyProperty dp;
                private double af;
                private FrameworkElement cnr;

                public void Animate(double Before, double After, double Duration, IEasingFunction Easing, DependencyProperty Property, FrameworkElement Item)
                {
                    Cancel = false;
                    IsCompleted = false;
                    if (Duration == 0 || Before == After)
                    {
                        Item.SetValue(Property, After);
                        return;
                    }

                    this.Before = Before;
                    Item.SetValue(Property, Before);
                    af = After;
                    dp = Property;
                    cnr = Item;

                    Storyboard sb = new Storyboard();
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        From = Before,
                        To = After,
                        Duration = TimeSpan.FromMilliseconds(Duration),
                        FillBehavior = FillBehavior.Stop,
                        EasingFunction = Easing
                    };

                    Storyboard.SetTargetProperty(da, new PropertyPath(Property));
                    sb.Children.Add(da);

                    sb.Completed += Sb_Completed;
                    Item.BeginStoryboard(sb);
                }

                private void Sb_Completed(object sender, EventArgs e)
                {
                    IsCompleted = true;
                    if (Cancel == false)
                    {
                        cnr.SetValue(dp, af);
                        OnAnimationCompleted(new AnimationEventArgs(cnr));
                    }
                }

                public bool Cancel { get; set; } = false;

                public bool IsCompleted { get; set; } = true;

                public double Before { get; private set; }
            }
        }

        public class BrushColorConverter
        {
            public static Brush ToBrush(Color Color)
            {
                return new SolidColorBrush(Color);
            }

            public static Color ToColor(Brush Brush)
            {
                SolidColorBrush brush = Brush as SolidColorBrush;
                if (brush != null)
                {
                    return brush.Color;
                }
                return Colors.Transparent;
            }
        }
    }

    /// <summary>
    /// intクラスを拡張する
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// [count] 回 処理を繰り返す
        ///
        /// 使用例： 3.Times( i => Console.WriteLine(i.ToString()));
        /// </summary>
        /// <param name="count">繰り返す回数</param>
        /// <param name="action">実行させたいActionデリゲート</param>
        public static void Times(this int count, Action<int> action)
        {
            for (int i = 1; i <= count; i++)
                action(i);
        }
    }
}