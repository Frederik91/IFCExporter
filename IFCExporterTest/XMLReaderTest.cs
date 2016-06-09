using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFCExporter.Helpers;
using System.IO;
using System.Collections.Generic;
using IFCExporter.Workers;
using IFCExporterAPI.Assets;
using IFCExporter.Models;
using IFCExporterAPI.Models;
using Microsoft.Win32;
using System.Windows;
using IFCExporterWindows.Models;

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
            DataStorage.ProjectInfo = new List<IfcProjectInfo>();
            DataStorage.ProjectInfo.Add(reader.GetprojectInfo(@"H:\IFCEXPORT\XML\AutoExportUNN.xml"));
            DataStorage.ProjectInfo.Add(reader.GetprojectInfo(@"H:\IFCEXPORT\XML\BUS2.xml"));
            var Exports = new List<string>();
            Exports.Add("E410");
            Exports.Add("E430");

            List<string> RIEFOLDERS = new List<string>();
            List<string> RIVFOLDERS = new List<string>();

            foreach (var project in DataStorage.ProjectInfo)
            {
                foreach (var Discipline in project.Disciplines)
                {

                    var ActiveExport = Discipline.Exports.FindAll(export => Exports.Contains(export.Name));

                    foreach (var Export in ActiveExport)
                    {
                        foreach (var Folder in Export.Folders)
                        {

                            if (Discipline.Name == "RIE")
                            {
                                RIEFOLDERS.Add(Folder.remote);
                            }

                            if (Discipline.Name == "RIV")
                            {
                                RIVFOLDERS.Add(Folder.remote);
                            }
                        }
                    }
                }
            }

            var UAX = new UnloadAllXrefs();

            var FileList = new List<string>();

            foreach (var project in DataStorage.ProjectInfo)
            {
                foreach (var Discipline in project.Disciplines)
                {
                    foreach (var Export in Discipline.Exports)
                    {
                        foreach (var Folder in Export.Folders)
                        {
                            var files = Directory.GetFiles(Folder.local, "*.dwg");

                            foreach (var file in files)
                            {
                                FileList.Add(file);
                            }
                        }
                    }
                }
            }
            //UAX.UnloadAllXref(FileList);


            Assert.AreNotEqual(0, RIEFOLDERS.Count);
        }

        [TestMethod]


        public void testwrite()
        {
            var ProjectInfo = new List<IfcProjectInfo>();
            var AviliableExports = new List<SelectedExport>();

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "XML-files (*.xml)|*.xml";
            fileDialog.Multiselect = true;
            fileDialog.ShowDialog();

            if (fileDialog.FileNames.Length > 0)
            {
                var reader = new XmlReader();
                foreach (var file in fileDialog.FileNames)
                {
                    try
                    {
                        ProjectInfo.Add(reader.GetprojectInfo(file));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to read file \"" + Path.GetFileNameWithoutExtension(file) + "\"");
                    }

                }
                try
                {
                    var list = new List<SelectedExport>();
                    foreach (var proj in ProjectInfo)
                    {
                        foreach (var Discipline in proj.Disciplines)
                        {
                            foreach (var Export in Discipline.Exports)
                            {
                                list.Add(new SelectedExport { Export = proj.ProjectName + " - " + Export.Name, IsSelected = false });
                            }
                        }
                        AviliableExports = list;
                    }
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Unable to read XML-file");
                    throw;
                }
            }

        }

    }
}
