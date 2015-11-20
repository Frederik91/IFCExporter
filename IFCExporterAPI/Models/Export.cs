using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporterAPI.Models
{
    public class Export
    {
        public string Name { get; set; }
        public List<Folder> Folders { get; set; }
        public string IFC { get; set; }
    }
}
