using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Helpers
{
    public class DrawingManager
    {
        public void OpenDrawing(string FileName)
        {           

            var DocMgr = Application.DocumentManager;

            foreach (Document Doc  in DocMgr)
            {
                if (Doc.Name == FileName)
                {
                    return;
                }
            }

            DocMgr.Open(FileName, false);
        }

        public Document ReturnActivateDrawing(string FileName)
        {
            var DocMgr = Application.DocumentManager;
            var document = DocMgr.MdiActiveDocument;

            foreach (Document doc in DocMgr)
            {
                if (doc.Name == FileName)
                {
                    return doc;
                }
            }
            return document;
        }

        public async void CheckIfDrawingIsOpen_CloseIfOpen(string FolderDir)
        {
                var dc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

                await dc.ExecuteInCommandContextAsync(async (o) => CloseDrawing(FolderDir), null);
        }


        public void CloseDrawing(string FolderDir)
        {
            DirectoryInfo DI = new DirectoryInfo(FolderDir);
            var SourceDirFiles = DI.GetFiles();
            var OpenDrawings = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            foreach (var File in SourceDirFiles)
            {
                foreach (Document drawing in OpenDrawings)
                {
                    var DrawingName = Path.GetFileName(drawing.Name);
                    var FileName = Path.GetFileName(File.Name);

                    if (DrawingName == FileName)
                    {
                        drawing.CloseAndDiscard();
                    }
                }
            }
        }

    }
}
