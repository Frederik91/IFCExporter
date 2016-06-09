using Autodesk.AutoCAD.ApplicationServices.Core;
using IFCExporter.Models;
using IFCExporterAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Workers
{
    public class FileDateComparer
    {
        public FileDateComparer()
        {
        }

        public List<IfcProjectInfo> returnChangedProjectsWithDetail(List<IfcProjectInfo> projectList)
        {
            var newProjectList = new List<IfcProjectInfo>();

            foreach (var project in projectList)
            {
                var newProject = new IfcProjectInfo { BaseFolder = project.BaseFolder, Disciplines = new List<Discipline>(), Files = project.Files, ProjectName = project.ProjectName, TomIFC = project.TomIFC };

                foreach (var dis in project.Disciplines)
                {
                    var discipline = new Discipline { Exports = new List<Export>(), Name = dis.Name, StartFile = dis.StartFile };
                    discipline.Exports = new List<Export>();

                    foreach (var exp in dis.Exports)
                    {
                        var IFCpath = project.TomIFC.Export + "\\" + exp.IFC + ".ifc";
                        var newestFileDate = new DateTime();
                        string newestFileName = "";

                        foreach (var folder in exp.Folders)
                        {
                            var Drawings = Directory.GetFiles(folder.remote, "*.dwg", SearchOption.TopDirectoryOnly);
                            foreach (var drawing in Drawings)
                            {
                                if (File.GetLastWriteTime(drawing) > newestFileDate)
                                {
                                    newestFileDate = File.GetLastWriteTime(drawing);
                                    newestFileName = drawing;
                                }
                            }
                        }

                        var ifcLastWrite = File.GetLastWriteTime(IFCpath);

                        if (ifcLastWrite < newestFileDate)
                        {
                            foreach (var newDis in project.Disciplines)
                            {
                                if (newDis.Name == dis.Name)
                                {
                                    discipline.Exports.Add(exp);
                                }
                            }
                        }
                    }
                    if (discipline.Exports.Count > 0)
                    {
                        newProject.Disciplines.Add(discipline);
                    }
                }
                if (newProject.Disciplines.Count > 0)
                {
                    newProjectList.Add(newProject);
                }
            }
            if (newProjectList.Count > 0)
            {
                return newProjectList;
            }
            else
            {
                return null;
            }
        }

        public bool CheckForChanges(List<string> ComparedList)
        {
            if (ComparedList.Count > 0)
            {
                return true; ;
            }
            return false;
        }

        public List<string> ReturnDwgsInChangedExports()
        {
            var list = new List<string>();

            foreach (var project in DataStorage.ExportsToRun)
            {

                foreach (var Exp in DataStorage.ExportsToRun)
                {
                    foreach (var discipline in project.Disciplines)
                    {
                        foreach (var Export in discipline.Exports)
                        {
                            foreach (var folder in Export.Folders)
                            {
                                foreach (var file in Directory.GetFiles(folder.remote).ToList())
                                {
                                    var _file = Path.GetExtension(file);
                                    if (Path.GetExtension(file) == ".dwg")
                                    {
                                        list.Add(file);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list.Distinct().ToList();
        }
    }
}
