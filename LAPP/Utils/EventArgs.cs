using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.Utils
{
    public class TypeEventArgs<T> : EventArgs
    {
        public TypeEventArgs(T Value)
        {
            this.Value = Value;
        }

        public T Value { get; set; }
    }

    public class ReturnableEventArgs<TValue, TRet> : TypeEventArgs<TValue>
    {
        public ReturnableEventArgs(TValue Value) : base(Value) { }

        public TRet Return { get; set; }
    }
}
