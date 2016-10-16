using ClearUC.ListViewItems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using NAudio.Wave;

namespace LAP.Utils
{
    public class Classes
    {
        public class Tag
        {
            public string Artist { get; set; } = "";
            public string Album { get; set; } = "";
            public string Title { get; set; } = "";
            public string Lyrics { get; set; } = "";
            public string ArtworkCachePath { get; set; } = "";
            public string Track { get; set; } = "";
            public string LastWriteTime { get; set; } = "";
            public string FilePath { get; set; } = "";

            public ImageSource GetArtwork()
            {
                return Converter.ToImageSource((Bitmap)Image.FromFile(ArtworkCachePath));
            }
        }

        public class PlaylistEventArgs : EventArgs
        {
            public PlaylistEventArgs(string Path, Page.Playlist.Playlist.PlaylistData Data)
            {
                this.Path = Path;
                this.Data = Data;
            }

            public string Path { get; set; }

            public Page.Playlist.Playlist.PlaylistData Data { get; set; }
        }

        public class File : ICloneable
        {
            public File(string Path, Tag Tag)
            {
                this.Path = Path;
                this.Tag = Tag;
            }

            public string Path { get; set; }

            public ImageSource Artwork { get; set; }

            public Tag Tag { get; set; }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        public class AudioFileReader : NWrapper.AudioFileReaderEx
        {
            public AudioFileReader(string FileName) : base(FileName) { }

            protected override void CreateReaderStream(string fileName)
            {
                for(int i = 0; PluginManager.InitializedPlugin.Count > i; i++)
                {
                    try
                    {
                        PluginManager.Plugin plg = PluginManager.InitializedPlugin[i];
                        plg.Instance.SetFilePath(fileName);
                        if (plg.Instance?.WaveStreams.Count > 0)
                        {
                            for (int s = 0; plg.Instance.WaveStreams.Count > s; s++)
                            {
                                try
                                {
                                    if (plg.Instance.WaveStreams[s].SupportedExtensions.Contains(System.IO.Path.GetExtension(fileName).ToLower()) ||
                                        plg.Instance.WaveStreams[s].SupportedExtensions.Contains(".*"))
                                    {
                                        readerStream = new PluginWaveStream(plg.Instance.WaveStreams[s]);
                                        LAP.Dialogs.LogWindow.Append("Created Reader From " + plg.Instance.Title);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LAP.Dialogs.LogWindow.Append(plg.Instance.Title + " Error : " + ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception) { }
                }

                base.CreateReaderStream(fileName);
            }

            public class PluginWaveStream : WaveStream
            {
                LAPP.Wave.WaveStreamPlugin bs;

                public PluginWaveStream(LAPP.Wave.WaveStreamPlugin BaseStream)
                {
                    bs = BaseStream;
                }

                public override long Length
                {
                    get { return bs.Length; }
                }

                public override long Position
                {
                    get { return bs.Position; }
                    set { bs.Position = value; }
                }

                public override WaveFormat WaveFormat
                {
                    get { return ToNAudioFormat(bs.WaveFormat); }
                }

                public override int Read(byte[] buffer, int offset, int count)
                {
                    return bs.Read(buffer, offset, count);
                }

                private WaveFormat ToNAudioFormat(LAPP.NAudio.Wave.WaveFormat Format)
                {
                    return new WaveFormat(Format.SampleRate, Format.BitsPerSample, Format.Channels);
                }

                public override string ToString()
                {
                    return bs.ToString();
                }
            }
        }
    }
}