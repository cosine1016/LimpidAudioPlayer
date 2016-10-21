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
        public event EventHandler AudioOutputChanged;

        public AudioOutSelector()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == false)
            {
                SelectedDevice = Utils.Config.Setting.WaveOut.OutputDevice;
                ASIOConfig = (Utils.WaveOut.ASIOConfig)Utils.Config.Setting.WaveOut.ASIO.Clone();
                WASAPIConfig = (Utils.WaveOut.WASAPIConfig)Utils.Config.Setting.WaveOut.WASAPI.Clone();
                DSConfig = (Utils.WaveOut.DirectSoundConfig)Utils.Config.Setting.WaveOut.DirectSound.Clone();
                SwitchButton();
            }
        }

        public Utils.WaveOut.ASIOConfig ASIOConfig { get; set; }
        public Brush DisabledButtonBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public Utils.WaveOut.DirectSoundConfig DSConfig { get; set; }
        public Brush EnabledButtonBrush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));

        public Utils.WaveOut.Devices SelectedDevice { get; set; }
        public Utils.WaveOut.WASAPIConfig WASAPIConfig { get; set; }
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
                case Utils.WaveOut.Devices.DirectSound:
                    Latency.IsEnabled = true;
                    Latency.Value = DSConfig.Latency;

                    DirectSound.Background = EnabledButtonBrush;
                    break;

                case Utils.WaveOut.Devices.WASAPI:
                    Latency.IsEnabled = true;
                    checkBox.IsEnabled = true;
                    comboBox.IsEnabled = true;

                    NAudio.CoreAudioApi.MMDeviceEnumerator dev = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                    NAudio.CoreAudioApi.MMDeviceCollection col =
                        dev.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active);

                    comboBox.Items.Add("<" + Utils.Config.Language.Strings.Default + ">");

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

                case Utils.WaveOut.Devices.ASIO:
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
                            Utils.Config.Language.Strings.ConfigWindow.Output.ASIODevNotFound, Utils.Config.Setting.Brushes.Notification.Error.Brush);
                        notice.ShowMessage();
                        if (Utils.Config.Setting.WaveOut.OutputDevice == SelectedDevice)
                        {
                            SelectedDevice = Utils.WaveOut.Devices.DirectSound;
                            SwitchButton();
                        }
                        else
                        {
                            SelectedDevice = Utils.Config.Setting.WaveOut.OutputDevice;
                            SwitchButton();
                        }
                        return;
                    }

                    DisableUpdate = true;
                    break;

                case Utils.WaveOut.Devices.Wave:
                    Wave.Background = EnabledButtonBrush;
                    break;
            }

            AudioOutputChanged?.Invoke(this, new EventArgs());

            DisableUpdate = false;
        }

        private void ASIO_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevice = Utils.WaveOut.Devices.ASIO;
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
                case Utils.WaveOut.Devices.ASIO:
                    ASIOConfig.DriverName = comboBox.SelectedItem.ToString();
                    break;

                case Utils.WaveOut.Devices.WASAPI:
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
            SelectedDevice = Utils.WaveOut.Devices.DirectSound;
            SwitchButton();
        }

        private void Latency_ValueChanged(object sender, EventArgs e)
        {
            if (DisableUpdate) return;

            switch (SelectedDevice)
            {
                case Utils.WaveOut.Devices.DirectSound:
                    DSConfig.Latency = Latency.Value;
                    break;

                case Utils.WaveOut.Devices.WASAPI:
                    WASAPIConfig.Latency = Latency.Value;
                    break;
            }
        }

        private void WASAPI_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevice = Utils.WaveOut.Devices.WASAPI;
            SwitchButton();
        }

        private void Wave_Click(object sender, RoutedEventArgs e)
        {
            SelectedDevice = Utils.WaveOut.Devices.Wave;
            SwitchButton();
        }
    }
}