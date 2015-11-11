using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IFCExporter.Helpers
{
    public class XMLReader
    {
        public IFCProjectInfo GetprojectInfo(string path)
        {

            var ProjectInfo = new IFCProjectInfo { Disciplines = new List<Discipline>(), Files = new List<FileInfo>(), TomIFC = new IFCFile(), BaseFolder = new FileInfo() } ;
            XElement xdoc = XElement.Load(path);
            ProjectInfo.ProjectName = xdoc.Attribute("Name").Value;
            var Dicsiplines = xdoc.Elements("Discipline");
            var disciplinecol = new List<Discipline>();
            foreach (var dis in Dicsiplines)
            {
                Discipline _discipline = new Discipline { Exports = new List<Export>() };
                _discipline.Name = dis.Attribute("Value").Value;

                _discipline.StartFile = new FileInfo { From = dis.Element("StartFile").Attribute("From").Value, To = dis.Element("StartFile").Attribute("To").Value };
                var exps = dis.Elements("Export");
                foreach (var exp in exps)
                {
                    var _export = new Export {Folders = new List<Folder>() };
                    _export.Name = exp.Attribute("Value").Value;

                    var folders = exp.Elements("Folder");
                    foreach (var folder in folders)
                    {
                        var From = folder.Attribute("From").Value;
                        var To = folder.Attribute("To").Value;
                        var IFC = folder.Attribute("IFC").Value;

                        var f = new Folder { From = From, To = To, IFC = IFC };
                        _export.Folders.Add(f);
                    }
                    _discipline.Exports.Add(_export);
                }

                ProjectInfo.Disciplines.Add(_discipline);
            }

            var BaseFolder = xdoc.Element("BaseFolder");
            ProjectInfo.BaseFolder.From = BaseFolder.Attribute("From").Value;
            ProjectInfo.BaseFolder.To = BaseFolder.Attribute("To").Value;          
            
               
            var files = xdoc.Element("Files").Elements("File");
            foreach (var file in files)
            {
                ProjectInfo.Files.Add(new FileInfo { From = file.Attribute("From").Value, To = file.Attribute("To").Value });                
            }

            var ifc = xdoc.Element("IFC");

            ProjectInfo.TomIFC.Export = ifc.Attribute("Export").Value;
            ProjectInfo.TomIFC.From = ifc.Attribute("From").Value;
            ProjectInfo.TomIFC.To = ifc.Attribute("To").Value;

            return ProjectInfo;
        }
    }
}
