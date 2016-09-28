using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Diagnostics;

namespace NWrapper
{
    public class SampleAggregator : ISampleProviderEx
    {
        // volume
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;

        private float maxValue;
        private float minValue;
        public int NotificationCount { get; set; }
        private int count;

        // FFT
        public event EventHandler<FftEventArgs> FftCalculated;

        public bool PerformFFT { get; set; }
        private readonly Complex[] fftBuffer;
        private readonly FftEventArgs fftArgs;
        private int fftPos;
        private readonly int fftLength;
        private int m;
        private readonly ISampleProvider source;

        private readonly int channels;

        public SampleAggregator(ISampleProvider source, int fftLength = 4096)
        {
            channels = source.WaveFormat.Channels;
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            this.m = (int)Math.Log(fftLength, 2.0);
            this.fftLength = fftLength;
            this.fftBuffer = new Complex[fftLength];
            this.fftArgs = new FftEventArgs(fftBuffer);
            this.source = source;
            redt = redc;
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public void Reset()
        {
            count = 0;
            maxValue = minValue = 0;
        }

        private bool Add(float value)
        {
            try
            {
                if (FftCalculated != null)
                {
                    fftBuffer[fftPos].X = (float)(value * FastFourierTransform.HammingWindow(fftPos, fftLength));
                    fftBuffer[fftPos].Y = 0;
                    fftPos++;
                    if (fftPos >= fftBuffer.Length)
                    {
                        fftPos = 0;
                        // 1024 = 2^10
                        FastFourierTransform.FFT(true, m, fftBuffer);
                        FftCalculated(this, fftArgs);
                        return true;
                    }
                }

                maxValue = Math.Max(maxValue, value);
                minValue = Math.Min(minValue, value);
                count++;
                if (count >= NotificationCount && NotificationCount > 0)
                {
                    MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(minValue, maxValue));
                    Reset();
                }
            }
            catch (Exception) {  }
            return false;
        }

        public WaveFormat WaveFormat { get { return source.WaveFormat; } }

        public bool Enabled { get; set; } = false;

        public int ReduceCount
        {
            get { return redc; }
            set
            {
                redc = value;
                redt = value;
            }
        }

        private int redt = 0, redc = 0;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);

            if (PerformFFT == false || Enabled == false) return samplesRead;

            if (redt >= ReduceCount)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    for (int n = 0; n < samplesRead; n += channels)
                        if (Add(buffer[n + offset])) break;
                });
                redt = 0;
            }
            else
                redt++;

            return samplesRead;
        }
    }

    public class MaxSampleEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public MaxSampleEventArgs(float minValue, float maxValue)
        {
            this.MaxSample = maxValue;
            this.MinSample = minValue;
        }

        public float MaxSample { get; private set; }
        public float MinSample { get; private set; }
    }

    public class FftEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public FftEventArgs(Complex[] result)
        {
            this.Result = result;
        }

        public Complex[] Result { get; private set; }
    }
}