using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Helpers;
using IFCExporter.Workers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace IFCExporter.Models
{
    public class ExportAll
    {
        private Copier CP = new Copier();
        private DrawingManager DM = new DrawingManager();
        private bool AutomaticBool;
        private System.Timers.Timer SleepTimer;
        private Writer writer = new Writer();

        public ExportAll(bool automaticBool)
        {
            AutomaticBool = automaticBool;
        }

        public void Run()
        {
            var DM = new DrawingManager();
            var Exports = DataStorage.ExportsToRun;
            var Disciplines = DataStorage.ProjectInfo.Disciplines;
            var UAX = new UnloadAllXrefs();

            //Last ned mappe med modellfiler og unload xrefer (kun ved automatisk modus)
            if (AutomaticBool)
            {
                //--Åpne ny tegning hvis alle tegningene ble lukket

                if (DataStorage.app.Documents.Count == 0)
                {
                    var newDrawing = DataStorage.app.Documents.Add("acad.dwt");
                    DataStorage.app.ActiveDocument = newDrawing;
                }

                writer.writeLine("Automatic Mode: Gathering files to unload xref");
                var FilesWithChanges = DataStorage.FilesWithChanges;
                var FilesToUnload = new List<string>();

                writer.writeLine("Found " + FilesWithChanges.Count + " files");
                foreach (var _file in FilesWithChanges)
                {
                    var ToPath = DataStorage.ProjectInfo.BaseFolder.To + _file.Substring(DataStorage.ProjectInfo.BaseFolder.From.Length);
                    CP.CopySingleFile(_file, ToPath);
                    FilesToUnload.Add(ToPath);
                }
                UAX.UnloadAllXref(FilesToUnload, AutomaticBool);
                writer.writeLine("Xrefs successfully unloaded");
            }

            foreach (var Discipline in Disciplines)
            {
                //Kun ved automatisk modus pga. måten filene kopieres ned på. 
                if (AutomaticBool)
                {
                    //--Åpne starttegning og sett som aktiv
                    writer.writeLine("Opening startfile");
                    DM.OpenDrawing(Discipline.StartFile.To);
                    Application.DocumentManager.MdiActiveDocument = DM.ReturnActivateDrawing(Discipline.StartFile.To);
                }

                foreach (var Export in Discipline.Exports)
                {
                    foreach (var exp in Exports)
                    {
                        if (exp == Export.Name)
                        {
                            writer.writeLine("Running export: " + Export.Name + "\n");

                            //Lag ny IFC for eksport
                            writer.writeLine("Creating new IFC");
                            CP.TomIFCCopy(DataStorage.ProjectInfo.TomIFC, Export.Name);

                            //Last ned single filer
                            writer.writeLine("Downloading " + DataStorage.ProjectInfo.Files.Count + " single files");
                            foreach (var File in DataStorage.ProjectInfo.Files)
                            {
                                CP.CopySingleFile(File.From, File.To);
                            }

                            //Ved automatisk modus lastes alle filene ned og xrefer unloades først. Ved manuell modus gjøres dette før hver individuelle eksport. Det er derfor nødvendig å åpne/lukke tegningene mellom hver eksport.
                            if (!AutomaticBool)
                            {
                                //Last ned mappe med modellfiler og unload xrefer (kun ved manuell modus)
                                writer.writeLine("Manual Mode: Gathering files to unload xref in export " + Export.Name);

                                foreach (var Folder in Export.Folders)
                                {
                                    CP.DirectoryCopy(Folder.From, Folder.To, false, ".dwg");
                                    UAX.UnloadAllXref(Directory.GetFiles(Folder.To).ToList(), AutomaticBool);
                                }

                                writer.writeLine("Xrefs successfully unloaded");

                                //--Åpne starttegning og sett som aktiv
                                writer.writeLine("Opening startfile");
                                DM.OpenDrawing(Discipline.StartFile.To);
                                Application.DocumentManager.MdiActiveDocument = DM.ReturnActivateDrawing(Discipline.StartFile.To);
                            }


                            //--Kjør eksport
                            writer.writeLine("Running export");
                            DataStorage.app.ActiveDocument.SendCommand("_.-MAGIIFCEXPORT " + Export.Name + "\n");

                            if (!AutomaticBool)
                            {
                                //--Lukk tegning
                                writer.writeLine("Closing drawing");
                                try
                                {
                                    DataStorage.app.ActiveDocument.Close(false, Discipline.StartFile);
                                }
                                catch (System.Exception e)
                                {
                                    System.Windows.MessageBox.Show(e.Message);
                                    //DataStorage.app.Documents.Close();
                                }

                            }

                            //--Last opp IFC
                            writer.writeLine("Uploading IFC");
                            var IfcFromPath = Path.GetDirectoryName(DataStorage.ProjectInfo.TomIFC.To) + "\\" + Export.Name + ".ifc";
                            var IfcToPath = DataStorage.ProjectInfo.TomIFC.Export + "\\" + Export.IFC + ".ifc";

                            bool fileCopied = false;
                            int Attempts = 0;

                            var emptyIfc = new FileInfo(Discipline.StartFile.From);
                            var exportedIfc = new FileInfo(IfcFromPath);

                            if (emptyIfc.Length != exportedIfc.Length)
                            {
                                while (!fileCopied)
                                {
                                    Attempts++;
                                    try
                                    {
                                        CP.CopySingleFile(IfcFromPath, IfcToPath);
                                        fileCopied = true;
                                        writer.writeLine("IFC successfully uploaded");
                                    }
                                    catch (System.Exception e)
                                    {
                                        if (Attempts == 10)
                                        {
                                            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Unable to upload IFC-file, error: " + e.Message);
                                            break;
                                        }
                                        Thread.Sleep(2000);
                                        fileCopied = false;
                                    }
                                }
                            }                            
                        }
                    }
                }
                if (AutomaticBool)
                {
                    //--Lukk tegning
                    writer.writeLine("Closing drawings");

                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            DataStorage.app.Documents.Close();
                            writer.writeLine("Drawings closed");
                        }
                        catch (System.Exception e)
                        {
                            if (e.HResult.Equals("0x8021006F"))
                            {
                                writer.writeLine("Failed to close drawings");
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }



                    //--Åpne ny tegning hvis alle tegningene ble lukket

                    if (DataStorage.app.Documents.Count == 0)
                    {
                        writer.writeLine("Creating new drawing");
                        var newDrawing = DataStorage.app.Documents.Add("acad.dwt");
                        DataStorage.app.ActiveDocument = newDrawing;
                        writer.writeLine("Drawing successfully created");
                    }



                }
            }
            if (AutomaticBool)
            {
                SleepBeforeReset();
            }
        }



        public void SleepBeforeReset()
        {
            writer.writeLine("Cooling down after export");
            SleepTimer = new System.Timers.Timer();
            SleepTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            SleepTimer.Interval = 15000;
            SleepTimer.Enabled = true;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                SleepTimer.Enabled = false;
                DataStorage.ExportsToRun.Clear();
                writer.writeLine("Export ended at " + DateTime.Now.ToString() + "\n");
                var FCA = new FileChangedActions();
                FCA.startMonitoring();
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                throw;
            }

        }
    }

}


