﻿using System;
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
        public TimeSpan Difference
        {
            get { return CalculateDifferance(); }
        }

        private TimeSpan CalculateDifferance()
        {
            if (FolderUpdate != null && IfcUpdate != null)
            {
                var diff = FolderUpdate - IfcUpdate;

                diff.ToString(@"hh\:mm");

                return diff;
            }
            return TimeSpan.Zero;
            
        }

    }
}
