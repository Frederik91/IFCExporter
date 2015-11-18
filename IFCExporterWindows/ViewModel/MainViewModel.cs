using IFCExporterAPI.Assets;
using IFCExporterAPI.Models;
using IFCExporterWindows.Models;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IFCExporterWindows.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string m_selectedProjectPath;
        private string m_toggleSelectedExportsText = "Select all";
        private ICommand m_toggleSelectedExportsCommand;
        private ICommand m_fileExplorerCommand;
        private ICommand m_startCommand;
        private bool m_projectSelected = false;
        public IfcProjectInfo ProjectInfo { get; set; }
        private List<SelectedExport> m_aviliableExports = new List<SelectedExport>();
        private bool m_automaticMode;
        private MainWindow MainWindow;
        public List<string> ExportsToRun = new List<string>();

        public string SelectedProjectPath
        {
            get { return m_selectedProjectPath; }
            set { m_selectedProjectPath = value; OnPropertyChanged("SelectedProjectpath"); }
        }

        public bool AutomaticMode
        {
            get { return m_automaticMode; }
            set { m_automaticMode = value; OnPropertyChanged("AutomaticMode"); OnPropertyChanged("ManualMode"); }
        }

        public bool ManualMode
        {
            get { return !m_automaticMode; }
            set { m_automaticMode = !value; OnPropertyChanged("ManualMode"); OnPropertyChanged("AutomaticMode"); }
        }

        public bool ProjectSelected
        {
            get { return m_projectSelected; }
            set { m_projectSelected = value; OnPropertyChanged("ProjectSelected"); }
        }

        public List<SelectedExport> AviliableExports
        {
            get { return m_aviliableExports; }
            set
            {
                m_aviliableExports = value;
                OnPropertyChanged("AviliableExports");
            }
        }

        public ICommand StartCommand
        {
            get { return m_startCommand; }
            set { m_startCommand = value; OnPropertyChanged("StartCommand"); }
        }

        public ICommand ToggleSelectedExportsCommand
        {
            get { return m_toggleSelectedExportsCommand; }
            set { m_toggleSelectedExportsCommand = value; OnPropertyChanged("ToggleSelectedExportsCommand"); }
        }
        
        public string ToggleSelectedExportsText
        {
            get { return m_toggleSelectedExportsText; }
            set { m_toggleSelectedExportsText = value; OnPropertyChanged("ToggleSelectedExportsText"); }
        }


        public ICommand FileExplorerCommand
        {
            get { return m_fileExplorerCommand; }
            set { m_fileExplorerCommand = value; OnPropertyChanged("FileExplorerCommand"); }
        }

        public MainViewModel(MainWindow _mainWindow)
        {
            MainWindow = _mainWindow;
            FileExplorerCommand = new DelegateCommand(OpenExplorerExecute);
            ToggleSelectedExportsCommand = new DelegateCommand(ToggleSelectedExports);
            StartCommand = new DelegateCommand(StartExport);
        }

        private void StartExport()
        {
            foreach (var Export in AviliableExports)
            {
                if (Export.IsSelected)
                {
                    ExportsToRun.Add(Export.Export);
                }                
            }

            MainWindow.Close();
        }

        private void ToggleSelectedExports()
        {
            var Selected = AviliableExports.FindAll(x => x.IsSelected == true);
            var list = new List<SelectedExport>();
            if (Selected == null || Selected.Count != AviliableExports.Count)
            {
                foreach (var Export in AviliableExports)
                {
                    list.Add(new SelectedExport { Export = Export.Export, IsSelected = true });
                }
                ToggleSelectedExportsText = "Unselect all";
            }
            else
            {
                foreach (var Export in AviliableExports)
                {
                    list.Add(new SelectedExport { Export = Export.Export, IsSelected = false });
                }
                ToggleSelectedExportsText = "Select all";
            }

            AviliableExports = list;
        }


        private void OpenExplorerExecute()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "XML-files (*.xml)|*.xml";
            fileDialog.ShowDialog();

            if (fileDialog.CheckFileExists && fileDialog.FileName != string.Empty)
            {
                var reader = new XmlReader();
                ProjectInfo = reader.GetprojectInfo(fileDialog.FileName);
                CreateExportList();
                ProjectSelected = true;
                SelectedProjectPath = fileDialog.FileName;
            }
        }

        private void CreateExportList()
        {
            try
            {
                var list = new List<SelectedExport>();
                foreach (var Discipline in ProjectInfo.Disciplines)
                {

                    foreach (var Export in Discipline.Exports)
                    {
                        list.Add(new SelectedExport { Export = Export.Name, IsSelected = false });
                    }
                }
                AviliableExports = list;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Unable to read XML-file");
                throw;
            }
        }
    }
}
