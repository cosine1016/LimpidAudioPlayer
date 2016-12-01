using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;

namespace ClearUC
{
    internal static class BitmapImageExtensions
    {
        public static bool IsEqual(this BitmapImage image1, BitmapImage image2)
        {
            if (image1 == null || image2 == null)
            {
                return false;
            }
            return image1.ToBytes().SequenceEqual(image2.ToBytes());
        }

        public static byte[] ToBytes(this BitmapImage image)
        {
            byte[] data = new byte[] { };
            if (image != null)
            {
                try
                {
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image, null, null, null));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        data = ms.ToArray();
                    }
                    return data;
                }
                catch (Exception)
                {
                }
            }
            return data;
        }
    }

    /// <summary>
    /// MaskImage.xaml の相互作用ロジック
    /// </summary>
    public partial class MaskImage : UserControl
    {
        public MaskImage()
        {
            InitializeComponent();

            VisibleImageControl = image1;
            HiddenImageControl = image2;
        }

        public int DecodePixelHeight { get; set; } = 100;

        public ImageSource Image
        {
            get { return VisibleImageControl.Source; }
            set
            {
                if (Transition)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (!((BitmapImage)value).IsEqual(((BitmapImage)VisibleImageControl.Source)))
                        {
                            BitmapImage bimage = (BitmapImage)value;
                            bimage.DecodePixelHeight = DecodePixelHeight;
                            bimage.DecodePixelWidth = (int)(1.0 * bimage.PixelHeight / DecodePixelHeight * bimage.PixelWidth);
                            HiddenImageControl.Source = bimage;

                            Utils.AnimationHelper.Double hide = new Utils.AnimationHelper.Double();
                            hide.Animate(VisibleImageControl.Opacity, 0, TransitionDuration, null, OpacityProperty, VisibleImageControl);

                            Utils.AnimationHelper.Double show = new Utils.AnimationHelper.Double();
                            show.Animate(HiddenImageControl.Opacity, 1, TransitionDuration, null, OpacityProperty, HiddenImageControl);

                            Image bf = VisibleImageControl, bb = HiddenImageControl;
                            VisibleImageControl = bb;
                            HiddenImageControl = bf;
                        }
                    }));
                }
                else
                {
                    BitmapImage bimage = (BitmapImage)value;
                    bimage.DecodePixelHeight = DecodePixelHeight;
                    bimage.DecodePixelWidth = (int)(1.0 * bimage.PixelHeight / DecodePixelHeight * bimage.PixelWidth);
                    VisibleImageControl.Source = bimage;
                }
            }
        }

        public Stretch Stretch
        {
            get { return VisibleImageControl.Stretch; }
            set
            {
                VisibleImageControl.Stretch = value;
                HiddenImageControl.Stretch = value;
            }
        }

        public StretchDirection StretchDirection
        {
            get { return VisibleImageControl.StretchDirection; }
            set
            {
                VisibleImageControl.StretchDirection = value;
                HiddenImageControl.StretchDirection = value;
            }
        }

        public bool Transition { get; set; } = true;

        public double TransitionDuration { get; set; } = 300;

        private Image VisibleImageControl { get; set; }

        private Image HiddenImageControl { get; set; }

        public KernelType KernelType
        {
            get { return ImageBlur.KernelType; }
            set { ImageBlur.KernelType = value; }
        }

        public double Radius
        {
            get { return ImageBlur.Radius; }
            set { ImageBlur.Radius = value; }
        }

        public RenderingBias RenderingBias
        {
            get { return ImageBlur.RenderingBias; }
            set { ImageBlur.RenderingBias = value; }
        }

        public Brush Mask
        {
            get { return mask.Fill; }
            set
            {
                mask.Fill = value;
            }
        }

        public double MaskOpacity
        {
            get { return mask.Opacity; }
            set
            {
                mask.Opacity = value;
            }
        }
    }
}