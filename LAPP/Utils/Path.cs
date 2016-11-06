using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.Utils
{
    public static class Path
    {
        public const string LAPSetting = "{LAP}";
        private static string LAPSetting_R = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\LAP\";

        /// <summary>
        /// 指定されたパスの文字列を置き換えたパスを返却します
        /// </summary>
        public static string GetPath(string Path)
        {
            string ret = Path;

            ret = ret.Replace(LAPSetting, LAPSetting_R);

            return ret;
        }

        /// <summary>
        /// 指定されたパスの文字列を置き換えたパスを返却します
        /// </summary>
        public static void GetPath(ref string Path)
        {
            Path = GetPath(Path);
        }
    }
}
