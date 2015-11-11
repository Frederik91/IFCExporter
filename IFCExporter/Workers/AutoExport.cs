using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Workers
{
    public class AutoExport
    {
        private IFCProjectInfo ProjectInfo;

        public AutoExport(IFCProjectInfo _projectInfo)
        {
            ProjectInfo = _projectInfo;
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


        public bool CompareFolderLists_ReturnTrueIfChanged(List<FolderDate> NewFolderDateList)
        {
            var ChangesFound = false;

            var exportsToRun = new List<string>();

            DataStorage.ExportsToRun.Clear();
            foreach (var newFolder in NewFolderDateList)
            {
                foreach (var oldFolder in DataStorage.OldFolderDateList)
                {
                    if (oldFolder.Export == newFolder.Export && oldFolder.LastUpdated < newFolder.LastUpdated)
                    {
                        exportsToRun.Add(newFolder.Export);
                        ChangesFound = true;
                    }
                }
            }

            if (ChangesFound)
            {
                DataStorage.ExportsToRun = exportsToRun;
            }
            return ChangesFound;
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
