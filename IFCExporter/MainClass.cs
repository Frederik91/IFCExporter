using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Helpers;
using System.IO;
using Autodesk.AutoCAD.Interop;
using System.Collections.Generic;
using IFCExporter.Workers;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Runtime.InteropServices;
using IFCExporter.Models;
using System.Timers;
using IFCExporterAPI.Models;

namespace IFCExporter
{
    public class MainClass
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        public List<string> ExportsToExecute = new List<string>();
        private Copier CP = new Copier();
        private DrawingManager DM = new DrawingManager();
        private Document Doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
        public bool RunForeverBool = false;
        public bool AutomaticBool = false;
        public List<IfcProjectInfo> ProjectInfo = new List<IfcProjectInfo>();
        private Writer writer = new Writer();


        public MainClass()
        {

        }

        [CommandMethod("IFCExporterAuto", CommandFlags.Session)]
        public void IFCExporterAuto()
        {
            DataStorage.app = Application.AcadApplication as AcadApplication;
            var NAcadTask = new NonAutoCADTasks();
            DataStorage.ProjectInfo = NAcadTask.Prepare_NoDialog();

            if (DataStorage.ProjectInfo == null || DataStorage.ProjectInfo.Count < 1)
            {
                return;
            }

            DataStorage.logFileLocation = string.Format("c:\\IFCEXPORT\\log-" + DataStorage.ProjectInfo[0].ProjectName + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
            //DataStorage.SelectedExports = NAcadTask.ExportsToExecute;
            DataStorage.ExportsToRun = new List<IfcProjectInfo>();
            RunForeverBool = NAcadTask.ContinuousMode;
            AutomaticBool = NAcadTask.AutomaticMode;

            switch (AutomaticBool)
            {
                case true:

                    var FCA = new FileChangedActions();
                    FCA.startMonitoring();

                    break;
                case false:

                    RunOnceIFC();
                    break;

            }
        }

        [CommandMethod("IFCExporter", CommandFlags.Session)]
        public void IFCExporter()
        {
            DataStorage.app = Application.AcadApplication as AcadApplication;
            DataStorage.ProjectChanges = new List<ProjectChanges>();
            var NAcadTask = new NonAutoCADTasks();
            DataStorage.ProjectInfo = NAcadTask.Prepare();

            if (DataStorage.ProjectInfo == null || DataStorage.ProjectInfo.Count < 1)
            {
                return;
            }

            DataStorage.logFileLocation = string.Format("c:\\IFCEXPORT\\log-" + DataStorage.ProjectInfo[0].ProjectName + "-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
            //DataStorage.SelectedExports = NAcadTask.ExportsToExecute;
            DataStorage.ExportsToRun = new List<IfcProjectInfo>();
            RunForeverBool = NAcadTask.ContinuousMode;
            AutomaticBool = NAcadTask.AutomaticMode;

            switch (AutomaticBool)
            {
                case true:

                    var FCA = new FileChangedActions();
                    FCA.startMonitoring();

                    break;
                case false:

                    RunOnceIFC();
                    break;

            }
        }

        [CommandMethod("AutoModeIFC", CommandFlags.Session)]
        public void AutoModeIFC()
        {
            var EA = new ExportAll(true);
            EA.Run(Guid.Empty);
        }

        [CommandMethod("AutomaticIFC", CommandFlags.Session)]
        public void AutomaticIFC()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptResult prConfigPath = ed.GetString("\nEnter path to xml-file: ");
            if (prConfigPath.Status != PromptStatus.OK)
            {
                ed.WriteMessage("No string was provided\n");
                return;
            }

            PromptResult prExportId = ed.GetString("\nEnter export id: ");
            if (prConfigPath.Status != PromptStatus.OK)
            {
                ed.WriteMessage("No string was provided\n");
                return;
            }

            if (Guid.TryParse(prExportId.StringResult, out Guid id))
            {
                var expAll = new ExportAll(false);
                expAll.Run(id, prConfigPath.StringResult);
            }

        }


        #region RunOnce

        [CommandMethod("RunOnceIFC", CommandFlags.Session)]
        public void RunOnceIFC()
        {
            if (RunForeverBool)
            {
                while (true)
                {
                    var expAll = new ExportAll(AutomaticBool);
                    expAll.Run(Guid.Empty);
                }
            }
            else
            {
                var text = new List<string>
                {
                    "Starting one-time IFC-export at: " + DateTime.Now,
                    "Will be running the following exports: "
                };
                DataStorage.ExportsToRun = ProjectInfo;
                //foreach (var exp in DataStorage.ExportsToRun)
                //{
                //    text.Add(exp);
                //}
                //writer.writeArray(text.ToArray());
                var expAll = new ExportAll(AutomaticBool);
                expAll.Run(Guid.Empty);
            }
        }

        #endregion
    }
}
