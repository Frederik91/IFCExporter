using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Workers;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using IFCExporterAPI.Assets;

namespace IFCExporterTest
{
    [TestClass]
    public class AutoExportTest
    {
        [TestMethod]
        public void TestFileWatcher()
        {
            DataStorage.ExportsToRun = new List<string>();

            var reader = new XmlReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2Test.xml");

            var FW = new FileDateComparer();

            var x = FW.GetNewFolderDateList();
            var y = FW.GetNewFolderDateList();

            while (!FW.CheckForChanges(FW.CompareFolderLists(x, y)))
            {
                x = FW.GetNewFolderDateList();
                System.Threading.Thread.Sleep(50);
            }

        }

        [TestMethod]
        public void FileChangedActionsTest()
        {
            DataStorage.ExportsToRun = new List<string>();

            var reader = new XmlReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2Test.xml");

            var x = new FileDateComparer();

            DataStorage.OldFolderDateList = x.GetNewFolderDateList();

            var FCA = new FileChangedActions();

            FCA.startMonitoring();

            while (!DataStorage.ExportInProgress)
            {
                System.Threading.Thread.Sleep(50);
            }

        }

        [TestMethod]
        public void IfcFileChangeTest()
        {
            DataStorage.ExportsToRun = new List<string>();

            var reader = new XmlReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\MH2Test.xml");

            var x = new FileDateComparer();

            DataStorage.IfcOldFolderDateList = x.GetNewIfcFileDateList();

            var IUW = new IfcUpdateWatcher();

            DataStorage.ExportInProgress = true;

            IUW.StartIfcMonitoring();

            while (DataStorage.ExportInProgress)
            {
                System.Threading.Thread.Sleep(50);
            }

        }

        [TestMethod]
        public void IfcOutOfDateTest()
        {
            DataStorage.ExportsToRun = new List<string>();

            var reader = new XmlReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2.xml");

            var FDC = new FileDateComparer();
            DataStorage.OldFolderDateList = FDC.GetNewFolderDateList();

            var FCA = new FileChangedActions();
            FCA.startMonitoring();

            while (DataStorage.ExportInProgress != true)
            {
                Thread.Sleep(500);
            }
            var expall = new ExportAll(true);
            expall.Run();
        }
    }
}
