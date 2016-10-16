using LAPP.NAudio.Wave;
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
}
