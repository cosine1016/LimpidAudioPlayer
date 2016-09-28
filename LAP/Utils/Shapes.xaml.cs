using System.Windows;
using System.Xaml;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace LAP.Utils
{
    /// <summary>
    /// Shapes.xaml の相互作用ロジック
    /// </summary>
    public partial class Shapes : System.Windows.Controls.Page
    {
        public Shapes()
        {
            InitializeComponent();
        }

        public enum ShapeData
        {
            Plus, Back, Cross
        }

        public static Viewbox GetViewboxFromXAML(string XAML)
        {
            System.IO.StringReader sr = new System.IO.StringReader(XAML);
            XamlXmlReader xxr = new XamlXmlReader(sr);
            XamlObjectWriter xow = new XamlObjectWriter(xxr.SchemaContext);
            while (xxr.Read())
            {
                xow.WriteNode(xxr);
            }

            sr.Close();

            Viewbox vb = (Viewbox)xow.Result;

            vb.HorizontalAlignment = HorizontalAlignment.Stretch;
            vb.VerticalAlignment = VerticalAlignment.Stretch;

            return vb;
        }

        public static Viewbox GetViewboxFromXAML(string XAML, double Width, double Height, Thickness Margin)
        {
            Viewbox c = GetViewboxFromXAML(XAML);
            c.Width = Width;
            c.Height = Height;
            c.Margin = Margin;

            return c;
        }

        public static Border GetBorderFromXAML(string XAML, double Width, double Height, Thickness Margin)
        {
            Viewbox c = GetViewboxFromXAML(XAML);
            c.Width = Width;
            c.Height = Height;
            c.Margin = Margin;
            Border b = new Border();
            b.Child = c;
            return b;
        }

        public static Shape GetShape(ShapeData Shape, double Width, double Height, Thickness Margin)
        {
            Shapes ss = new Shapes();
            Shape s = null;
            switch (Shape)
            {
                case ShapeData.Back:
                    s = ss.Back;
                    break;
                case ShapeData.Plus:
                    s = ss.Plus;
                    break;
                case ShapeData.Cross:
                    s = ss.Cross;
                    break;
            }

            ss.container.Children.Remove(s);
            s.Width = Width;
            s.Height = Height;
            s.Margin = Margin;
            return s;
        }
    }
}
