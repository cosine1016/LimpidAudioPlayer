using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAP.Utils
{
    public class Values
    {
        [Flags]
        public enum WindowState
        {
            Minimized = 0, Normal = 1, Maximized = 2, Activated = 3, Deactivated = 4, None = 5
        }

        /// <summary>
        /// フーリエ変換を行うタイミングを指定します
        /// </summary>
        public WindowState CalculateFFTWindowState { get; set; } = WindowState.Maximized | WindowState.Activated;

        public string MicDevice { get; set; } = null;

        public double BackgroundImageAnimationDuration { get; set; } = 200;

        public double PlayingStatusAnimationDuration { get; set; } = 200;

        public double MediaInformationAnimationDuration { get; set; } = 200;

        public double MediaInformationLyricsAnimationDuration { get; set; } = 200;

        public double ListViewItemChangedAnimationDuration { get; set; } = 500;

        public int NotificationAnimationDuration { get; set; } = 200;

        public int NotificationShowingDuration { get; set; } = 2000;

        public int EnvVisualizerUpdateInterval { get; set; } = 200;

        public int QueueWindowAnimationDuration { get; set; } = 300;

        public int EnvVisualizerPointLimit { get; set; } = 20;

        public int SpectrumBarCount { get; set; } = 64;

        public int MaxGain { get; set; } = 15;

        public int MinGain { get; set; } = -15;

        public DialogsValues Dialogs { get; set; } = new DialogsValues();

        public class DialogsValues
        {
        }
    }
}