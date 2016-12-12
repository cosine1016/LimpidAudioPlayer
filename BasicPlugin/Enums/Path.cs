using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin.Enums
{
    public enum Path
    {
        [LAPP.Management.Config("Config.xml")]
        ConfigFileName
    }
}
