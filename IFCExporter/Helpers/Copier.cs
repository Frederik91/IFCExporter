using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Helpers
{
    public class Copier
    {
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
            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                string temppath = Path.Combine(DestDir, file.Name);

                if (Path.GetExtension(file.Name) == FileType)
                {
                    file.CopyTo(temppath, true);
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

        public void CopySingleFile_NewName(string SourceDir, string DestDir, string NewName)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(SourceDir);
            file.CopyTo(DestDir + NewName, true);
        }

        public void CopySingleFile(string SourceDir, string DestDir)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(SourceDir);
            file.CopyTo(DestDir, true);
        }

        public void TomIFCCopy(IFCFile TomIFC, string NewName)
        {
            var SaveDir = Path.GetDirectoryName(TomIFC.To);
            Directory.CreateDirectory(SaveDir);

            System.IO.FileInfo file = new System.IO.FileInfo(TomIFC.From);
            file.CopyTo(SaveDir + "\\" + NewName + ".ifc", true);
        }
    }

}
