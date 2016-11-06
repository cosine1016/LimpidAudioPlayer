using System;
using NAudio.Dsp;
using NAudio.Wave;

namespace NWrapper
{
    public class Equalizer : IManagableProvider
    {
        private ISampleProvider sourceProvider;
        private EqualizerBand[] bands;
        private BiQuadFilter[,] filters;
        private int channels;
        private int bandCount;
        private bool updated;

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

        public void Initialize(ISampleProvider BaseProvider)
        {
            sourceProvider = BaseProvider; ;
            CreateFilters();
        }

        public void SetBands(EqualizerBand[] Bands)
        {
            bands = Bands;
            bandCount = Bands.Length;
            channels = sourceProvider.WaveFormat.Channels;
            filters = new BiQuadFilter[channels, bands.Length];
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public class EqualizerBand : ICloneable
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