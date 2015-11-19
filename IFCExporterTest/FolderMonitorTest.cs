using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Helpers;
using System.IO;
using System.Collections.Generic;
using IFCExporter.Workers;

namespace IFCExporterTest
{
    [TestClass]
    public class FolderMonitorTest
    {
        [TestMethod]
        public void TestCopy()
        {
            var CP = new Copier();

            CP.CopySingleFile(@"O:\A005000\A009727\BUSP 2\3.7 Tegninger\E41\00-Modellfiler\F50-02-E-410-00-31.dwg", @"C:\IFCEXPORT\Projects\BUS2\E41\00-Modellfiler\F50-02-E-410-00-31.dwg");
        }

    }
}
