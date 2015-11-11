using Autodesk.AutoCAD.Interop;
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
        public static IFCProjectInfo ProjectInfo { get; set; }
        public static List<string> ExportsToRun { get; set; }
        public static bool ExportInProgress { get; set; }
        public static List<string> TempExportsToRun { get; set; }
        public static List<FolderDate> OldFolderDateList { get; set; }
        public static List<string> FilesWithChanges { get; set; }
        public static List<string> AllFiles { get; set; }
        public static AcadApplication app { get; set; }
    }
}
