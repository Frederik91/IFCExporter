using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Models
{
    public class FolderDate
    {
        public string Export { get; set; }
        public string IfcName { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<FileDate> Files { get; set; }
    }
}
