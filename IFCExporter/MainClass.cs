using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Helpers;
using System.IO;
using Autodesk.AutoCAD.Interop;
using IFCExporter.Forms;
using System.Collections.Generic;
using IFCExporter.Workers;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Runtime.InteropServices;
using IFCExporter.Models;
using System.Timers;

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
        public IFCProjectInfo ProjectInfo = new IFCProjectInfo();


        [CommandMethod("IFCExporter", CommandFlags.Session)]
        public void IFCExporter()
        {
            DataStorage.app = Application.AcadApplication as AcadApplication;

            var NAcadTask = new NonAutoCADTasks();
            DataStorage.ProjectInfo = NAcadTask.Prepare();

            RunForeverBool = NAcadTask.RunForeverBool;
            AutomaticBool = NAcadTask.AutomaticMode;
            DataStorage.ExportsToRun = NAcadTask.ExportsToExecute;
            DataStorage.TempExportsToRun = new List<string>();


            switch (AutomaticBool)
            {
                case true:
                    FileWatcher AE = new FileWatcher();
                    DataStorage.OldFolderDateList = AE.GetNewFolderDateList();
                    var FCA = new FileChangedActions(AE);
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
                var expAll = new ExportAll(AutomaticBool);
                expAll.Run();
            }
        }

        #endregion
    }
}
