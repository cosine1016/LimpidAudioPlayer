using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LAPP.ListItems
{
    public class ListSubItem : ListItem
    {
        public ListSubItem() : base(true, true)
        {
        }

        public ListSubItem(bool IncludeSearchTarget, bool ExcludeResult) : base(IncludeSearchTarget, ExcludeResult)
        {
        }

        private bool tl = false;
        private string ml, sl, st, nl;
        private Visibility slv = Visibility.Visible, stv = Visibility.Visible;
        private LeftItems litem = LeftItems.Nothing;
        private Shape lefts = null;
        private Stretch str = Stretch.Uniform;
        private StretchDirection strd = StretchDirection.Both;

        public bool TitleLabelVisible
        {
            get { return tl; }
            set
            {
                tl = value;
                OnPropertyChanged();
            }
        }

        public string MainLabelText
        {
            get { return ml; }
            set
            {
                ml = value;
                OnPropertyChanged();
            }
        }

        public string SubLabelText
        {
            get { return sl; }
            set
            {
                sl = value;
                OnPropertyChanged();
            }
        }

        public string StatusLabelText
        {
            get { return st; }
            set
            {
                st = value;
                OnPropertyChanged();
            }
        }

        public string NumberLabelText
        {
            get { return nl; }
            set
            {
                nl = value;
                OnPropertyChanged();
            }
        }

        public Visibility SubLabelVisibility
        {
            get { return slv; }
            set
            {
                slv = value;
                OnPropertyChanged();
            }
        }

        public Visibility StatusLabelVisibility
        {
            get { return stv; }
            set
            {
                stv = value;
                OnPropertyChanged();
            }
        }

        public enum LeftItems { Image, Number, Shape, Nothing }

        public LeftItems LeftItem
        {
            get { return litem; }
            set
            {
                litem = value;
                OnPropertyChanged();
            }
        }

        public Shape ShapeItem
        {
            get { return lefts; }
            set
            {
                lefts = value;
                OnPropertyChanged();
            }
        }

        public Stretch Stretch
        {
            get { return str; }
            set
            {
                str = value;
                OnPropertyChanged();
            }
        }

        public StretchDirection StretchDirection
        {
            get { return strd; }
            set
            {
                strd = value;
                OnPropertyChanged();
            }
        }
    }
}
