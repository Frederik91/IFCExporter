using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.IO;

namespace IFCExporter.Helpers
{
    class SetView
    {
        public void Set2DView(Document acDoc, Database acCurDb, Transaction acTrans)
        {
            // Open the Viewport table for read
            ViewportTable acVportTbl;
            acVportTbl = acTrans.GetObject(acCurDb.ViewportTableId, OpenMode.ForWrite) as ViewportTable;
            {
                var first = true;
                foreach (ObjectId acObjId in acVportTbl)
                {
                    // Open the object for read
                    ViewportTableRecord acVportTblRec;
                    acVportTblRec = acTrans.GetObject(acObjId, OpenMode.ForWrite) as ViewportTableRecord;

                    if (first)
                    {
                        var vStyle = Get2DVisualStyle(acDoc, acCurDb);


                        acVportTblRec.UpgradeOpen();
                        acVportTblRec.SetViewDirection(OrthographicView.TopView);

                        first = false;

                        if (vStyle == null)
                        {
                            return;
                        }

                        acVportTblRec.VisualStyleId = vStyle.Id;

                    }
                    else
                    {
                        acVportTblRec.Erase();
                    }
                }
            }
        }


        private DBVisualStyle Get2DVisualStyle(Document acDoc, Database acCurDb)
        {

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                DBDictionary vStyles = acTrans.GetObject(acCurDb.VisualStyleDictionaryId,
                                                         OpenMode.ForRead) as DBDictionary;

                // Step through the dictionary
                foreach (DBDictionaryEntry entry in vStyles)
                {
                    // Get the dictionary entry
                    DBVisualStyle vStyle = vStyles.GetAt(entry.Key).GetObject(OpenMode.ForRead) as DBVisualStyle;

                    // If the visual style is not marked for internal use then output its name
                    if (vStyle.InternalUseOnly == false)
                    {
                        if (vStyle.Name.ToUpper().Contains("WIRE"))
                        {
                            return vStyle;
                        }
                    }
                }
            }
            return null;
        }

    }
}
