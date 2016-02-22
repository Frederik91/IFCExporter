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
        public IfcProjectInfo ProjectInfo = new IfcProjectInfo();
        private Writer writer = new Writer();


        public MainClass()
        {

        }


        [CommandMethod("IFCExporter", CommandFlags.Session)]
        public void IFCExporter()
        {
            DataStorage.app = Application.AcadApplication as AcadApplication;
            var NAcadTask = new NonAutoCADTasks();
            DataStorage.ProjectInfo = NAcadTask.Prepare();

            if (DataStorage.ProjectInfo == null)
            {
                return;
            }

            DataStorage.logFileLocation = "c:\\IFCEXPORT\\log-" + DataStorage.ProjectInfo.ProjectName + ".txt";
            DataStorage.SelectedExports = NAcadTask.ExportsToExecute;
            DataStorage.ExportsToRun = new List<string>();
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
            EA.Run();
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
                    expAll.Run();
                }
            }
            else
            {
                var text = new List<string>();
                text.Add("Starting one-time IFC-export at: " + DateTime.Now);
                text.Add("Will be running the following exports: ");
                DataStorage.ExportsToRun = DataStorage.SelectedExports;
                foreach (var exp in DataStorage.ExportsToRun)
                {
                    text.Add(exp);
                }
                writer.writeArray(text.ToArray());
                var expAll = new ExportAll(AutomaticBool);
                expAll.Run();
            }
        }

        #endregion
    }
}
