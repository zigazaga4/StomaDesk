namespace StomaDesk.Forms
{
    partial class PatientForm
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
            this.components = new System.ComponentModel.Container();
            this.lblLastName = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.lblCnp = new System.Windows.Forms.Label();
            this.txtCnp = new System.Windows.Forms.TextBox();
            this.lblCnpInfo = new System.Windows.Forms.Label();
            this.lblBirth = new System.Windows.Forms.Label();
            this.dtpBirth = new System.Windows.Forms.DateTimePicker();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblAllergies = new System.Windows.Forms.Label();
            this.txtAllergies = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // lblLastName
            //
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(16, 19);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(48, 15);
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Nume *";
            //
            // txtLastName
            //
            this.txtLastName.Location = new System.Drawing.Point(130, 16);
            this.txtLastName.MaxLength = 60;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(340, 23);
            this.txtLastName.TabIndex = 1;
            //
            // lblFirstName
            //
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(16, 50);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(63, 15);
            this.lblFirstName.TabIndex = 2;
            this.lblFirstName.Text = "Prenume *";
            //
            // txtFirstName
            //
            this.txtFirstName.Location = new System.Drawing.Point(130, 47);
            this.txtFirstName.MaxLength = 60;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(340, 23);
            this.txtFirstName.TabIndex = 3;
            //
            // lblCnp
            //
            this.lblCnp.AutoSize = true;
            this.lblCnp.Location = new System.Drawing.Point(16, 81);
            this.lblCnp.Name = "lblCnp";
            this.lblCnp.Size = new System.Drawing.Size(31, 15);
            this.lblCnp.TabIndex = 4;
            this.lblCnp.Text = "CNP";
            //
            // txtCnp
            //
            this.txtCnp.Location = new System.Drawing.Point(130, 78);
            this.txtCnp.MaxLength = 13;
            this.txtCnp.Name = "txtCnp";
            this.txtCnp.Size = new System.Drawing.Size(160, 23);
            this.txtCnp.TabIndex = 5;
            this.txtCnp.TextChanged += new System.EventHandler(this.txtCnp_TextChanged);
            //
            // lblCnpInfo
            //
            this.lblCnpInfo.ForeColor = System.Drawing.Color.DimGray;
            this.lblCnpInfo.Location = new System.Drawing.Point(130, 104);
            this.lblCnpInfo.Name = "lblCnpInfo";
            this.lblCnpInfo.Size = new System.Drawing.Size(360, 18);
            this.lblCnpInfo.TabIndex = 6;
            this.lblCnpInfo.Text = "";
            //
            // lblBirth
            //
            this.lblBirth.AutoSize = true;
            this.lblBirth.Location = new System.Drawing.Point(16, 131);
            this.lblBirth.Name = "lblBirth";
            this.lblBirth.Size = new System.Drawing.Size(80, 15);
            this.lblBirth.TabIndex = 7;
            this.lblBirth.Text = "Data nașterii";
            //
            // dtpBirth
            //
            this.dtpBirth.CustomFormat = "dd.MM.yyyy";
            this.dtpBirth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBirth.Location = new System.Drawing.Point(130, 128);
            this.dtpBirth.Name = "dtpBirth";
            this.dtpBirth.ShowCheckBox = true;
            this.dtpBirth.Size = new System.Drawing.Size(160, 23);
            this.dtpBirth.TabIndex = 8;
            //
            // lblPhone
            //
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(16, 162);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(45, 15);
            this.lblPhone.TabIndex = 9;
            this.lblPhone.Text = "Telefon";
            //
            // txtPhone
            //
            this.txtPhone.Location = new System.Drawing.Point(130, 159);
            this.txtPhone.MaxLength = 20;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(160, 23);
            this.txtPhone.TabIndex = 10;
            //
            // lblEmail
            //
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(16, 193);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(36, 15);
            this.lblEmail.TabIndex = 11;
            this.lblEmail.Text = "Email";
            //
            // txtEmail
            //
            this.txtEmail.Location = new System.Drawing.Point(130, 190);
            this.txtEmail.MaxLength = 100;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(340, 23);
            this.txtEmail.TabIndex = 12;
            //
            // lblAddress
            //
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(16, 224);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(43, 15);
            this.lblAddress.TabIndex = 13;
            this.lblAddress.Text = "Adresă";
            //
            // txtAddress
            //
            this.txtAddress.Location = new System.Drawing.Point(130, 221);
            this.txtAddress.MaxLength = 200;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(340, 23);
            this.txtAddress.TabIndex = 14;
            //
            // lblAllergies
            //
            this.lblAllergies.AutoSize = true;
            this.lblAllergies.ForeColor = System.Drawing.Color.Firebrick;
            this.lblAllergies.Location = new System.Drawing.Point(16, 255);
            this.lblAllergies.Name = "lblAllergies";
            this.lblAllergies.Size = new System.Drawing.Size(42, 15);
            this.lblAllergies.TabIndex = 15;
            this.lblAllergies.Text = "Alergii";
            //
            // txtAllergies
            //
            this.txtAllergies.Location = new System.Drawing.Point(130, 252);
            this.txtAllergies.MaxLength = 300;
            this.txtAllergies.Multiline = true;
            this.txtAllergies.Name = "txtAllergies";
            this.txtAllergies.Size = new System.Drawing.Size(340, 44);
            this.txtAllergies.TabIndex = 16;
            //
            // lblNotes
            //
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(16, 307);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(65, 15);
            this.lblNotes.TabIndex = 17;
            this.lblNotes.Text = "Observații";
            //
            // txtNotes
            //
            this.txtNotes.AcceptsReturn = true;
            this.txtNotes.Location = new System.Drawing.Point(130, 304);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(340, 80);
            this.txtNotes.TabIndex = 18;
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(274, 402);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(96, 32);
            this.btnOk.TabIndex = 19;
            this.btnOk.Text = "Salvează";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(376, 402);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 32);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Renunță";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // errorProvider
            //
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // PatientForm
            //
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(504, 450);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.lblCnp);
            this.Controls.Add(this.txtCnp);
            this.Controls.Add(this.lblCnpInfo);
            this.Controls.Add(this.lblBirth);
            this.Controls.Add(this.dtpBirth);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblAllergies);
            this.Controls.Add(this.txtAllergies);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PatientForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pacient";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label lblCnp;
        private System.Windows.Forms.TextBox txtCnp;
        private System.Windows.Forms.Label lblCnpInfo;
        private System.Windows.Forms.Label lblBirth;
        private System.Windows.Forms.DateTimePicker dtpBirth;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblAllergies;
        private System.Windows.Forms.TextBox txtAllergies;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
