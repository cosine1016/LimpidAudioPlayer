using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ClearUC
{
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

        public ImageSource Image
        {
            get { return VisibleImageControl.Source; }
            set
            {
                if (Transition)
                {
                    HiddenImageControl.Source = value;

                    Utils.AnimationHelper.Double hide = new Utils.AnimationHelper.Double();
                    hide.Animate(VisibleImageControl.Opacity, 0, TransitionDuration, null, OpacityProperty, VisibleImageControl);

                    Utils.AnimationHelper.Double show = new Utils.AnimationHelper.Double();
                    show.Animate(HiddenImageControl.Opacity, 1, TransitionDuration, null, OpacityProperty, HiddenImageControl);

                    Image bf = VisibleImageControl, bb = HiddenImageControl;
                    VisibleImageControl = bb;
                    HiddenImageControl = bf;
                }
                else
                {
                    VisibleImageControl.Source = value;
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

        public System.Windows.Media.Brush Mask
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