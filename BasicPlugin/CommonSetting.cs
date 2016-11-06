using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin
{
    public class Common
    {
        internal static Common Setting = new Common();
        public string Remove { get; set; } = "Remove";
        public string ShowInExplorer { get; set; } = "Show in explorer";
        public string Play { get; set; } = "Play";
        public string Pause { get; set; } = "Pause";
    }
}
