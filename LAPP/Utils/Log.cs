using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.Utils
{
    /// <summary>
    /// ログへの書き込みを行います
    /// </summary>
    public static class Log
    {
        public static void Append(string Msg)
        {
            Events.AppendMsgToLog(Msg);
        }
    }
}
