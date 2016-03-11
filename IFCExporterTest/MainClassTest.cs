using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter;
using System;

namespace IFCExporterTest
{
    [TestClass]
    public class MainClassTest
    {
        [TestMethod]
        public void TestMethod_Test()
        {
            string n = string.Format("text-{0:yyyy-MM-dd_hh-mm-ss-tt}.bin",
    DateTime.Now);
        }

    }
}
