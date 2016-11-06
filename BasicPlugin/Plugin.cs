using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin
{
    public class Plugin : LAPP.LimpidAudioPlayerPlugin
    {
        public Plugin()
        {
            InitializePages();
            InitializeFunctions();
        }

        private void InitializePages()
        {
            Pages.Add(new Pages.Album.Page());
        }

        private void InitializeFunctions()
        {

        }

        public override string Author { get; } = "Kaisei Sunaga";

        public override string Description { get; } = "Basic function and page plugin";

        public override string Title { get; } = "Basic Plugin";

        public override string URL { get; } = "http://ksprogram.mods.jp/WordPress/";

        public override Version Version { get; } = new Version(1, 0, 0, 0);

        public override void SetFilePath(string FilePath)
        {
        }
    }
}
