using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LAPP.Wave.Helper
{
    public class Buffer<T>
    {
        public event EventHandler<BufferEventArgs<T>> FillBuffer;
        
        private long _length, _position;
        private int _notify;

        public Buffer(long Length, int NotifyLeftOver, int Capacity, T[] Buffer)
        {
            _length = Length;
            _notify = NotifyLeftOver;

            this.Capacity = Capacity;

            if (Buffer != null)
                for (int i = 0; Buffer.Length > i; i++)
                    Queue.Enqueue(Buffer[i]);
        }

        public Buffer(long Length, int NotifyLeftOver, int Capacity) : this(Length, NotifyLeftOver, Capacity, null) { }

        public bool CanRead { get; } = true;
        public bool CanSeek { get; } = true;
        public bool CanWrite { get; } = true;
        public int Capacity { get; }

        public long Length
        {
            get { return _length; }
        }
        public long Position
        {
            get { return _position; }

            set
            {
                if (value > Length) throw new Exception("PositionはLength以下にしてください");

                int bSize = Queue.Count;
                long size = Position - value;
                if (bSize < size || size < 0)
                    Queue.Clear();
                else
                {
                    for (int i = 0; size > i; i++)
                        Queue.Dequeue();
                }

                _position = value;

                Notify();
            }
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }
        public void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public int Read(T[] buffer, int offset, int count)
        {
            Notify();
            int read = 0;

            if (Length > Position)
            {
                int size = Math.Min(count, Queue.Count);
                if (Position + size > Length) size = (int)(Length - Position);
                for (int i = 0; size > i; i++)
                    buffer[offset + i] = Queue.Dequeue();

                read += size;

                Notify();

                _position += read;
            }

            return read;
        }

        public long Seek(long offset, SeekOrigin origin)
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
                    Position += offset;
                    break;
            }

            return Position;
        }

        public FilterCollection<T> Filters { get; set; } = new FilterCollection<T>();

        protected Queue<T> Queue { get; set; } = new Queue<T>();

        public int NotifyLeftOver
        {
            get { return _notify; }
            set { _notify = value; }
        }

        protected virtual void Notify()
        {
            int size = Queue.Count;
            if (Position + size >= Length) return;

            if(size <= _notify)
            {
                BufferEventArgs<T> e = new BufferEventArgs<T>(0, Capacity - size, Filters);
                FillBuffer?.Invoke(this, e);

                for (int i = 0; e.ActualWrite > i; i++)
                    Queue.Enqueue(e.Buffer[i]);
            }
        }

        protected void OnFillBuffer(BufferEventArgs<T> e)
        {
            FillBuffer?.Invoke(this, e);
        }
    }

    public class FixedBuffer<T> : Buffer<T>
    {
        private int CountPerWrite;
        public FixedBuffer(long Length, int NotifyLeftOver, int Capacity, int CountPerWrite) : base(Length, NotifyLeftOver, Capacity)
        {
            this.CountPerWrite = CountPerWrite;
        }

        protected override void Notify()
        {
            int size = Queue.Count;
            if (Position + size >= Length) return;

            if (size <= NotifyLeftOver)
            {
                BufferEventArgs<T> e = new BufferEventArgs<T>(0, CountPerWrite, Filters);
                OnFillBuffer(e);

                for (int i = 0; e.ActualWrite > i; i++)
                    Queue.Enqueue(e.Buffer[i]);
            }
        }
    }

    public class BufferEventArgs<T> : EventArgs
    {
        public BufferEventArgs(int Offset, int Length, FilterCollection<T> Filters)
        {
            this.Offset = Offset;
            this.Length = Length;
            this.Filters = Filters;
            Buffer = new T[Length];
        }

        public int Length { get; }
        public int Offset { get; }
        public FilterCollection<T> Filters { get; set; }
        internal T[] Buffer { get; }
        internal int ActualWrite { get; private set; }

        public void Write(T[] Data, int Offset, int Length)
        {
            if (Data.Length > Length)
                throw new Exception("Out of capacity");

            Array.Copy(Data, Offset, Buffer, this.Offset, Length);
            ActualWrite = Length;

            if (Filters == null) return;
            for(int i = 0;Filters.Count > i; i++)
            {
                Filters[i].Execute(Buffer);
            }
        }
    }

    public class FilterCollection<T> : Collection<IFilter<T>>
    {
        public void AddRange(IFilter<T>[] items)
        {
            for (int i = 0; items.Length > i; i++)
                Add(items[i]);
        }
    }

    public class BufferStream<T> : Stream
    {
        private IByteConverter<T> _converter;
        private Buffer<T> _base;

        public event EventHandler<BufferEventArgs<T>> FillBuffer;

        public BufferStream(Buffer<T> Buffer, IByteConverter<T> Converter)
        {
            Buffer.FillBuffer += Buffer_FillBuffer;
            _base = Buffer;
            _converter = Converter;
        }

        private void Buffer_FillBuffer(object sender, BufferEventArgs<T> e)
        {
            FillBuffer(sender, e);
        }

        public override bool CanRead
        {
            get { return _base.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _base.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _base.CanWrite; }
        }

        public override long Length
        {
            get { return _base.Length; }
        }

        public override long Position
        {
            get { return _base.Position; }
            set { _base.Position = value; }
        }

        public override void Flush()
        {
            _base.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            T[] bbuf = new T[count];
            int read = _base.Read(bbuf, offset, count);

            buffer = _converter.ToBytes(bbuf);

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _base.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _base.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new Exception("WriteメソッドではなくFillBufferイベントを使用してください");
        }
    }
}