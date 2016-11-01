using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
namespace NWrapper
{
    public class Amplifier : ISampleProviderEx
    {
        private ISampleProvider source;
        public Amplifier(ISampleProvider Source, float Amplify)
        {
            source = Source;
            this.Amplify = Amplify;
        }

        public Amplifier(ISampleProvider Source) : this(Source, 1) { }

        public bool Enabled { get; set; } = false;

        public float Amplify { get; set; } = 0;

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (Enabled)
            {
                int read = 0;
                int rcou = count - offset;
                read += source.Read(buffer, offset, count);

                for (int i = 0; rcou > i; i++)
                {
                    buffer[i] *= (Amplify + 1.0f);
                }

                return read;
            }
            else
                return source.Read(buffer, offset, count);
        }
    }
}
