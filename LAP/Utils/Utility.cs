using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Interop;

namespace LAP.Utils
{
    public class Utility
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Exception();
            }

            return wpfBitmap;
        }

        internal static long GetUnixTime(DateTime targetTime)
        {
            DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            targetTime = targetTime.ToUniversalTime();
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;
            return (long)elapsedTime.TotalSeconds;
        }

        internal static void SafeClose(int ExitCode = 0)
        {
            Window w = Application.Current.MainWindow;
            MainWindow mw = w as MainWindow;
            if (mw != null)
            {
                mw.StopFile(true);
            }

            System.Environment.Exit(ExitCode);
        }



        internal static WriteableBitmap GetImageSourceFromFile(string Path)
        {
            MemoryStream data = new MemoryStream(File.ReadAllBytes(Path));
            WriteableBitmap wbmp = new WriteableBitmap(BitmapFrame.Create(data));
            data.Close();

            return wbmp;
        }

        internal static class ArtworkManager
        {
            private static List<WriteableBitmap> Bitmaps = new List<WriteableBitmap>();
            private static List<string> artpaths = new List<string>();

            public static WriteableBitmap GetArtwork(string ArtworkCachePath)
            {
                if (string.IsNullOrEmpty(ArtworkCachePath)) return null;
                bool Available = false;
                int Index = 0;

                for (int c = 0; artpaths.Count > c; c++)
                {
                    if (ArtworkCachePath == artpaths[c])
                    {
                        Available = true;
                        Index = c;
                        break;
                    }
                }

                if (Available)
                {
                    return Bitmaps[Index];
                }
                else
                {
                    WriteableBitmap wb = GetImageSourceFromFile(ArtworkCachePath);
                    artpaths.Add(ArtworkCachePath);
                    Bitmaps.Add(wb);
                    return wb;
                }
            }
        }

        internal static class EventInvoker
        {
            static public void Invoke<TEventHandler>(TEventHandler handler, object[] args)
            {
                System.Delegate d = handler as System.Delegate;
                System.ComponentModel.ISynchronizeInvoke invoker = d.Target as System.ComponentModel.ISynchronizeInvoke;
                invoker.Invoke(d, args);
            }

            static public System.IAsyncResult BeginInvoke<TEventHandler>(TEventHandler handler, object[] args)
            {
                System.Delegate d = handler as System.Delegate;
                System.ComponentModel.ISynchronizeInvoke invoker = d.Target as System.ComponentModel.ISynchronizeInvoke;
                return invoker.BeginInvoke(d, args);
            }
        }

        internal static LAPP.Page GetPageFromString(string str)
        {
            switch (str)
            {
                default:
                    return null;
            }
        }

        internal static IWavePlayer CreateSoundDevice()
        {
            Config.WaveOut.Devices device = Config.Current.Output.OutputDevice;
            if (InstanceData.OverrideOutput)
            {
                device = InstanceData.OverrideDevice;
                LAP.Dialogs.LogWindow.Append("Output Device Overridden : " + device.ToString());
            }


            LAP.Dialogs.LogWindow.Append("Output : " + device.ToString());

            switch (device)
            {
                case Config.WaveOut.Devices.ASIO:
                    if (string.IsNullOrEmpty(Config.Current.Output.ASIO.DriverName))
                        return new AsioOut();
                    else
                        return new AsioOut(Config.Current.Output.ASIO.DriverName);

                case Config.WaveOut.Devices.DirectSound:
                    return new DirectSoundOut(Config.Current.Output.DirectSound.Latency);

                case Config.WaveOut.Devices.Wave:
                    return new NAudio.Wave.WaveOut();

                case Config.WaveOut.Devices.WASAPI:
                    Config.Current.Output.WASAPI.ShareMode = AudioClientShareMode.Exclusive;
                    if (string.IsNullOrEmpty(Config.Current.Output.WASAPI.DeviceFriendlyName))
                        return new WasapiOut(Config.Current.Output.WASAPI.ShareMode, false, Config.Current.Output.WASAPI.Latency);
                    else
                    {
                        MMDeviceCollection col = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                        MMDevice dev = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                        for (int i = 0; col.Count > i; i++)
                            if (col[i].FriendlyName == Config.Current.Output.WASAPI.DeviceFriendlyName)
                            {
                                dev = col[i];
                                break;
                            }

                        return new WasapiOut(dev, Config.Current.Output.WASAPI.ShareMode, false, Config.Current.Output.WASAPI.Latency);
                    }

                default:
                    return new DirectSoundOut();
            }
        }

        private static MMDevice dev = null;

        internal static MMDevice GetCaptureDevice()
        {
            if (dev == null)
            {
                MMDeviceEnumerator Devices = new MMDeviceEnumerator();
                if (string.IsNullOrEmpty(Config.Current.sValue[Enums.sValue.MicDeviceName]))
                {
                    dev = Devices.GetDefaultAudioEndpoint(DataFlow.Capture,
                        Role.Communications);
                    Config.Current.sValue[Enums.sValue.MicDeviceName] = dev.ToString();
                }
                else
                {
                    MMDeviceCollection devColl = Devices.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                    for (int i = 0; devColl.Count > i; i++)
                    {
                        if (devColl[i].ToString() == Config.Current.sValue[Enums.sValue.MicDeviceName])
                        {
                            dev = devColl[i];
                            break;
                        }
                    }
                    if (dev == null) Devices.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                }
            }

            return dev;
        }
    }
}