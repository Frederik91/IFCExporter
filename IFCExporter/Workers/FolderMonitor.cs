﻿using IFCExporter.Models;
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

namespace IFCExporter.Workers
{
    public class FolderMonitor
    {        
        public MainClass MC;
        private bool _drawing = false;
        public System.IO.FileSystemWatcher _fsw;
        private Autodesk.AutoCAD.ApplicationServices.DocumentCollection dm = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
        private Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
        private Copier CP = new Copier();
        private DrawingManager DM = new DrawingManager();

        public FolderMonitor(MainClass _MC)
        {
            MC = _MC;
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
                _fsw.Path = MC.ProjectInfo.BaseFolder.From;
                _fsw.Filter = "*.dwg";
                _fsw.NotifyFilter = NotifyFilters.LastWrite;
                _fsw.Changed += new FileSystemEventHandler(OnChanged);
                _fsw.EnableRaisingEvents = true;
            }
        }

        [CommandMethod("ChangeDetected", CommandFlags.Session)]
        public  void OnChanged(object sender, FileSystemEventArgs e)
        {
            var dir = Path.GetDirectoryName(e.FullPath);

            var Export = LocateDrawingExport(dir, MC.ProjectInfo.Disciplines);

            var ExportList = new List<string>();
            ExportList.Add(Export);

            MC.ExportsToExecute = ExportList;
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
            //var EA = new ExportAll(MC);
            //EA.Run();

            AcadApplication app = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication as AcadApplication;
            app.ActiveDocument.SendCommand("RunOnceIFC");
        }

        public string LocateDrawingExport(string FolderPath, List<Discipline> Disciplines)
        {
            var FolderDateList = new List<FolderDate>();

            foreach (var Discipline in Disciplines)
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
