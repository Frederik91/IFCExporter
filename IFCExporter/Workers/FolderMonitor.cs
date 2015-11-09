using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace IFCExporter.Workers
{
    public class FolderMonitor
    {
        public System.IO.FileSystemWatcher m_Watcher;
        public bool m_bDirty = false;
        private MainClass MC;

        public void Watcher(MainClass _MC)
        {
            MC = _MC;

            m_Watcher = new System.IO.FileSystemWatcher();
         
            // m_Watcher.Filter = txtFile.Text.Substring(txtFile.Text.LastIndexOf('\\') + 1);
            m_Watcher.Path = @"C:\TestMappe\Drawings";
            m_Watcher.IncludeSubdirectories = true;

            m_Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            m_Watcher.Changed += new FileSystemEventHandler(OnChanged);
            m_Watcher.Created += new FileSystemEventHandler(OnChanged);
            m_Watcher.Deleted += new FileSystemEventHandler(OnChanged);
            m_Watcher.Renamed += new RenamedEventHandler(OnRenamed);
            m_Watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!m_bDirty)
            {
                var dir = Path.GetDirectoryName(e.FullPath);

                MC.Execute(LocateDrawingExport(dir, MC.ProjectInfo.Disciplines));
                m_bDirty = true;
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!m_bDirty)
            {
              

                m_bDirty = true;
            }
        }


        //private List<FolderDate> OldFolderList;
        //private List<Discipline> Disciplines;


        //public FolderMonitor(List<Discipline> disciplines)
        //{
        //    Disciplines = disciplines;
        //}

        public string LocateDrawingExport(string FolderPath, List<Discipline> Disciplines)
        {
            var FolderDateList = new List<FolderDate>();

            foreach (var Discipline in Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    foreach (var Folder in Export.Folders)
                    {
                        if (FolderPath == Folder.From)
                        {
                            return Export.Name;
                        }
                    }
                }
            }
            return "";
        }

        //private FolderDate CheckIfFolderIsUpdated(List<FolderDate> NewFolderList)
        //{
        //    foreach (var oldFolder in OldFolderList)
        //    {
        //        foreach (var newFolder in NewFolderList)
        //        {
        //            if (newFolder.Path == oldFolder.Path)
        //            {
        //                foreach (var oldFile in oldFolder.Files)
        //                {
        //                    foreach (var newFile in newFolder.Files)
        //                    {
        //                        if (newFile.Path == oldFile.Path)
        //                        {
        //                            if (newFile.LastUpdated != oldFile.LastUpdated)
        //                            {
        //                                return newFolder;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return new FolderDate();
        //}

        //public async Task<FolderDate> StartMonitoring()
        //{         

        //    OldFolderList = MonitorFolders();
        //    FolderDate CheckFolderResult = new FolderDate();
        //    while (true)
        //    {
        //        var newList = MonitorFolders();
        //        CheckFolderResult = CheckIfFolderIsUpdated(newList);

        //        if (!string.IsNullOrEmpty(CheckFolderResult.Path))
        //        {
        //            break;
        //        }
        //        //System.Threading.Thread.Sleep(1000);
        //    }
        //    return CheckFolderResult;
        //}

    }
}
