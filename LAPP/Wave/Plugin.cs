using NAudio.Wave;
using System.IO;
using System.Linq;

namespace LAPP.Wave
{
    public abstract class WaveStreamPlugin : WaveStream
    {
        public abstract string[] SupportedExtensions { get; }

        protected virtual bool IsFileSupported(string FilePath)
        {
            return SupportedExtensions.Contains(Path.GetExtension(FilePath).ToLower());
        }

        public override abstract string ToString();
    }

    public abstract class SampleProviderPlugin : NWrapper.IManagableProvider
    {
        public abstract WaveFormat WaveFormat { get; }
        public abstract void Dispose();
        public abstract void Initialize(ISampleProvider BaseProvider);
        public abstract int Read(float[] buffer, int offset, int count);
    }
}
