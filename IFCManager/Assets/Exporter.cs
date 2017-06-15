using Autodesk.AutoCAD.Interop;
using IFCExporter.Helpers;
using IFCExporterAPI.Models;
using IFCManager.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace IFCMonitor.Assets
{
    public class Exporter
    {
        public static bool Export(FileFolderDate fileFolderDate, string xmlPath)
        {
            // "AutoCAD.Application.17" uses 2007 or 2008,
            //  whichever was most recently run

            // "AutoCAD.Application.17.1" uses 2008, specifically

            const string progID = "AutoCAD.Application.20.1";

            AcadApplication acApp = null;
            try
            {
                acApp = (AcadApplication)Marshal.GetActiveObject(progID);
            }
            catch
            {
                try
                {
                    Type acType = Type.GetTypeFromProgID(progID);
                    acApp = (AcadApplication)Activator.CreateInstance(acType, true);
                }
                catch
                {
                    // log here
                    return false;
                }
            }
            if (acApp != null)
            {
                // By the time this is reached AutoCAD is fully
                // functional and can be interacted with through code

                var finished = false;
                var exportCompleted = false;

                while (!finished)
                {
                    try
                    {
                        if (!exportCompleted)
                        {
                            acApp.Visible = true;
                            acApp.ActiveDocument.SendCommand("AutomaticIFC" + " " + xmlPath + " " + fileFolderDate.Id.ToString() + " ");
                            acApp.ActiveDocument.SendCommand("_.MAGIEPROJECT2 ");
                            acApp.ActiveDocument.SendCommand("_.MAGIHPVPROJECT2 ");
                            acApp.ActiveDocument.SendCommand("_.-MAGIIFC " + fileFolderDate.Name + "\n");
                            CopyIfc(fileFolderDate.Project, fileFolderDate.Name, fileFolderDate.IfcName);
                            exportCompleted = true;
                        }


                        while (acApp.GetType().InvokeMember("ActiveDocument", BindingFlags.GetProperty, null, acApp, null) is object ActiveDocument)
                        {
                            object[] dataArry = new object[2];
                            dataArry[0] = false; //no save
                            dataArry[1] = ""; //drawing file name.. if saving
                            ActiveDocument.GetType().InvokeMember("close", BindingFlags.InvokeMethod, null, ActiveDocument, dataArry);
                        }
                    }
                    catch (Exception e)
                    {
                        if (!e.Message.Contains("Busy"))
                        {
                            finished = true;
                        }
                        if (e.Message == "Failed to get the Document object")
                        {
                            acApp.Quit();
                            finished = true;
                        }
                    }
                }
            }

            return true;
        }

        private static void CopyIfc(IfcProjectInfo project, string exportName, string IfcName)
        {
            //--Last opp IFC
            var writer = new Writer();
            writer.WriteLine("Uploading IFC");
            var IfcFromPath = Path.GetDirectoryName(project.TomIFC.To) + "\\" + exportName + ".ifc";
            var IfcToPath = project.TomIFC.Export + "\\" + IfcName + ".ifc";

            var emptyIfc = new FileInfo(project.TomIFC.From);
            var exportedIfc = new FileInfo(IfcFromPath);

            if (emptyIfc.LastWriteTime != exportedIfc.LastWriteTime || emptyIfc.Length != exportedIfc.Length)
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var CP = new Copier();
                        CP.CopySingleFile(IfcFromPath, IfcToPath);
                        writer.WriteLine("IFC successfully uploaded");
                        break;
                    }
                    catch { }
                }
            }
            else
            {
                writer.WriteLine("IFC-file is empty, skipping upload");
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        static void CloseWindow(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
