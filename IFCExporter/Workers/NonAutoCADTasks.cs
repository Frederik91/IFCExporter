using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using IFCExporter.Helpers;
using IFCExporter.Models;
using IFCExporterAPI.Assets;
using IFCExporterAPI.Models;
using IFCExporterWindows.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IFCExporter.Workers
{
    public class NonAutoCADTasks
    {
        private DrawingManager DM = new DrawingManager();
        private Copier CP = new Copier();
        public bool ContinuousMode;
        public List<string> ExportsToExecute;
        private string XMLFolder;
        public bool AutomaticMode;

        public List<IfcProjectInfo> Prepare()
        {
            //Finn prosjektmappe

            //Velg prosjekt

            var Projects = new List<IfcProjectInfo>();

            var window = new IFCExporterWindows.MainWindow();
            window.ShowDialog();

            var x = window.MainViewModel;

            ExportsToExecute = x.ExportsToRun;
            XMLFolder = x.SelectedProjectPath;
            AutomaticMode = x.AutomaticMode;
            ContinuousMode = x.ContinuousMode;
            Projects = x.ProjectInfo;

            if (Projects == null || Projects[0] == null)
            {
                System.Windows.Forms.MessageBox.Show("No project file selected, exiting.");
                return null;
            }

            try
            {
                prepareFirstTime(Projects);

            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Failed first time setup: " + e.Message);
                return null;
            }


            return Projects;
        }

        public List<IfcProjectInfo> Prepare_NoDialog()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            var Projects = new List<IfcProjectInfo>();

            PromptResult _XMLFolder = ed.GetString("\nEnter XML filepath");
            if (_XMLFolder.Status != PromptStatus.OK)
            {
                ed.WriteMessage("No string was provided\n");
                return null;
            }

            XMLFolder = _XMLFolder.StringResult;
            AutomaticMode = true;
            ContinuousMode = false;

            if (!File.Exists(XMLFolder))
            {
                System.Windows.Forms.MessageBox.Show("No project file selected, exiting.");
                return null;
            }


            //Les inn XMLfil
            var reader = new XmlReader();
            var ProjectInfo = reader.GetprojectInfo(XMLFolder);

            try
            {
                prepareFirstTimeAuto(ProjectInfo);

            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Failed first time setup: " + e.Message);
                return null;
            }

            Projects.Add(ProjectInfo);

            return Projects;
        }

        public void prepareFirstTime(List<IfcProjectInfo> ProjectInfo)
        {
            foreach (var project in ProjectInfo)
            {
                foreach (var Discipline in project.Disciplines)
                {
                    foreach (var Export in Discipline.Exports)
                    {
                        foreach (var Folder in Export.Folders)
                        {
                            try
                            {
                                DM.CloseIfOpen(Folder.remote);
                                CP.DirectoryCopyWithoutOverwrite(Folder.remote, Folder.local, false, ".dwg");
                            }
                            catch (System.Exception e)
                            {
                                System.Windows.Forms.MessageBox.Show("Failed creating Directory: " + Folder.local + " - " + e.Message);
                            }
                        }
                    }
                }
                Directory.CreateDirectory(Path.GetDirectoryName(project.TomIFC.To));
            }
        }

        public void prepareFirstTimeAuto(IfcProjectInfo ProjectInfo)
        {
            var allExportsList = new List<string>();

            foreach (var Discipline in ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    allExportsList.Add(Export.Name);
                    foreach (var Folder in Export.Folders)
                    {
                        try
                        {
                            DM.CloseIfOpen(Folder.remote);
                            CP.DirectoryCopyWithoutOverwrite(Folder.remote, Folder.local, false, ".dwg");
                        }
                        catch (System.Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show("Failed creating Directory: " + Folder.local + " - " + e.Message);
                        }
                    }
                }

            }
            Directory.CreateDirectory(Path.GetDirectoryName(ProjectInfo.TomIFC.To));
            ExportsToExecute = allExportsList.Distinct().ToList();
        }
    }
}
