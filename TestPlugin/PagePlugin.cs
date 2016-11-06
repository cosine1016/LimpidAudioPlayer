using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearUC.ListViewItems;
using LAPP.IO;
using LAPP;
using NAudio.Wave;
using System.IO;
using System.Windows.Controls;

namespace TestPlugin
{
    public class DirectoryPage : ManageablePage
    {
        protected override void InitializeTopItems()
        {
            TopItems.Clear();

            OpenB = new ListButtonsItem.ListButton(OpenDirItem);
            OpenB.Content = "Open";
            OpenB.Click += OpenB_Click;

            OpenDirItem.Add(OpenB);
            OpenDirPageItem = new PageItem(OpenDirItem);

            TopItems.Add(OpenDirPageItem);
        }

        private PageItemCollection TopItems = new PageItemCollection();
        System.Windows.Forms.FolderBrowserDialog FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();

        PageItem OpenDirPageItem;
        ListButtonsItem OpenDirItem = new ListButtonsItem();
        ListButtonsItem.ListButton OpenB;

        public override Border Border { get; protected set; }

        public override string Title { get; protected set; } = "Directory";

        public override void Dispose() { }

        public override void PlaybackStateChanged(PlaybackState PlaybackState)
        {
        }

        public override void Update()
        {

        }

        private void OpenB_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(FolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TopItems.Clear();
                TopItems.Add(OpenDirPageItem);

                string[] paths = Directory.GetFiles(FolderBrowser.SelectedPath);
                for(int i = 0;paths.Length > i; i++)
                {
                    MediaFile mf = new MediaFile(paths[i]);
                    TopItems.Add(new FileItem(mf, new ListSubItem() { MainLabelText = mf.Title, SubLabelText = mf.Artist }, true));
                }

                UpdatePage(Level.Top);
            }
        }

        protected override PageItemCollection GetTopItems()
        {
            return TopItems;
        }
    }
}
