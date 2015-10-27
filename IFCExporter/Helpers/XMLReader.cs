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
        public IFCProjectInfo GetprojectInfo(string _XMLPath)
        {
            XDocument xDoc = XDocument.Load(_XMLPath);

            var ProjectInfo = new IFCProjectInfo
            {
                Files = new List<CopyDetails>(),
                Folders = new List<CopyDetails>(),
                StartFiles = new List<StartFile>(),
                Disciplines = new List<string>(),
                TomIFC = new CopyDetails()
            };
            var Folders = xDoc.Element("Project").Element("Folders").Elements("Folder");
            var Files = xDoc.Element("Project").Element("Files").Elements("File");
            var StartFiles = xDoc.Element("Project").Element("StartFiles").Elements("StartFile");
            var Disciplines = xDoc.Element("Project").Element("Disciplines").Elements("Discipline").Attributes("Name");

            foreach (var Folder in Folders)
            {
                ProjectInfo.Folders.Add(new CopyDetails
                {
                    From = Folder.Attribute("From").Value,
                    To = Folder.Attribute("To").Value,
                    Export = Folder.Attribute("Export").Value,
                    Discipline = Folder.Attribute("Discipline").Value
                });
            }

            foreach (var File in Files)
            {
                ProjectInfo.Files.Add(new CopyDetails
                {
                    From = File.Attribute("From").Value,
                    To = File.Attribute("To").Value,
                    Export = File.Attribute("Export").Value
                });
            }

            foreach (var StartFile in StartFiles)
            {
                ProjectInfo.StartFiles.Add(new StartFile
                {
                    Path = StartFile.Attribute("Path").Value,
                    Discipline = StartFile.Attribute("Discipline").Value
                });
            }

            foreach (var Discipline in Disciplines)
            {
                ProjectInfo.Disciplines.Add(Discipline.Value);
            }

            ProjectInfo.TomIFC = new CopyDetails{
                From = xDoc.Element("Project").Element("IFC").Attribute("From").Value,
                To = xDoc.Element("Project").Element("IFC").Attribute("To").Value
            };

            return ProjectInfo;
        }
    }
}
