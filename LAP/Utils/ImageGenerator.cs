using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Dsp;

namespace LAP.Utils
{
    internal class ImageGenerator
    {
        public event EventHandler<SpectrumProgressChangedEventArgs> Generated;

        public event EventHandler<SpectrumProgressChangedEventArgs> ProgressChanged;

        private Classes.AudioFileReader afr;

        private List<float> floats = new List<float>();

        private string path;

        public ImageGenerator(string FilePath)
        {
            path = FilePath;
            Init();
        }

        public int Height { get; set; }

        public Pen Pen { get; set; } = Pens.Aqua;

        public float ProgressInterval { get; set; } = 0.2f;

        public bool Supported { get; set; } = false;

        public int Width { get; set; }

        public void Generate()
        {
            Task.Run(() =>
            {
                afr = new Classes.AudioFileReader(path);
                if (afr.Length > 0)
                    Supported = true;

                int PerRead = (int)(afr.Length / Width);
                float max = 0, low = float.MaxValue;
                float ave = 0;
                int count = 0;
                for(long i = 0;afr.Length > i; i+= PerRead)
                {
                    float added = AddFloat(PerRead);
                    if (added > 0)
                    {
                        count++;
                        ave += added;
                        low = added < low ? added : low;

                        int Limit = 100000;
                        float diff = max - added;
                        if ((-Limit < diff && diff < Limit) || i == 0)
                            max = added > max ? added : max;
                    }
                }
                ave = ave / count;

                Bitmap sp = new Bitmap(Width, Height);
                
                Graphics g = Graphics.FromImage(sp);
                for (int i = 0; floats.Count > i; i++)
                {
                    float h = floats[i] <= low ? 0 : (float)(Math.Log(floats[i], max));
                    h *= Height;
                    g.DrawLine(Pen, i, 0, i, Height - h);
                }

                g.Dispose();
                sp.Save("Test.jpg");
                Generated?.Invoke(this, new SpectrumProgressChangedEventArgs(sp));
            });
        }

        private float AddFloat(int ReadLength)
        {
            byte[] buffer = new byte[ReadLength];
            int read = afr.Read(buffer, 0, ReadLength);
            if (floats.Count >= Width) return -1;
            if (read > 0)
            {
                float total = 0;
                for (int i = 0; read > i; i++)
                    total += buffer[i];
                floats.Add(total);
                return total;
            }
            else
                return -1;
        }

        private void Init()
        {
            int mw = 2000, mh = 200;
            foreach (Screen s in Screen.AllScreens)
            {
                mw = s.Bounds.Width > mw ? s.Bounds.Width : mw;
                mh = s.Bounds.Height > mh ? s.Bounds.Height : mh;
            }
            Width = mw;
            Height = mh;
        }

        public class SpectrumProgressChangedEventArgs : EventArgs
        {
            public SpectrumProgressChangedEventArgs(Image Image)
            {
                this.Image = Image;
            }

            public Image Image { get; set; }
        }
    }
}