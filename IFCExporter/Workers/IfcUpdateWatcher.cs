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
        public IfcUpdateWatcher()
        {

        }

        public void StartIfcMonitoring()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 2000;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CheckChangesIFC();
        }


        private void CheckChangesIFC()
        {
            //--Last opp IFC

            var FW = new FileDateComparer();
            var CP = new Copier();
            var NewIfcFileDateList = FW.GetNewIfcFileDateList();

            foreach (var NewIfcFile in NewIfcFileDateList)
            {
                var OldIfcList = DataStorage.IfcOldFolderDateList;
                foreach (var OldIfcFile in OldIfcList)
                {
                    var size = new System.IO.FileInfo(NewIfcFile.Path).Length;
                    if (size > 1048576 && OldIfcFile.EditDate < NewIfcFile.EditDate && OldIfcFile.Path == NewIfcFile.Path)
                    {
                        CP.CopySingleFile(NewIfcFile.Path, DataStorage.ProjectInfo.TomIFC.Export + "\\" + Path.GetFileName(NewIfcFile.Path));
                        DataStorage.IfcOldFolderDateList = NewIfcFileDateList;
                        break;
                    }
                }
            }
        }
    }
}
