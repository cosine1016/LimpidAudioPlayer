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
        public class AudioFileReader : NWrapper.AudioFileReaderEx
        {
            public AudioFileReader(string FilePath) : base(FilePath) { }

            protected override void CreateReaderStream(string fileName)
            {
                LAPP.DisposableItemCollection<LAPP.Wave.WaveStreamPlugin> streams
                    = PluginManager.GetWaveStreams();
                for (int i = 0; streams.Count > i; i++)
                {
                    if (streams[i].SupportedExtensions.Contains(
                        System.IO.Path.GetExtension(fileName).ToLower()) ||
                        streams[i].SupportedExtensions.Contains(".*"))
                    {
                        readerStream = streams[i];
                        return;
                    }
                }
                base.CreateReaderStream(fileName);
            }
        }
    }
}