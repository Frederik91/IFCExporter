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
        private FileDateComparer FDC = new FileDateComparer();
        private System.Timers.Timer DwgTimer;

        public void startMonitoring()
        {
            DwgTimer = new System.Timers.Timer();
            DwgTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            DwgTimer.Interval = 2000;
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
            stopMonitoring();
            if (DataStorage.ExportInProgress)
            {
                return;
            }

            var newFolderList = FDC.GetNewFolderDateList();
            var newIfcFileList = FDC.GetNewIfcFileDateList();
            var newExportList = FDC.CompareFolderIfcDateLists(newFolderList, newIfcFileList);
            DataStorage.ExportsToRun = newExportList.Distinct().ToList();

            if (DataStorage.ExportsToRun.Count != 0)
            {                
                RunExport();
            }
            else
            {
                startMonitoring();
            }
        }

        private void RunExport()
        {
            var x = FDC.GetNewFolderDateList();
            DataStorage.FilesWithChanges = FDC.ReturnChangedFiles(DataStorage.OldFolderDateList, x);
            DataStorage.OldFolderDateList = FDC.GetNewFolderDateList();
            DataStorage.ExportInProgress = true;
            File.AppendAllText("c:\\IFCEXPORT\\log.txt", "ExportInProgress = " + DataStorage.ExportInProgress.ToString() + ", at " + DateTime.Now.ToString() + "\n");
            DataStorage.app.ActiveDocument.SendCommand("AutoModeIFC ");
        }
    }
}
