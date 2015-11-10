using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Autodesk.AutoCAD.EditorInput;
using IFCExporter.Helpers;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Forms;
using System.Runtime.InteropServices;

namespace IFCExporter.Workers
{
    public class FolderMonitor
    {
        private bool _drawing = false;
        public FileSystemWatcher _fsw;
        private Autodesk.AutoCAD.ApplicationServices.DocumentCollection dm = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
        private Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
        private Copier CP = new Copier();
        private DrawingManager DM = new DrawingManager();
        private AcadApplication app;

        public FolderMonitor()
        {

        }

        [CommandMethod("EventCommand", CommandFlags.Session)]
        public void EventCommand()
        {
            if (doc == null)
                return;

            var ed = doc.Editor;

            if (_fsw == null)
            {
                _fsw = new FileSystemWatcher();
                _fsw.IncludeSubdirectories = true;
                _fsw.Path = DataStorage.ProjectInfo.BaseFolder.From;
                _fsw.NotifyFilter = NotifyFilters.LastWrite;
                _fsw.InternalBufferSize = 65536;
                _fsw.Filter = "*.dwl";
                _fsw.Changed += new FileSystemEventHandler(OnChanged);
                _fsw.EnableRaisingEvents = true;
            }
        }

        [CommandMethod("ChangeDetected", CommandFlags.Session)]
        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            var dir = Path.GetDirectoryName(e.FullPath);

            var Export = LocateDrawingExport(dir);

            if (string.IsNullOrEmpty(Export))
            {
                return;
            }

            var RunExportBool = false;
            var ExportIsInList = false;
            var ForCount = 0;
            
            foreach (var exp in DataStorage.ExportUpdateList)
            {
                if (exp.Name == Export)
                {
                    if (exp.LastUpdated.AddSeconds(10) < DateTime.Now)
                    {
                        RunExportBool = true;
                        ExportIsInList = true;
                        break;                   
                    }
                    else
                    {
                        ExportIsInList = true;
                        return;
                    }
                }
                ForCount++;
            }

            if (RunExportBool) //Må komme før ExportIsInList-if-løkken
            {
                DataStorage.ExportUpdateList[ForCount] = new ExportUpdateInfo { Name = Export, LastUpdated = DateTime.Now };
            }

            if (!ExportIsInList) //Må komme etter RunExportBool-if-løkken
            {
                DataStorage.ExportUpdateList.Add(new ExportUpdateInfo { Name = Export, LastUpdated = DateTime.Now });
                RunExportBool = true;
            }

            if (RunExportBool) //Siste If-løkke i rekken.
            {
                if (DataStorage.ExportUpdateList.Count > 0)
                {
                    DataStorage.ExportUpdateList[ForCount] = new ExportUpdateInfo { Name = Export, LastUpdated = DateTime.Now };
                }               

            }

            DataStorage.ExportToRun = Export;


            //MessageBox.Show("Change in file: " + e.FullPath + ". Change: " + e.ChangeType + ". FileName: " + e.Name + "Last Updated: " + DataStorage.ExportUpdateList[ForCount].LastUpdated);
            RunExport();

        }

        public async void RunExport()
        {
            if (!_drawing)
            {
                _drawing = true;

                var dc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;

                await dc.ExecuteInCommandContextAsync(async (o) => RunSpecifiedExport(), null);
                _drawing = false;
            }
        }

        private void RunSpecifiedExport()
        {
            //ON GUI THREAD

            app = (AcadApplication)Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;

            app.ActiveDocument.SendCommand("MonitorIFC ");



        }

        public string LocateDrawingExport(string FolderPath)
        {
            var FolderDateList = new List<FolderDate>();

            foreach (var Discipline in DataStorage.ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    foreach (var Folder in Export.Folders)
                    {
                        if (FolderPath == Folder.From)
                        {
                            return Export.Name;
                        }
                    }
                }
            }
            return "";
        }
    }
}
