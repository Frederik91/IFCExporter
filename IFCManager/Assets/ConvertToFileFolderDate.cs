using IFCExporter.Models;
using IFCExporter.Workers;
using IFCExporterAPI.Models;
using IFCManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IFCManager.Assets
{
    public class ConvertToFileFolderDate
    {
        public List<FileFolderDate> returnNewDateList(List<IfcProjectInfo> ProjectList)
        {
            var FDC = new FileDateComparer();
            var newDateList = new List<FileFolderDate>();

            foreach (var project in ProjectList)
            {
                foreach (var discipline in project.Disciplines)
                {
                    foreach (var export in discipline.Exports)
                    {
                        var IFCpath = project.TomIFC.Export + "\\" + export.IFC + ".ifc";
                        var newestFileDate = new DateTime();
                        string lastSavedBy;
                        string newestDrawing = "";                        

                        foreach (var folder in export.Folders)
                        {
                            var Drawings = Directory.GetFiles(folder.remote, "*.dwg", SearchOption.TopDirectoryOnly);
                            foreach (var drawing in Drawings)
                            {
                                var lastWriteTime = File.GetLastWriteTime(drawing);

                                if (lastWriteTime > newestFileDate)
                                {
                                    newestFileDate = lastWriteTime;
                                    newestDrawing = drawing;
                                }
                            }
                        }



                        FileInfo file = new FileInfo(newestDrawing);                        
                        var fs = file.GetAccessControl();
                        var ir = fs.GetOwner(typeof(NTAccount));
                        lastSavedBy = ir.Value;

                        var ifcLastWrite = File.GetLastWriteTime(IFCpath);

                        newDateList.Add(new FileFolderDate { FileName = export.IFC, Export = export.Name, FolderUpdate = newestFileDate, IfcUpdate = ifcLastWrite, LastSavedBy = lastSavedBy });
                    }
                }
            }

            return newDateList;
        }
    }
}
