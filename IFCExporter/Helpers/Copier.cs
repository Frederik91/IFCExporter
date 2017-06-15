using IFCExporter.Models;
using IFCExporterAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IFCExporter.Helpers
{
    public class Copier
    {
        private Writer writer = new Writer();

        public void DirectoryCopy(string SourceDir, string DestDir, bool copySubDir, string FileType)
        {

            //Finn undermapper
            DirectoryInfo dir = new DirectoryInfo(SourceDir);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Fant ikke mappen \"" + SourceDir + "\"");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            //Sjekk om desitnasjonsmappe eksisterer, hvis ikke lag den.
            if (!Directory.Exists(DestDir))
            {
                Directory.CreateDirectory(DestDir);
            }

            //Finn filene i kildemappen og kopier dem til destinasjonsmappen
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name.Contains("recover"))
                {
                    continue;
                }

                string temppath = Path.Combine(DestDir, file.Name);

                if (Path.GetExtension(file.Name) == FileType)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            file.CopyTo(temppath, true);
                            var copiedFile = new FileInfo(temppath);

                            if (file.Length == copiedFile.Length)
                            {
                                break;
                            }

                            Thread.Sleep(500);
                        }
                        catch (Exception e)
                        {
                            writer.WriteLine(e.Message + "Caused by: " +  e.TargetSite);
                            continue;
                        }
                    }
                }
            }

            //Hvis undermapper skal kopieres, kopier dem med innhold til destinasjonsmappen
            if (copySubDir)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(DestDir, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDir, FileType);
                }
            }
        }

        public void DirectoryCopyWithoutOverwrite(string SourceDir, string DestDir, bool copySubDir, string FileType)
        {
            //Finn undermapper
            DirectoryInfo dir = new DirectoryInfo(SourceDir);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Fant ikke mappen \"" + SourceDir + "\"");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            //Sjekk om desitnasjonsmappe eksisterer, hvis ikke lag den.
            if (!Directory.Exists(DestDir))
            {
                Directory.CreateDirectory(DestDir);
            }

            //Finn filene i kildemappen og kopier dem til destinasjonsmappen
            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                string temppath = Path.Combine(DestDir, file.Name);

                if (Path.GetExtension(file.Name) == FileType)
                {
                    if (!File.Exists(temppath))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            try
                            {
                                file.CopyTo(temppath, true);

                                var originalFile = new FileInfo(file.Name);
                                var copiedFile = new FileInfo(temppath);

                                if (originalFile.Length == copiedFile.Length)
                                {
                                    break;
                                }
                                Thread.Sleep(500);
                            }
                            catch (Exception)
                            {
                                Thread.Sleep(500);
                            }
                        }
                    }
                }
            }

            //Hvis undermapper skal kopieres, kopier dem med innhold til destinasjonsmappen
            if (copySubDir)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(DestDir, subdir.Name);
                    DirectoryCopyWithoutOverwrite(subdir.FullName, temppath, copySubDir, FileType);
                }
            }
        }

        public void CopySingleFile_NewName(string SourceDir, string DestDir, string NewName)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(SourceDir);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    file.CopyTo(DestDir + NewName, true);
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }


        }

        public void CopySingleFile(string SourceDir, string DestDir)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(SourceDir);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    file.CopyTo(DestDir, true);
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public void TomIFCCopy(IFCFile TomIFC, string NewName)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var SaveDir = Path.GetDirectoryName(TomIFC.To);
                    Directory.CreateDirectory(SaveDir);

                    System.IO.FileInfo file = new System.IO.FileInfo(TomIFC.From);
                    file.CopyTo(SaveDir + "\\" + NewName + ".ifc", true);
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
