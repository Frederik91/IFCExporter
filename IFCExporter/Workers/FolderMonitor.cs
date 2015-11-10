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
        public System.IO.FileSystemWatcher _fsw;
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
                _fsw.Changed += new FileSystemEventHandler(OnChanged);
                _fsw.EnableRaisingEvents = true;
            }
        }

        [CommandMethod("ChangeDetected", CommandFlags.Session)]
        public  void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.FullPath) == ".dwg")
            {
                return;
            }
            var dir = Path.GetDirectoryName(e.FullPath);

            var Export = LocateDrawingExport(dir);

            if (string.IsNullOrEmpty(Export))
            {
                return;
            }

            DataStorage.ExportToRun = Export;
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
