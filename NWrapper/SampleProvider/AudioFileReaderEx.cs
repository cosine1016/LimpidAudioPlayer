using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace NWrapper
{
    public class AudioFileReaderEx : AudioFileReader
    {
        public string FilePath;

        public AudioFileReaderEx(string FileName) : base(FileName)
        {
            FilePath = FileName;
        }

        public WaveStream GetReaderStream { get { return readerStream; } }
    }
}