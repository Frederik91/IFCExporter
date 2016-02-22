using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCExporter.Helpers
{
    public class Writer
    {
        public void writeArray(string[] text)
        {
            checkFileSize();

            var textArray = new List<string>();

            foreach (var line in text)
            {
                textArray.Add(DateTime.Now + " @ " + line);
            }

            File.AppendAllLines(DataStorage.logFileLocation, textArray);

        }

        public void removeLastLine(string[] text)
        {
            checkFileSize();

            var newTextfile = new List<string>();
            for (int i = 0; i < text.Length - 1; i++)
            {
                newTextfile.Add(text[i]);
            }

            File.Delete(DataStorage.logFileLocation);

            File.AppendAllLines(DataStorage.logFileLocation, newTextfile.ToArray());

        }

        public void writeLine(params string[] text)
        {
            checkFileSize();

            var textArray = new List<string>();

            foreach (var line in text)
            {
                textArray.Add(DateTime.Now + " @ " + line);
            }

            File.AppendAllLines(DataStorage.logFileLocation, textArray);
        }

        private void checkFileSize()
        {
            if (File.Exists(DataStorage.logFileLocation))
            {
                var file = new FileInfo(DataStorage.logFileLocation);

                if (file.Length > 1048576)
                {
                    file.Delete();
                }
            }
        }
    }


}
