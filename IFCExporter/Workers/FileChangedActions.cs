using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace IFCExporter.Workers
{
    public class FileChangedActions
    {
        private FileWatcher FW;

        public FileChangedActions(FileWatcher _FW)
        {
            FW = _FW;
        }

        public void startMonitoring()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 2000;
            aTimer.Enabled = true;
        }



        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (!DataStorage.ExportInProgress)
            {
                CheckForChange();
            }           

        }

        public void CheckForChange()
        {

            var x = FW.CompareFolderLists_ReturnTrueIfChanged(FW.GetNewFolderDateList());
            if (x)
            {
                DataStorage.OldFolderDateList = FW.GetNewFolderDateList();
                RunExport();
            }
        }

        private void RunExport()
        {
            DataStorage.FilesWithChanges = FW.ReturnChangedFiles(DataStorage.OldFolderDateList, FW.GetNewFolderDateList());
            DataStorage.ExportInProgress = true;
            DataStorage.app.ActiveDocument.SendCommand("AutoModeIFC ");

        }
    }
}
