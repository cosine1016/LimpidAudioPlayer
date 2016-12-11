using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPlugin
{
    public class Plugin : LAPP.LimpidAudioPlayerPlugin
    {
        private MediaPanels.Spectrum spectrum = new MediaPanels.Spectrum();

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

        private void InitializeMediaPanel(LAPP.IO.MediaFile File)
        {
            MediaPanelItems.Clear();

            if (File.Artwork != null)
            {
                MediaPanels.Artwork art = new MediaPanels.Artwork() { Source = File.Artwork, Label = File.Album };
                MediaPanelItems.Add(art);
            }

            if (!string.IsNullOrEmpty(File.Lyrics))
            {
                MediaPanelItems.Add(new MediaPanels.Lyrics() { Text = File.Lyrics });
            }

            MediaPanelItems.Add(spectrum);
        }

        private void InitializeProviders(LAPP.IO.MediaFile File)
        {
            Providers.Clear();

            Providers.Add(new Providers.VolumeEx());

            Providers.SampleAggregator sa = new Providers.SampleAggregator();
            sa.SetFFT(new Providers.SampleAggregator.FFTInfo() { Enable = false, Length = 256 });
            spectrum.SampleAggreator = sa;
            Providers.Add(sa);
        }

        public override string Author { get; } = "Kaisei Sunaga";

        public override string Description { get; } = "Basic function and page plugin";

        public override string Title { get; } = "Basic Plugin";

        public override string URL { get; } = "http://ksprogram.mods.jp/WordPress/";

        public override Version Version { get; } = new Version(1, 0, 0, 0);

        public override void SetFile(LAPP.IO.MediaFile File)
        {
            InitializeMediaPanel(File);
            InitializeProviders(File);
        }
    }
}
