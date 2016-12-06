using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace LAP.Enums
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
            get { return BrushToString(Brush); }
            set { Brush = StringToBrush(value); }
        }

        private static string BrushToString(SolidColorBrush Brush)
        {
            return Brush.ToString();
        }

        private static SolidColorBrush StringToBrush(string Brush)
        {
            return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Brush));
        }
    }
}
