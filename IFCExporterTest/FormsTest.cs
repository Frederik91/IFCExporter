using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Helpers;
using System.IO;
using System.Collections.Generic;
using IFCExporter.Forms;

namespace IFCExporterTest
{
    [TestClass]
    public class FormsTest
    {
        [TestMethod]
        public void SelectProjectFormTest_CheckContent()
        {
            string _selectedProject = "";

            SelectProjectForm SPF = new SelectProjectForm();
            var result = SPF.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _selectedProject = SPF.SelectedProject;
            }

            Assert.AreNotEqual("", _selectedProject);
        }

    }
}
