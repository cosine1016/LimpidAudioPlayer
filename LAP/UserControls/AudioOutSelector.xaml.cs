using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LAP.UserControls
{
    /// <summary>
    /// AudioOutSelector.xaml の相互作用ロジック
    /// </summary>
    public partial class AudioOutSelector : UserControl
    {
        internal event EventHandler AudioOutputChanged;

        internal AudioOutSelector()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == false)
            {
                SelectedDevice = Config.Current.Output.OutputDevice;
                ASIOConfig = (Config.WaveOut.ASIOConfig)Config.Current.Output.ASIO.Clone();
                WASAPIConfig = (Config.WaveOut.WASAPIConfig)Config.Current.Output.WASAPI.Clone();
                DSConfig = (Config.WaveOut.DirectSoundConfig)Config.Current.Output.DirectSound.Clone();
                AmplifyN.Value = (int)(Config.Current.Output.Amplify * 100);
                SwitchButton();
            }

            AmplifyL.Content = Localize.Get("AMPLIFY");
            LatencyL.Content = Localize.Get("LATENCY");
        }

        internal Config.WaveOut.ASIOConfig ASIOConfig { get; set; }
        internal Brush DisabledButtonBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        internal Config.WaveOut.DirectSoundConfig DSConfig { get; set; }
        internal Brush EnabledButtonBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));

        internal Config.WaveOut.Devices SelectedDevice { get; set; }
        internal Config.WaveOut.WASAPIConfig WASAPIConfig { get; set; }
        private bool DisableUpdate { get; set; } = false;

        public void SwitchButton()
        {
            DisableUpdate = true;

            comboBox.Items.Clear();

            comboBox.IsEnabled = false;
            Latency.IsEnabled = false;
            checkBox.IsEnabled = false;
            checkBox.IsChecked = false;

            DirectSound.Background = DisabledButtonBrush;
            WASAPI.Background = DisabledButtonBrush;
            ASIO.Background = DisabledButtonBrush;
            Wave.Background = DisabledButtonBrush;
            switch (SelectedDevice)
            {
                case Config.WaveOut.Devices.DirectSound:
                    Latency.IsEnabled = true;
                    Latency.Value = DSConfig.Latency;

                    DirectSound.Background = EnabledButtonBrush;
                    break;

                case Config.WaveOut.Devices.WASAPI:
                    Latency.IsEnabled = true;
                    checkBox.IsEnabled = true;
                    comboBox.IsEnabled = true;

                    NAudio.CoreAudioApi.MMDeviceEnumerator dev = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                    NAudio.CoreAudioApi.MMDeviceCollection col =
                        dev.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active);

                    comboBox.Items.Add("<" + Localize.Get("DEFAULT") + ">");

                    for (int i = 0; col.Count > i; i++)
                    {
                        comboBox.Items.Add(col[i].FriendlyName);
                    }

                    if (comboBox.Items.Count > WASAPIConfig.DeviceIndex)
                        comboBox.SelectedIndex = WASAPIConfig.DeviceIndex;
                    else
                        comboBox.SelectedIndex = 0;

                    if (WASAPIConfig.ShareMode == NAudio.CoreAudioApi.AudioClientShareMode.Exclusive)
                        checkBox.IsChecked = true;
                    else
                        checkBox.IsChecked = false;

                    Latency.Value = WASAPIConfig.Latency;

                    WASAPI.Background = EnabledButtonBrush;
                    break;

                case Config.WaveOut.Devices.ASIO:
                    comboBox.IsEnabled = true;
                    DisableUpdate = false;

                    ASIO.Background = EnabledButtonBrush;
                    string[] asiodev = NAudio.Wave.Asio.ASIODriver.GetASIODriverNames();
                    if (asiodev.Length > 0)
                    {
                        for (int i = 0; asiodev.Length > i; i++)
                        {
                            comboBox.Items.Add(asiodev[i]);
                            if (asiodev[i] == ASIOConfig.DriverName) comboBox.SelectedIndex = i;
                        }
                        if (comboBox.SelectedIndex < 0) comboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        Utils.Notification notice = new Utils.Notification(Parent,
                            Localize.Get("ASIO_DEV_NOTFOUND"), Constants.ErrorBrush);
                        notice.ShowMessage();
                        if (Config.Current.Output.OutputDevice == SelectedDevice)
                        {
                            SelectedDevice = Config.WaveOut.Devices.DirectSound;
                            SwitchButton();
                        }
                        else
                        {
                            SelectedDevice = Config.Current.Output.OutputDevice;
                            SwitchButton();
                        }
                        return;
                    }

                    DisableUpdate = true;
                    break;

                case Config.WaveOut.Devices.Wave:
                    Wave.Background = EnabledButtonBrush;
                    break;
            }

            AudioOutputChanged?.Invoke(this, new EventArgs());

            DisableUpdate = false;
        }

        private void ASIO_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevice = Config.WaveOut.Devices.ASIO;
            SwitchButton();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (DisableUpdate) return;

            WASAPIConfig.ShareMode = NAudio.CoreAudioApi.AudioClientShareMode.Exclusive;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (DisableUpdate) return;

            WASAPIConfig.ShareMode = NAudio.CoreAudioApi.AudioClientShareMode.Shared;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DisableUpdate) return;
            if (comboBox.SelectedItem == null) return;
            switch (SelectedDevice)
            {
                case Config.WaveOut.Devices.ASIO:
                    ASIOConfig.DriverName = comboBox.SelectedItem.ToString();
                    break;

                case Config.WaveOut.Devices.WASAPI:
                    if (comboBox.SelectedIndex > 0)
                        WASAPIConfig.DeviceFriendlyName = comboBox.SelectedItem.ToString();
                    else
                        WASAPIConfig.DeviceFriendlyName = null;
                    WASAPIConfig.DeviceIndex = comboBox.SelectedIndex;
                    break;
            }
        }

        private void DirectSound_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevice = Config.WaveOut.Devices.DirectSound;
            SwitchButton();
        }

        private void Latency_ValueChanged(object sender, EventArgs e)
        {
            if (DisableUpdate) return;

            switch (SelectedDevice)
            {
                case Config.WaveOut.Devices.DirectSound:
                    DSConfig.Latency = Latency.Value;
                    break;

                case Config.WaveOut.Devices.WASAPI:
                    WASAPIConfig.Latency = Latency.Value;
                    break;
            }
        }

        private void WASAPI_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevice = Config.WaveOut.Devices.WASAPI;
            SwitchButton();
        }

        private void Wave_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevice = Config.WaveOut.Devices.Wave;
            SwitchButton();
        }
    }
}