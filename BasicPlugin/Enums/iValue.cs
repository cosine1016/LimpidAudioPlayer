using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP.Management;

namespace BasicPlugin.Enums
{
    public enum iValue
    {
        [Config(100)]
        DefaultAnimation,

        [Config(300)]
        AlbumVisibleAnimation
    }
}
