using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;

namespace IFCExporter.Workers
{
    public class FileChangedActions
    {
        private FileDateComparer FDC = new FileDateComparer();
        private System.Timers.Timer DwgTimer;
        private Writer writer = new Writer();

        public FileChangedActions()
        {
            writer.writeLine("New FileDateComparer created");

        }

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
            stopMonitoring();

            CheckForChanges();
        }

        public void CheckForChanges()
        {
            var newFolderList = FDC.GetNewFolderDateList();
            var newIfcFileList = FDC.GetIfcFileDateList(DataStorage.ProjectInfo.TomIFC.Export);
            var newExportList = FDC.ReturnExpiredExports(newFolderList, newIfcFileList).Distinct().ToList();

            foreach (var Export in newExportList)
            {
                if (DataStorage.SelectedExports.Contains(Export))
                {
                    DataStorage.ExportsToRun.Add(Export);
                }
            }

            if (DataStorage.ExportsToRun.Count > 0)
            {
                writer.writeLine("Filechange detected, starting new export");
                RunExport();
            }
            else
            {
                var textfile = File.ReadAllLines(DataStorage.logFileLocation);
                var length = textfile.Length;
                if (textfile[length - 1].Contains("No change detected"))
                {
                    writer.removeLastLine(textfile);
                }

                writer.writeLine("No change detected, waiting...");
                startMonitoring();
            }
        }

        private void RunExport()
        {
            DataStorage.FilesWithChanges = FDC.ReturnDwgsInChangedExports();
            var text = new List<string>();
            text.Add("Following exports will be run");
            foreach (var exp in DataStorage.ExportsToRun)
            {
                text.Add(exp);
            }
            writer.writeArray(text.ToArray());
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using (DocumentLock docLock = Application.DocumentManager.MdiActiveDocument.LockDocument())
                    {
                        var app = Application.AcadApplication as AcadApplication;
                        app.Visible = true;
                        app.ActiveDocument.SendCommand("AutoModeIFC ");
                        break;
                    }

                }
                catch (System.Exception e)
                {
                    if (i == 4)
                    {
                        writer.writeLine("Error: " + e.Message);
                        writer.writeLine("Failed to start export");
                        break;
                    }
                    Thread.Sleep(2000);
                }
            }

            Thread.CurrentThread.Abort();
        }
    }
}
