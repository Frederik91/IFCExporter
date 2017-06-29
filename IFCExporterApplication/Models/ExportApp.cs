using AutoCAD;

namespace IFCExporterApplication.Models
{
    public class ExportApp
    {
        public AcadApplication App { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
    }
}
