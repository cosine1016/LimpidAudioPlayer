using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Enums
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    internal sealed class ConfigAttribute : Attribute
    {
        public ConfigAttribute(object DefaultValue)
        {
            Default = DefaultValue;
        }

        public object Default { get; set; }
    }
}
