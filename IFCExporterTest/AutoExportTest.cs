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
        public void TestFileChangedActions()
        {
            DataStorage.logFileLocation = @"C:\IFCEXPORT\Log - MH2.xml";

            DataStorage.ExportsToRun = new List<string>();

            var reader = new XmlReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\MH2.xml");

            var FDC = new FileDateComparer();

            DataStorage.ExportsToRun = new List<string>();
            DataStorage.SelectedExports = new List<string>();

            DataStorage.SelectedExports.Add("V31");
            DataStorage.SelectedExports.Add("V33");
            DataStorage.SelectedExports.Add("V34");
            DataStorage.SelectedExports.Add("V36");


            while (DataStorage.ExportsToRun.Count == 0)
            {
                var FCA = new FileChangedActions();
                FCA.CheckForChanges();
            }

            MessageBox.Show("lol");
        }

        [TestMethod]
        public void FileChangedActionsTest()
        {
            DataStorage.ExportsToRun = new List<string>();           

            var reader = new XmlReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\MH2.xml");



            var FDC = new FileDateComparer();

            var newFolderList = FDC.GetNewFolderDateList();
            var newIfcFileList = FDC.GetIfcFileDateList(DataStorage.ProjectInfo.TomIFC.Export);
            var newExportList = FDC.ReturnExpiredExports(newFolderList, newIfcFileList);
            DataStorage.ExportsToRun = newExportList.Distinct().ToList();

            var x = FDC.ReturnDwgsInChangedExports();

            var y = x;

        }

        [TestMethod]
        public void IfcOutOfDateTest()
        {
            DataStorage.ExportsToRun = new List<string>();

            var reader = new XmlReader();

            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2.xml");

            var x = new FileDateComparer();

            var y = x.GetNewFolderDateList();

            var FCA = new FileChangedActions();
            FCA.startMonitoring();

            while (DataStorage.FilesWithChanges.Count == 0)
            {
                DataStorage.FilesWithChanges = x.ReturnChangedFiles(y);
                Thread.Sleep(500);
            }

            var NewPathList = new List<string>();

            var FilesWithChanges = DataStorage.FilesWithChanges;
            var FilesToUnload = new List<string>();

            foreach (var _file in FilesWithChanges)
            {
                var ToPath = DataStorage.ProjectInfo.BaseFolder.To + _file.Substring(DataStorage.ProjectInfo.BaseFolder.From.Length);
                FilesToUnload.Add(ToPath);
            }
        }

    }
}
