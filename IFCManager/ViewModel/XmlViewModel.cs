using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IFCManager.ViewModel
{
    public class XmlViewModel : ViewModelBase
    {
        private ICommand m_browseCommand;

        public ICommand BrowseCommand
        {
            get
            {
                return m_browseCommand;
            }
            set
            {
                m_browseCommand = value;
                OnPropertyChanged("BrowseCommand");
            }
        }

        public XmlViewModel()
        {
            m_browseCommand = new DelegateCommand(SelectXml);
        }

        private void SelectXml()
        {
            MessageBox.Show("Hello");
        }
    }
}
