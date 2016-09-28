using ClearUC.ListViewItems;
using LAP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LAP.Page.TestPage
{
    internal class ScanTest : ListViewPage
    {
        public ScanTest()
        {
            LSI.MainLabelText = "Scan";
            GetTag.MainLabelText = "GetTag";
            LSI.SubLabelVisibility = System.Windows.Visibility.Hidden;
            Status.SubLabelVisibility = System.Windows.Visibility.Hidden;
            GetTag.SubLabelVisibility = System.Windows.Visibility.Hidden;
            ItemSelected += ScanTest_ItemSelected;
        }

        private void ScanTest_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            if (e.Item == LSI)
            {
                Utils.BackgroundScanner bs = new Utils.BackgroundScanner();
                bs.TaskBegin += Bs_TaskBegin;
                bs.TaskComplete += Bs_TaskComplete;
                bs.PartOfTask += Bs_PartOfTask;
                bs.Scan();
            }

            if (e.Item == GetTag)
            {
                Utils.BackgroundScanner bs = new Utils.BackgroundScanner();
                bs.GetTag(@"C:\Users\skkby\Music\iTunes\iTunes Media\Music\Echosmith\Talking Dreams\03 Cool Kids.m4a");
            }
        }

        private void Bs_PartOfTask(object sender, Utils.BackgroundScanner.TaskStateChangedArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Status.MainLabelText = e.Path;
            }));
        }

        private void Bs_TaskComplete(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Status.MainLabelText = "Task Completed";
            }));
        }

        private void Bs_TaskBegin(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Status.MainLabelText = "Task Begin";
            }));
        }

        public override Border Border { get; protected set; } = null;

        public override string Title { get; protected set; } = "ScanTest";

        private ListSubItem LSI = new ListSubItem();
        private ListSubItem Status = new ListSubItem();
        private ListSubItem GetTag = new ListSubItem();

        public override void Dispose()
        {
        }

        public override ListItem[] GetPageItems()
        {
            return new ListItem[] { };
        }

        public override ListItem[] GetTopPageItems()
        {
            return new ListItem[] { };
        }

        public override void PlayAnyFile()
        {
        }

        public override void Update()
        {
            Add(LSI);
            Add(Status);
            Add(GetTag);
        }
    }
}