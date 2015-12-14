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
        private IfcProjectInfo ProjectInfo = DataStorage.ProjectInfo;

        public FileDateComparer()
        {
        }

        public List<FolderDate> GetNewFolderDateList()
        {
            var NewFolderDateList = new List<FolderDate>();

            foreach (var Discipline in ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    foreach (var Folder in Export.Folders)
                    {
                        DateTime MostRecent = DateTime.MinValue;

                        var _files = Directory.GetFiles(Folder.From, "*.dwg");
                        var FileDateList = new List<FileDate>();

                        foreach (var file in _files)
                        {
                            var date = System.IO.File.GetLastWriteTime(file);

                            FileDateList.Add(new FileDate { Path = file, EditDate = date });

                            if (date > MostRecent)
                            {
                                MostRecent = date;
                            }
                        }

                        NewFolderDateList.Add(new FolderDate
                        {
                            Export = Export.IFC,
                            LastUpdated = MostRecent,
                            Files = FileDateList
                        });

                    }
                }

            }


            return NewFolderDateList;
        }

        public List<FileDate> GetIfcFileDateList(string dir)
        {
            var NewIFCFileDateList = new List<FileDate>();



            DateTime MostRecent = DateTime.MinValue;

            var _files = Directory.GetFiles(dir, "*.ifc");
            var Date = new List<FileDate>();

            foreach (var file in _files)
            {
                NewIFCFileDateList.Add(new FileDate
                {
                    Path = file,
                    EditDate = File.GetLastWriteTime(file)
                });
            }

            return NewIFCFileDateList;
        }

        public List<string> CompareFolderLists(List<FolderDate> NewFolderDateList, List<FolderDate> OldFolderDateList)
        {
            var exportsToRun = new List<string>();

            foreach (var newFolder in NewFolderDateList)
            {
                foreach (var oldFolder in OldFolderDateList)
                {
                    if (oldFolder.Export == newFolder.Export && oldFolder.LastUpdated < newFolder.LastUpdated)
                    {
                        exportsToRun.Add(newFolder.Export);
                    }
                }
            }
            return exportsToRun;
        }

        public List<string> CompareFolderIfcDateLists(List<FolderDate> NewFolderDateList, List<FileDate> IfcFileDateList)
        {
            var exportsToRun = new List<string>();

            foreach (var newFolder in NewFolderDateList)
            {
                foreach (var ifcFile in IfcFileDateList)
                {
                    if (Path.GetFileNameWithoutExtension(ifcFile.Path) == newFolder.Export && ifcFile.EditDate < newFolder.LastUpdated)
                    {
                        exportsToRun.Add(newFolder.Export);
                    }
                }
            }
            return exportsToRun;
        }


        public bool CheckForChanges(List<string> ComparedList)
        {
            if (ComparedList.Count > 0)
            {
                return true; ;
            }
            return false;
        }

        public List<string> ReturnChangedFiles(List<FolderDate> FolderDateList)
        {
            var changedFileList = new List<string>();

            List<FileDate> ifcFileDate = new List<FileDate>();

            foreach (var exp in DataStorage.ExportsToRun)
            {
                foreach (var ifcFilePath in Directory.GetFiles(DataStorage.ProjectInfo.TomIFC.Export, "*.ifc"))
                {
                    var ifcFile = new System.IO.FileInfo(ifcFilePath);

                    foreach (var discipline in DataStorage.ProjectInfo.Disciplines)
                    {
                        foreach (var export in discipline.Exports)
                        {
                            var expName = Path.GetFileNameWithoutExtension(ifcFile.FullName);

                            if (export.IFC == expName && exp == expName)
                            {
                                foreach (var folder in export.Folders)
                                {
                                    ifcFileDate.Add(new FileDate { Path = folder.From, EditDate = ifcFile.LastWriteTime });
                                }
                            }
                        }
                    }

                }
            }

            foreach (var Folder in FolderDateList)
            {
                foreach (var File in Folder.Files)
                {
                    foreach (var ifcFile in ifcFileDate)
                    {
                        if (ifcFile.Path == Path.GetDirectoryName(File.Path) && ifcFile.EditDate < File.EditDate)
                        {
                            changedFileList.Add(File.Path);
                        }
                    }
                }

            }

            return changedFileList;
        }

    }
}
