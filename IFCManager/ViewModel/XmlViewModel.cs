using Autodesk.AutoCAD.Interop;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IFCManager.ViewModel
{
    public class XmlViewModel : ViewModelBase
    {
        public XmlViewModel()
        {
            TestCommand = new DelegateCommand(TestMethod);
        }
        public ICommand TestCommand { get; set; }

        public void TestMethod()
        {
            const string progID = "AutoCAD.Application";

            AcadApplication acApp = null;
            try
            {
                acApp =
                  (AcadApplication)Marshal.GetActiveObject(progID);
            }
            catch
            {
                try
                {
                    Type acType =
                      Type.GetTypeFromProgID(progID);
                    acApp =
                      (AcadApplication)Activator.CreateInstance(
                        acType,
                        true
                      );
                }
                catch
                {
                    MessageBox.Show(
                      "Cannot create object of type \"" +
                      progID + "\""
                    );
                }
            }
            if (acApp != null)
            {
                // By the time this is reached AutoCAD is fully
                // functional and can be interacted with through code
                acApp.Visible = true;
                acApp.ActiveDocument.SendCommand("_NETLOAD ");
            }
        }

    }
}
