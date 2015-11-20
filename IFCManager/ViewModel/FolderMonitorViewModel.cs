﻿using IFCExporter.Models;
using IFCExporter.Workers;
using IFCManager.Assets;
using IFCManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCManager.ViewModel
{
    public class FolderMonitorViewModel : ViewModelBase
    {
        private string m_projectName;
        private int m_selectedTabIndex;
        private List<FileFolderDate> m_fileFolderLastUpdatedList;

        public List<FileFolderDate> FileFolderLastUpdatedList
        {
            get { return m_fileFolderLastUpdatedList; }
            set
            {
                m_fileFolderLastUpdatedList = value;
                OnPropertyChanged("FileFolderLastUpdatedList");
            }
        }

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


        public string ProjectName
        {
            get { return m_projectName; }
            set { m_projectName = value; OnPropertyChanged("ProjectName"); }
        }

        public FolderMonitorViewModel()
        {
        }

        public void StartMonitoring()
        {
            var FDC = new FileDateComparer();
            var Conv = new ConvertToFileFolderDate();
            FileFolderLastUpdatedList = Conv.Convert(FDC.GetNewFolderDateList(), FDC.GetIfcFileDateList(DataStorage.ProjectInfo.TomIFC.Export));
            var monitor = new Monitor(this);
            ProjectName = DataStorage.ProjectInfo.ProjectName;
            monitor.StartMonitoring();
        }
    }
}
