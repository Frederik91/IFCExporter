using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporterAPI.Models
{
    public class IfcProjectInfo
    {
        public List<Discipline> Disciplines { get; set; }
        public IFCFile TomIFC { get; set; }
        public List<FileInfo> Files { get; set; }
        public FileInfo BaseFolder { get; set; }
        public string ProjectName { get; set; }
    }
}
