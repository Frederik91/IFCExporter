using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IFCExporter.Helpers
{
    public class DrawingManager
    {
        public void CloseAllOtherDrawings(string fileName)
        {
            var DocMgr = Application.DocumentManager;

            foreach (Document Doc in DocMgr)
            {
                if (Doc.Name != fileName)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            Doc.CloseAndDiscard();
                            break;
                        }
                        catch (System.Exception)
                        {
                            Thread.Sleep(2000);
                            if (i == 9)
                            {
                                var writer = new Writer();
                                writer.WriteLine("Unable to close drawing " + Doc.Name);
                            }
                        }
                    }
                }
            }
        }

        public void CloseNotReadOnlyDrawings()
        {
            var DocMgr = Application.DocumentManager;

            foreach (Document Doc in DocMgr)
            {
                if (!Doc.IsReadOnly)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            Doc.CloseAndDiscard();
                            break;
                        }
                        catch (System.Exception)
                        {
                            Thread.Sleep(2000);
                            if (i == 9)
                            {
                                var writer = new Writer();
                                writer.WriteLine("Unable to close drawing " + Doc.Name);
                            }
                        }
                    }
                }
            }
        }

        public void OpenDrawing(string FileName)
        {
            var DocMgr = Application.DocumentManager;

            foreach (Document Doc in DocMgr)
            {
                if (Doc.Name == FileName)
                {
                    return;
                }
            }

            DocMgr.Open(FileName, false);
        }

        public void OpenDrawingReadOnly(string FileName)
        {
            var DocMgr = Application.DocumentManager;

            foreach (Document Doc in DocMgr)
            {
                if (Doc.Name == FileName)
                {
                    return;
                }
            }

            DocMgr.Open(FileName, true);
        }

        public void CloseAndDiscardAllDrawings()
        {
            var DocMgr = Application.DocumentManager;

            foreach (Document doc in DocMgr)
            {
                doc.CloseAndDiscard();
            }
        }

        public Document GetDrawingByName(string FileName)
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

        public void CloseIfOpen(string FolderDir)
        {
            DirectoryInfo DI = new DirectoryInfo(FolderDir);
            var SourceDirFiles = DI.GetFiles();
            var OpenDrawings = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;

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

        public void CloseAndDiscardDrawing(string DrawingName)
        {
            var OpenDrawings = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;

            foreach (Document drawing in OpenDrawings)
            {
                if (DrawingName == drawing.Name)
                {
                    drawing.CloseAndDiscard();
                    return;
                }

            }
        }

    }
}
