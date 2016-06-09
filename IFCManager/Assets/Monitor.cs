using IFCExporter.Models;
using IFCExporter.Workers;
using IFCManager.Models;
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
            aTimer.Interval = 3000;
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
            var newList = new List<FileFolderDate>();
            foreach (var project in DataStorage.ProjectInfo)
            {
                var list = Conv.returnNewDateList(DataStorage.ProjectInfo);

                foreach (var item in list)
                {
                    newList.Add(item);
                }
            }
            foreach (var currentFileFolder in FolderMonitorViewModel.FileFolderLastUpdatedList)
            {
                foreach (var newFileFolder in newList)
                {
                    if (currentFileFolder.Export == newFileFolder.Export)
                    {
                        if (currentFileFolder.IfcUpdate != newFileFolder.IfcUpdate || currentFileFolder.FolderUpdate != newFileFolder.FolderUpdate || currentFileFolder.FileName != newFileFolder.FileName || currentFileFolder.Difference != newFileFolder.Difference || currentFileFolder.Updated != newFileFolder.Updated || currentFileFolder.LastSavedBy != newFileFolder.LastSavedBy)
                        {
                            FolderMonitorViewModel.FileFolderLastUpdatedList = newList;
                            return;
                        }
                    }
                }
            }
        }

    }
}
