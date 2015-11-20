using IFCExporter.Helpers;
using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IFCExporter.Workers
{
    public class IfcUpdateWatcher
    {
        private System.Timers.Timer aTimer = new System.Timers.Timer();

        public IfcUpdateWatcher()
        {

        }

        public void StartIfcMonitoring()
        {
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckChangesIFC();
        }


        private void CheckChangesIFC()
        {
            //--Last opp IFC
            aTimer.Enabled = false;

            var FW = new FileDateComparer();
            var CP = new Copier();
            var ExportIfcFileDateList = FW.GetIfcFileDateList(DataStorage.ProjectInfo.TomIFC.Export);

            foreach (var ExportIfcFile in ExportIfcFileDateList)
            {
                var LocalIfcList = DataStorage.LocalIfcFolderDateList;
                foreach (var LocalIfcFile in LocalIfcList)
                {
                    var size = new System.IO.FileInfo(LocalIfcFile.Path).Length;
                    if (size > 1048576 && LocalIfcFile.EditDate > ExportIfcFile.EditDate && Path.GetFileName(LocalIfcFile.Path) == Path.GetFileName(ExportIfcFile.Path))
                    {
                        CP.CopySingleFile(ExportIfcFile.Path, DataStorage.ProjectInfo.TomIFC.Export + "\\" + Path.GetFileName(ExportIfcFile.Path));
                        DataStorage.LocalIfcFolderDateList = ExportIfcFileDateList;
                        break;
                    }
                }
            }
            aTimer.Enabled = true;
        }
    }
}
