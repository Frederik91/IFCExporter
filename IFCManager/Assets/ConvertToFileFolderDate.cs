using IFCExporter.Models;
using IFCManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCManager.Assets
{
    public class ConvertToFileFolderDate
    {
        public List<FileFolderDate> Convert(List<FolderDate> FolderList, List<FileDate> FileList)
        {
            var FileFolderList = new List<FileFolderDate>();

            foreach (var discipline in DataStorage.ProjectInfo.Disciplines)
            {
                foreach (var export in discipline.Exports)
                {
                    FileFolderList.Add(new FileFolderDate { Export = export.Name, FileName = export.IFC });
                }
            }

            foreach (var fileFolder in FileFolderList)
            {
                if (FileList.Exists(x => Path.GetFileNameWithoutExtension(x.Path) == fileFolder.FileName))
                {
                    fileFolder.IfcUpdate = FileList.Find(x => Path.GetFileNameWithoutExtension(x.Path) == fileFolder.FileName).EditDate;
                }


                if (FolderList.Exists(x => Path.GetFileNameWithoutExtension(x.Export) == fileFolder.Export))
                {
                    fileFolder.FolderUpdate = FolderList.Find(x => Path.GetFileNameWithoutExtension(x.Export) == fileFolder.Export).LastUpdated;
                }
            }



            //foreach (var discipline in DataStorage.ProjectInfo.Disciplines)
            //{
            //    foreach (var export in discipline.Exports)
            //    {
            //        foreach (var folder in export.Folders)
            //        {
            //            foreach (var _folder in FolderList)
            //            {
            //                if (_folder.Export == export.Name)
            //                {
            //                    foreach (var file in FileList)
            //                    {
            //                        if (Path.GetFileNameWithoutExtension(file.Path) == export.IFC)
            //                        {
            //                            FileFolderList.Add(new FileFolderDate { Export = export.Name, FileName = export.IFC, FolderUpdate = _folder.LastUpdated, IfcUpdate = file.EditDate });
            //                            continue;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}



            return FileFolderList;
        }
    }
}
