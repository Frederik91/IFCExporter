using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Models
{
    public class Export
    {
        public string Name { get; set; }
        public List<Folder> Folders { get; set; }
    }
}
