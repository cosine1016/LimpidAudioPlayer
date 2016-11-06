using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;

namespace LAPP.Utils
{
    public static class BorderHelper
    {
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
    }
}
