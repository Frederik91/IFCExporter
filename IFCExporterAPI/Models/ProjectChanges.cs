using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporterAPI.Models
{
    public class ProjectChanges
    {
        public string Name { get; set; }
        public List<string> FilesWithChanges { get; set; }
    }
}
