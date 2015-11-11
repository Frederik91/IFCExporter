﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Models
{
    public class IFCProjectInfo
    {
        public List<Discipline> Disciplines { get; set; }
        public IFCFile TomIFC { get; set; }
        public List<FileInfo> Files { get; set; }
        public FileInfo BaseFolder { get; set; }
        public string ProjectName { get; set; }

    }
}
