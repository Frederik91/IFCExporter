using Autodesk.AutoCAD.Interop;
using IFCExporterAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFCExporter.Models
{
    public static class DataStorage
    {
        public static List<IfcProjectInfo> ProjectInfo { get; set; }
        public static List<IfcProjectInfo> ExportsToRun { get; set; }
        public static List<string> ProjectsToRun { get; set; }
        public static List<ProjectChanges> SelectedExports { get; set; }
        public static List<ProjectChanges> ProjectChanges { get; set; }
        public static List<string> AllFiles { get; set; }
        public static AcadApplication app { get; set; }
        public static string logFileLocation { get; set; }
    }
}
