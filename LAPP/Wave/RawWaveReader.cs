using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP.NAudio.Wave;
using System.IO;

namespace LAPP.Wave
{
    public class WaveStreamFaker : Stream
    {
        const int HeaderSize = 44;
        private Stream stream;

        public WaveStreamFaker(Stream Stream)
        {
            stream = Stream;
        }

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = true;

        public override bool CanWrite { get; } = false;

        public override long Length
        {
            get { return stream.Length + HeaderSize; }
        }

        long pos;
        public override long Position
        {
            get { return pos; }
            set
            {
                pos = value;
                SetPos(value);
            }
        }

        private void SetPos(long Position)
        {
            if (Position <= 44)
                stream.Position = 0;
            else
                stream.Position = Position - 44;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int Read = 0;

            if(Position <= HeaderSize)
            {
                byte[] header = GetWaveFileHeader(stream.Length);

                int min = Math.Min(HeaderSize, count - offset);
                for (int i = 0; min > i; i++)
                {
                    buffer[offset] = header[i];
                    offset++;
                    pos++;
                    Read++;
                }

                if (count - offset <= 0) return Read;
            }

            long Min = Math.Min(count - offset, Length - Position);
            int streamRead = stream.Read(buffer, offset, count - offset);
            offset += streamRead;
            pos += streamRead;
            Read += streamRead;

            return Read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        private byte[] GetSize(long Size)
        {
            byte[] bytes = BitConverter.GetBytes(Size);
            return bytes;
        }

        private byte[] GetWaveFileHeader(long Length)
        {
            const int HeaderSize = 44;
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
    }
}
