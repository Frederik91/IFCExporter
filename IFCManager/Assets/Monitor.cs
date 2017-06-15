using IFCExporter.Models;
using IFCExporter.Workers;
using IFCManager.Models;
using IFCManager.ViewModel;
using IFCMonitor.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IFCManager.Assets
{
    public class Monitor
    {
        private FolderMonitorViewModel FolderMonitorViewModel;
        private System.Timers.Timer aTimer = new System.Timers.Timer();
        private static System.Timers.Timer unwantedWindowTimer = new System.Timers.Timer();

        public Monitor(FolderMonitorViewModel _folderMonitorViewModel)
        {
            FolderMonitorViewModel = _folderMonitorViewModel;
        }

        public void StartMonitoring()
        {          
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 3000;
            aTimer.Enabled = true;

            unwantedWindowTimer.Elapsed += new ElapsedEventHandler(CloseUnwantedWindows);
            unwantedWindowTimer.Interval = 100;
            unwantedWindowTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;
            CheckChangesIFC();
            aTimer.Enabled = true;
        }


        private void CheckChangesIFC()
        {
            var FDC = new FileDateComparer();
            var Conv = new ConvertToFileFolderDate();
            var newList = new List<FileFolderDate>();
            foreach (var project in DataStorage.ProjectInfo)
            {
                var list = Conv.ReturnNewDateList(DataStorage.ProjectInfo);

                foreach (var item in list)
                {
                    newList.Add(item);
                }
            }
            foreach (var currentFileFolder in FolderMonitorViewModel.FileFolderLastUpdatedList)
            {
                var newFileFolder = newList.FirstOrDefault(x => x.Export == currentFileFolder.Export);
                if (newFileFolder is FileFolderDate && !newFileFolder.Equals(currentFileFolder))
                {
                    FolderMonitorViewModel.FileFolderLastUpdatedList = newList;
                }
            }

            var expiredExports = newList.Where(x => x.Difference != null).Where(x => x.Updated == UpdateStatus.Expired).Select(x => x);

            if (expiredExports.Count() > 0)
            {                
                foreach (var export in expiredExports)
                {
                    Exporter.Export(export, FolderMonitorViewModel.MainViewModel.XmlPath);
                }
            }
        }

        private static void CloseUnwantedWindows(object source, ElapsedEventArgs e)
        {
            var unwantedWindowTitlesContains = new List<string>
            {
                "MagiCAD-E - Project Management",
                "MagiCAD V&P - Select project",
                "MagiCAD-E - Select Project",
                "MagiCAD V&P - Project Management",
                "MagiCAD - Project Wizard"
            };

            var unwantedWindowTitlesEquals = new List<string>
            {
                "MagiCAD-E"
            };

            var windows = OpenWindowCollector.GetOpenWindows().Where(x => unwantedWindowTitlesContains.Any(y => x.Value.Contains(y)) || unwantedWindowTitlesEquals.Any(y => x.Value == y));
            foreach (var window in windows)
            {
                CloseWindow(window.Key);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        static void CloseWindow(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
