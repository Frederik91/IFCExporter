using IFCExporter.Helpers;
using IFCExporter.Models;
using IFCExporter.Workers;
using IFCExporterAPI.Assets;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IFCManager.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private bool m_isSettingsOpen;
        private ICommand m_openSettings;
        private ICommand m_fileExplorerCommand;
        private XmlViewModel m_xmlViewModel;
        private ICommand m_newProjectCommand;


        public XmlViewModel XmlViewModel { get { return m_xmlViewModel; } set { m_xmlViewModel = value; OnPropertyChanged("XmlViewModel"); } }


        public ICommand NewProjectCommand
        {
            get { return m_newProjectCommand; }
            set
            {
                m_newProjectCommand = value;
                OnPropertyChanged("NewProjectCommand");
            }
        }

        public ICommand OpenSettings
        {
            get { return m_openSettings; }
            set
            {
                m_openSettings = value;
                OnPropertyChanged("OpenSettings");
            }
        }

        public ICommand FileExplorerCommand
        {
            get { return m_fileExplorerCommand; }
            set { m_fileExplorerCommand = value; OnPropertyChanged("FileExplorerCommand"); }
        }

        public bool IsSettingsOpen
        {
            get { return m_isSettingsOpen; }
            set
            {
                m_isSettingsOpen = value;
                OnPropertyChanged("IsSettingsOpen");
            }
        }

        public MainViewModel()
        {
            NewProjectCommand = new DelegateCommand(AddNewProject);
            m_openSettings = new DelegateCommand(flip);
            FileExplorerCommand = new DelegateCommand(OpenExplorerExecute);
            XmlViewModel = new XmlViewModel();
        }

        private void flip()
        {
            IsSettingsOpen = !IsSettingsOpen;
        }

        private void OpenExplorerExecute()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.ShowDialog();

            if (fileDialog.CheckFileExists)
            {
                var reader = new XmlReader();
                DataStorage.ProjectInfo = reader.GetprojectInfo(fileDialog.FileName);
                var FDC = new FileDateComparer();
                DataStorage.OldFolderDateList = FDC.GetNewFolderDateList();
                XmlViewModel.FolderMonitorViewModels[XmlViewModel.SelectedTabIndex].StartMonitoring();
                IsSettingsOpen = false;      
            }
        }


        private void AddNewProject()
        {
            var newProject = new FolderMonitorViewModel { ProjectName = "New Project" };

            XmlViewModel.FolderMonitorViewModels.Add(newProject);

            if (XmlViewModel.SelectedTabIndex > 0)
            {
                XmlViewModel.SelectedTabIndex = XmlViewModel.FolderMonitorViewModels.Count - 1;
            }
        }



    }
}
