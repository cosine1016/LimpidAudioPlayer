﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAPP.IO;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace BasicPlugin.WaveOutputs
{
    public class ASIO : LAPP.Wave.IWaveOutPlugin
    {
        public string Title { get; } = "ASIO";

        public string DriverName { get; set; } = "";

        public IWavePlayer CreateWavePlayer(MediaFile File)
        {
            if (string.IsNullOrEmpty(DriverName))
                return new AsioOut();
            else
                return new AsioOut(DriverName);
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class WASAPI : LAPP.Wave.IWaveOutPlugin
    {
        public string Title { get; } = "WASAPI";

        public bool EventSync { get; set; } = false;

        public bool Shared { get; set; } = false;

        public int Latency { get; set; } = 300;

        public string DeviceID { get; set; } = "";

        public IWavePlayer CreateWavePlayer(MediaFile File)
        {
            MMDevice dev = null;
            MMDeviceEnumerator deven = new MMDeviceEnumerator();
            if (string.IsNullOrEmpty(DeviceID))
                dev = deven.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            else
                dev = deven.GetDevice(DeviceID);

            deven.Dispose();

            if (Shared)
                return new WasapiOut(dev, AudioClientShareMode.Shared, EventSync, Latency);
            else
                return new WasapiOut(dev, AudioClientShareMode.Exclusive, EventSync, Latency);
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class Wave : LAPP.Wave.IWaveOutPlugin
    {
        public string Title { get; } = "Wave";

        public IWavePlayer CreateWavePlayer(MediaFile File)
        {
            return new WaveOut();
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class DirectSound : LAPP.Wave.IWaveOutPlugin
    {
        public string Title { get; } = "DirectSound";

        public Guid Device { get; set; } = Guid.Empty;

        public int Latency { get; set; } = 300;

        public IWavePlayer CreateWavePlayer(MediaFile File)
        {
            return new DirectSoundOut(Device, Latency);
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
