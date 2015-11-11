using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Workers;
using IFCExporter.Helpers;
using IFCExporter.Models;
using System.Windows.Forms;
using System.Collections.Generic;

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

            var x = new FileWatcher();

            DataStorage.OldFolderDateList = x.GetNewFolderDateList();

            var y = x.GetNewFolderDateList();

            while (!x.CompareFolderLists_ReturnTrueIfChanged(y))
            {
                y = x.GetNewFolderDateList();
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

            //while (!FCA.CheckForChange())
            //{
            //    System.Threading.Thread.Sleep(50);
            //}

        }
    }
}
