using System;

namespace LAPP.Wave.Helper
{
    public interface IFilter<T>
    {
        void Execute(T[] Data);
    }
}