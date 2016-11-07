using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAPP.Player
{
    public static class Receiver
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void RaiseReceivedEvent(EventReceiveArgs Args)
        {
            EventReceived?.Invoke(null, Args);
        }

        public static event EventHandler<EventReceiveArgs> EventReceived;

        public class EventReceiveArgs : EventArgs
        {
            public EventReceiveArgs(Action Action, params object[] Args)
            {
                this.Action = Action;
                this.Args = Args;
            }

            public Action Action { get; set; }
            public object[] Args { get; set; }
        }

        public enum Action
        {
            Boot, WindowClosing, Exited,
            Render, Rendered, RendererDisposed,
            PlaybackState,
            Seek, VolumeChanged,
            FastForward, Rewind,
            Shuffle, Repeat,
            MediaInformation, MediaLyrics, MediaArtwork, MediaHidden,
            ToolStrip,
            WindowState,
            TabIndexChanged
        }
    }
}
