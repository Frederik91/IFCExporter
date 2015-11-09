using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Workers
{
    public class Exporter
    {
        private string ExclusiveExport;
        private IFCProjectInfo ProjectInfo;
        private List<string> ExportsToExecute;
        private bool RunForeverBool;
        private DrawingManager DM = new DrawingManager();
        private Copier CP = new Copier();

        public Exporter(IFCProjectInfo projectInfo, List<string> exportsToExecute, string exclusiveExport, bool runForeverBool)
        {
            ExclusiveExport = exclusiveExport;
            ProjectInfo = projectInfo;
            ExportsToExecute = exportsToExecute;
            RunForeverBool = runForeverBool;
        }

        public void ExportAsync(FolderMonitor FM)
        {
            foreach (var Discipline in ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    var RunExport = false;
                    foreach (var Exp in ExportsToExecute)
                    {
                        if (Export.Name == Exp)
                        {
                            if (RunForeverBool)
                            {
                                if (!string.IsNullOrEmpty(ExclusiveExport))
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
                            DM.CheckIfDrawingIsOpen_CloseIfOpen(Folder.From);

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

                        FM.nSquaresInContext(Discipline.StartFile.To, Export.Name, this);

                    }
                }
            }
        }

        public void ExportSync(IFCProjectInfo ProjectInfo, string StartFile, string ExportName)
        {
            //--Åpne starttegning og sett som aktiv
            DM.OpenDrawing(StartFile);
            Application.DocumentManager.MdiActiveDocument = DM.ReturnActivateDrawing(StartFile);


            //--Kjør eksport
            AcadApplication app = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication as AcadApplication;
            app.ActiveDocument.SendCommand("_.-MAGIIFCEXPORT " + ExportName + "\n");

            //--Last opp IFC

            string fromPath = Path.GetDirectoryName(ProjectInfo.TomIFC.To) + "\\" + ExportName + ".ifc";
            string toPath = ProjectInfo.TomIFC.Export + "\\" + ExportName + ".ifc";
            CP.CopySingleFile(fromPath, toPath);
        }
    }
}
