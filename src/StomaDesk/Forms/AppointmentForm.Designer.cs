namespace StomaDesk.Forms
{
    partial class AppointmentForm
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
            this.lblPatient = new System.Windows.Forms.Label();
            this.cboPatient = new System.Windows.Forms.ComboBox();
            this.lblDoctor = new System.Windows.Forms.Label();
            this.cboDoctor = new System.Windows.Forms.ComboBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblTime = new System.Windows.Forms.Label();
            this.cboTime = new System.Windows.Forms.ComboBox();
            this.lblDuration = new System.Windows.Forms.Label();
            this.numDuration = new System.Windows.Forms.NumericUpDown();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblReason = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.lblConflict = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            this.SuspendLayout();
            //
            // lblPatient
            //
            this.lblPatient.AutoSize = true;
            this.lblPatient.Location = new System.Drawing.Point(16, 19);
            this.lblPatient.Name = "lblPatient";
            this.lblPatient.Size = new System.Drawing.Size(47, 15);
            this.lblPatient.TabIndex = 0;
            this.lblPatient.Text = "Pacient";
            //
            // cboPatient
            //
            this.cboPatient.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboPatient.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboPatient.Location = new System.Drawing.Point(120, 16);
            this.cboPatient.MaxDropDownItems = 15;
            this.cboPatient.Name = "cboPatient";
            this.cboPatient.Size = new System.Drawing.Size(330, 23);
            this.cboPatient.TabIndex = 1;
            //
            // lblDoctor
            //
            this.lblDoctor.AutoSize = true;
            this.lblDoctor.Location = new System.Drawing.Point(16, 52);
            this.lblDoctor.Name = "lblDoctor";
            this.lblDoctor.Size = new System.Drawing.Size(42, 15);
            this.lblDoctor.TabIndex = 2;
            this.lblDoctor.Text = "Medic";
            //
            // cboDoctor
            //
            this.cboDoctor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDoctor.Location = new System.Drawing.Point(120, 49);
            this.cboDoctor.Name = "cboDoctor";
            this.cboDoctor.Size = new System.Drawing.Size(330, 23);
            this.cboDoctor.TabIndex = 3;
            this.cboDoctor.SelectedIndexChanged += new System.EventHandler(this.Inputs_Changed);
            //
            // lblDate
            //
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(16, 85);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(31, 15);
            this.lblDate.TabIndex = 4;
            this.lblDate.Text = "Data";
            //
            // dtpDate
            //
            this.dtpDate.CustomFormat = "dd.MM.yyyy";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(120, 82);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(130, 23);
            this.dtpDate.TabIndex = 5;
            this.dtpDate.ValueChanged += new System.EventHandler(this.Inputs_Changed);
            //
            // lblTime
            //
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(290, 85);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(26, 15);
            this.lblTime.TabIndex = 6;
            this.lblTime.Text = "Ora";
            //
            // cboTime
            //
            this.cboTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTime.Location = new System.Drawing.Point(330, 82);
            this.cboTime.MaxDropDownItems = 16;
            this.cboTime.Name = "cboTime";
            this.cboTime.Size = new System.Drawing.Size(120, 23);
            this.cboTime.TabIndex = 7;
            this.cboTime.SelectedIndexChanged += new System.EventHandler(this.Inputs_Changed);
            //
            // lblDuration
            //
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(16, 118);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(78, 15);
            this.lblDuration.TabIndex = 8;
            this.lblDuration.Text = "Durată (min)";
            //
            // numDuration
            //
            this.numDuration.Increment = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numDuration.Location = new System.Drawing.Point(120, 115);
            this.numDuration.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
            this.numDuration.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numDuration.Name = "numDuration";
            this.numDuration.Size = new System.Drawing.Size(80, 23);
            this.numDuration.TabIndex = 9;
            this.numDuration.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numDuration.ValueChanged += new System.EventHandler(this.Inputs_Changed);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(222, 118);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 15);
            this.lblStatus.TabIndex = 10;
            this.lblStatus.Text = "Status";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(280, 115);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(170, 23);
            this.cboStatus.TabIndex = 11;
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.Inputs_Changed);
            //
            // lblReason
            //
            this.lblReason.AutoSize = true;
            this.lblReason.Location = new System.Drawing.Point(16, 151);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(39, 15);
            this.lblReason.TabIndex = 12;
            this.lblReason.Text = "Motiv";
            //
            // txtReason
            //
            this.txtReason.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtReason.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtReason.Location = new System.Drawing.Point(120, 148);
            this.txtReason.MaxLength = 200;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(330, 23);
            this.txtReason.TabIndex = 13;
            //
            // lblConflict
            //
            this.lblConflict.ForeColor = System.Drawing.Color.Firebrick;
            this.lblConflict.Location = new System.Drawing.Point(120, 180);
            this.lblConflict.Name = "lblConflict";
            this.lblConflict.Size = new System.Drawing.Size(330, 40);
            this.lblConflict.TabIndex = 14;
            this.lblConflict.Text = "";
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(252, 230);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(96, 32);
            this.btnOk.TabIndex = 15;
            this.btnOk.Text = "Salvează";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(354, 230);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 32);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Renunță";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // AppointmentForm
            //
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(472, 278);
            this.Controls.Add(this.lblPatient);
            this.Controls.Add(this.cboPatient);
            this.Controls.Add(this.lblDoctor);
            this.Controls.Add(this.cboDoctor);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.cboTime);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.numDuration);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblReason);
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.lblConflict);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppointmentForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Programare";
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPatient;
        private System.Windows.Forms.ComboBox cboPatient;
        private System.Windows.Forms.Label lblDoctor;
        private System.Windows.Forms.ComboBox cboDoctor;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.ComboBox cboTime;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.NumericUpDown numDuration;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label lblReason;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.Label lblConflict;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
