﻿using IFCExporter.Models;
using IFCExporter.Workers;
using IFCManager.Assets;
using IFCManager.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public MainViewModel MainViewModel;
        private bool m_monitorRunning = false;
        private ObservableCollection<FolderMonitorViewModel> m_folderMonitorViewModels;
        private int m_selectedTabIndex;

        public ObservableCollection<FolderMonitorViewModel> FolderMonitorViewModels { get { return m_folderMonitorViewModels; } set { m_folderMonitorViewModels = value; OnPropertyChanged("FolderMonitorViewModels"); } }



        public int SelectedTabIndex
        {
            get
            {
                return m_selectedTabIndex;
            }
            set
            {

                m_selectedTabIndex = value;
                OnPropertyChanged("SelectedTabIndex");
            }
        }

        public XmlViewModel(MainViewModel _mainViewModel)
        {
            MainViewModel = _mainViewModel;
            FolderMonitorViewModels = InitialSetupXmlViewModel();
        }
        public ICommand TestCommand { get; set; }

        public bool MonitorRunning
        {
            get { return m_monitorRunning; }
            set { m_monitorRunning = value; OnPropertyChanged("MonitorRunning"); }
        }


        private ObservableCollection<FolderMonitorViewModel> InitialSetupXmlViewModel()
        {
            ObservableCollection<FolderMonitorViewModel> xmlVms = new ObservableCollection<FolderMonitorViewModel>
            {
                new FolderMonitorViewModel(MainViewModel)
                {
                    ProjectName = "No project selected"
                }
            };

            return xmlVms;
        }
    }
}
