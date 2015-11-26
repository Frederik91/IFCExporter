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
                        ed.WriteMessage("\nUnable to read the drawingfile.");
                        return;
                    }
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        db.ResolveXrefs(true, false);

                        XrefGraph xg = db.GetHostDwgXrefGraph(true);
                        int xrefcount = xg.NumNodes - 1;

                        if (xrefcount != 0)                        
                        {
                            ObjectIdCollection XrefColl = new ObjectIdCollection();

                            for (int r = 1; r < (xrefcount + 1); r++)
                            {
                                XrefGraphNode xrefNode = xg.GetXrefNode(r);

                                ObjectId xrefId = xrefNode.BlockTableRecordId;
                                XrefColl.Add(xrefId);                               

                            }
                            db.UnloadXrefs(XrefColl);
                            tr.Commit();
                        }
                    }
                    // Overwrite the current drawing file with new updated XRef paths
                    db.SaveAs(file, false, DwgVersion.Current, null);

                }
            }
        }
    }
}
