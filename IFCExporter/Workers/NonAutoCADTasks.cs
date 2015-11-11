using IFCExporter.Forms;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Workers
{
    public class NonAutoCADTasks
    {
        private DrawingManager DM = new DrawingManager();
        private Copier CP = new Copier();
        public bool RunForeverBool;
        public List<string> ExportsToExecute;
        private string XMLFolder;
        public bool AutomaticMode;

        public IFCProjectInfo Prepare()
        {
            //Finn prosjektmappe

            //Velg prosjekt

            using (var form = new SelectProjectForm())
            {
                var result = form.ShowDialog();
                RunForeverBool = form.RunForeverBool;
                ExportsToExecute = form.ExportsToRun;
                XMLFolder = form.XMLPath;
                AutomaticMode = form.AutomaticMode;
                if (form.SelectedProject == "")
                {
                    System.Windows.Forms.MessageBox.Show("No project file selected, exiting.");
                }
            }


            //Les inn XMLfil
            var reader = new XMLReader();
            var ProjectInfo = reader.GetprojectInfo(XMLFolder);

            try
            {
                prepareFirstTime(ProjectInfo);

            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Failed first time setup: " + e.Message);
            }
            return ProjectInfo;
        }

        public void prepareFirstTime(IFCProjectInfo ProjectInfo)
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
    }
}
