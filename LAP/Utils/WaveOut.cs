using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    public class WaveOut
    {
        public enum Devices
        {
            ASIO, WASAPI, Wave, DirectSound
        }

        public Devices OutputDevice { get; set; } = Devices.WASAPI;

        public DirectSoundConfig DirectSound { get; set; } = new DirectSoundConfig();

        public ASIOConfig ASIO { get; set; } = new ASIOConfig();

        public WASAPIConfig WASAPI { get; set; } = new WASAPIConfig(NAudio.CoreAudioApi.AudioClientShareMode.Shared);

        [Serializable()]
        public class DirectSoundConfig : ICloneable
        {
            public int Latency { get; set; } = 300;

            public object Clone()
            {
                return Utility.DeepCopy(this);
            }
        }

        [Serializable()]
        public class ASIOConfig : ICloneable
        {
            public string DriverName { get; set; }

            public object Clone()
            {
                return Utility.DeepCopy(this);
            }
        }

        [Serializable()]
        public class WASAPIConfig : ICloneable
        {
            public WASAPIConfig()
            {
            }

            public WASAPIConfig(NAudio.CoreAudioApi.AudioClientShareMode ShareMode)
            {
                this.ShareMode = ShareMode;
            }

            public NAudio.CoreAudioApi.AudioClientShareMode ShareMode { get; set; } = NAudio.CoreAudioApi.AudioClientShareMode.Shared;

            public int Latency { get; set; } = 50;

            public string DeviceFriendlyName { get; set; }

            public int DeviceIndex { get; set; } = 0;

            public object Clone()
            {
                return Utility.DeepCopy(this);
            }
        }
    }
}