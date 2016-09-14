using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IFCExporter.Helpers
{
    public class UnloadAllXrefs
    {
        private Writer writer = new Writer();

        public void UnloadAllXref(List<string> LocalFilePaths)
        {
            //Get the document
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = Doc.Editor;


            foreach (var file in LocalFilePaths)
            {
               
                //create a database and try to load the file
                Database db = new Database(false, true);
                using (db)
                {
                    try
                    {
                        db.ReadDwgFile(file, FileShare.ReadWrite, false, "");
                    }
                    catch (System.Exception)
                    {
                        writer.writeLine("Failed to unload xrefs in file \"" + Path.GetFileName(file) + "\"");
                        continue;
                    }
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        //Detach xref
                        XrefGraph xg = db.GetHostDwgXrefGraph(true);
                        int xrefcount = xg.NumNodes - 1;

                        if (xrefcount != 0)
                        {
                            ObjectIdCollection XrefColl = new ObjectIdCollection();

                            for (int r = 1; r < (xrefcount + 1); r++)
                            {                                
                                XrefGraphNode xrefNode = xg.GetXrefNode(r);

                                writer.writeLine("Unloading xref \"" + xrefNode.Name + "\"");

                                ObjectId xrefId = xrefNode.BlockTableRecordId;
                                db.DetachXref(xrefId);
                                writer.writeLine("Xref unloaded");
                            }
                        }

                        var setView = new setView();
                        setView.set2DView(Doc, db, tr);
                        writer.writeLine("Setting Viewport to 2D");

                        writer.writeLine("Comitting");
                        tr.Commit();
                        writer.writeLine("Disposing");
                        tr.Dispose();
                        writer.writeLine("Successfully unloaded xrefs in file \"" + Path.GetFileName(file) + "\""); 
                    }
                    // Overwrite the current drawing file with new updated XRef paths
                    writer.writeLine("Starting save prosess");
                    try
                    {
                        var FI = new FileInfo(file);
                        writer.writeLine("Deleting old version");
                        FI.Delete();
                        writer.writeLine("Saving new version");
                        db.SaveAs(file, false, DwgVersion.Current, null);
                        writer.writeLine("Successfully saved file \"" + Path.GetFileName(file) + "\"");
                    }
                    catch (System.Exception)
                    {
                        writer.writeLine("Failed to save file \"" + Path.GetFileName(file) + "\"");
                        continue;
                    }
                }
            }
        }
    }
}
