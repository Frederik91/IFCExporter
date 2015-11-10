namespace IFCExporter.Forms
{
    partial class ControlForm
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
            this.Start_stop_button = new System.Windows.Forms.Button();
            this.Quit_button = new System.Windows.Forms.Button();
            this.Status_label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Start_stop_button
            // 
            this.Start_stop_button.Location = new System.Drawing.Point(114, 31);
            this.Start_stop_button.Name = "Start_stop_button";
            this.Start_stop_button.Size = new System.Drawing.Size(86, 48);
            this.Start_stop_button.TabIndex = 0;
            this.Start_stop_button.Text = "Start/Stop";
            this.Start_stop_button.UseVisualStyleBackColor = true;
            this.Start_stop_button.Click += new System.EventHandler(this.Start_stop_button_Click);
            // 
            // Quit_button
            // 
            this.Quit_button.Location = new System.Drawing.Point(12, 31);
            this.Quit_button.Name = "Quit_button";
            this.Quit_button.Size = new System.Drawing.Size(86, 48);
            this.Quit_button.TabIndex = 1;
            this.Quit_button.Text = "Quit";
            this.Quit_button.UseVisualStyleBackColor = true;
            this.Quit_button.Click += new System.EventHandler(this.Quit_button_Click);
            // 
            // Status_label
            // 
            this.Status_label.AutoSize = true;
            this.Status_label.Location = new System.Drawing.Point(58, 9);
            this.Status_label.Name = "Status_label";
            this.Status_label.Size = new System.Drawing.Size(0, 13);
            this.Status_label.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Status:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(54, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 39);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // ControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(212, 145);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Status_label);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Quit_button);
            this.Controls.Add(this.Start_stop_button);
            this.Name = "ControlForm";
            this.Text = "ControlForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Start_stop_button;
        private System.Windows.Forms.Button Quit_button;
        private System.Windows.Forms.Label Status_label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}