using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Workers
{
    public class FolderMonitor
    {
        private List<FolderDate> MonitorFolders(List<Discipline> Disciplines)
        {
            var FolderDateList = new List<FolderDate>();

            foreach (var Discipline in Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    foreach (var Folder in Export.Folders)
                    {
                        FolderDateList.Add(new FolderDate { Path = Folder.From, LastUpdatet = Directory.GetLastWriteTime(Folder.From), Discipline = Discipline.Name, Export = Export.Name});
                    }                    
                }
            }
            return FolderDateList;
        }

        private void CheckIfFolderIsUpdated(List<FolderDate> NewFolderList, List<FolderDate> OldFolderList)
        {
            foreach (var oldFolder in OldFolderList)
            {
                foreach (var newFolder in NewFolderList)
                {
                    if (newFolder.Path == oldFolder.Path)
                    {

                    }
                }
            }
        }

    }
}
