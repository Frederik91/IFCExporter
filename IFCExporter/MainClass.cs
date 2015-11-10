using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System.IO;
using Autodesk.AutoCAD.Interop;
using IFCExporter.Forms;
using System.Collections.Generic;
using IFCExporter.Workers;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Runtime.InteropServices;

namespace IFCExporter
{
    public class MainClass
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        public List<string> ExportsToExecute = new List<string>();
        private Copier CP = new Copier();
        private DrawingManager DM = new DrawingManager();
        private Document Doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
        private string _selectedProject;
        private string XMLFolder = "";
        public bool RunForeverBool = false;
        public IFCProjectInfo ProjectInfo = new IFCProjectInfo();
        private AcadApplication app;



        [CommandMethod("IFCExporter", CommandFlags.Session)]
        public void IFCExporter()
        {
            const string strProgId = "AutoCAD.Application.20.1";
            app = (AcadApplication)Marshal.GetActiveObject(strProgId);

            Prepare();

            var x = System.Windows.Forms.MessageBox.Show("Yes = Automatic, No = Manual", "Mode select", System.Windows.Forms.MessageBoxButtons.YesNo);

            switch (x)
            {
                case System.Windows.Forms.DialogResult.Yes:
                    AutoModeIFC();
                    break;
                case System.Windows.Forms.DialogResult.No:
                    RunOnceIFC();                   
                    break;

            }
        }

        [CommandMethod("RunOnceIFC", CommandFlags.Session)]
        public void RunOnceIFC()
        {
            var expAll = new ExportAll(this, ExportsToExecute);
            expAll.Run(app);
        }


        [CommandMethod("AutoModeIFC", CommandFlags.Session)]
        public void AutoModeIFC()
        {
            FolderMonitor FM = new FolderMonitor(this, app);
            FM.EventCommand();
        }


        private void Prepare()
        {
            //Finn prosjektmappe

            //Velg prosjekt

            using (var form = new SelectProjectForm())
            {
                var result = form.ShowDialog();
                _selectedProject = form.SelectedProject;
                RunForeverBool = form.RunForeverBool;
                ExportsToExecute = form.ExportsToRun;
                XMLFolder = form.XMLPath;
                if (_selectedProject == "")
                {
                    System.Windows.Forms.MessageBox.Show("No project file selected, exiting.");
                    return;
                }
            }


            //Les inn XMLfil
            var reader = new XMLReader();
            ProjectInfo = reader.GetprojectInfo(XMLFolder);

            try
            {
                prepareFirstTime();

            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Failed first time setup: " + e.Message);
            }

        }

        private void prepareFirstTime()
        {
            foreach (var Discipline in ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    foreach (var Folder in Export.Folders)
                    {
                        try
                        {
                            DM.CloseIfOpen(Folder.From);
                            CP.DirectoryCopy(Folder.From, Folder.To, false, ".dwg");
                        }
                        catch (System.Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show("Failed creating Directory: " + Folder.To + " - " + e.Message);
                        }
                    }
                }

            }
            Directory.CreateDirectory(ProjectInfo.TomIFC.To);
        }

        [CommandMethod("ActiveDrawing")]
        public void ActiveDrawing()
        {
            var t = Application.DocumentManager.MdiActiveDocument.Name;

            Application.ShowAlertDialog(t);
        }
    }
}
