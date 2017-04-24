using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP.IO;
using NetworkKit.UDP;
using NetworkKit.Talking;

namespace MobileSupportPlugin
{
    public class Plugin : LAPP.LimpidAudioPlayerPlugin
    {
        public Plugin()
        {
            Client client = new Client();
            client.Received += Client_Received;
            client.StartListening("239.255.10.10", 11072).Wait();
        }

        private void Client_Received(object sender, NetworkKit.DataReceivedEventArgs e)
        {
            string text = Encoding.UTF8.GetString(e.Data);
            Console.WriteLine(text);
        }

        public override string Author { get; } = "Kaisei Sunaga";

        public override string Description { get; } = "Talking with mobile limpid audio player plugin";

        public override string Title { get; } = "Mobile Support Plugin";

        public override string URL { get; } = "http://ksprogram.mods.jp/WordPress/";

        public override Version Version { get; } = new Version(1, 0, 0, 0);

        public override void SetFile(MediaFile File)
        {
        }

        protected override void LanguageChanged(int LCID)
        {
        }
    }
}
