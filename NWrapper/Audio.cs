using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace NWrapper
{
    /// <summary>
    /// このプロバイダのDisposeメソッドでは初期化に用いたプロバイダを破棄する必要はありません
    /// </summary>
    public interface IManagableProvider : ISampleProvider, IDisposable
    {
        void Initialize(ISampleProvider BaseProvider);
    }

    /// <summary>
    /// サンプルプロバイダを管理するコレクションです。アイテムは自動で破棄されます
    /// メモリリークを防ぐため、可能な限り参照情報は残さないでください
    /// </summary>
    public class ManagableProviderCollection : IList<IManagableProvider>, IDisposable
    {
        List<IManagableProvider> providers = new List<IManagableProvider>();

        public IManagableProvider this[int index]
        {
            get { return providers[index]; }
            set
            {
                DisposeItem(providers[index]);
                providers[index] = value;
            }
        }

        public AudioFileReader BaseReader { get; set; } = null;

        public int Count
        {
            get { return providers.Count; }
        }

        public bool IsReadOnly { get; } = false;

        public void Add(IManagableProvider item)
        {
            providers.Add(item);
        }

        public void AddRange(IManagableProvider[] items)
        {
            for(int i = 0;Count > i; i++)
            {
                Add(items[i]);
            }
        }

        public void Clear()
        {
            for(int i = 0; Count > i; i++)
            {
                DisposeItem(providers[i]);
            }

            providers.Clear();
        }

        public bool Contains(IManagableProvider item)
        {
            return providers.Contains(item);
        }

        public void CopyTo(IManagableProvider[] array, int arrayIndex)
        {
            providers.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IManagableProvider> GetEnumerator()
        {
            return providers.GetEnumerator();
        }

        public int IndexOf(IManagableProvider item)
        {
            return providers.IndexOf(item);
        }

        public void Insert(int index, IManagableProvider item)
        {
            providers.Insert(index, item);
        }

        public bool Remove(IManagableProvider item)
        {
            bool s = providers.Remove(item);
            if (s) DisposeItem(item);
            return s;
        }

        public void RemoveAt(int index)
        {
            DisposeItem(providers[index]);
            providers.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return providers.GetEnumerator();
        }

        public ISampleProvider Initialize()
        {
            if (Count == 0) return BaseReader as ISampleProvider;

            providers[0].Initialize(BaseReader as ISampleProvider);

            for(int i = 1;Count > i; i++)
                providers[i].Initialize(providers[i - 1]);

            return providers[Count - 1];
        }

        public void Dispose()
        {
            Clear();
            if(BaseReader != null)
            {
                BaseReader.Dispose();
                BaseReader = null;
            }
        }

        private void DisposeItem(IManagableProvider Provider)
        {
            if(Provider != null) Provider.Dispose();
        }
    }

    public class Audio : IDisposable
    {
        public Audio() { }
        public Audio(IManagableProvider[] Providers) { this.Providers.AddRange(Providers); }

        public class ASIOException : Exception { }

        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public enum Status { Unknown, Playing, Paused, Stopped }

        private IWavePlayer iwp = null;

        public IWavePlayer WavePlayer
        {
            get { return iwp; }
            private set
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

        public ManagableProviderCollection Providers { get; set; } = new ManagableProviderCollection();

        public AudioFileReaderEx AudioFileReader { get; private set; } = null;

        public void OpenFile(string FileName, AudioFileReaderEx AudioFileReader, IWavePlayer WavePlayer)
        {
            try
            {
                this.AudioFileReader = AudioFileReader;
                this.WavePlayer = WavePlayer;

                Providers.BaseReader = AudioFileReader;
                WavePlayer.Init(Providers.Initialize());
            }
            catch (Exception e)
            {
                Dispose();
                throw e;
            }
        }

        private void CloseFile()
        {
            AudioFileReader?.Dispose();
            AudioFileReader = null;

            WavePlayer?.Dispose();
            WavePlayer = null;

            Providers?.Dispose();
            Providers = null;
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
            if (WavePlayer != null && AudioFileReader != null)
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
            if (AudioFileReader != null)
                AudioFileReader.Position = 0;
        }

        public void Dispose()
        {
            CloseFile();
        }
    }
}