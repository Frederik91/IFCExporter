using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System.IO;
using Autodesk.AutoCAD.Interop;
using IFCExporter.Forms;
using System.Collections.Generic;
using IFCExporter.Workers;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace IFCExporter
{
    public class MainClass
    {
        DocumentCollection dm = Application.DocumentManager;
        Document doc = Application.DocumentManager.MdiActiveDocument;
        private List<string> ExportsToExecute = new List<string>();
        private string MonitoredExport = "";
        private Copier CP = new Copier();
        private OpenActivateClass DM = new OpenActivateClass();
        private Document Doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        private string _selectedProject;
        private string XMLFolder = "";
        private bool runForeverBool = false;
        public IFCProjectInfo ProjectInfo = new IFCProjectInfo();
        public System.IO.FileSystemWatcher _fsw;
        private Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;


        [CommandMethod("IFCEXPORTER", CommandFlags.Session)]
        public void IFCExporter()
        {
            Prepare();

            switch (runForeverBool)
            {

                case true:
                    EventCommand();
                    break;
                case false:
                    Execute("");
                    break;
            }
        }

        private void Prepare()
        {
            //Finn prosjektmappe

            //Velg prosjekt

            using (var form = new SelectProjectForm())
            {
                var result = form.ShowDialog();
                _selectedProject = form.SelectedProject;
                runForeverBool = form.RunForeverBool;
                ExportsToExecute = form.ExportsToRun;
                XMLFolder = form.XMLPath;
                if (_selectedProject == "")
                {
                    System.Windows.Forms.MessageBox.Show("No project file selected, exiting.");
                    System.Windows.Forms.Application.Exit();
                }
            }




            //Les inn XMLfil
            var reader = new XMLReader();
            ProjectInfo = reader.GetprojectInfo(XMLFolder);

            try
            {
                prepareFirstTime();

            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Failed first time setup: " + e.Message);
            }

        }

        public void Execute(string ExcusiveExport)
        {

            if (ExcusiveExport != "")
            {
                MonitoredExport = ExcusiveExport;
            }

            foreach (var Discipline in ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    var RunExport = false;
                    foreach (var Exp in ExportsToExecute)
                    {
                        if (Export.Name == Exp)
                        {
                            if (runForeverBool)
                            {
                                if (Export.Name == MonitoredExport)
                                {
                                    RunExport = true;
                                    break;

                                }
                            }
                            else
                            {
                                RunExport = true;
                                break;
                            }
                        }
                    }

                    if (RunExport)
                    {
                        foreach (var Folder in Export.Folders)
                        {
                            //--Last ned filer (modellfiler og tom.ifc)

                            //Sjekk om tegning som er åpen skal overskrives, lukke så denne mens den kopieres
                            CheckIfDrawingIsOpen_CloseIfOpen(Folder.From);

                            //Last ned mappe med modellfiler
                            CP.DirectoryCopy(Folder.From, Folder.To, false, ".dwg");

                            UnloadAllXrefs UAX = new UnloadAllXrefs();
                            UAX.UnloadAllXref(Folder.To);
                        }

                        //Lag ny IFC for eksport
                        CP.TomIFCCopy(ProjectInfo.TomIFC, Export.Name);

                        //Last ned single filer
                        foreach (var File in ProjectInfo.Files)
                        {
                            CP.CopySingleFile(File.From, File.To);
                        }

                        //--Åpne starttegning og sett som aktiv
                        var OAC = new OpenActivateClass();
                        OAC.OpenDrawing(Discipline.StartFile.To);
                        Application.DocumentManager.MdiActiveDocument = OAC.ReturnActivateDrawing(Discipline.StartFile.To);
                    

                        //--Kjør eksport
                        AcadApplication app = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication as AcadApplication;
                        app.ActiveDocument.SendCommand("_.-MAGIIFCEXPORT " + Export.Name + "\n");

                        //--Last opp IFC

                        string fromPath = Path.GetDirectoryName(ProjectInfo.TomIFC.To) + "\\" + Export.Name + ".ifc";
                        string toPath = ProjectInfo.TomIFC.Export + "\\" + Export.Name + ".ifc";
                        CP.CopySingleFile(fromPath, toPath);
                    }
                }
            }
        }



        private void prepareFirstTime()
        {
            foreach (var Discipline in ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    foreach (var Folder in Export.Folders)
                    {
                        try
                        {
                            CheckIfDrawingIsOpen_CloseIfOpen(Folder.From);
                            CP.DirectoryCopy(Folder.From, Folder.To, false, ".dwg");
                        }
                        catch (System.Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show("Failed creating Directory: " + Folder.To + " - " + e.Message);
                        }
                    }
                }

            }
            Directory.CreateDirectory(ProjectInfo.TomIFC.To);


        }

        private void CheckIfDrawingIsOpen_CloseIfOpen(string FolderDir)
        {
            DirectoryInfo DI = new DirectoryInfo(FolderDir);
            var SourceDirFiles = DI.GetFiles();
            var OpenDrawings = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            foreach (var File in SourceDirFiles)
            {
                foreach (Document drawing in OpenDrawings)
                {
                    var DrawingName = Path.GetFileName(drawing.Name);
                    var FileName = Path.GetFileName(File.Name);

                    if (DrawingName == FileName)
                    {
                        drawing.CloseAndDiscard();
                    }
                }
            }
        }

        #region test

        const string path = "C:\\TestMappe\\Drawings\\Folder1";
        private bool _drawing = false;


        private void EventCommand()
        {
           
            if (doc == null)
                return;

            var ed = doc.Editor;

            // Create a FileSystemWatcher for the path, looking for
            // write changes and drawing more squares as needed

            if (_fsw == null)
            {
                _fsw = new FileSystemWatcher();
                _fsw.Path = path;
             //  _fsw.Changed += (o, s) => nSquaresInContext(dm, ed, path);
                _fsw.NotifyFilter = NotifyFilters.LastWrite;
                _fsw.Changed += new FileSystemEventHandler(OnChanged);
                _fsw.EnableRaisingEvents = true;
            }
        }


        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            var dir = Path.GetDirectoryName(e.FullPath);

            var Export = LocateDrawingExport(dir, ProjectInfo.Disciplines);
            nSquaresInContext(dm, ed, Export);

        }
#pragma warning disable 1998

        private async void nSquaresInContext(DocumentCollection dc, Editor ed, string Export)
        {
            if (!_drawing)
            {
                _drawing = true;

                // Call our square creation function asynchronously

                await dc.ExecuteInCommandContextAsync(
                  async (o) => nSquares(ed, Export),
                  null
                );

                _drawing = false;
            }
        }

#pragma warning restore 1998

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
            Execute(Export);
        }

        #endregion


    }
}
