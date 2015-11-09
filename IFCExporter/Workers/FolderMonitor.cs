using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFCExporter.Workers
{
    public class FolderMonitor
    {
        private List<FolderDate> OldFolderList;
        private List<Discipline> Disciplines;

        public FolderMonitor(List<Discipline> disciplines)
        {
            Disciplines = disciplines;
        }

        private List<FolderDate> MonitorFolders()
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

        private FolderDate CheckIfFolderIsUpdated(List<FolderDate> NewFolderList)
        {
            foreach (var oldFolder in OldFolderList)
            {
                foreach (var newFolder in NewFolderList)
                {
                    if (newFolder.Path == oldFolder.Path)
                    {
                        if (newFolder.LastUpdatet != oldFolder.LastUpdatet)
                        {
                            return newFolder;
                        }
                    }
                }
            }
            return new FolderDate();
        }

        public FolderDate StartMonitoring()
        {
            OldFolderList = MonitorFolders();
            FolderDate CheckFolderResult = new FolderDate();
            while (true)
            {
                var newList = MonitorFolders();
                CheckFolderResult = CheckIfFolderIsUpdated(newList);

                if (!string.IsNullOrEmpty(CheckFolderResult.Path))
                {
                    break;
                }
            }
            MessageBox.Show("Folder \"" + CheckFolderResult.Path + "\" was updated at: " + CheckFolderResult.LastUpdatet.ToShortDateString());
            return CheckFolderResult;
        }

    }
}
