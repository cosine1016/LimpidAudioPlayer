using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
namespace NWrapper
{
    public class VolumeProvider : IManagableProvider
    {
        private ISampleProvider source = null;

        public float Volume { get; set; } = 1.0f;

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = 0;
            int rcou = count - offset;
            read += source.Read(buffer, offset, count);

            if(Volume == 1.0f)
                return read;

            for (int i = 0; rcou > i; i++)
            {
                buffer[i] *= Volume;
            }

            return read;
        }

        public void Initialize(ISampleProvider BaseProvider)
        {
            source = BaseProvider;
        }

        public void Dispose() { }
    }
}
