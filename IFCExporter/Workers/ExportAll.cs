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
using System.Threading;
using System.Threading.Tasks;

namespace IFCExporter.Models
{
    public class ExportAll
    {
        private Copier CP = new Copier();
        private DrawingManager DM = new DrawingManager();
        private bool AutomaticBool;

        public ExportAll(bool automaticBool)
        {
            AutomaticBool = automaticBool;
        }

        public void Run()
        {

            foreach (var Discipline in DataStorage.ProjectInfo.Disciplines)
            {

                foreach (var Export in Discipline.Exports)
                {

                    foreach (var exp in DataStorage.ExportsToRun)
                    {

                        if (exp == Export.Name)
                        {
                            foreach (var Folder in Export.Folders)
                            {

                                //--Last ned filer (modellfiler og tom.ifc)

                                //Sjekk om tegning som er åpen skal overskrives, lukke så denne mens den kopieres
                                DM.CloseIfOpen(Folder.From);

                                //Last ned mappe med modellfiler

                                UnloadAllXrefs UAX = new UnloadAllXrefs();
                                if (AutomaticBool)
                                {
                                    foreach (var _file in DataStorage.FilesWithChanges)
                                    {
                                        var ToPath = DataStorage.ProjectInfo.BaseFolder.To + _file.Substring(DataStorage.ProjectInfo.BaseFolder.From.Length);
                                        //System.Windows.Forms.MessageBox.Show("Copy from: \"" + _file + "\" to \"" + ToPath + "\"");
                                        CP.CopySingleFile(_file, ToPath);
                                    }
                                    UAX.UnloadAllXref(DataStorage.FilesWithChanges, AutomaticBool);
                                }
                                else
                                {
                                    CP.DirectoryCopy(Folder.From, Folder.To, false, ".dwg");

                                    UAX.UnloadAllXref(Directory.GetFiles(Folder.To).ToList(), AutomaticBool);
                                }
                            }
       
                            //Lag ny IFC for eksport
                            CP.TomIFCCopy(DataStorage.ProjectInfo.TomIFC, Export.Name);

                            //Last ned single filer
                            foreach (var File in DataStorage.ProjectInfo.Files)
                            {
                                CP.CopySingleFile(File.From, File.To);
                            }

                            //--Åpne starttegning og sett som aktiv
                            var OAC = new DrawingManager();
                            OAC.OpenDrawing(Discipline.StartFile.To);
                            Application.DocumentManager.MdiActiveDocument = OAC.ReturnActivateDrawing(Discipline.StartFile.To);

                            //--Kjør eksport
                            DataStorage.app.ActiveDocument.SendCommand("_.-MAGIIFCEXPORT " + Export.Name + "\n"); //Venter ikke på at denne skal bli ferdig, må fikses.


                            //--Last opp IFC

                            string fromPath = Path.GetDirectoryName(DataStorage.ProjectInfo.TomIFC.To) + "\\" + Export.Name + ".ifc";
                            string toPath = DataStorage.ProjectInfo.TomIFC.Export + "\\" + Export.Name + ".ifc";
                            CP.CopySingleFile(fromPath, toPath);
                        }
                    }
                }
            }
            DataStorage.ExportInProgress = false;
        }
    }
}


