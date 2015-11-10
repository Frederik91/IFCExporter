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
    public partial class ControlForm : Form
    {
        public bool RunBool { get; set; }
        public bool QuitBool { get; set; }
        public string Status
        {
            get { return label2.Text; }
            set
            {
                label2.Text = value;
            }
        }

            public ControlForm()
        {
                InitializeComponent();
                RunBool = false;
                QuitBool = false;
            }

            private void Start_stop_button_Click(object sender, EventArgs e)
        {
            RunBool = !RunBool;

            switch (RunBool)
            {
                case (true):
                    Status_label.Text = "Running";
                    Status_label.ForeColor = Color.Green;
                    break;
                case (false):
                    Status_label.Text = "Stopping";
                    Status_label.ForeColor = Color.Red;
                    break;
            }

        }

        private void Quit_button_Click(object sender, EventArgs e)
        {
            QuitBool = !QuitBool;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
