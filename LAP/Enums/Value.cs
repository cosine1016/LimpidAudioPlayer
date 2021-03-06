﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP.Management;

namespace LAP.Enums
{
    public enum iValue
    {
        [Config(7000)]
        TimeOut
    }

    public enum sValue
    {
        [Config("")]
        MicDeviceName,

        [Config("")]
        WaveOutput
    }

    public enum bValue
    {
        [Config(true)]
        AutoUpdate,

        [Config(true)]
        UseNewPlugin
    }

    public enum sArrayValue
    {
        [Config(new string[] { "Plugin" })]
        Pages,

        [Config(new string[] { "General", "Output", "Plugin" })]
        ConfigCategories
    }
}
