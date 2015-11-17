using IFCExporter.Workers;
using IFCManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IFCManager.Assets
{
    public class Monitor
    {
        private XmlViewModel XmlViewModel;
        private System.Timers.Timer aTimer = new System.Timers.Timer();

        public Monitor(XmlViewModel _xmlViewModel)
        {
            XmlViewModel = _xmlViewModel;
        }

        public void StartMonitoring()
        {          
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (!XmlViewModel.MonitorRunning)
            {
                aTimer.Enabled = false;
                return;
            }

            CheckChangesIFC();
        }


        private void CheckChangesIFC()
        {
            var FDC = new FileDateComparer();
            var Conv = new ConvertToFileFolderDate();
            XmlViewModel.FileFolderLastUpdatedList = Conv.Convert(FDC.GetNewFolderDateList(), FDC.GetNewIfcFileDateList());
        }

    }
}
