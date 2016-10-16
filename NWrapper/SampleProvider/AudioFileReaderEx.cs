using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace NWrapper
{
    public class AudioFileReaderEx : AudioFileReader
    {
        public string FilePath;
        string ReaderName = "Unknown";

        public AudioFileReaderEx(string FileName) : base(FileName)
        {
            FilePath = FileName;
        }

        protected override void CreateReaderStream(string fileName)
        {
            base.CreateReaderStream(fileName);
            if (readerStream as WaveFileReader != null) ReaderName = "WaveFileReader";
            if (readerStream as BlockAlignReductionStream != null) ReaderName = "BlockAlignReductionStream";
            if (readerStream as Mp3FileReader != null) ReaderName = "Mp3FileReader";
            if (readerStream as AiffFileReader != null) ReaderName = "AiffFileReader";
            if (readerStream as MediaFoundationReader != null) ReaderName = "MediaFoundationReader";
        }

        public override string ToString()
        {
            return ReaderName;
        }

        public WaveStream GetReaderStream { get { return readerStream; } }
    }
}