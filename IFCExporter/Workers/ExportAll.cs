using AutoCAD;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Helpers;
using IFCExporter.Workers;
using IFCExporterAPI.Assets;
using IFCExporterAPI.Models;
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

        public void Run(Guid id, string xmlPath = null)
        {
            var DM = new DrawingManager();
            var Projects = DataStorage.ExportsToRun;
            var UAX = new UnloadAllXrefs();

            if (id != Guid.Empty)
            {
                var reader = new XmlReader();
                if (File.Exists(xmlPath))
                {
                    Projects = new List<IfcProjectInfo> { reader.GetprojectInfo(xmlPath) };
                }
                else
                {
                    return;
                }

            }

            foreach (var project in Projects)
            {
                foreach (var discipline in project.Disciplines)
                {
                    foreach (var folder in discipline.Exports.SelectMany(x => x.Folders))
                    {
                        if (Directory.Exists(folder.local))
                        {
                            var filesInRemote = Directory.GetFiles(folder.remote).ToList();
                            var filesInLocal = Directory.GetFiles(folder.local).ToList();
                            var filesNotInLocal = filesInRemote.Where(x => !filesInLocal.Contains(x));
                            if (filesNotInLocal.Count() > 0)
                            {
                                foreach (var file in filesNotInLocal)
                                {
                                    File.Copy(file, Path.Combine(folder.local, Path.GetFileName(file)), true);
                                }
                            }                            
                        }
                        else
                        {
                            CP.DirectoryCopy(folder.remote, folder.local, false, ".dwg");
                        }
                    }
                    foreach (var export in discipline.Exports)
                    {
                        if (id != Guid.Empty && id != export.Id)
                        {
                            continue;
                        }

                        writer.WriteLine("Preparing export: " + export.Name + "\n");

                        //Last ned DWG-filer
                        foreach (var folder in export.Folders)
                        {
                            writer.WriteLine("Downloading folder:  " + folder.remote + "\n");
                            CP.DirectoryCopy(folder.remote, folder.local, false, ".dwg");

                            var drawings = Directory.GetFiles(folder.local, "*.dwg").ToList();

                            //writer.WriteLine("Unloading Xrefs");
                            //UAX.UnloadAllXref(drawings);
                        }

                        if (!File.Exists(discipline.StartFile.To))
                        {
                            File.Copy(discipline.StartFile.From, discipline.StartFile.To);
                        }


                        //Lag ny IFC for eksport
                        writer.WriteLine("Creating new IFC");
                        CP.TomIFCCopy(project.TomIFC, export.Name);

                        //Last ned single filer
                        writer.WriteLine("Downloading " + project.Files.Count + " single files");
                        foreach (var File in project.Files)
                        {
                            CP.CopySingleFile(File.From, File.To);
                        }

                        //--Åpne starttegning
                        writer.WriteLine("Opening startfile");
                        DM.OpenDrawingReadOnly(discipline.StartFile.To);

                        //--Kjør eksport
                        if (Application.DocumentManager.MdiActiveDocument.Name != discipline.StartFile.To)
                        {
                            writer.WriteLine("Setting active document");
                            Application.DocumentManager.MdiActiveDocument = DM.GetDrawingByName(discipline.StartFile.To);
                        }

                        if (Application.DocumentManager.MdiActiveDocument == null || Application.DocumentManager.MdiActiveDocument.Name != discipline.StartFile.To)
                        {
                            writer.WriteLine("Unable to get application data");
                            writer.WriteLine("Export failed");
                            continue;
                        }

                        var ifcLogFile = Path.GetDirectoryName(project.TomIFC.To) + "\\" + export.Name + ".ifc.txt";

                        writer.WriteLine("Running export: " + export.Name + "\n");

                        bool exportError = false;

                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                using (DocumentLock docLock = Application.DocumentManager.MdiActiveDocument.LockDocument())
                                {
                                    var app = Application.AcadApplication as AcadApplication;
                                    app.Visible = true;
                                    if (id != Guid.Empty)
                                    {
                                        return;
                                    }
                                    app.ActiveDocument.SendCommand("_.-MAGIIFCEXPORT " + export.Name + "\n");
                                    break;
                                }

                            }
                            catch (System.Exception e)
                            {
                                if (i == 4)
                                {
                                    writer.WriteLine("Error: " + e.Message);
                                    writer.WriteLine("Failed to run export");
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
                        writer.WriteLine("Uploading IFC");
                        var IfcFromPath = Path.GetDirectoryName(project.TomIFC.To) + "\\" + export.Name + ".ifc";
                        var IfcToPath = project.TomIFC.Export + "\\" + export.IFC + ".ifc";

                        var emptyIfc = new FileInfo(project.TomIFC.From);
                        var exportedIfc = new FileInfo(IfcFromPath);

                        if (emptyIfc.LastWriteTime != exportedIfc.LastWriteTime || emptyIfc.Length != exportedIfc.Length)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                try
                                {
                                    CP.CopySingleFile(IfcFromPath, IfcToPath);
                                    writer.WriteLine("IFC successfully uploaded");
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
                            writer.WriteLine("IFC-file is empty, skipping upload");
                        }
                    }

                }
            }

            //-- Lukker eventuelle tegninger som ikke ble lukket av MagiCAD
            writer.WriteLine("Closing other files");
            DM.CloseNotReadOnlyDrawings();

            if (AutomaticBool)
            {
                SleepBeforeReset();
            }
        }

        public void SleepBeforeReset()
        {
            writer.WriteLine("Cooling down after export");
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
                writer.WriteLine("Export ended at " + DateTime.Now.ToString() + "\n");
                DataStorage.ExportsToRun = new List<IFCExporterAPI.Models.IfcProjectInfo>();
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


