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
        public void WriteArray(string[] text)
        {
            CheckFileSize();

            var textArray = new List<string>();

            foreach (var line in text)
            {
                textArray.Add(DateTime.Now + " @ " + line);
            }

            File.AppendAllLines(DataStorage.LogFileLocation, textArray);

        }

        public void RemoveLastLine(string[] text)
        {
            CheckFileSize();

            var newTextfile = new List<string>();
            for (int i = 0; i < text.Length - 1; i++)
            {
                newTextfile.Add(text[i]);
            }

            File.Delete(DataStorage.LogFileLocation);

            try
            {
                File.AppendAllLines(DataStorage.LogFileLocation, newTextfile.ToArray());
            }
            catch {}



        }

        public void WriteLine(params string[] text)
        {
            CheckFileSize();

            var textArray = new List<string>();

            foreach (var line in text)
            {
                textArray.Add(DateTime.Now + " @ " + line);
            }

            try
            {
                File.AppendAllLines(DataStorage.LogFileLocation, textArray);
            }
            catch {}


        }

        private void CheckFileSize()
        {
            if (File.Exists(DataStorage.LogFileLocation))
            {
                var file = new FileInfo(DataStorage.LogFileLocation);

                if (file.Length > 1048576)
                {
                    file.Delete();
                }
            }
        }
    }


}
