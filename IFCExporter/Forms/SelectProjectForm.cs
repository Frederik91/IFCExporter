using IFCExporter.Helpers;
using IFCExporter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFCExporter.Forms
{
    public partial class SelectProjectForm : Form
    {
        public List<string> ExportsToRun { get; set; }
        public string SelectedProject { get; set; }
        public bool RunForeverBool { get; set; }
        private IFCProjectInfo projectInfo;
        public string XMLPath { get; set; }

        public SelectProjectForm()
        {
            ExportsToRun = new List<string>();

            InitializeComponent();
            checkedListBox1.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void ProjectSelectedButton_Click(object sender, EventArgs e)
        {
            RunForeverBool = RunForeverCheckBox.Checked;

            ExportsToRun.Clear();
            if (AllExports_Checkbox.Checked)
            {
                foreach (var Discipline in projectInfo.Disciplines)
                {
                    foreach (var Export in Discipline.Exports)
                    {
                        ExportsToRun.Add(Export.Name);
                    }
                }
            }
            else
            {
                foreach (var SelectedExport in checkedListBox1.CheckedItems)
                {
                    ExportsToRun.Add(SelectedExport.ToString());
                }
            }

            ActiveForm.Close();
        }

        private void RunForeverCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Browse_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog FD = new OpenFileDialog();
            FD.Filter = "XML-files (*.xml)|*.xml";
            FD.ShowDialog();
            XMLPath = FD.FileName;
            SelectedFilePath_TextBox.Text = FD.FileName;
            SelectedProject = Path.GetFileNameWithoutExtension(FD.FileName);

            try
            {
                XMLReader reader = new XMLReader();
                projectInfo = reader.GetprojectInfo(FD.FileName);

                checkedListBox1.Items.Clear();

                foreach (var Discipline in projectInfo.Disciplines)
                {

                    foreach (var Export in Discipline.Exports)
                    {
                        checkedListBox1.Items.Add(Export.Name);
                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to read XML-file");
                throw;
            }

            checkedListBox1.Enabled = true;

        }

        private void AllExports_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (AllExports_Checkbox.Checked)
            {
                checkedListBox1.Enabled = false;
            }
            else
            {
                checkedListBox1.Enabled = true;
            }

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }


}
