using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        private System.Timers.Timer DwgTimer;

        public FileChangedActions(FileWatcher _FW)
        {
            FW = _FW;
        }

        public void startMonitoring()
        {
            DwgTimer = new System.Timers.Timer();
            DwgTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            DwgTimer.Interval = 500;
            DwgTimer.Enabled = true;
        }

        public void stopMonitoring()
        {
            DwgTimer.Enabled = false;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckForChangeDWG();
           
        }

        private void CheckForChangeDWG()
        {
            if (DataStorage.ExportInProgress || DataStorage.ExportsToRun.Count == 0)
            {
                var newFolderList = FW.GetNewFolderDateList();
                var newExportList = FW.CompareFolderLists(newFolderList, DataStorage.OldFolderDateList);
                foreach (var Export in newExportList)
                {
                    DataStorage.ExportsToRun.Add(Export);
                }

                DataStorage.ExportsToRun = DataStorage.ExportsToRun.Distinct().ToList();

            }
            else
            {
                var x = FW.CheckForChanges(FW.CompareFolderLists(FW.GetNewFolderDateList(), DataStorage.OldFolderDateList));
                if (x)
                {
                    RunExport();
                }
            }


        }

        private void RunExport()
        {
            var x = FW.GetNewFolderDateList();
            DataStorage.FilesWithChanges = FW.ReturnChangedFiles(DataStorage.OldFolderDateList, x);
            DataStorage.ExportsToRun = FW.CompareFolderLists(x, DataStorage.OldFolderDateList);
            DataStorage.OldFolderDateList = FW.GetNewFolderDateList();
            DataStorage.ExportInProgress = true;
            DataStorage.app.ActiveDocument.SendCommand("AutoModeIFC ");
        }
    }
}
