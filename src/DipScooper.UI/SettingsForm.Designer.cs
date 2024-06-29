namespace DipScooper.UI
{
    partial class SettingsForm
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
            txtNormalDipThreshold = new DevExpress.XtraEditors.TextEdit();
            txtBigDipThreshold = new DevExpress.XtraEditors.TextEdit();
            txtSuperDipThreshold = new DevExpress.XtraEditors.TextEdit();
            lblControlNormalDip = new DevExpress.XtraEditors.LabelControl();
            lblControlBigDip = new DevExpress.XtraEditors.LabelControl();
            lblControlSuperDip = new DevExpress.XtraEditors.LabelControl();
            BtnSaveSettings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)txtNormalDipThreshold.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtBigDipThreshold.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSuperDipThreshold.Properties).BeginInit();
            SuspendLayout();
            // 
            // txtNormalDipThreshold
            // 
            txtNormalDipThreshold.Location = new System.Drawing.Point(144, 16);
            txtNormalDipThreshold.Name = "txtNormalDipThreshold";
            txtNormalDipThreshold.Properties.Appearance.BackColor = System.Drawing.Color.Black;
            txtNormalDipThreshold.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtNormalDipThreshold.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            txtNormalDipThreshold.Properties.Appearance.Options.UseBackColor = true;
            txtNormalDipThreshold.Properties.Appearance.Options.UseFont = true;
            txtNormalDipThreshold.Properties.Appearance.Options.UseForeColor = true;
            txtNormalDipThreshold.Size = new System.Drawing.Size(100, 22);
            txtNormalDipThreshold.TabIndex = 1;
            // 
            // txtBigDipThreshold
            // 
            txtBigDipThreshold.Location = new System.Drawing.Point(144, 44);
            txtBigDipThreshold.Name = "txtBigDipThreshold";
            txtBigDipThreshold.Properties.Appearance.BackColor = System.Drawing.Color.Black;
            txtBigDipThreshold.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtBigDipThreshold.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            txtBigDipThreshold.Properties.Appearance.Options.UseBackColor = true;
            txtBigDipThreshold.Properties.Appearance.Options.UseFont = true;
            txtBigDipThreshold.Properties.Appearance.Options.UseForeColor = true;
            txtBigDipThreshold.Size = new System.Drawing.Size(100, 22);
            txtBigDipThreshold.TabIndex = 2;
            // 
            // txtSuperDipThreshold
            // 
            txtSuperDipThreshold.Location = new System.Drawing.Point(144, 71);
            txtSuperDipThreshold.Name = "txtSuperDipThreshold";
            txtSuperDipThreshold.Properties.Appearance.BackColor = System.Drawing.Color.Black;
            txtSuperDipThreshold.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtSuperDipThreshold.Properties.Appearance.ForeColor = System.Drawing.Color.White;
            txtSuperDipThreshold.Properties.Appearance.Options.UseBackColor = true;
            txtSuperDipThreshold.Properties.Appearance.Options.UseFont = true;
            txtSuperDipThreshold.Properties.Appearance.Options.UseForeColor = true;
            txtSuperDipThreshold.Size = new System.Drawing.Size(100, 22);
            txtSuperDipThreshold.TabIndex = 3;
            // 
            // lblControlNormalDip
            // 
            lblControlNormalDip.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblControlNormalDip.Appearance.ForeColor = System.Drawing.Color.White;
            lblControlNormalDip.Appearance.Options.UseFont = true;
            lblControlNormalDip.Appearance.Options.UseForeColor = true;
            lblControlNormalDip.Location = new System.Drawing.Point(23, 19);
            lblControlNormalDip.Name = "lblControlNormalDip";
            lblControlNormalDip.Size = new System.Drawing.Size(115, 13);
            lblControlNormalDip.TabIndex = 4;
            lblControlNormalDip.Text = "Normal Dip Threshold:";
            // 
            // lblControlBigDip
            // 
            lblControlBigDip.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblControlBigDip.Appearance.ForeColor = System.Drawing.Color.White;
            lblControlBigDip.Appearance.Options.UseFont = true;
            lblControlBigDip.Appearance.Options.UseForeColor = true;
            lblControlBigDip.Location = new System.Drawing.Point(23, 47);
            lblControlBigDip.Name = "lblControlBigDip";
            lblControlBigDip.Size = new System.Drawing.Size(95, 13);
            lblControlBigDip.TabIndex = 5;
            lblControlBigDip.Text = "Big Dip Threshold:";
            // 
            // lblControlSuperDip
            // 
            lblControlSuperDip.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblControlSuperDip.Appearance.ForeColor = System.Drawing.Color.White;
            lblControlSuperDip.Appearance.Options.UseFont = true;
            lblControlSuperDip.Appearance.Options.UseForeColor = true;
            lblControlSuperDip.Location = new System.Drawing.Point(23, 75);
            lblControlSuperDip.Name = "lblControlSuperDip";
            lblControlSuperDip.Size = new System.Drawing.Size(111, 13);
            lblControlSuperDip.TabIndex = 6;
            lblControlSuperDip.Text = "SUPER-DIP Threshold:";
            // 
            // BtnSaveSettings
            // 
            BtnSaveSettings.BackColor = System.Drawing.Color.Black;
            BtnSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnSaveSettings.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            BtnSaveSettings.ForeColor = System.Drawing.Color.Gold;
            BtnSaveSettings.Location = new System.Drawing.Point(76, 135);
            BtnSaveSettings.Name = "BtnSaveSettings";
            BtnSaveSettings.Size = new System.Drawing.Size(114, 27);
            BtnSaveSettings.TabIndex = 8;
            BtnSaveSettings.Text = "Save";
            BtnSaveSettings.UseVisualStyleBackColor = false;
            BtnSaveSettings.Click += BtnSaveSettings_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.DarkSlateGray;
            ClientSize = new System.Drawing.Size(267, 174);
            Controls.Add(BtnSaveSettings);
            Controls.Add(lblControlSuperDip);
            Controls.Add(lblControlBigDip);
            Controls.Add(lblControlNormalDip);
            Controls.Add(txtSuperDipThreshold);
            Controls.Add(txtBigDipThreshold);
            Controls.Add(txtNormalDipThreshold);
            Name = "SettingsForm";
            Text = "SettingsForm";
            Load += SettingsForm_Load;
            ((System.ComponentModel.ISupportInitialize)txtNormalDipThreshold.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtBigDipThreshold.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSuperDipThreshold.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtNormalDipThreshold;
        private DevExpress.XtraEditors.TextEdit txtBigDipThreshold;
        private DevExpress.XtraEditors.TextEdit txtSuperDipThreshold;
        private DevExpress.XtraEditors.LabelControl lblControlNormalDip;
        private DevExpress.XtraEditors.LabelControl lblControlBigDip;
        private DevExpress.XtraEditors.LabelControl lblControlSuperDip;
        private System.Windows.Forms.Button BtnSaveSettings;
    }
}