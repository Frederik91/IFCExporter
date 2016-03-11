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

        public IfcProjectInfo Prepare()
        {
            //Finn prosjektmappe

            //Velg prosjekt

            var window = new IFCExporterWindows.MainWindow();
            window.ShowDialog();

            var x = window.MainViewModel;


            ExportsToExecute = x.ExportsToRun;
            XMLFolder = x.SelectedProjectPath;
            AutomaticMode = x.AutomaticMode;
            ContinuousMode = x.ContinuousMode;

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
                prepareFirstTime(ProjectInfo);

            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Failed first time setup: " + e.Message);
                return null;
            }
            return ProjectInfo;
        }

        public void prepareFirstTime(IfcProjectInfo ProjectInfo)
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
                            CP.DirectoryCopyWithoutOverwrite(Folder.From, Folder.To, false, ".dwg");
                        }
                        catch (System.Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show("Failed creating Directory: " + Folder.To + " - " + e.Message);
                        }
                    }
                }

            }
            Directory.CreateDirectory(Path.GetDirectoryName(ProjectInfo.TomIFC.To));
        }
    }
}
