using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Diagnostics;
using NWrapper;
using System.Threading.Tasks;

namespace BasicPlugin.Providers
{
    public class SampleAggregator : IManagableProvider
    {
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

        public class FFTInfo
        {
            public bool Enable { get; set; } = false;

            public int Length { get; set; } = 0;
        }
        // volume
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;

        private float maxValue;
        private float minValue;
        public int NotificationCount { get; set; }
        private int count;

        // FFT
        public event EventHandler<FftEventArgs> FftCalculated;

        private FFTInfo Info = new FFTInfo();
        private Complex[] fftBuffer;
        private FftEventArgs fftArgs;
        private int fftPos;
        private int m;
        private ISampleProvider source;

        private bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public void Reset()
        {
            count = 0;
            maxValue = minValue = 0;
        }

        private void Add(float value)
        {
            if (FftCalculated != null)
            {
                fftBuffer[fftPos].X = (float)(value * FastFourierTransform.HammingWindow(fftPos, Info.Length));
                fftBuffer[fftPos].Y = 0;
                fftPos++;
                if (fftPos >= fftBuffer.Length)
                {
                    fftPos = 0;
                    // 1024 = 2^10
                    FastFourierTransform.FFT(true, m, fftBuffer);
                    FftCalculated(this, fftArgs);
                    return;
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

        public WaveFormat WaveFormat { get { return source.WaveFormat; } }
        
        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);

            if (Info == null || Info.Enable == false || Info.Length < 2)
                return samplesRead;
            else
            {
                Task.Run(() =>
                {
                    for (int n = 0; n < samplesRead; n += source.WaveFormat.Channels)
                        Add(buffer[n + offset]);
                });

                redTime++;

                return samplesRead;
            }
        }

        int redTime = 0;
        int redCount = 1;

        public bool Enabled
        {
            get { return Info.Enable; }
            set { Info.Enable = value; }
        }

        public void Initialize(ISampleProvider BaseProvider)
        {
            source = BaseProvider;
        }

        public void SetFFT(FFTInfo Info)
        {
            this.Info = Info;

            m = (int)Math.Log(Info.Length, 2.0);
            fftBuffer = new Complex[Info.Length];
            fftArgs = new FftEventArgs(fftBuffer);
        }

        public void Dispose() { }
    }
}