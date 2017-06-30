﻿using AutoCAD;
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
using System.Timers;

namespace IFCMonitor.Assets
{
    public class Exporter
    {
        private static System.Timers.Timer exportTimeoutTimer = new System.Timers.Timer();
        private static AcadApplication acApp;

        public Exporter()
        {
            exportTimeoutTimer.Elapsed += new ElapsedEventHandler(CloseAcadTimeoutEvent);
            exportTimeoutTimer.Interval = exportTimeoutTimer.Interval = 1800000; ;
        }

        public static bool Export(FileFolderDate fileFolderDate, string xmlPath)
        {
            // "AutoCAD.Application.17" uses 2007 or 2008,
            //  whichever was most recently run

            // "AutoCAD.Application.17.1" uses 2008, specifically

            const string progID = "AutoCAD.Application";

            acApp = null;
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

                var exportCompleted = false;

                while (true)
                {
                    try
                    {
                        if (!exportCompleted)
                        {
                            if (!exportTimeoutTimer.Enabled)
                            {
                                exportTimeoutTimer.Enabled = true;
                                exportTimeoutTimer.Start();
                            }
                            acApp.Visible = true;
                            acApp.ActiveDocument.SendCommand("AutomaticIFC" + " " + xmlPath + " " + fileFolderDate.Id.ToString() + " ");
                            acApp.ActiveDocument.SendCommand("_.MAGIEPROJECT2 ");
                            acApp.ActiveDocument.SendCommand("_.MAGIHPVPROJECT2 ");
                            Thread.Sleep(2000);
                            acApp.ActiveDocument.SendCommand("_.-MAGIIFC " + fileFolderDate.Name + "\n");

                            exportCompleted = true;
                            CopyIfc(fileFolderDate.Project, fileFolderDate.Name, fileFolderDate.IfcName);
                        }

                        CloseAcad();
                        exportTimeoutTimer.Enabled = false;
                        exportTimeoutTimer.Stop();
                        return true;
                    }
                    catch (Exception e)
                    {
                        try
                        {

                            if (!e.Message.Contains("Busy") && acApp != null && acApp.Visible)
                            {
                                CloseAcad();
                                exportTimeoutTimer.Enabled = false;
                                exportTimeoutTimer.Stop();
                                return true;
                            }
                        }
                        catch
                        {
                            if (e.Message.Contains("The RPC server is unavailable"))
                            {
                                exportTimeoutTimer.Enabled = false;
                                exportTimeoutTimer.Stop();
                                return true;
                            }
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

        private static void CloseAcadTimeoutEvent(object sender, ElapsedEventArgs e)
        {
            CloseAcad();
            exportTimeoutTimer.Enabled = false;            
        }

        private static void CloseAcad()
        {
            try
            {
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
                if (e.Message == "Failed to get the Document object")
                {
                    acApp.Quit();
                    acApp = null;
                }
                else
                {
                    var processes = Process.GetProcessesByName("Acad").ToList();
                    foreach (var process in processes)
                    {
                        process.Kill();
                        acApp = null;
                    }
                }
            }

        }
    }
}
