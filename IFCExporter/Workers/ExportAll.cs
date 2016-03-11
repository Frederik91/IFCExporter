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

            #region prepare drawing files

            //Last ned mappe med modellfiler og unload xrefer (kun ved automatisk modus)
            if (AutomaticBool)
            {
                writer.writeLine("Automatic Mode: Gathering files to unload xref");
                var FilesWithChanges = DataStorage.FilesWithChanges.Distinct().ToList();
                var FilesToUnload = new List<string>();

                writer.writeLine("Found " + FilesWithChanges.Count + " files");
                foreach (var _file in FilesWithChanges)
                {
                    var ToPath = DataStorage.ProjectInfo.BaseFolder.To + _file.Substring(DataStorage.ProjectInfo.BaseFolder.From.Length);
                    CP.CopySingleFile(_file, ToPath);
                    FilesToUnload.Add(ToPath);
                }
                UAX.UnloadAllXref(FilesToUnload);
                writer.writeLine("Xrefs successfully unloaded");
            }
            else
            {
                foreach (var discipline in Disciplines)
                {
                    foreach (var export in discipline.Exports)
                    {
                        if (Exports.Contains(export.Name))
                        {
                            //Last ned mappe med modellfiler og unload xrefer (kun ved manuell modus)
                            writer.writeLine("Manual Mode: Gathering files to unload xref in export " + export.Name);

                            foreach (var Folder in export.Folders)
                            {
                                CP.DirectoryCopy(Folder.From, Folder.To, false, ".dwg");
                                UAX.UnloadAllXref(Directory.GetFiles(Folder.To).ToList());
                            }
                        }
                    }
                }

            }
            #endregion

            foreach (var Discipline in Disciplines)
            {
                var ActiveExport = Discipline.Exports.FindAll(export => Exports.Contains(export.Name));

                foreach (var Export in ActiveExport)
                {
                    if (Exports.Contains(Export.Name))
                    {
                        writer.writeLine("Preparing export: " + Export.Name + "\n");

                        //Lag ny IFC for eksport
                        writer.writeLine("Creating new IFC");
                        CP.TomIFCCopy(DataStorage.ProjectInfo.TomIFC, Export.Name);

                        //Last ned single filer
                        writer.writeLine("Downloading " + DataStorage.ProjectInfo.Files.Count + " single files");
                        foreach (var File in DataStorage.ProjectInfo.Files)
                        {
                            CP.CopySingleFile(File.From, File.To);
                        }

                        //--Åpne starttegning
                        writer.writeLine("Opening startfile");
                        DM.OpenDrawingReadOnly(Discipline.StartFile.To);

                        //--Kjør eksport
                        if (Application.DocumentManager.MdiActiveDocument.Name != Discipline.StartFile.To)
                        {
                            writer.writeLine("Setting active document");
                            Application.DocumentManager.MdiActiveDocument = DM.GetDrawingByName(Discipline.StartFile.To);
                        }

                        if (Application.DocumentManager.MdiActiveDocument == null || Application.DocumentManager.MdiActiveDocument.Name != Discipline.StartFile.To)
                        {
                            writer.writeLine("Unable to get application data");
                            writer.writeLine("Export failed");
                            continue;
                        }

                        var ifcLogFile = Path.GetDirectoryName(DataStorage.ProjectInfo.TomIFC.To) + "\\" + Export.Name + ".ifc.txt";

                        writer.writeLine("Running export: " + Export.Name + "\n");

                        bool exportError = false;

                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                using (DocumentLock docLock = Application.DocumentManager.MdiActiveDocument.LockDocument())
                                {
                                    var app = Application.AcadApplication as AcadApplication;
                                    app.ActiveDocument.SendCommand("_.-MAGIIFCEXPORT " + Export.Name + "\n");
                                    break;
                                }

                            }
                            catch (System.Exception e)
                            {
                                if (i==4)
                                {
                                    writer.writeLine("Error: " + e.Message);
                                    writer.writeLine("Failed to run export");
                                    exportError = true;
                                    break;
                                }
                                Thread.Sleep(2000);
                            }
                        }

                        if (exportError)
                        {
                            continue;
                        }


                        //--Last opp IFC
                        writer.writeLine("Uploading IFC");
                        var IfcFromPath = Path.GetDirectoryName(DataStorage.ProjectInfo.TomIFC.To) + "\\" + Export.Name + ".ifc";
                        var IfcToPath = DataStorage.ProjectInfo.TomIFC.Export + "\\" + Export.IFC + ".ifc";

                        var emptyIfc = new FileInfo(DataStorage.ProjectInfo.TomIFC.From);
                        var exportedIfc = new FileInfo(IfcFromPath);

                        if (emptyIfc.LastWriteTime != exportedIfc.LastWriteTime || emptyIfc.Length != exportedIfc.Length)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                try
                                {
                                    CP.CopySingleFile(IfcFromPath, IfcToPath);
                                    writer.writeLine("IFC successfully uploaded");
                                    break;
                                }
                                catch (System.Exception e)
                                {
                                    if (i == 9)
                                    {
                                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Unable to upload IFC-file, error: " + e.Message);
                                        break;
                                    }
                                    Thread.Sleep(2000);
                                }
                            }
                        }
                        else
                        {
                            writer.writeLine("IFC-file is empty, skipping upload");
                        }

                    }
                }
            }

            //-- Lukker eventuelle tegninger som ikke ble lukket av MagiCAD
            writer.writeLine("Closing other files");
            DM.CloseNotReadOnlyDrawings();

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


