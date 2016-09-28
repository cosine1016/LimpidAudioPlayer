using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace LAPP.ListItems
{
    public abstract class ListItem
    {
        public class ImageSourceList
        {
            public List<ImageSource> Items = new List<ImageSource>();
        }

        public event EventHandler PropertyChanged;

        protected virtual void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler IndexChanged;

        protected virtual void OnIndexChanged(EventArgs e)
        {
            IndexChanged?.Invoke(this, e);
        }

        public event EventHandler ImageIndexChanged;

        protected virtual void OnImageIndexChanged(EventArgs e)
        {
            ImageIndexChanged?.Invoke(this, e);
        }

        public ListItem(bool IncludeSearchTarget, bool ExcludeResult)
        {
            this.IncludeSearchTarget = IncludeSearchTarget;
            this.ExcludeResult = ExcludeResult;
            Init(null);
        }

        public ListItem()
        {
            Init(null);
        }

        public ListItem(ImageSourceList ImageSources)
        {
            Init(ImageSources);
        }

        private void Init(ImageSourceList ImageSources)
        {
            this.ImageSources = ImageSources;
        }

        int ii = -1;

        public ImageSourceList ImageSources { get; set; } = null;

        public int ImageIndex
        {
            get { return ii; }
            set
            {
                ii = value;
                OnImageIndexChanged(new EventArgs());
            }
        }

        public string SearchText { get; set; } = "";

        public bool IncludeSearchTarget { get; set; } = false;

        public bool ExcludeResult { get; set; } = true;

        public object Data { get; set; }

        public Type DataType { get; set; }
    }
}
