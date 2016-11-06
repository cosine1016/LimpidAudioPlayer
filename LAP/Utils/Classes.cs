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
                for (int i = 0; PluginManager.InitializedPlugin.Count > i; i++)
                {
                    try
                    {
                        PluginManager.Plugin plg = PluginManager.InitializedPlugin[i];
                        if (plg.Enabled)
                        {
                            plg.Instance.SetFilePath(fileName);
                            for (int s = 0; plg.Instance.WaveStreams.Count > s; s++)
                            {
                                try
                                {
                                    if (plg.Instance.WaveStreams[s].SupportedExtensions.Contains(
                                        System.IO.Path.GetExtension(fileName).ToLower()) ||
                                        plg.Instance.WaveStreams[s].SupportedExtensions.Contains(".*"))
                                    {
                                        readerStream = plg.Instance.WaveStreams[s];
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
        }
    }
}