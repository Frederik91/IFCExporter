using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Helpers;
using System.IO;
using System.Collections.Generic;

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
            var ProjectInfo = reader.GetprojectInfo("H:\\IFCEXPORT\\XML\\BUS2.xml");

            int countOfMatches = 0;
            List<string> RIEFOLDERS = new List<string>();
            List<string> RIVFOLDERS = new List<string>();

            foreach (var Folder in ProjectInfo.Folders)
            {
                foreach (var Discipline in ProjectInfo.Disciplines)
                {
                    if (Discipline == Folder.Discipline)
                    {
                        countOfMatches++;
                    }

                    if (Folder.Discipline == "RIE")
                    {
                        RIEFOLDERS.Add(Folder.From);
                    }

                    if (Folder.Discipline == "RIV")
                    {
                        RIVFOLDERS.Add(Folder.From);
                    }



                }
            }

            Assert.AreNotEqual(0, countOfMatches);
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
