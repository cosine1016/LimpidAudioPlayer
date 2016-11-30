using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.Wave.Helper
{
    public interface IByteConverter<T>
    {
        byte[] ToBytes(T[] data);
        T[] ToData(byte[] bytes);
    }
}
