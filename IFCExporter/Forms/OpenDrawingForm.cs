using IFCExporter.Helpers;
using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFCExporter.Forms
{
    public partial class OpenDrawingForm : Form
    {
        private string m_curTo = "";
        private Discipline Disc;
        OpenActivateClass DM = new OpenActivateClass();

        public OpenDrawingForm(Discipline _Disc)
        {
            Disc = _Disc;
            InitializeComponent();
        }

        public void OpenDrawing()
        {
            m_curTo = Disc.StartFile.To;


            this.BeginInvoke(new Action(lol)); // la til denne


            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument = DM.ReturnActivateDrawing(Disc.StartFile.To);
        }

        private void lol()// la til denne
        {
            DM.OpenDrawing(m_curTo);
        }
    }
}
