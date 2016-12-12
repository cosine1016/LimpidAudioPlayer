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
using LAPP.Setting;
using System.Windows.Controls;
using System.Windows;

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

        internal class GeneralCategory : ISettingItem
        {
            public GeneralCategory()
            {
                Action = new Action(() =>
                {
                    Header = Localize.Get("GENERAL");
                });
                Localize.AddLanguageChangedAction(Action);

                UIControl = gen;
            }

            private Action Action;

            public Border Border { get; set; }

            public string Header { get; set; }

            public UIElement UIControl { get; set; }

            private UserControls.General gen = new UserControls.General();

            public ApplyInfo Apply()
            {
                return gen.Apply();
            }

            public void Dispose()
            {
                Localize.RemoveLanguageChangedAction(Action);
            }
        }

        internal class OutputCategory : ISettingItem
        {
            public OutputCategory()
            {
                Action = new Action(() =>
                {
                    Header = Localize.Get("CONFIG_OUTPUT");
                });
                Localize.AddLanguageChangedAction(Action);

                UIControl = aos;
            }

            private Action Action;

            public Border Border { get; set; }

            public string Header { get; set; }

            public UIElement UIControl { get; set; }

            private UserControls.AudioOutSelector aos = new UserControls.AudioOutSelector();

            public ApplyInfo Apply()
            {
                try
                {
                    Config.Current.Output.OutputDevice = aos.SelectedDevice;
                    Config.Current.Output.ASIO = aos.ASIOConfig;
                    Config.Current.Output.WASAPI = aos.WASAPIConfig;
                    Config.Current.Output.DirectSound = aos.DSConfig;
                    Config.Current.Output.Amplify = (float)aos.AmplifyN.Value / 100;

                    return new ApplyInfo(true, false, false, aos.RerenderFile);
                }
                catch (Exception) { return new ApplyInfo(false); }
            }

            public void Dispose()
            {
                Localize.RemoveLanguageChangedAction(Action);
            }
        }

        internal class Plugin : ISettingItem
        {
            public Plugin()
            {
                Action = new Action(() =>
                {
                    Header = Localize.Get(Strings.Plugin);
                });
                Localize.AddLanguageChangedAction(Action);

                UIControl = new UserControls.PluginOption();
            }

            private Action Action;

            public Border Border { get; set; }

            public string Header { get; set; }

            public UIElement UIControl { get; set; }

            public ApplyInfo Apply()
            {
                bool restart = ((UserControls.PluginOption)UIControl).Apply();
                return new ApplyInfo(true, restart, false, false);
            }

            public void Dispose()
            {
                Localize.RemoveLanguageChangedAction(Action);
            }
        }
    }
}