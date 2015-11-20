using IFCExporter.Models;
using IFCExporter.Workers;
using IFCManager.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IFCManager.Assets
{
    public class Monitor
    {
        private FolderMonitorViewModel FolderMonitorViewModel;
        private System.Timers.Timer aTimer = new System.Timers.Timer();

        public Monitor(FolderMonitorViewModel _folderMonitorViewModel)
        {
            FolderMonitorViewModel = _folderMonitorViewModel;

        }

        public void StartMonitoring()
        {          
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckChangesIFC();
        }


        private void CheckChangesIFC()
        {
            var FDC = new FileDateComparer();
            var Conv = new ConvertToFileFolderDate();
            FolderMonitorViewModel.FileFolderLastUpdatedList = Conv.Convert(FDC.GetNewFolderDateList(), FDC.GetIfcFileDateList(DataStorage.ProjectInfo.TomIFC.Export));
        }

    }
}
