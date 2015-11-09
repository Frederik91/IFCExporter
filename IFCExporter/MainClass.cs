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

        [CommandMethod("IFCEXPORTER", CommandFlags.Session)]
        public void IFCExporter()
        {
            Prepare();

            switch (runForeverBool)
            {

                case true:
                    FolderMonitor FM = new FolderMonitor(this);
                    FM.EventCommand();
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


    }
}
