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

        public ExportAll(bool automaticBool)
        {
            AutomaticBool = automaticBool;
        }

        public void Run()
        {
            var OAC = new DrawingManager();
            var Exports = DataStorage.ExportsToRun;
            var Disciplines = DataStorage.ProjectInfo.Disciplines;

            foreach (var Discipline in Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    foreach (var exp in Exports)
                    {
                        if (exp == Export.Name)
                        {
                            var text = new List<string>();
                            text.Add("Running export: " + Export.Name + "\n");
                            File.AppendAllLines("c:\\IFCEXPORT\\log.txt", text);

                            foreach (var Folder in Export.Folders)
                            {
                                //Last ned mappe med modellfiler

                                UnloadAllXrefs UAX = new UnloadAllXrefs();
                                if (AutomaticBool)
                                {
                                    var FilesWithChanges = DataStorage.FilesWithChanges;
                                    var FilesToUnload = new List<string>();

                                    foreach (var _file in FilesWithChanges)
                                    {
                                        var ToPath = DataStorage.ProjectInfo.BaseFolder.To + _file.Substring(DataStorage.ProjectInfo.BaseFolder.From.Length);
                                        CP.CopySingleFile(_file, ToPath);
                                        FilesToUnload.Add(ToPath);
                                    }
                                    UAX.UnloadAllXref(FilesToUnload, AutomaticBool);


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
                            OAC.OpenDrawing(Discipline.StartFile.To);
                            Application.DocumentManager.MdiActiveDocument = OAC.ReturnActivateDrawing(Discipline.StartFile.To);

                            //--Kjør eksport
                            DataStorage.app.ActiveDocument.SendCommand("_.-MAGIIFCEXPORT " + Export.Name + "\n");

                            //--Lukk tegning
                            Document doc = Application.DocumentManager.MdiActiveDocument;
                            doc.CloseAndDiscard();

                            //--Last opp IFC
                            var IfcFromPath = Path.GetDirectoryName(DataStorage.ProjectInfo.TomIFC.To) + "\\" + Export.Name + ".ifc";
                            var IfcToPath = DataStorage.ProjectInfo.TomIFC.Export + "\\" + Export.IFC + ".ifc";

                            bool fileCopied = true;
                            int Attempts = 0;

                            while (fileCopied)
                            {
                                Attempts++;
                                try
                                {
                                    CP.CopySingleFile(IfcFromPath, IfcToPath);
                                    fileCopied = false;
                                }
                                catch (System.Exception e)
                                {
                                    if (Attempts == 10)
                                    {
                                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Unable to upload IFC-file, error: " + e.Message);
                                        break;
                                    }
                                    Thread.Sleep(2000);
                                    fileCopied = true;
                                }
                            }
                        }
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
            SleepTimer = new System.Timers.Timer();
            SleepTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            SleepTimer.Interval = 15000;
            SleepTimer.Enabled = true;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            SleepTimer.Enabled = false;
            DataStorage.ExportsToRun.Clear();
            DataStorage.ExportInProgress = false;
            var text = new List<string>();
            text.Add("Export ended at " + DateTime.Now.ToString() + "\n");
            File.AppendAllLines("c:\\IFCEXPORT\\log.txt", text);
            var FCA = new FileChangedActions();
            FCA.startMonitoring();
        }
    }

}


