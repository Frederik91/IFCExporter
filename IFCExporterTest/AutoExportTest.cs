using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Workers;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace IFCExporterTest
{
    [TestClass]
    public class AutoExportTest
    {
        [TestMethod]
        public void TestFileWatcher()
        {
            DataStorage.ExportsToRun = new List<string>();

            var reader = new XMLReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2Test.xml");

            var FW = new FileWatcher();

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

            var reader = new XMLReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2Test.xml");

            var x = new FileWatcher();

            DataStorage.OldFolderDateList = x.GetNewFolderDateList();

            var FCA = new FileChangedActions(x);

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

            var reader = new XMLReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\MH2.xml");

            var x = new FileWatcher();

            DataStorage.IfcOldFolderDateList = x.GetNewIfcFileDateList(Path.GetDirectoryName(DataStorage.ProjectInfo.TomIFC.To));

            var IUW = new IfcUpdateWatcher();

            DataStorage.ExportInProgress = true;

            IUW.StartIfcMonitoring();

            while (DataStorage.ExportInProgress)
            {
                System.Threading.Thread.Sleep(50);
            }

        }
    }
}
