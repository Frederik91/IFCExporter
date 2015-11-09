using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Helpers;
using System.IO;
using System.Collections.Generic;
using IFCExporter.Forms;
using IFCExporter.Workers;

namespace IFCExporterTest
{
    [TestClass]
    public class FolderMonitorTest
    {
        [TestMethod]
        public void CheckFoldeUpdateDetection()
        {
            XMLReader reader = new XMLReader();
            var ProjectData = reader.GetprojectInfo("C:\\TestMappe\\TestProsjekt.xml");
            FolderMonitor FM = new FolderMonitor(ProjectData.Disciplines);
            var res = FM.StartMonitoring();
            Assert.AreNotEqual(null, res.Path);
        }

    }
}
