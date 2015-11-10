using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Models
{
    public static class DataStorage
    {
        public static IFCProjectInfo ProjectInfo { get; set; }
        public static string ExportToRun { get; set; }
    }
}
