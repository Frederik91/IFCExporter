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
            var reader = new XMLReader();
            var ProjectInfo = reader.GetprojectInfo("C:\\IFCEXPORT\\XML\\MH2.xml");

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



    }
}
