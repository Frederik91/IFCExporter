using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Helpers
{
    public class DocumentManager
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

    }
}
