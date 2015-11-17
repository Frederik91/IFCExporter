using IFCExporter.Models;
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
        private List<FolderDate> m_folderLastUpdatedList;
        private MainViewModel MainViewModel;
        private bool monitorRunning = false;
        private ICommand m_monitorToggleButtonCommand;
        private string m_monitorToggleButtonText = "Start monitoring";

        public XmlViewModel()
        {
            TestCommand = new DelegateCommand(TestMethod);
            MonitorToggleButtonCommand = new DelegateCommand(ToggleMonitoring);
        }
        public ICommand TestCommand { get; set; }

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

        public void TestMethod()
        {

        }

        public List<FolderDate> FolderLastUpdatedList
        {
            get { return m_folderLastUpdatedList; }
            set
            {
                m_folderLastUpdatedList = value;
                OnPropertyChanged("FolderLastUpdatedList");
            }
        }

        private async void ToggleMonitoring()
        {
            if (DataStorage.ProjectInfo == null)
            {
                var window = Application.Current.MainWindow as MetroWindow;
                await window.ShowMessageAsync("No project selected", "You need to select a project before you can start monitoring folders", MessageDialogStyle.Affirmative);
            }

            switch (monitorRunning)
            {
                case (true):
                    MonitorToggleButtonText = "Start monitoring";
                    monitorRunning = false;
                    break;
                case (false):
                    MonitorToggleButtonText = "Stop monitoring";
                    monitorRunning = true;
                    break;
                default:
                    break;
            }
        }

    }
}
