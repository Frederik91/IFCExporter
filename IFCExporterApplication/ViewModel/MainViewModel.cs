using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
using IFCExporterApplication.Models;
using IFCManager.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IFCExporterApplication.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        List<ExportApp> ExportAppList = new List<ExportApp>();

        private ICommand m_startAutoCAD;
        private ICommand m_closeAutoCad;

        public ICommand startAutoCADCommand
        {
            get { return m_startAutoCAD; }
            set { m_startAutoCAD = value; OnPropertyChanged("startAutoCADCommand"); }
        }

        public ICommand closeAutoCADCommand
        {
            get { return m_closeAutoCad; }
            set { m_closeAutoCad = value; OnPropertyChanged("closeAutoCADCommand"); }
        }

        public MainViewModel()
        {
            m_startAutoCAD = new DelegateCommand(startAutoCAD);
            m_closeAutoCad = new DelegateCommand(closeAutoCAD);
        }

        private void startAutoCAD()
        {
            var export = new ExportApp();

            export.filePath = @"H:\IFCEXPORT\XML\AutoExportUNN.xml";
            export.name = Path.GetFileNameWithoutExtension(export.filePath);


            AcadApplication app = null;

            try
            {
                app = new AcadApplication();
            }
            catch (Exception) { }


            while (!app.Visible)
            {
                try
                {
                    app.Visible = true;
                }
                catch (Exception)
                {
                }
            }

            startExport(export);
        }

        private void startExport(ExportApp exp)
        {
            while (true)
            {
                try
                {
                    exp.app.ActiveDocument.SendCommand("IFCExporterAuto" + " " + exp.filePath + " ");
                    break;
                }
                catch (Exception)
                {
                }
            }
        }

        private void closeAutoCAD()
        {
            foreach (var export in ExportAppList)
            {

                Object acadObject = export.app;
                Object ActiveDocument = acadObject.GetType().InvokeMember("ActiveDocument", BindingFlags.GetProperty, null, acadObject, null);

                object[] dataArry = new object[2];
                dataArry[0] = false; //no save
                dataArry[1] = ""; //drawing file name.. if saving
                ActiveDocument.GetType().InvokeMember("close", BindingFlags.InvokeMethod, null, ActiveDocument, dataArry);
                export.app.Quit();
            }
        }
    }
}
