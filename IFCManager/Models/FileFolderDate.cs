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
        public string FileName { get; set; }
        public string Export { get; set; }
        public DateTime FolderUpdate { get; set; }
        public DateTime IfcUpdate { get; set; }
        public string Difference
        {
            get { return CalculateDifferance(); }
        }

        public string Updated
        {
            get { return m_updated; }
        }
        public string LastSavedBy { get; set; }

        private string m_updated;

        private string CalculateDifferance()
        {
            if (FolderUpdate != null && IfcUpdate != null)
            {
                var diff = FolderUpdate - IfcUpdate;

                if (diff > TimeSpan.Zero)
                {
                    m_updated = "Expired";
                }
                else
                {
                    m_updated = "Up to date";
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
}
