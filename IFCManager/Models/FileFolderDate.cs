using IFCExporterAPI.Models;
using IFCMonitor.Assets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCManager.Models
{
    public class FileFolderDate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Export { get; set; }
        public string FileName { get; set; }
        public IfcProjectInfo Project { get; set; }
        public string IfcName { get; set; }
        public DateTime FolderUpdate { get; set; }
        public DateTime IfcUpdate { get; set; }
        public string Difference
        {
            get { return CalculateDifferance(); }
        }
        public bool Expired { get; set; }
        public UpdateStatus Updated
        {
            get { return m_updated; }
        }
        public string LastSavedBy { get; set; }

        private UpdateStatus m_updated;

        private string CalculateDifferance()
        {
            if (FolderUpdate != null && IfcUpdate != null)
            {
                var diff = FolderUpdate - IfcUpdate;

                if (diff > TimeSpan.Zero)
                {
                    m_updated = UpdateStatus.Expired;
                }
                else
                {
                    m_updated = UpdateStatus.Updated;
                }

                if (diff < TimeSpan.Zero)
                {
                    return "-" + diff.ToString(@"hh\:mm\:ss");
                }
                return diff.ToString(@"hh\:mm\:ss");
                               
            }

            return TimeSpan.Zero.TotalHours.ToString(@"hh\:mm\:ss");
            
        }

    }

    public enum UpdateStatus
    {
        Expired,
        Updated
    }
}
