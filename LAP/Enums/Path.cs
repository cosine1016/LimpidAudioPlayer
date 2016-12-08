using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Enums
{
    public enum Path
    {
        [Config(@"$LAP$Languages\English.loc")]
        LanguageFile,

        [Config(@"$LAP$Languages\")]
        LanguageDirectory,

        [Config(@"$LAP$Setting.lcnf")]
        SettingFile,

        [Config(@"$LAP$Plugin\Management.xml")]
        PluginManagementFile,

        [Config(@"$PRG$Plugin\")]
        PluginDirectory
    }
}
