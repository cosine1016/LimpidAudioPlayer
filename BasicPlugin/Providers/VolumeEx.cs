using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace BasicPlugin.Providers
{
    public class VolumeEx : NWrapper.IManagableProvider
    {
        ISampleProvider _prov = null;

        public WaveFormat WaveFormat
        {
            get { return _prov.WaveFormat; }
        }

        public void Dispose()
        {
            
        }

        public void Initialize(ISampleProvider BaseProvider)
        {
            _prov = BaseProvider;
        }

        public float Volume { get; set; } = 1f;

        public float Maximum { get; set; } = 3f;

        private float GetVolume()
        {
            if (Volume > Maximum)
                Volume = Maximum;
            else if (Volume < 0f)
                Volume = 0f;

            if (Maximum < 0f)
                Maximum = 0f;
            
            return Volume;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = _prov.Read(buffer, offset, count);

            if (Volume == 1f)
                return read;
            else
            {
                float vol = GetVolume();

                int min = Math.Min(count, buffer.Length);
                for (int i = 0; min > i; i++)
                    buffer[i] *= vol;

                return read;
            }
        }
    }
}
