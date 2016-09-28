using System;
using System.Windows;
using System.Windows.Controls;

namespace LAP.Dialogs
{
    /// <summary>
    /// Equalizer.xaml の相互作用ロジック
    /// </summary>
    public partial class Equalizer : Window
    {
        private System.Windows.Forms.Timer Timer = new System.Windows.Forms.Timer();

        public Equalizer()
        {
            InitializeComponent();
            InitUI();

            Caption.Title = Utils.Config.Language.Strings.Equalizer;

            InitEQBox();

            if (Utils.Config.Setting.Values.MinGain > Utils.Config.Setting.Values.MaxGain)
            {
                Utils.Config.Setting.Values.MaxGain = 15;
                Utils.Config.Setting.Values.MinGain = -15;
            }

            Equalize.Maximum = Utils.Config.Setting.Values.MaxGain;
            Equalize.Minimum = Utils.Config.Setting.Values.MinGain;
        }

        public void InitUI()
        {
            Timer.Interval = 1000;
            Timer.Tick += Timer_Tick;

            Caption.Title = Utils.Config.Language.Strings.Equalizer;
            PresetLabel.Content = Utils.Config.Language.Strings.Window.EQ.Preset;
            SaveEQButton.Content = Utils.Config.Language.Strings.Window.EQ.SavePreset;
            DeleteEQButton.Content = Utils.Config.Language.Strings.Window.EQ.DeletePreset;
            EQName.Content = Utils.Config.Language.Strings.Window.EQ.SavePreset;
            SaveEQButton.Content = Utils.Config.Language.Strings.Window.EQ.SavePreset;
        }

        private void InitEQBox()
        {
            EQBox.Items.Clear();

            ComboBoxItem DefItem = new ComboBoxItem();
            DefItem.Content = "<" + Utils.Config.Language.Strings.Default + ">";

            ComboBoxItem CustomItem = new ComboBoxItem();
            CustomItem.Content = "<" + Utils.Config.Language.Strings.Window.EQ.CustomItem + ">";

            EQBox.Items.Add(DefItem);
            EQBox.Items.Add(CustomItem);

            int index = 0;
            string[] fns = Utils.Equalizer.GetEqualizerFileNames(out index);
            for (int i = 0; fns.Length > i; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = fns[i];
                EQBox.Items.Add(cbi);
            }

            EQBox.SelectedIndex = index;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Timer.Stop();

            if (Utils.Config.Setting.Boolean.AnimateItems)
            {
                Utils.Animation.Visible va = new Utils.Animation.Visible();

                EventHandler handler = null;
                handler = (s, ev) =>
                {
                    va.AnimationCompleted -= handler;
                    FreqGrid.Opacity = 0.8;
                };
                va.AnimationCompleted += handler;
                va.Animate(300, FreqGrid, Visibility.Hidden);
            }
            else
            {
                SaveGrid.Visibility = Visibility.Hidden;
            }
        }

        private void Equalize_GainChanged(object sender, MVPUC.Equalizer.Equalize.GainChangedEventArgs e)
        {
            if (EQBox.SelectedIndex != 1)
            {
                EQBox.SelectionChanged -= EQBox_SelectionChanged;
                EQBox.SelectedIndex = 1;
                EQBox.SelectionChanged += EQBox_SelectionChanged;
            }

            Timer.Stop();
            if (e.ChangedIndex > -1)
            {
                Timer.Start();

                FreqGrid.Visibility = Visibility.Visible;

                FreqLabel.Content = Utils.Equalizer.Bands[e.ChangedIndex].Frequency + "Hz : Gain " + Utils.Equalizer.Bands[e.ChangedIndex].Gain + "db";

                Utils.Equalizer.TempBands = new NWrapper.Equalizer.EqualizerBand[Equalize.Sliders.Count];
                for (int i = 0; Equalize.Sliders.Count > i; i++)
                {
                    Utils.Equalizer.TempBands[i] = new NWrapper.Equalizer.EqualizerBand()
                    {
                        Frequency = Utils.Equalizer.Bands[i].Frequency,
                        Bandwidth = Utils.Equalizer.Bands[i].Bandwidth,
                        Gain = Utils.Equalizer.Bands[i].Gain
                    };
                }

                Utils.Equalizer.Bands[e.ChangedIndex].Gain = e.Gain;
                Utils.Equalizer.OnEqualizerChanged();
            }
        }

        private void EQBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetPreset(EQBox.SelectedIndex);
        }

        public void SetPreset(int Index)
        {
            Equalize.Sliders.Clear();

            switch (EQBox.SelectedIndex)
            {
                case 0:
                    Utils.Equalizer.Bands = new NWrapper.Equalizer.EqualizerBand[Utils.Equalizer.DefaultBands.Length];
                    for (int i = 0; Utils.Equalizer.Bands.Length > i; i++)
                    {
                        Equalize.Sliders.Add(Utils.Equalizer.DefaultBands[i].Gain);
                        Utils.Equalizer.Bands[i] = (NWrapper.Equalizer.EqualizerBand)Utils.Equalizer.DefaultBands[i].Clone();
                    }
                    break;

                case 1:
                    Utils.Equalizer.Bands = new NWrapper.Equalizer.EqualizerBand[Utils.Equalizer.TempBands.Length];
                    for (int i = 0; Utils.Equalizer.TempBands.Length > i; i++)
                    {
                        Equalize.Sliders.Add(Utils.Equalizer.TempBands[i].Gain);
                        Utils.Equalizer.Bands[i] = (NWrapper.Equalizer.EqualizerBand)Utils.Equalizer.TempBands[i].Clone();
                    }
                    break;

                default:
                    if (EQBox.SelectedIndex > -1)
                    {
                        string file = ((ComboBoxItem)EQBox.SelectedItem).Content.ToString();
                        Utils.Equalizer.ReadBandFile(file);
                        for (int i = 0; Utils.Equalizer.TempBands.Length > i; i++)
                        {
                            Equalize.Sliders.Add(Utils.Equalizer.Bands[i].Gain);
                        }
                        Utils.Config.Setting.Paths.EQPath = Utils.Config.Setting.Paths.Equalizer + file
                            + Utils.Config.Setting.Paths.EqualizerExtension;
                    }
                    break;
            }

            Utils.Equalizer.OnEqualizerChanged();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Utils.Equalizer.WriteBandFile(EQTextBox.Text, Utils.Equalizer.Bands);

            if (Utils.Config.Setting.Boolean.AnimateItems)
            {
                Utils.Animation.Visible va = new Utils.Animation.Visible();
                va.Animate(300, SaveGrid, Visibility.Hidden);
            }
            else
            {
                SaveGrid.Visibility = Visibility.Hidden;
            }

            InitEQBox();
        }

        private void SaveEQButton_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.Config.Setting.Boolean.AnimateItems)
            {
                Utils.Animation.Visible va = new Utils.Animation.Visible();
                va.Animate(300, SaveGrid, Visibility.Visible);
            }
            else
            {
                SaveGrid.Visibility = Visibility.Visible;
            }
        }

        private void DeleteEQButton_Click(object sender, RoutedEventArgs e)
        {
            if(EQBox.SelectedIndex > 1)
            {
                string file = ((ComboBoxItem)EQBox.SelectedItem).Content.ToString();
                System.IO.File.Delete(Utils.Config.Setting.Paths.Equalizer + file
                    + Utils.Config.Setting.Paths.EqualizerExtension);

                InitEQBox();
            }
        }
    }
}