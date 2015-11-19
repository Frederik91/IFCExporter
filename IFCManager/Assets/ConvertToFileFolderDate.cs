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

            foreach (var File in FileList)
            {
                foreach (var Folder in FolderList)
                {
                    if (Path.GetFileNameWithoutExtension(File.Path) == Folder.Export)
                    {
                        var inList = false;
                        foreach (var FileFolder in FileFolderList)
                        {
                            if (FileFolder.FileName == Folder.Export)
                            {
                                inList = true;
                                break;
                            }
                        }
                        if (!inList)
                        {
                            FileFolderList.Add(new FileFolderDate { FileName = Path.GetFileNameWithoutExtension(File.Path), Export = Folder.Export, FolderUpdate = Folder.LastUpdated, IfcUpdate = File.EditDate });
                        }
                    }
                }
            }

            return FileFolderList;
        }
    }
}
