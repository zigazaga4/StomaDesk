namespace StomaDesk.Forms
{
    partial class SettingsView
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupDoctors = new System.Windows.Forms.GroupBox();
            this.gridDoctors = new System.Windows.Forms.DataGridView();
            this.panelDoctorTools = new System.Windows.Forms.Panel();
            this.btnDoctorColor = new System.Windows.Forms.Button();
            this.lblDoctorHint = new System.Windows.Forms.Label();
            this.groupProcedures = new System.Windows.Forms.GroupBox();
            this.gridProcedures = new System.Windows.Forms.DataGridView();
            this.lblProcedureHint = new System.Windows.Forms.Label();
            this.groupClinic = new System.Windows.Forms.GroupBox();
            this.lblClinicName = new System.Windows.Forms.Label();
            this.txtClinicName = new System.Windows.Forms.TextBox();
            this.lblClinicAddress = new System.Windows.Forms.Label();
            this.txtClinicAddress = new System.Windows.Forms.TextBox();
            this.lblClinicPhone = new System.Windows.Forms.Label();
            this.txtClinicPhone = new System.Windows.Forms.TextBox();
            this.lblClinicFiscal = new System.Windows.Forms.Label();
            this.txtClinicFiscal = new System.Windows.Forms.TextBox();
            this.lblDataFile = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRevert = new System.Windows.Forms.Button();
            this.groupDoctors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDoctors)).BeginInit();
            this.panelDoctorTools.SuspendLayout();
            this.groupProcedures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcedures)).BeginInit();
            this.groupClinic.SuspendLayout();
            this.SuspendLayout();
            //
            // groupDoctors
            //
            this.groupDoctors.Controls.Add(this.gridDoctors);
            this.groupDoctors.Controls.Add(this.panelDoctorTools);
            this.groupDoctors.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupDoctors.Location = new System.Drawing.Point(0, 0);
            this.groupDoctors.Name = "groupDoctors";
            this.groupDoctors.Padding = new System.Windows.Forms.Padding(8);
            this.groupDoctors.Size = new System.Drawing.Size(470, 529);
            this.groupDoctors.TabIndex = 0;
            this.groupDoctors.TabStop = false;
            this.groupDoctors.Text = "Medici";
            //
            // gridDoctors
            //
            this.gridDoctors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDoctors.Location = new System.Drawing.Point(8, 24);
            this.gridDoctors.Name = "gridDoctors";
            this.gridDoctors.Size = new System.Drawing.Size(454, 441);
            this.gridDoctors.TabIndex = 0;
            this.gridDoctors.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridDoctors_CellFormatting);
            this.gridDoctors.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_Edited);
            this.gridDoctors.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.Grid_RowsEdited);
            this.gridDoctors.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.Grid_RowsEdited);
            this.gridDoctors.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.gridDoctors_UserDeletingRow);
            //
            // panelDoctorTools
            //
            this.panelDoctorTools.Controls.Add(this.btnDoctorColor);
            this.panelDoctorTools.Controls.Add(this.lblDoctorHint);
            this.panelDoctorTools.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDoctorTools.Location = new System.Drawing.Point(8, 465);
            this.panelDoctorTools.Name = "panelDoctorTools";
            this.panelDoctorTools.Size = new System.Drawing.Size(454, 56);
            this.panelDoctorTools.TabIndex = 1;
            //
            // btnDoctorColor
            //
            this.btnDoctorColor.Location = new System.Drawing.Point(0, 8);
            this.btnDoctorColor.Name = "btnDoctorColor";
            this.btnDoctorColor.Size = new System.Drawing.Size(140, 30);
            this.btnDoctorColor.TabIndex = 0;
            this.btnDoctorColor.Text = "Culoare în agendă...";
            this.btnDoctorColor.UseVisualStyleBackColor = true;
            this.btnDoctorColor.Click += new System.EventHandler(this.btnDoctorColor_Click);
            //
            // lblDoctorHint
            //
            this.lblDoctorHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblDoctorHint.Location = new System.Drawing.Point(148, 4);
            this.lblDoctorHint.Name = "lblDoctorHint";
            this.lblDoctorHint.Size = new System.Drawing.Size(300, 40);
            this.lblDoctorHint.TabIndex = 1;
            this.lblDoctorHint.Text = "Un medic plecat se debifează la „Activ”: dispare din agendă, istoricul rămâne.";
            //
            // groupProcedures
            //
            this.groupProcedures.Controls.Add(this.gridProcedures);
            this.groupProcedures.Controls.Add(this.lblProcedureHint);
            this.groupProcedures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupProcedures.Location = new System.Drawing.Point(470, 0);
            this.groupProcedures.Name = "groupProcedures";
            this.groupProcedures.Padding = new System.Windows.Forms.Padding(8);
            this.groupProcedures.Size = new System.Drawing.Size(694, 529);
            this.groupProcedures.TabIndex = 1;
            this.groupProcedures.TabStop = false;
            this.groupProcedures.Text = "Listă de prețuri";
            //
            // gridProcedures
            //
            this.gridProcedures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridProcedures.Location = new System.Drawing.Point(8, 24);
            this.gridProcedures.Name = "gridProcedures";
            this.gridProcedures.Size = new System.Drawing.Size(678, 461);
            this.gridProcedures.TabIndex = 0;
            this.gridProcedures.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Grid_Edited);
            this.gridProcedures.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.Grid_RowsEdited);
            this.gridProcedures.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.Grid_RowsEdited);
            //
            // lblProcedureHint
            //
            this.lblProcedureHint.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblProcedureHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblProcedureHint.Location = new System.Drawing.Point(8, 485);
            this.lblProcedureHint.Name = "lblProcedureHint";
            this.lblProcedureHint.Size = new System.Drawing.Size(678, 36);
            this.lblProcedureHint.TabIndex = 1;
            this.lblProcedureHint.Text = "Un preț nou se aplică doar procedurilor planificate de acum înainte; planurile existente își păstrează prețul.";
            this.lblProcedureHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // groupClinic
            //
            this.groupClinic.Controls.Add(this.lblClinicName);
            this.groupClinic.Controls.Add(this.txtClinicName);
            this.groupClinic.Controls.Add(this.lblClinicAddress);
            this.groupClinic.Controls.Add(this.txtClinicAddress);
            this.groupClinic.Controls.Add(this.lblClinicPhone);
            this.groupClinic.Controls.Add(this.txtClinicPhone);
            this.groupClinic.Controls.Add(this.lblClinicFiscal);
            this.groupClinic.Controls.Add(this.txtClinicFiscal);
            this.groupClinic.Controls.Add(this.lblDataFile);
            this.groupClinic.Controls.Add(this.btnSave);
            this.groupClinic.Controls.Add(this.btnRevert);
            this.groupClinic.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupClinic.Location = new System.Drawing.Point(0, 529);
            this.groupClinic.Name = "groupClinic";
            this.groupClinic.Size = new System.Drawing.Size(1164, 140);
            this.groupClinic.TabIndex = 2;
            this.groupClinic.TabStop = false;
            this.groupClinic.Text = "Date clinică (apar pe deviz și în SMS)";
            //
            // lblClinicName
            //
            this.lblClinicName.AutoSize = true;
            this.lblClinicName.Location = new System.Drawing.Point(14, 31);
            this.lblClinicName.Name = "lblClinicName";
            this.lblClinicName.Size = new System.Drawing.Size(43, 15);
            this.lblClinicName.TabIndex = 0;
            this.lblClinicName.Text = "Nume:";
            //
            // txtClinicName
            //
            this.txtClinicName.Location = new System.Drawing.Point(100, 28);
            this.txtClinicName.Name = "txtClinicName";
            this.txtClinicName.Size = new System.Drawing.Size(360, 23);
            this.txtClinicName.TabIndex = 1;
            this.txtClinicName.TextChanged += new System.EventHandler(this.Field_Edited);
            //
            // lblClinicAddress
            //
            this.lblClinicAddress.AutoSize = true;
            this.lblClinicAddress.Location = new System.Drawing.Point(14, 64);
            this.lblClinicAddress.Name = "lblClinicAddress";
            this.lblClinicAddress.Size = new System.Drawing.Size(46, 15);
            this.lblClinicAddress.TabIndex = 2;
            this.lblClinicAddress.Text = "Adresă:";
            //
            // txtClinicAddress
            //
            this.txtClinicAddress.Location = new System.Drawing.Point(100, 61);
            this.txtClinicAddress.Name = "txtClinicAddress";
            this.txtClinicAddress.Size = new System.Drawing.Size(360, 23);
            this.txtClinicAddress.TabIndex = 3;
            this.txtClinicAddress.TextChanged += new System.EventHandler(this.Field_Edited);
            //
            // lblClinicPhone
            //
            this.lblClinicPhone.AutoSize = true;
            this.lblClinicPhone.Location = new System.Drawing.Point(490, 31);
            this.lblClinicPhone.Name = "lblClinicPhone";
            this.lblClinicPhone.Size = new System.Drawing.Size(48, 15);
            this.lblClinicPhone.TabIndex = 4;
            this.lblClinicPhone.Text = "Telefon:";
            //
            // txtClinicPhone
            //
            this.txtClinicPhone.Location = new System.Drawing.Point(560, 28);
            this.txtClinicPhone.Name = "txtClinicPhone";
            this.txtClinicPhone.Size = new System.Drawing.Size(180, 23);
            this.txtClinicPhone.TabIndex = 5;
            this.txtClinicPhone.TextChanged += new System.EventHandler(this.Field_Edited);
            //
            // lblClinicFiscal
            //
            this.lblClinicFiscal.AutoSize = true;
            this.lblClinicFiscal.Location = new System.Drawing.Point(490, 64);
            this.lblClinicFiscal.Name = "lblClinicFiscal";
            this.lblClinicFiscal.Size = new System.Drawing.Size(31, 15);
            this.lblClinicFiscal.TabIndex = 6;
            this.lblClinicFiscal.Text = "CUI:";
            //
            // txtClinicFiscal
            //
            this.txtClinicFiscal.Location = new System.Drawing.Point(560, 61);
            this.txtClinicFiscal.Name = "txtClinicFiscal";
            this.txtClinicFiscal.Size = new System.Drawing.Size(180, 23);
            this.txtClinicFiscal.TabIndex = 7;
            this.txtClinicFiscal.TextChanged += new System.EventHandler(this.Field_Edited);
            //
            // lblDataFile
            //
            this.lblDataFile.AutoSize = true;
            this.lblDataFile.ForeColor = System.Drawing.Color.DimGray;
            this.lblDataFile.Location = new System.Drawing.Point(14, 104);
            this.lblDataFile.Name = "lblDataFile";
            this.lblDataFile.Size = new System.Drawing.Size(90, 15);
            this.lblDataFile.TabIndex = 8;
            this.lblDataFile.Text = "Fișier de date:";
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(846, 26);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(170, 32);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Salvează modificările";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnRevert
            //
            this.btnRevert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRevert.Location = new System.Drawing.Point(1022, 26);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(130, 32);
            this.btnRevert.TabIndex = 10;
            this.btnRevert.Text = "Renunță";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            //
            // SettingsView
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.groupProcedures);
            this.Controls.Add(this.groupDoctors);
            this.Controls.Add(this.groupClinic);
            this.Name = "SettingsView";
            this.Size = new System.Drawing.Size(1164, 669);
            this.groupDoctors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDoctors)).EndInit();
            this.panelDoctorTools.ResumeLayout(false);
            this.groupProcedures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridProcedures)).EndInit();
            this.groupClinic.ResumeLayout(false);
            this.groupClinic.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupDoctors;
        private System.Windows.Forms.DataGridView gridDoctors;
        private System.Windows.Forms.Panel panelDoctorTools;
        private System.Windows.Forms.Button btnDoctorColor;
        private System.Windows.Forms.Label lblDoctorHint;
        private System.Windows.Forms.GroupBox groupProcedures;
        private System.Windows.Forms.DataGridView gridProcedures;
        private System.Windows.Forms.Label lblProcedureHint;
        private System.Windows.Forms.GroupBox groupClinic;
        private System.Windows.Forms.Label lblClinicName;
        private System.Windows.Forms.TextBox txtClinicName;
        private System.Windows.Forms.Label lblClinicAddress;
        private System.Windows.Forms.TextBox txtClinicAddress;
        private System.Windows.Forms.Label lblClinicPhone;
        private System.Windows.Forms.TextBox txtClinicPhone;
        private System.Windows.Forms.Label lblClinicFiscal;
        private System.Windows.Forms.TextBox txtClinicFiscal;
        private System.Windows.Forms.Label lblDataFile;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRevert;
    }
}
