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
using System.Threading.Tasks;

namespace IFCExporter.Helpers
{
    public class UnloadAllXrefs
    {
        [CommandMethod("DetachAllXref")]
        public void UnloadAllXref(List<string> OriginalFilePaths, bool Automatic) 
        {
            //Get the document
            Document Doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = Doc.Editor;

            var LocalFilePaths = new List<string>();
            //Locate files in base folders
            if (Automatic)
            {
                foreach (var file in LocalFilePaths)
                {
                    LocalFilePaths.Add(Directory.GetFiles(DataStorage.ProjectInfo.BaseFolder.To, Path.GetFileName(file)).First());
                }
            }
            else
            {
                LocalFilePaths = OriginalFilePaths;
            }

            ed.WriteMessage("\nScanning " + LocalFilePaths.Count + " files");

            foreach (var file in LocalFilePaths)
            {
                ed.WriteMessage("\n File Name : " + file);
                //create a database and try to load the file
                Database db = new Database(false, true);
                using (db)
                {
                    try
                    {
                        ed.WriteMessage("\nOpening file: " + Path.GetFileName(file));
                        db.ReadDwgFile(file, FileShare.ReadWrite, false, "");
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
                    db.SaveAs(file, false, DwgVersion.Current, null);

                }
            }
        }
    }
}
