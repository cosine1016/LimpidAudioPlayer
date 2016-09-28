using NAudio.Dsp;
using NAudio.Wave;

namespace NWrapper
{
    public class Equalizer : ISampleProviderEx
    {
        private readonly ISampleProvider sourceProvider;
        private EqualizerBand[] bands;
        private readonly BiQuadFilter[,] filters;
        private readonly int channels;
        private readonly int bandCount;
        private bool updated;

        public Equalizer(ISampleProvider sourceProvider, EqualizerBand[] Bands)
        {
            this.sourceProvider = sourceProvider;
            bands = Bands;
            bandCount = Bands.Length;
            channels = sourceProvider.WaveFormat.Channels;
            filters = new BiQuadFilter[channels, bands.Length];
            CreateFilters();
        }

        private void CreateFilters()
        {
            for (int bandIndex = 0; bandIndex < bandCount; bandIndex++)
            {
                var band = bands[bandIndex];
                for (int n = 0; n < channels; n++)
                {
                    if (filters[n, bandIndex] == null)
                        filters[n, bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                    else
                        filters[n, bandIndex].SetPeakingEq(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                }
            }
        }

        public void Update(EqualizerBand[] Bands)
        {
            updated = true;
            bands = Bands;
            CreateFilters();
        }

        public WaveFormat WaveFormat { get { return sourceProvider.WaveFormat; } }

        public bool Enabled { get; set; } = true;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            if (Enabled == false) return samplesRead;

            if (updated)
            {
                CreateFilters();
                updated = false;
            }

            for (int n = 0; n < samplesRead; n++)
            {
                int ch = n % channels;

                for (int band = 0; band < bandCount; band++)
                {
                    buffer[offset + n] = filters[ch, band].Transform(buffer[offset + n]);
                }
            }
            return samplesRead;
        }

        public class EqualizerBand : System.ICloneable
        {
            public float Frequency { get; set; }
            public float Gain { get; set; }
            public float Bandwidth { get; set; }

            public object Clone()
            {
                return new EqualizerBand() { Frequency = Frequency, Gain = Gain, Bandwidth = Bandwidth };
            }
        }
    }
}