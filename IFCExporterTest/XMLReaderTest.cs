﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Helpers;
using System.IO;
using System.Collections.Generic;
using IFCExporter.Workers;
using IFCExporterAPI.Assets;
using IFCExporter.Models;
using IFCExporterAPI.Models;

namespace IFCExporterTest
{
    [TestClass]
    public class XMLReaderTest
    {
        [TestMethod]
        public void GetprojectInfo_CheckContent()
        {
            //var CP = new Copier();
            var reader = new XmlReader();
            DataStorage.ProjectInfo = reader.GetprojectInfo(@"H:\IFCEXPORT\XML\AutoExport UNN.xml");
            var Exports = new List<string>();
            Exports.Add("E410");
            Exports.Add("E430");

            List<string> RIEFOLDERS = new List<string>();
            List<string> RIVFOLDERS = new List<string>();

            foreach (var Discipline in DataStorage.ProjectInfo.Disciplines)
            {

                var ActiveExport = Discipline.Exports.FindAll(export => Exports.Contains(export.Name));

                foreach (var Export in ActiveExport)
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

            //var UAX = new UnloadAllXrefs();

            //var FileList = new List<string>();

            //foreach (var Discipline in DataStorage.ProjectInfo.Disciplines)
            //{
            //    foreach (var Export in Discipline.Exports)
            //    {
            //        foreach (var Folder in Export.Folders)
            //        {
            //            var files = Directory.GetFiles(Folder.To, "*.dwg");

            //            foreach (var file in files)
            //            {
            //                FileList.Add(file);
            //            }
            //        }
            //    }
            //}

            //UAX.UnloadAllXref(FileList, false);


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
