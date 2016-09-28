using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    public class InstanceData
    {
        public static bool UseDefaultSetting { get; set; } = false;

        public static bool SafeMode { get; set; } = true;

        public static bool AutoSave { get; set; } = true;

        public static bool ErrorRaise { get; set; } = false;

        public static bool LogMode { get; set; } = false;

        public static bool LogExp { get; set; } = false;

        public static string LogExpPath { get; set; } = null;

        public static bool DoNotInitialize { get; set; } = false;

        public static class SrtLib
        {
            static SrtLib()
            {
                Asm = Assembly.LoadFrom("SrtLib.dll");

                Auth = Activator.CreateInstance(Asm.GetType("SrtLib.Auth"));
                Auth.AuthorizeLAP();

                FTPS = Activator.CreateInstance(Asm.GetType("SrtLib.FTPS"), new object[] { Auth });
            }

            public static Assembly Asm { get; set; }
            public static dynamic Auth { get; set; }
            public static dynamic FTPS { get; set; }
        }
    }
}