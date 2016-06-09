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
        public void Test()
        {
            var x = Directory.GetFiles(@"C:\IFCEXPORT\Projects\UNN\V33\Modellfiler", "*.dwg");
        }

    }
}
