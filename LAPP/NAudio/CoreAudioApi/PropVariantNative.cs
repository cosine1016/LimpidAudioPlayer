using System;
using System.Runtime.InteropServices;

namespace LAPP.NAudio.CoreAudioApi.Interfaces
{
    internal class PropVariantNative
    {
        [DllImport("ole32.dll")]
        internal static extern int PropVariantClear(ref PropVariant pvar);

        [DllImport("ole32.dll")]
        internal static extern int PropVariantClear(IntPtr pvar);
    }
}