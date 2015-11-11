using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Models
{
    public class Discipline
    {
        public string Name { get; set; }
        public List<Export> Exports { get; set; }
        public FileInfo StartFile { get; set; }
    }
}
