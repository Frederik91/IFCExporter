namespace IFCExporter.Forms
{
    partial class SelectProjectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProjectSelectedButton = new System.Windows.Forms.Button();
            this.RunForeverCheckBox = new System.Windows.Forms.CheckBox();
            this.Browse_Button = new System.Windows.Forms.Button();
            this.SelectedFilePath_TextBox = new System.Windows.Forms.TextBox();
            this.AllExports_Checkbox = new System.Windows.Forms.CheckBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.AutoModeCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ProjectSelectedButton
            // 
            this.ProjectSelectedButton.Location = new System.Drawing.Point(202, 36);
            this.ProjectSelectedButton.Name = "ProjectSelectedButton";
            this.ProjectSelectedButton.Size = new System.Drawing.Size(75, 23);
            this.ProjectSelectedButton.TabIndex = 1;
            this.ProjectSelectedButton.Text = "OK";
            this.ProjectSelectedButton.UseVisualStyleBackColor = true;
            this.ProjectSelectedButton.Click += new System.EventHandler(this.ProjectSelectedButton_Click);
            // 
            // RunForeverCheckBox
            // 
            this.RunForeverCheckBox.AutoSize = true;
            this.RunForeverCheckBox.Location = new System.Drawing.Point(195, 72);
            this.RunForeverCheckBox.Name = "RunForeverCheckBox";
            this.RunForeverCheckBox.Size = new System.Drawing.Size(82, 17);
            this.RunForeverCheckBox.TabIndex = 2;
            this.RunForeverCheckBox.Text = "Run forever";
            this.RunForeverCheckBox.UseVisualStyleBackColor = true;
            this.RunForeverCheckBox.CheckedChanged += new System.EventHandler(this.RunForeverCheckBox_CheckedChanged);
            // 
            // Browse_Button
            // 
            this.Browse_Button.Location = new System.Drawing.Point(202, 7);
            this.Browse_Button.Name = "Browse_Button";
            this.Browse_Button.Size = new System.Drawing.Size(75, 23);
            this.Browse_Button.TabIndex = 3;
            this.Browse_Button.Text = "Browse";
            this.Browse_Button.UseVisualStyleBackColor = true;
            this.Browse_Button.Click += new System.EventHandler(this.Browse_Button_Click);
            // 
            // SelectedFilePath_TextBox
            // 
            this.SelectedFilePath_TextBox.Location = new System.Drawing.Point(13, 9);
            this.SelectedFilePath_TextBox.Name = "SelectedFilePath_TextBox";
            this.SelectedFilePath_TextBox.Size = new System.Drawing.Size(183, 20);
            this.SelectedFilePath_TextBox.TabIndex = 4;
            // 
            // AllExports_Checkbox
            // 
            this.AllExports_Checkbox.AutoSize = true;
            this.AllExports_Checkbox.Location = new System.Drawing.Point(13, 72);
            this.AllExports_Checkbox.Name = "AllExports_Checkbox";
            this.AllExports_Checkbox.Size = new System.Drawing.Size(75, 17);
            this.AllExports_Checkbox.TabIndex = 5;
            this.AllExports_Checkbox.Text = "All Exports";
            this.AllExports_Checkbox.UseVisualStyleBackColor = true;
            this.AllExports_Checkbox.CheckedChanged += new System.EventHandler(this.AllExports_Checkbox_CheckedChanged);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(13, 95);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(282, 184);
            this.checkedListBox1.TabIndex = 6;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // AutoModeCheckBox
            // 
            this.AutoModeCheckBox.AutoSize = true;
            this.AutoModeCheckBox.Location = new System.Drawing.Point(13, 40);
            this.AutoModeCheckBox.Name = "AutoModeCheckBox";
            this.AutoModeCheckBox.Size = new System.Drawing.Size(102, 17);
            this.AutoModeCheckBox.TabIndex = 7;
            this.AutoModeCheckBox.Text = "Automatic mode";
            this.AutoModeCheckBox.UseVisualStyleBackColor = true;
            this.AutoModeCheckBox.CheckedChanged += new System.EventHandler(this.AutoModeCheckBox_CheckedChanged);
            // 
            // SelectProjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 293);
            this.Controls.Add(this.AutoModeCheckBox);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.AllExports_Checkbox);
            this.Controls.Add(this.SelectedFilePath_TextBox);
            this.Controls.Add(this.Browse_Button);
            this.Controls.Add(this.RunForeverCheckBox);
            this.Controls.Add(this.ProjectSelectedButton);
            this.Name = "SelectProjectForm";
            this.Text = "Select project";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ProjectSelectedButton;
        private System.Windows.Forms.CheckBox RunForeverCheckBox;
        private System.Windows.Forms.Button Browse_Button;
        private System.Windows.Forms.TextBox SelectedFilePath_TextBox;
        private System.Windows.Forms.CheckBox AllExports_Checkbox;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.CheckBox AutoModeCheckBox;
    }
}