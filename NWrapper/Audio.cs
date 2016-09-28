using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;

namespace NWrapper
{
    public class Audio : IDisposable
    {
        public class ASIOException : Exception { }
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public enum Status { Unknown, Playing, Paused, Stopped }

        private IWavePlayer iwp = null;

        public IWavePlayer WavePlayer
        {
            get { return iwp; }
            set
            {
                if (iwp != value)
                {
                    if (iwp != null)
                        iwp.PlaybackStopped -= Iwp_PlaybackStopped;
                    if (value != null)
                        value.PlaybackStopped += Iwp_PlaybackStopped;

                    iwp = value;
                }
            }
        }

        private void Iwp_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackStopped?.Invoke(sender, e);
        }

        public AudioFileReaderEx WaveStream { get; set; } = null;

        public SampleAggregator SampleAggregator { get; set; } = null;

        public Equalizer Equalizer { get; set; } = null;

        public PSEMicMixProvider PSEMicMixer { get; set; } = null;

        public FadeInOutSampleProvider Fade { get; set; } = null;

        public Equalizer.EqualizerBand[] EqualizerBand { get; set; } = new Equalizer.EqualizerBand[]
        {
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 100, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 150, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 200, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 300, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 400, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 600, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 800, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 1000, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 1200, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 1800, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 2400, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 3600, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 4800, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 7200, Gain = 0},
            new Equalizer.EqualizerBand {Bandwidth = 1.5f, Frequency = 9600, Gain = 0},
        };

        public System.ComponentModel.ISynchronizeInvoke SynchronizingObject { get; set; } = null;

        public void SetEqualizerBand(Equalizer.EqualizerBand[] EqualizerBand)
        {
            this.EqualizerBand = EqualizerBand;
            Equalizer.Update(EqualizerBand);
        }

        public Audio()
        {
        }

        public void OpenFile(string FileName, MMDevice CaptureDevice)
        {
            if (WavePlayer != null)
            {
                try
                {
                    if(WaveStream == null) WaveStream = new AudioFileReaderEx(FileName);

                    if (WavePlayer as AsioOut != null)
                    {
                        if (WaveStream.GetReaderStream as MediaFoundationReader != null)
                            throw new ASIOException();
                    }
                    

                    Equalizer = new Equalizer(WaveStream, EqualizerBand);
                    SetEqualizerBand(EqualizerBand);

                    Fade = new FadeInOutSampleProvider(Equalizer, false);

                    PSEMicMixer = new PSEMicMixProvider(Fade, CaptureDevice);
                    PSEMicMixer.MicMixEnd += PSEMicMixer_MicMixEnd;

                    SampleAggregator = new SampleAggregator(PSEMicMixer, fftLenght);

                    WavePlayer.Init(SampleAggregator);
                }
                catch (Exception e)
                {
                    CloseFile();
                    throw e;
                }
            }
            else
                throw new Exception("WavePlayerを先に初期化してください");
        }

        private void PSEMicMixer_MicMixEnd(object sender, EventArgs e)
        {
            Fade.BeginFadeIn(300);
        }

        public int fftLenght = 4096;

        public void CloseFile()
        {
            if (WaveStream != null)
            {
                WaveStream.Dispose();
                WaveStream = null;
            }
        }

        public static WaveOutCapabilities[] GetDevices()
        {
            List<WaveOutCapabilities> dev = new List<WaveOutCapabilities>();
            for (int id = 0; id < WaveOut.DeviceCount; id++)
            {
                dev.Add(WaveOut.GetCapabilities(id));
            }
            return dev.ToArray();
        }

        public Status StreamStatus
        {
            get
            {
                if (WavePlayer != null)
                {
                    switch (WavePlayer.PlaybackState)
                    {
                        case PlaybackState.Paused:
                            return Status.Paused;

                        case PlaybackState.Playing:
                            return Status.Playing;

                        case PlaybackState.Stopped:
                            return Status.Stopped;
                    }
                }

                return Status.Unknown;
            }
            set
            {
                if (WavePlayer != null)
                {
                    switch (value)
                    {
                        case Status.Playing:
                            WavePlayer.Play();
                            break;

                        case Status.Paused:
                            WavePlayer.Pause();
                            break;

                        case Status.Stopped:
                            WavePlayer.Stop();
                            break;
                    }
                }
            }
        }

        private void Play()
        {
            if (WavePlayer != null && WaveStream != null)
                WavePlayer.Pause();
        }

        private void Pause()
        {
            if (WavePlayer != null)
                WavePlayer.Pause();
        }

        private void Stop()
        {
            if (WavePlayer != null)
                WavePlayer.Stop();
            if (WaveStream != null)
                WaveStream.Position = 0;
        }

        public void Dispose()
        {
            if (PSEMicMixer != null)
            {
                PSEMicMixer.WasapiCapture.StopRecording();
                PSEMicMixer.MicMixEnd -= PSEMicMixer_MicMixEnd;
            }

            Stop();
            CloseFile();
            if (WavePlayer != null)
            {
                WavePlayer.Dispose();
                WavePlayer = null;
            }

            if (WaveStream != null) WaveStream.Dispose();
            if (PSEMicMixer != null) PSEMicMixer.Dispose();

            WaveStream = null;
            SampleAggregator = null;
            Equalizer = null;
            PSEMicMixer = null;
            Fade = null;
        }
    }
}