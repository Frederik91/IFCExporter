using Autodesk.AutoCAD.ApplicationServices.Core;
using IFCExporter.Models;
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
        private IFCProjectInfo ProjectInfo = DataStorage.ProjectInfo;

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
                            Export = Export.Name,
                            LastUpdated = MostRecent,
                            Files = FileDateList
                        });

                    }
                }

            }
            return NewFolderDateList;
        }

        public List<FileDate> GetNewIfcFileDateList()
        {
            var NewIFCFileDateList = new List<FileDate>();
            var dir = Path.GetDirectoryName(ProjectInfo.TomIFC.To);


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
                    var f = new System.IO.FileInfo(ifcFile.Path);
                    if (f.Length < 10240)
                    {
                        break;
                    }

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

        public List<string> ReturnChangedFiles(List<FolderDate> OldFolderDateList, List<FolderDate> NewfolderDateList)
        {
            var changedFileList = new List<string>();

            foreach (var oldFolder in OldFolderDateList)
            {
                foreach (var newFolder in NewfolderDateList)
                {
                    foreach (var oldFile in oldFolder.Files)
                    {
                        foreach (var newFile in newFolder.Files)
                        {
                            if (oldFile.Path == newFile.Path && newFile.EditDate > oldFile.EditDate)
                            {
                                changedFileList.Add(newFile.Path);
                            }
                        }
                    }
                }
            }

            return changedFileList;
        }

    }
}
