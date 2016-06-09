using Autodesk.AutoCAD.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporterApplication.Models
{
    public class ExportApp
    {
        public AcadApplication app { get; set; }
        public string name { get; set; }
        public string filePath { get; set; }
    }
}
