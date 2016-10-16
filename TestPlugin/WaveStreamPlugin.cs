using System;
using System.IO;
using LAPP.NAudio.Wave;

namespace TestPlugin
{
    public class WaveStreamPlugin : LAPP.Wave.WaveStreamPlugin
    {
        const int HeaderSize = 44;

        public override long Length { get; } = 500000;

        public override long Position { get; set; } = 0;

        public override string[] SupportedExtensions { get; } = new string[] { ".txt" };

        public override WaveFormat WaveFormat { get; } = new WaveFormat();

        private byte[] GetSize(long Size)
        {
            byte[] bytes = BitConverter.GetBytes(Size);
            return bytes;
        }

        private byte[] GetWaveFileHeader()
        {
            byte[] Header = new byte[HeaderSize];
            MemoryStream ms = new MemoryStream(Header);
            ms.Write(new byte[] { 0x52, 0x49, 0x46, 0x46 }, 0, 4);
            ms.Write(GetSize(Length - 8), 0, 4);
            ms.Write(new byte[] { 0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20, 0x10, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02,
                0x00, 0x44, 0xAC, 0x00, 0x00, 0x10, 0xB1, 0x02, 0x00, 0x04, 0x00, 0x10, 0x00, 0x64, 0x61, 0x74, 0x61 }, 0, 32);
            ms.Write(GetSize(Length - 44), 0, 4);

            ms.Close();

            return Header;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;

            if (Position >= Length) return 0;

            if (Position < HeaderSize)
            {
                int toCopy = Math.Min(count - offset, HeaderSize);
                byte[] Header = GetWaveFileHeader();

                for (int i = 0; toCopy > i; i++)
                {
                    buffer[offset + i] = Header[Position + i];
                }

                offset += toCopy;
                bytesRead += toCopy;
                Position += toCopy;

                if (count - offset <= 0)
                    return bytesRead;
            }

            while (count - offset > 0)
            {
                int toCopy = count - offset;

                for (int i = 0; toCopy > i; i++)
                    buffer[offset + i] = 200;

                bytesRead += toCopy;
                Position += toCopy;
                offset += toCopy;
            }

            return bytesRead;
        }

        public override string ToString()
        {
            return "TestReader";
        }
    }
}
