using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Composition;
using System.Xml.Serialization;

namespace LAP.Utils
{
    public class SerializableBrush
    {
        public SerializableBrush(Color Color)
        {
            this.Color = Color;
        }

        public SerializableBrush(SolidColorBrush Brush)
        {
            Color = Brush.Color;
        }

        public SerializableBrush() { }
        
        [XmlIgnore]
        public Color Color { get; set; }

        [XmlIgnore]
        public SolidColorBrush Brush
        {
            get { return new SolidColorBrush(Color); }
            set { Color = value.Color; }
        }

        public string StringBrush
        {
            get { return Converter.BrushToString(Brush); }
            set { Brush = Converter.StringToBrush(value); }
        }
    }

    public class Brushes
    {
        public NotificationBrushes Notification { get; set; } = new NotificationBrushes();

        public LabelBrushes Label { get; set; } = new LabelBrushes();

        public class NotificationBrushes
        {
            public SerializableBrush Error { get; set; } = new SerializableBrush(Color.FromArgb(255, 163, 61, 61));

            public SerializableBrush Message { get; set; } = new SerializableBrush(Color.FromArgb(255, 0, 0, 0));
        }


        public class LabelBrushes
        {
            public SerializableBrush TitleLabel { get; set; } = new SerializableBrush(Color.FromArgb(255, 255, 255, 255));
        }
    }
}
