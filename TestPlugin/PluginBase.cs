﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    public class PluginBase : LAPP.LimpidAudioPlayerPlugin
    {
        public PluginBase()
        {
            Pages.Add(new DirectoryPage());
            WaveStreams.Add(new WaveStreamPlugin());
            LAPP.Player.Receiver.EventReceived += Player_EventReceived;
        }

        private void Player_EventReceived(object sender, LAPP.Player.Receiver.EventReceiveArgs e)
        {
            Console.WriteLine(e.Action.ToString());
        }

        public override string Author
        {
            get
            {
                return "Kaisei Sunaga";
            }
        }

        public override string Description
        {
            get
            {
                return "Test Plugin For LAP";
            }
        }

        public override string Title
        {
            get
            {
                return "Test Plugin";
            }
        }

        public override string URL
        {
            get
            {
                return null;
            }
        }

        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        public override void SetFile(LAPP.IO.MediaFile File)
        {
        }

        protected override void LanguageChanged(int LCID)
        {
        }
    }
}
