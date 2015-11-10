using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Models
{
    public class ExportAll
    {
        private MainClass MC;
        private Copier CP = new Copier();
        private DrawingManager DM = new DrawingManager();
        private List<string> ExportsToRun;

        public ExportAll(MainClass _MC, List<string> exportsToRun)
        {
            MC = _MC;
            ExportsToRun = exportsToRun;
        }

        public void Run(AcadApplication app)
        {
            MC.ExportsToExecute = ExportsToRun;
            foreach (var Discipline in MC.ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    var RunExport = false;
                    foreach (var Exp in MC.ExportsToExecute)
                    {
                        if (Export.Name == Exp)
                        {
                            RunExport = true;
                            break;
                        }
                    }

                    if (RunExport)
                    {
                        foreach (var Folder in Export.Folders)
                        {
                            //--Last ned filer (modellfiler og tom.ifc)

                            //Sjekk om tegning som er åpen skal overskrives, lukke så denne mens den kopieres
                            DM.CloseIfOpen(Folder.From);

                            //Last ned mappe med modellfiler
                            CP.DirectoryCopy(Folder.From, Folder.To, false, ".dwg");

                            UnloadAllXrefs UAX = new UnloadAllXrefs(); 
                            UAX.UnloadAllXref(Folder.To);
                        }

                        //Lag ny IFC for eksport
                        CP.TomIFCCopy(MC.ProjectInfo.TomIFC, Export.Name);

                        //Last ned single filer
                        foreach (var File in MC.ProjectInfo.Files)
                        {
                            CP.CopySingleFile(File.From, File.To);
                        }

                        //--Åpne starttegning og sett som aktiv
                        var OAC = new DrawingManager();
                        OAC.OpenDrawing(Discipline.StartFile.To);                        
                        Application.DocumentManager.MdiActiveDocument = OAC.ReturnActivateDrawing(Discipline.StartFile.To);

                        //--Kjør eksport
                        app.ActiveDocument.SendCommand("_.-MAGIIFCEXPORT " + Export.Name + "\n");  // KRASJER HER!!
          
                        //--Last opp IFC

                        string fromPath = Path.GetDirectoryName(MC.ProjectInfo.TomIFC.To) + "\\" + Export.Name + ".ifc";
                        string toPath = MC.ProjectInfo.TomIFC.Export + "\\" + Export.Name + ".ifc";
                        CP.CopySingleFile(fromPath, toPath);
                    }
                }
            }
        }
    }
}

