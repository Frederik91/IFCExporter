using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporterXmlManager.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public MainViewModel()
        {
            CurrentViewModel = new FirstScreenViewModel();
        }

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                this.OnPropertyChanged("CurrentViewModel");
            }
        }
    }
}
