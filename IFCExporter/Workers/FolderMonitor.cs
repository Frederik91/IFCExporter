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

namespace IFCExporter.Workers
{
    public class FolderMonitor
    {
        public MainClass MC;
        const string path = "C:\\TestMappe\\Drawings\\Folder1";
        private bool _drawing = false;
        public System.IO.FileSystemWatcher _fsw;
        private Autodesk.AutoCAD.ApplicationServices.DocumentCollection dm = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;
        private Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
        private Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

        public FolderMonitor(MainClass _MC)
        {
            MC = _MC;
        }

        public void EventCommand()
        {

            if (doc == null)
                return;

            var ed = doc.Editor;

            if (_fsw == null)
            {
                _fsw = new FileSystemWatcher();
                _fsw.Path = path;
                _fsw.NotifyFilter = NotifyFilters.LastWrite;
                _fsw.Changed += new FileSystemEventHandler(OnChanged);
                _fsw.EnableRaisingEvents = true;
            }
        }


        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            var dir = Path.GetDirectoryName(e.FullPath);

            var Export = LocateDrawingExport(dir, MC.ProjectInfo.Disciplines);
            nSquaresInContext(dm, ed, Export);

        }

        private async void nSquaresInContext(Autodesk.AutoCAD.ApplicationServices.DocumentCollection dc, Editor ed, string Export)
        {
            if (!_drawing)
            {
                _drawing = true;

                await dc.ExecuteInCommandContextAsync(async (o) => nSquares(ed, Export), null);

                _drawing = false;
            }
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

        private void nSquares(Editor ed, string Export)
        {
            //ON GUI THREAD
            MC.Execute(Export);
        }

    }
}
