using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP.Management;

namespace LAP.Enums
{
    public enum Animation
    {
        [Config(100)]
        Default,

        [Config(3000)]
        Notification,

        [Config(300)]
        BackgroundImage,

        [Config(200)]
        MediaPanel
    }
}
