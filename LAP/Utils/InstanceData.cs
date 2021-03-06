﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    internal class InstanceData
    {
        public static bool UseDefaultSetting { get; set; } = false;

        public static bool SafeMode { get; set; } = false;

        public static bool AutoSave { get; set; } = true;

        public static bool ErrorRaise { get; set; } = false;

        public static bool LogMode { get; set; } = false;

        public static bool LogExp { get; set; } = false;

        public static string LogExpPath { get; set; } = null;

        public static bool DoNotInitialize { get; set; } = false;

        public static bool UpdateMode { get; set; } = false;

        public static bool OverrideLanguage { get; set; } = false;

        public static string LocalizeFilePath { get; set; }

        public static System.Diagnostics.ProcessStartInfo UpdateProcessInfo { get; set; } = null;
    }
}