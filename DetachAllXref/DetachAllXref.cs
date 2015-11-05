using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetachAllXref
{
    public class DetachAllXref
    {
        [CommandMethod("DetachAllXref")]
        public void DetachAllXref(string filepath) // This method can have any name
        {
            //Get the document
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = Doc.Editor;

            string[] filepaths = Directory.GetFiles(filepath, "*.dwg", SearchOption.AllDirectories);
            int filecount = filepaths.Length;
            ed.WriteMessage("\nScanning " + filecount + " files");

            for (int i = 0; i < filecount; i++)
            {
                ed.WriteMessage("\n File Name : " + filepaths[i]);
                //create a database and try to load the file
                Database db = new Database(false, true);
                using (db)
                {
                    try
                    {
                        ed.WriteMessage("\nOpening file: " + Path.GetFileName(filepaths[i]));
                        db.ReadDwgFile(filepaths[i], FileShare.ReadWrite, false, "");
                    }
                    catch (System.Exception)
                    {
                        ed.WriteMessage("\nUnable to read the drawingfile.");
                        return;
                    }
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        ed.WriteMessage("\n--------Xrefs Details--------");
                        db.ResolveXrefs(true, false);

                        XrefGraph xg = db.GetHostDwgXrefGraph(true);
                        int xrefcount = xg.NumNodes - 1;

                        if (xrefcount == 0)
                        {
                            ed.WriteMessage("\nNo xrefs found in the drawing");
                        }
                        else
                        {
                            ObjectIdCollection XrefColl = new ObjectIdCollection();
                            for (int r = 1; r < (xrefcount + 1); r++)
                            {
                                ed.WriteMessage("\nXref Name: " + xg.GetXrefNode(r).Name);
                                XrefGraphNode xrefNode = xg.GetXrefNode(r);
                                ObjectId xrefId = xrefNode.BlockTableRecordId;
                                ed.WriteMessage("\nXref NodeID: " + xrefId.ToString());
                                XrefColl.Add(xrefId);

                                
                            }
                            db.UnloadXrefs(XrefColl);
                            ed.WriteMessage("\nCommitting");
                            tr.Commit();
                            ed.WriteMessage("\nCommitted");
                        }
                    }
                    // Overwrite the current drawing file with new updated XRef paths
                    db.SaveAs(filepaths[i], false, DwgVersion.Current, null);

                }
            }
        }
    }
}