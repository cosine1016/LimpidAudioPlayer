using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using NAudio.Dsp;

namespace NWrapper
{
    public class PSEMicMixProvider : ISampleProviderEx, IDisposable
    {
        public event EventHandler MicMixEnd;

        private ISampleProvider baseProv;
        private AudioMeterInformation Meter = null;

        public PSEMicMixProvider(ISampleProvider Source, MMDevice Device)
        {
            baseProv = Source;
            CaptureDevice = Device;
            Meter = Device.AudioMeterInformation;

            WasapiCapture = new WasapiCapture(Device);

            PSE.VolumeRequest += PSE_VolumeRequest;
            PSE.ExtractedDegreeOfRisk += PSE_ExtractedDegreeOfRisk;
            PSE.Enable = true;

            PSEFadeTimer.Interval = 3000;
            PSEFadeTimer.Tick += PSEFadeTimer_Tick;

            BufferedWaveProvider = new BufferedWaveProvider(WaveFormat);

            WasapiCapture.DataAvailable += WasapiCapture_DataAvailable;
            WasapiCapture.StartRecording();

            List<ISampleProvider> sources = new List<ISampleProvider>();
            sources.Add(baseProv);
            sources.Add(WaveExtensionMethods.ToSampleProvider(BufferedWaveProvider));

            Mixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(sources);
        }

        private bool rec = false;
        
        private void PSE_ExtractedDegreeOfRisk(object sender, PerilousSoundEventArgs e)
        {
            if (Enabled == false) return;

            int lev = (int)e.DangerLevel;
            if (lev > (int)MicDisableRisk)
            {
                Perform = true;
                PSEFadeTimer.Start();
            }
            else
            {
                Perform = false;
                BufferedWaveProvider.ClearBuffer();
            }
        }

        private void PSEFadeTimer_Tick(object sender, EventArgs e)
        {
            if (Enabled == false) return;
            PSEFadeTimer.Stop();
            MicMixEnd?.Invoke(this, new EventArgs());
            BufferedWaveProvider.ClearBuffer();
        }

        private void PSE_VolumeRequest(object sender, EventArgs e)
        {
            if (Enabled == false) return;
            PSE.Volume = Meter.MasterPeakValue;
        }

        private void WasapiCapture_DataAvailable(object sender, WaveInEventArgs e)
        {
            if(Perform)
                BufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        private bool ena = false;

        public bool Enabled
        {
            get { return ena; }
            set
            {
                if (value == false) BufferedWaveProvider.ClearBuffer();
                ena = value;
                PSE.Enable = value;
            }
        }

        private bool Perform = false;

        public DegreeOfRisk MicDisableRisk { get; set; } = DegreeOfRisk.Middle;

        public PerilousSoundExtractor PSE { get; set; } = new PerilousSoundExtractor();

        public MMDevice CaptureDevice { get; private set; }

        public WaveFormat WaveFormat { get { return baseProv.WaveFormat; } }

        public WasapiCapture WasapiCapture;

        private BufferedWaveProvider BufferedWaveProvider;

        private System.Windows.Forms.Timer PSEFadeTimer { get; set; } = new System.Windows.Forms.Timer();

        private NAudio.Wave.SampleProviders.MixingSampleProvider Mixer;

        public int Read(float[] buffer, int offset, int count)
        {
            return Mixer.Read(buffer, offset, count);
        }

        public void Dispose()
        {
            PSE.Enable = false;
            PSE.VolumeRequest -= PSE_VolumeRequest;
            PSE.ExtractedDegreeOfRisk -= PSE_ExtractedDegreeOfRisk;
            PSEFadeTimer.Stop();
            PSEFadeTimer.Dispose();
            PSE.Dispose();
            if (Mixer != null) Mixer.RemoveAllMixerInputs();
            if (WasapiCapture != null) WasapiCapture.Dispose();
            if (BufferedWaveProvider != null) BufferedWaveProvider.ClearBuffer();

            Mixer = null;
            WasapiCapture = null;
            BufferedWaveProvider = null;
        }

        ~PSEMicMixProvider()
        {
            Dispose();
        }
    }
}