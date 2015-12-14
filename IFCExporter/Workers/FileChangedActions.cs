using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace IFCExporter.Workers
{
    public class FileChangedActions
    {
        private FileDateComparer FDC = new FileDateComparer();
        private System.Timers.Timer DwgTimer;

        public FileChangedActions()
        {
            var text = new List<string>();
            text.Add("New FileDateComparer created at: " + DateTime.Now.ToShortTimeString() + "\n");

            File.AppendAllLines(@"C:\IFCEXPORT\log.txt", text);

        }

        public void startMonitoring()
        {
            DwgTimer = new Timer();
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
            var newIfcFileList = FDC.GetIfcFileDateList(DataStorage.ProjectInfo.TomIFC.Export);
            var newExportList = FDC.CompareFolderIfcDateLists(newFolderList, newIfcFileList);
            DataStorage.ExportsToRun = newExportList.Distinct().ToList();
            var x = FDC.GetNewFolderDateList();

            if (DataStorage.ExportsToRun.Count != 0 && FDC.ReturnChangedFiles(x).Count() != 0)
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
            DataStorage.FilesWithChanges = FDC.ReturnChangedFiles(x);
            DataStorage.ExportInProgress = true;
            var text = new List<string>();
            text.Add("Export started at " + DateTime.Now.ToString());
            text.Add("Following exports will be run");
            foreach (var exp in DataStorage.ExportsToRun)
            {
                text.Add(exp);
            }
            File.AppendAllLines("c:\\IFCEXPORT\\log.txt", text);
            //DataStorage.app.ActiveDocument.SendCommand("AutoModeIFC ");
        }
    }
}
