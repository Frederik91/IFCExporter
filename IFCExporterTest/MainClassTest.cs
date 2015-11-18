using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter;

namespace IFCExporterTest
{
    [TestClass]
    public class MainClassTest
    {
        [TestMethod]
        public void TestMethod_Test()
        {
            var window = new IFCExporterWindows.MainWindow();
            window.ShowDialog();
            var x = window.MainViewModel;

        }

    }
}
