﻿using IFCExporter.Helpers;
using IFCExporter.Models;
using IFCExporterAPI.Assets;
using IFCExporterAPI.Models;
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
        private IfcProjectInfo projectInfo;
        public string XMLPath { get; set; }
        public bool AutomaticMode { get; set; }

        public SelectProjectForm()
        {
            ExportsToRun = new List<string>();
            var path = @"H:\IFCEXPORT\XML\BUS2Test.xml";

            InitializeComponent();

            XMLPath = path;
            checkedListBox1.Enabled = false;
            SelectedFilePath_TextBox.Text = path;
            AutoModeCheckBox.Checked = true;
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
                XmlReader reader = new XmlReader();
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

        private void AutoModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoModeCheckBox.Checked)
            {
                AllExports_Checkbox.Enabled = false;
                checkedListBox1.Enabled = false;
                RunForeverCheckBox.Enabled = false;
                AutomaticMode = true;
            }

            if (!AutoModeCheckBox.Checked)
            {
                AllExports_Checkbox.Enabled = true;
                checkedListBox1.Enabled = true;
                RunForeverCheckBox.Enabled = true;
                AutomaticMode = false;
            }
        }
    }


}
