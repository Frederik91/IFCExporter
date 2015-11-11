using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Helpers;
using System.IO;
using System.Collections.Generic;
using IFCExporter.Workers;

namespace IFCExporterTest
{
    [TestClass]
    public class XMLReaderTest
    {
        [TestMethod]
        public void GetprojectInfo_CheckContent()
        {
            var CP = new Copier();
            var reader = new XMLReader();
            var ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2.xml");

            List<string> RIEFOLDERS = new List<string>();
            List<string> RIVFOLDERS = new List<string>();

            foreach (var Discipline in ProjectInfo.Disciplines)
            {
                foreach (var Export in Discipline.Exports)
                {
                    foreach (var Folder in Export.Folders)
                    {

                        if (Discipline.Name == "RIE")
                        {
                            RIEFOLDERS.Add(Folder.From);
                        }

                        if (Discipline.Name == "RIV")
                        {
                            RIVFOLDERS.Add(Folder.From);
                        }
                    }
                }

            }

            Assert.AreNotEqual(0, RIEFOLDERS.Count);
        }

        [TestMethod]


        public void testwrite()
        {
            string path = "C:\\IFCEXPORT\\log.txt";
            string Content = "ny linje";

            if (!File.Exists(path))
            {
                File.Create(path);
            }
            var writer = new StreamWriter(path, true);
            writer.WriteLine(Content);
            writer.Close();
        }



    }
}
