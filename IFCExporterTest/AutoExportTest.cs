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
using IFCExporterAPI.Models;

namespace IFCExporterTest
{
    [TestClass]
    public class AutoExportTest
    {
        [TestMethod]
        public void TestFileChangedActions()
        {
            DataStorage.logFileLocation = @"C:\IFCEXPORT\Log - MH2.xml";

            DataStorage.ProjectChanges = new List<ProjectChanges>();
            DataStorage.ExportsToRun = new List<IfcProjectInfo>();

            var reader = new XmlReader();
            DataStorage.ProjectInfo = new List<IfcProjectInfo>();

            DataStorage.ProjectInfo.Add(reader.GetprojectInfo(@"H:\IFCEXPORT\XML\MH2.xml"));
            DataStorage.ProjectInfo.Add(reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2.xml"));
            DataStorage.ProjectInfo.Add(reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS1.xml"));
            DataStorage.ProjectInfo.Add(reader.GetprojectInfo(@"H:\IFCEXPORT\XML\AutoExportUNN.xml"));

            var FDC = new FileDateComparer();

            //DataStorage.ExportsToRun = new List<string>();
            //DataStorage.SelectedExports = new List<string>();

            //DataStorage.SelectedExports.Add("V31");
            //DataStorage.SelectedExports.Add("V33");
            //DataStorage.SelectedExports.Add("V34");
            //DataStorage.SelectedExports.Add("V36");


            while (DataStorage.ExportsToRun.Count == 0)
            {
                var FCA = new FileChangedActions();
                FCA.CheckForChanges();
            }

            MessageBox.Show("lol");
        }


    }
}
