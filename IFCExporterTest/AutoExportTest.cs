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
        public void TestMethod1()
        {
            DataStorage.ExportsToRun = new List<string>();

            var reader = new XMLReader();

            var ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2.xml");

            var x = new AutoExport(ProjectInfo);

            DataStorage.OldFolderDateList = x.GetNewFolderDateList();

            var y = x.GetNewFolderDateList();

            while (!x.CompareFolderLists_ReturnTrueIfChanged(y))
            {
                y = x.GetNewFolderDateList();
                System.Threading.Thread.Sleep(50);
            }

        }
    }
}
