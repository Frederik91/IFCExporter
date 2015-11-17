using IFCExporter.Models;
using IFCExporter.Workers;
using IFCManager.Assets;
using IFCManager.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IFCManager.ViewModel
{
    public class XmlViewModel : ViewModelBase
    {
        private List<FileFolderDate> m_fileFolderLastUpdatedList;
        private bool m_monitorRunning = false;
        private ICommand m_monitorToggleButtonCommand;
        private string m_monitorToggleButtonText = "Start monitoring";

        public XmlViewModel()
        {
            MonitorToggleButtonCommand = new DelegateCommand(ToggleMonitoring);
        }
        public ICommand TestCommand { get; set; }

        public bool MonitorRunning
        {
            get { return m_monitorRunning; }
            set { m_monitorRunning = value; OnPropertyChanged("MonitorRunning"); }
        }

        public ICommand MonitorToggleButtonCommand
        {
            get { return m_monitorToggleButtonCommand; }
            set { m_monitorToggleButtonCommand = value; OnPropertyChanged("MonitorToggleButtonCommand"); }
        }

        public string MonitorToggleButtonText
        {
            get { return m_monitorToggleButtonText; }
            set { m_monitorToggleButtonText = value; OnPropertyChanged("MonitorToggleButtonText"); }
        }

        public List<FileFolderDate> FileFolderLastUpdatedList
        {
            get { return m_fileFolderLastUpdatedList; }
            set
            {
                m_fileFolderLastUpdatedList = value;
                OnPropertyChanged("FileFolderLastUpdatedList");
            }
        }

        private async void ToggleMonitoring()
        {
            if (DataStorage.ProjectInfo == null)
            {
                var window = Application.Current.MainWindow as MetroWindow;
                await window.ShowMessageAsync("No project selected", "You need to select a project before you can start monitoring folders", MessageDialogStyle.Affirmative);
                return;
            }

            switch (MonitorRunning)
            {
                case (true):
                    MonitorToggleButtonText = "Start monitoring";
                    MonitorRunning = false;
                    break;
                case (false):
                    MonitorToggleButtonText = "Stop monitoring";
                    var FDC = new FileDateComparer();
                    var Conv = new ConvertToFileFolderDate();
                    FileFolderLastUpdatedList = Conv.Convert(FDC.GetNewFolderDateList(), FDC.GetNewIfcFileDateList());
                    var monitor = new Monitor(this);
                    monitor.StartMonitoring();
                    MonitorRunning = true;
                    break;
                default:
                    break;
            }
        }

    }
}
