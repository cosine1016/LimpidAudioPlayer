using System;
using System.Threading;
using System.Threading.Tasks;

namespace NWrapper
{
    public enum DegreeOfRisk { Safe = 0, Low = 1, Middle = 2, High = 3 }

    public class PerilousSoundEventArgs : EventArgs
    {
        public DegreeOfRisk DangerLevel { get; internal set; } = DegreeOfRisk.Safe;
    }

    public class PerilousSoundExtractor : IDisposable
    {
        private Thread bgWorker;
        private Thread bgLooper;
        public static DegreeOfRisk Detect(float Volume, float High, float Middle, float Low)
        {
            int risk = 0;

            risk += Volume > Low ? 1 : 0;
            risk += Volume > Middle ? 1 : 0;
            risk += Volume > High ? 1 : 0;

            return (DegreeOfRisk)risk;
        }

        public PerilousSoundExtractor()
        {
            timer.Interval = 10;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (detected) ExtractedDegreeOfRisk?.Invoke(this, new PerilousSoundEventArgs() { DangerLevel = risk });
            detected = false;
        }

        public event EventHandler<PerilousSoundEventArgs> ExtractedDegreeOfRisk;

        public event EventHandler VolumeRequest;

        public float Volume { get; set; } = 0;

        public float MaximumVolume { get; set; } = 1;

        public float MinimumVolume { get; set; } = 0;

        public float Threshold { get; set; } = 0.2f;

        public float High { get; private set; } = 0.9f;

        public float Middle { get; private set; } = 0.7f;

        public float Low { get; private set; } = 0.5f;

        public float Lowest { get; set; } = 0.7f;

        private bool enable = false;
        public bool Enable
        {
            get { return enable; }
            set
            {
                bool bk = enable;
                enable = value;
                if (bk == false && value == true)
                {
                    bgLooper = new Thread(LoopThr);
                    bgLooper.Start();

                    bgWorker = new Thread(WorkerThr);
                    bgWorker.Start();

                    timer.Start();
                }
                else if (value == false)
                {
                    timer.Stop();
                }
            }
        }

        public int SamplingDuration { get; set; } = 1000;

        public int SampleTimes { get; set; } = 10;

        public int NormalizeInterval { get; set; } = 1000;

        private bool Exit { get; set; } = false;

        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private bool detected = false;
        private DegreeOfRisk risk = DegreeOfRisk.Low;

        private void LoopThr()
        {
            while (Enable)
            {
                if (Exit) break;
                int risk = 0;

                VolumeRequest?.Invoke(this, new EventArgs());
                risk += Volume > Low ? 1 : 0;
                risk += Volume >= Middle ? 1 : 0;
                risk += Volume >= High ? 1 : 0;

                detected = true;
                this.risk = (DegreeOfRisk)risk;
            }
        }

        private void WorkerThr()
        {
            while (Enable)
            {
                if (Exit) break;
                int per = SamplingDuration / SampleTimes;
                float Total = 0;
                for (int i = 0; SampleTimes >= i; i++)
                {
                    VolumeRequest?.Invoke(this, new EventArgs());
                    Total += Volume;
                    if (Exit) break;
                    Thread.Sleep(per);
                }

                float Av = Total / SampleTimes;
                if (Av >= Lowest) Av = Lowest;
                Low = Av;
                Middle = 1 >= Low + Threshold ? Low + Threshold : 1;
                High = 1 >= Middle + Threshold ? Middle + Threshold : 1;

                if (Exit) break;
                Thread.Sleep(NormalizeInterval);
            }
        }

        public void Dispose()
        {
            Exit = true;
        }
    }
}