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


                if (FolderList.Exists(x => Path.GetFileNameWithoutExtension(x.Export) == fileFolder.FileName))
                {
                    fileFolder.FolderUpdate = FolderList.Find(x => Path.GetFileNameWithoutExtension(x.Export) == fileFolder.FileName).LastUpdated;
                }
            }


            return FileFolderList;
        }
    }
}
