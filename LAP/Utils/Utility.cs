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

        public static LAPP.MTag.TagEx ToLAPTag(LAPP.MTag.Tag BaseTag, string FilePath, string ArtworkPath)
        {
            LAPP.MTag.TagEx Ret = new LAPP.MTag.TagEx();
            Ret.ArtworkCachePath = ArtworkPath;
            Ret.Album = BaseTag.Album;
            Ret.Artist = BaseTag.Artist;
            Ret.Title = BaseTag.Title;
            Ret.Lyrics = BaseTag.Lyrics;
            Ret.Track = BaseTag.Track;
            Ret.FilePath = FilePath;
            Ret.LastWriteTime = System.IO.File.GetLastWriteTime(FilePath).ToString();
            return Ret;
        }

        public static LAPP.MTag.Tag ToMTag(LAPP.MTag.TagEx BaseTag)
        {
            LAPP.MTag.Tag Ret = new LAPP.MTag.Tag();
            if(!string.IsNullOrEmpty(BaseTag.ArtworkCachePath) && File.Exists(BaseTag.ArtworkCachePath))
                Ret.Artwork = Image.FromFile(BaseTag.ArtworkCachePath);
            Ret.Album = BaseTag.Album;
            Ret.Artist = BaseTag.Artist;
            Ret.Title = BaseTag.Title;
            Ret.Lyrics = BaseTag.Lyrics;
            Ret.Track = BaseTag.Track;
            return Ret;
        }

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

        internal static object DeepCopy(object target)
        {
            object result;
            BinaryFormatter b = new BinaryFormatter();

            MemoryStream mem = new MemoryStream();

            try
            {
                b.Serialize(mem, target);
                mem.Position = 0;
                result = b.Deserialize(mem);
            }
            finally
            {
                mem.Close();
            }

            return result;
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

        internal static Page.ListViewPage GetPageFromString(string str)
        {
            switch (str)
            {
                case "Album":
                    return new Page.Album.Page();

                case "Playlist":
                    return new Page.Playlist.Page();

                case "ScanTest":
                    return new Page.TestPage.ScanTest();

                default:
                    return null;
            }
        }

        internal static void ShowExplorerWithFile(string File)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE",
                @"/select,""" + File + "\"");
        }

        internal static void ShowExplorerWithDirectory(string Directory)
        {
            System.Diagnostics.Process.Start(Directory);
        }

        internal static IWavePlayer CreateSoundDevice()
        {
            switch (Config.Setting.WaveOut.OutputDevice)
            {
                case WaveOut.Devices.ASIO:
                    if (string.IsNullOrEmpty(Config.Setting.WaveOut.ASIO.DriverName))
                        return new AsioOut();
                    else
                        return new AsioOut(Config.Setting.WaveOut.ASIO.DriverName);

                case WaveOut.Devices.DirectSound:
                    return new DirectSoundOut(Config.Setting.WaveOut.DirectSound.Latency);

                case WaveOut.Devices.Wave:
                    return new NAudio.Wave.WaveOut();

                case WaveOut.Devices.WASAPI:
                    Config.Setting.WaveOut.WASAPI.ShareMode = AudioClientShareMode.Exclusive;
                    if (string.IsNullOrEmpty(Config.Setting.WaveOut.WASAPI.DeviceFriendlyName))
                        return new WasapiOut(Config.Setting.WaveOut.WASAPI.ShareMode, false, Config.Setting.WaveOut.WASAPI.Latency);
                    else
                    {
                        MMDeviceCollection col = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                        MMDevice dev = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                        for (int i = 0; col.Count > i; i++)
                            if (col[i].FriendlyName == Config.Setting.WaveOut.WASAPI.DeviceFriendlyName)
                            {
                                dev = col[i];
                                break;
                            }

                        return new WasapiOut(dev, Config.Setting.WaveOut.WASAPI.ShareMode, false, Config.Setting.WaveOut.WASAPI.Latency);
                    }

                default:
                    return new DirectSoundOut();
            }
        }

        private static MMDevice dev = null;

        internal static NAudio.CoreAudioApi.MMDevice GetCaptureDevice()
        {
            if (dev == null)
            {
                MMDeviceEnumerator Devices = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                if (string.IsNullOrEmpty(Config.Setting.Values.MicDevice))
                {
                    dev = Devices.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Capture,
                        Role.Communications);
                    Config.Setting.Values.MicDevice = dev.ToString();
                }
                else
                {
                    MMDeviceCollection devColl = Devices.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                    for (int i = 0; devColl.Count > i; i++)
                    {
                        if (devColl[i].ToString() == Config.Setting.Values.MicDevice)
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