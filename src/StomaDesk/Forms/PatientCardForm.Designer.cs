namespace StomaDesk.Forms
{
    partial class PatientCardForm
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblName = new System.Windows.Forms.Label();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblAllergies = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.btnEditPatient = new System.Windows.Forms.Button();
            this.tabCard = new System.Windows.Forms.TabControl();
            this.tabOdontogram = new System.Windows.Forms.TabPage();
            this.odontogram = new StomaDesk.Controls.OdontogramControl();
            this.panelTooth = new System.Windows.Forms.Panel();
            this.lblTooth = new System.Windows.Forms.Label();
            this.cboToothState = new System.Windows.Forms.ComboBox();
            this.lblToothNote = new System.Windows.Forms.Label();
            this.txtToothNote = new System.Windows.Forms.TextBox();
            this.btnApplyTooth = new System.Windows.Forms.Button();
            this.btnPlanTooth = new System.Windows.Forms.Button();
            this.lblToothHint = new System.Windows.Forms.Label();
            this.tabPlan = new System.Windows.Forms.TabPage();
            this.gridPlan = new System.Windows.Forms.DataGridView();
            this.panelPlanAdd = new System.Windows.Forms.Panel();
            this.lblProcedure = new System.Windows.Forms.Label();
            this.cboProcedure = new System.Windows.Forms.ComboBox();
            this.lblPlanTooth = new System.Windows.Forms.Label();
            this.cboPlanTooth = new System.Windows.Forms.ComboBox();
            this.lblPlanDoctor = new System.Windows.Forms.Label();
            this.cboPlanDoctor = new System.Windows.Forms.ComboBox();
            this.lblDiscount = new System.Windows.Forms.Label();
            this.numDiscount = new System.Windows.Forms.NumericUpDown();
            this.btnAddProcedure = new System.Windows.Forms.Button();
            this.panelPlanActions = new System.Windows.Forms.Panel();
            this.lblSetStatus = new System.Windows.Forms.Label();
            this.cboPlanStatus = new System.Windows.Forms.ComboBox();
            this.btnApplyStatus = new System.Windows.Forms.Button();
            this.btnDeleteTreatment = new System.Windows.Forms.Button();
            this.btnPrintEstimate = new System.Windows.Forms.Button();
            this.btnSaveEstimate = new System.Windows.Forms.Button();
            this.lblPlanTotals = new System.Windows.Forms.Label();
            this.tabAppointments = new System.Windows.Forms.TabPage();
            this.gridAppointments = new System.Windows.Forms.DataGridView();
            this.panelAppointmentActions = new System.Windows.Forms.Panel();
            this.btnNewAppointment = new System.Windows.Forms.Button();
            this.btnEditAppointment = new System.Windows.Forms.Button();
            this.tabPayments = new System.Windows.Forms.TabPage();
            this.gridPayments = new System.Windows.Forms.DataGridView();
            this.panelPaymentAdd = new System.Windows.Forms.Panel();
            this.lblAmount = new System.Windows.Forms.Label();
            this.numAmount = new System.Windows.Forms.NumericUpDown();
            this.lblMethod = new System.Windows.Forms.Label();
            this.cboMethod = new System.Windows.Forms.ComboBox();
            this.lblPaymentNote = new System.Windows.Forms.Label();
            this.txtPaymentNote = new System.Windows.Forms.TextBox();
            this.btnAddPayment = new System.Windows.Forms.Button();
            this.lblPaymentTotals = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.tabCard.SuspendLayout();
            this.tabOdontogram.SuspendLayout();
            this.panelTooth.SuspendLayout();
            this.tabPlan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPlan)).BeginInit();
            this.panelPlanAdd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDiscount)).BeginInit();
            this.panelPlanActions.SuspendLayout();
            this.tabAppointments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAppointments)).BeginInit();
            this.panelAppointmentActions.SuspendLayout();
            this.tabPayments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPayments)).BeginInit();
            this.panelPaymentAdd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.panelHeader.Controls.Add(this.lblName);
            this.panelHeader.Controls.Add(this.lblDetails);
            this.panelHeader.Controls.Add(this.lblAllergies);
            this.panelHeader.Controls.Add(this.lblBalance);
            this.panelHeader.Controls.Add(this.btnEditPatient);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1064, 86);
            this.panelHeader.TabIndex = 0;
            //
            // lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(12, 8);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(90, 28);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Pacient";
            //
            // lblDetails
            //
            this.lblDetails.AutoSize = true;
            this.lblDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDetails.Location = new System.Drawing.Point(15, 42);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(40, 15);
            this.lblDetails.TabIndex = 1;
            this.lblDetails.Text = "Detalii";
            //
            // lblAllergies
            //
            this.lblAllergies.AutoSize = true;
            this.lblAllergies.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAllergies.Location = new System.Drawing.Point(15, 62);
            this.lblAllergies.Name = "lblAllergies";
            this.lblAllergies.Size = new System.Drawing.Size(45, 15);
            this.lblAllergies.TabIndex = 2;
            this.lblAllergies.Text = "Alergii";
            //
            // lblBalance
            //
            this.lblBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBalance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBalance.Location = new System.Drawing.Point(660, 8);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(390, 28);
            this.lblBalance.TabIndex = 3;
            this.lblBalance.Text = "Sold";
            this.lblBalance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnEditPatient
            //
            this.btnEditPatient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditPatient.Location = new System.Drawing.Point(910, 44);
            this.btnEditPatient.Name = "btnEditPatient";
            this.btnEditPatient.Size = new System.Drawing.Size(140, 30);
            this.btnEditPatient.TabIndex = 4;
            this.btnEditPatient.Text = "Editează datele";
            this.btnEditPatient.UseVisualStyleBackColor = true;
            this.btnEditPatient.Click += new System.EventHandler(this.btnEditPatient_Click);
            //
            // tabCard
            //
            this.tabCard.Controls.Add(this.tabOdontogram);
            this.tabCard.Controls.Add(this.tabPlan);
            this.tabCard.Controls.Add(this.tabAppointments);
            this.tabCard.Controls.Add(this.tabPayments);
            this.tabCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCard.Location = new System.Drawing.Point(0, 86);
            this.tabCard.Name = "tabCard";
            this.tabCard.Padding = new System.Drawing.Point(14, 6);
            this.tabCard.SelectedIndex = 0;
            this.tabCard.Size = new System.Drawing.Size(1064, 615);
            this.tabCard.TabIndex = 1;
            //
            // tabOdontogram
            //
            this.tabOdontogram.Controls.Add(this.odontogram);
            this.tabOdontogram.Controls.Add(this.panelTooth);
            this.tabOdontogram.Location = new System.Drawing.Point(4, 30);
            this.tabOdontogram.Name = "tabOdontogram";
            this.tabOdontogram.Padding = new System.Windows.Forms.Padding(6);
            this.tabOdontogram.Size = new System.Drawing.Size(1056, 581);
            this.tabOdontogram.TabIndex = 0;
            this.tabOdontogram.Text = "Odontogramă";
            this.tabOdontogram.UseVisualStyleBackColor = true;
            //
            // odontogram
            //
            this.odontogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.odontogram.Location = new System.Drawing.Point(6, 6);
            this.odontogram.Name = "odontogram";
            this.odontogram.Size = new System.Drawing.Size(1044, 489);
            this.odontogram.TabIndex = 0;
            this.odontogram.ToothClicked += new System.EventHandler<StomaDesk.Controls.ToothEventArgs>(this.odontogram_ToothClicked);
            this.odontogram.SelectedToothChanged += new System.EventHandler(this.odontogram_SelectedToothChanged);
            //
            // panelTooth
            //
            this.panelTooth.Controls.Add(this.lblTooth);
            this.panelTooth.Controls.Add(this.cboToothState);
            this.panelTooth.Controls.Add(this.lblToothNote);
            this.panelTooth.Controls.Add(this.txtToothNote);
            this.panelTooth.Controls.Add(this.btnApplyTooth);
            this.panelTooth.Controls.Add(this.btnPlanTooth);
            this.panelTooth.Controls.Add(this.lblToothHint);
            this.panelTooth.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelTooth.Location = new System.Drawing.Point(6, 495);
            this.panelTooth.Name = "panelTooth";
            this.panelTooth.Size = new System.Drawing.Size(1044, 80);
            this.panelTooth.TabIndex = 1;
            //
            // lblTooth
            //
            this.lblTooth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTooth.Location = new System.Drawing.Point(4, 14);
            this.lblTooth.Name = "lblTooth";
            this.lblTooth.Size = new System.Drawing.Size(190, 23);
            this.lblTooth.TabIndex = 0;
            this.lblTooth.Text = "Alegeți un dinte";
            this.lblTooth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboToothState
            //
            this.cboToothState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboToothState.Location = new System.Drawing.Point(198, 14);
            this.cboToothState.Name = "cboToothState";
            this.cboToothState.Size = new System.Drawing.Size(170, 23);
            this.cboToothState.TabIndex = 1;
            //
            // lblToothNote
            //
            this.lblToothNote.AutoSize = true;
            this.lblToothNote.Location = new System.Drawing.Point(380, 18);
            this.lblToothNote.Name = "lblToothNote";
            this.lblToothNote.Size = new System.Drawing.Size(36, 15);
            this.lblToothNote.TabIndex = 2;
            this.lblToothNote.Text = "Notă:";
            //
            // txtToothNote
            //
            this.txtToothNote.Location = new System.Drawing.Point(422, 14);
            this.txtToothNote.MaxLength = 200;
            this.txtToothNote.Name = "txtToothNote";
            this.txtToothNote.Size = new System.Drawing.Size(270, 23);
            this.txtToothNote.TabIndex = 3;
            //
            // btnApplyTooth
            //
            this.btnApplyTooth.Location = new System.Drawing.Point(702, 11);
            this.btnApplyTooth.Name = "btnApplyTooth";
            this.btnApplyTooth.Size = new System.Drawing.Size(130, 30);
            this.btnApplyTooth.TabIndex = 4;
            this.btnApplyTooth.Text = "Salvează dintele";
            this.btnApplyTooth.UseVisualStyleBackColor = true;
            this.btnApplyTooth.Click += new System.EventHandler(this.btnApplyTooth_Click);
            //
            // btnPlanTooth
            //
            this.btnPlanTooth.Location = new System.Drawing.Point(838, 11);
            this.btnPlanTooth.Name = "btnPlanTooth";
            this.btnPlanTooth.Size = new System.Drawing.Size(200, 30);
            this.btnPlanTooth.TabIndex = 5;
            this.btnPlanTooth.Text = "Adaugă procedură pe dinte...";
            this.btnPlanTooth.UseVisualStyleBackColor = true;
            this.btnPlanTooth.Click += new System.EventHandler(this.btnPlanTooth_Click);
            //
            // lblToothHint
            //
            this.lblToothHint.AutoSize = true;
            this.lblToothHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblToothHint.Location = new System.Drawing.Point(4, 52);
            this.lblToothHint.Name = "lblToothHint";
            this.lblToothHint.Size = new System.Drawing.Size(400, 15);
            this.lblToothHint.TabIndex = 6;
            this.lblToothHint.Text = "Click pe un dinte îl selectează, click dreapta schimbă rapid starea, săgețile mută selecția.";
            //
            // tabPlan
            //
            this.tabPlan.Controls.Add(this.gridPlan);
            this.tabPlan.Controls.Add(this.panelPlanActions);
            this.tabPlan.Controls.Add(this.panelPlanAdd);
            this.tabPlan.Location = new System.Drawing.Point(4, 30);
            this.tabPlan.Name = "tabPlan";
            this.tabPlan.Padding = new System.Windows.Forms.Padding(6);
            this.tabPlan.Size = new System.Drawing.Size(1056, 581);
            this.tabPlan.TabIndex = 1;
            this.tabPlan.Text = "Plan de tratament";
            this.tabPlan.UseVisualStyleBackColor = true;
            //
            // gridPlan
            //
            this.gridPlan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPlan.Location = new System.Drawing.Point(6, 56);
            this.gridPlan.Name = "gridPlan";
            this.gridPlan.Size = new System.Drawing.Size(1044, 469);
            this.gridPlan.TabIndex = 1;
            //
            // panelPlanAdd
            //
            this.panelPlanAdd.Controls.Add(this.lblProcedure);
            this.panelPlanAdd.Controls.Add(this.cboProcedure);
            this.panelPlanAdd.Controls.Add(this.lblPlanTooth);
            this.panelPlanAdd.Controls.Add(this.cboPlanTooth);
            this.panelPlanAdd.Controls.Add(this.lblPlanDoctor);
            this.panelPlanAdd.Controls.Add(this.cboPlanDoctor);
            this.panelPlanAdd.Controls.Add(this.lblDiscount);
            this.panelPlanAdd.Controls.Add(this.numDiscount);
            this.panelPlanAdd.Controls.Add(this.btnAddProcedure);
            this.panelPlanAdd.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPlanAdd.Location = new System.Drawing.Point(6, 6);
            this.panelPlanAdd.Name = "panelPlanAdd";
            this.panelPlanAdd.Size = new System.Drawing.Size(1044, 50);
            this.panelPlanAdd.TabIndex = 0;
            //
            // lblProcedure
            //
            this.lblProcedure.AutoSize = true;
            this.lblProcedure.Location = new System.Drawing.Point(0, 17);
            this.lblProcedure.Name = "lblProcedure";
            this.lblProcedure.Size = new System.Drawing.Size(61, 15);
            this.lblProcedure.TabIndex = 0;
            this.lblProcedure.Text = "Procedură";
            //
            // cboProcedure
            //
            this.cboProcedure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProcedure.DropDownWidth = 420;
            this.cboProcedure.Location = new System.Drawing.Point(70, 13);
            this.cboProcedure.MaxDropDownItems = 16;
            this.cboProcedure.Name = "cboProcedure";
            this.cboProcedure.Size = new System.Drawing.Size(340, 23);
            this.cboProcedure.TabIndex = 1;
            this.cboProcedure.SelectedIndexChanged += new System.EventHandler(this.cboProcedure_SelectedIndexChanged);
            //
            // lblPlanTooth
            //
            this.lblPlanTooth.AutoSize = true;
            this.lblPlanTooth.Location = new System.Drawing.Point(422, 17);
            this.lblPlanTooth.Name = "lblPlanTooth";
            this.lblPlanTooth.Size = new System.Drawing.Size(36, 15);
            this.lblPlanTooth.TabIndex = 2;
            this.lblPlanTooth.Text = "Dinte";
            //
            // cboPlanTooth
            //
            this.cboPlanTooth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlanTooth.Location = new System.Drawing.Point(462, 13);
            this.cboPlanTooth.MaxDropDownItems = 16;
            this.cboPlanTooth.Name = "cboPlanTooth";
            this.cboPlanTooth.Size = new System.Drawing.Size(90, 23);
            this.cboPlanTooth.TabIndex = 3;
            //
            // lblPlanDoctor
            //
            this.lblPlanDoctor.AutoSize = true;
            this.lblPlanDoctor.Location = new System.Drawing.Point(564, 17);
            this.lblPlanDoctor.Name = "lblPlanDoctor";
            this.lblPlanDoctor.Size = new System.Drawing.Size(42, 15);
            this.lblPlanDoctor.TabIndex = 4;
            this.lblPlanDoctor.Text = "Medic";
            //
            // cboPlanDoctor
            //
            this.cboPlanDoctor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlanDoctor.Location = new System.Drawing.Point(612, 13);
            this.cboPlanDoctor.Name = "cboPlanDoctor";
            this.cboPlanDoctor.Size = new System.Drawing.Size(170, 23);
            this.cboPlanDoctor.TabIndex = 5;
            //
            // lblDiscount
            //
            this.lblDiscount.AutoSize = true;
            this.lblDiscount.Location = new System.Drawing.Point(794, 17);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(68, 15);
            this.lblDiscount.TabIndex = 6;
            this.lblDiscount.Text = "Discount %";
            //
            // numDiscount
            //
            this.numDiscount.Location = new System.Drawing.Point(868, 13);
            this.numDiscount.Name = "numDiscount";
            this.numDiscount.Size = new System.Drawing.Size(56, 23);
            this.numDiscount.TabIndex = 7;
            //
            // btnAddProcedure
            //
            this.btnAddProcedure.Location = new System.Drawing.Point(936, 10);
            this.btnAddProcedure.Name = "btnAddProcedure";
            this.btnAddProcedure.Size = new System.Drawing.Size(100, 30);
            this.btnAddProcedure.TabIndex = 8;
            this.btnAddProcedure.Text = "Adaugă";
            this.btnAddProcedure.UseVisualStyleBackColor = true;
            this.btnAddProcedure.Click += new System.EventHandler(this.btnAddProcedure_Click);
            //
            // panelPlanActions
            //
            this.panelPlanActions.Controls.Add(this.lblSetStatus);
            this.panelPlanActions.Controls.Add(this.cboPlanStatus);
            this.panelPlanActions.Controls.Add(this.btnApplyStatus);
            this.panelPlanActions.Controls.Add(this.btnDeleteTreatment);
            this.panelPlanActions.Controls.Add(this.btnPrintEstimate);
            this.panelPlanActions.Controls.Add(this.btnSaveEstimate);
            this.panelPlanActions.Controls.Add(this.lblPlanTotals);
            this.panelPlanActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPlanActions.Location = new System.Drawing.Point(6, 525);
            this.panelPlanActions.Name = "panelPlanActions";
            this.panelPlanActions.Size = new System.Drawing.Size(1044, 50);
            this.panelPlanActions.TabIndex = 2;
            //
            // lblSetStatus
            //
            this.lblSetStatus.AutoSize = true;
            this.lblSetStatus.Location = new System.Drawing.Point(0, 17);
            this.lblSetStatus.Name = "lblSetStatus";
            this.lblSetStatus.Size = new System.Drawing.Size(88, 15);
            this.lblSetStatus.TabIndex = 0;
            this.lblSetStatus.Text = "Status selecție:";
            //
            // cboPlanStatus
            //
            this.cboPlanStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlanStatus.Location = new System.Drawing.Point(96, 13);
            this.cboPlanStatus.Name = "cboPlanStatus";
            this.cboPlanStatus.Size = new System.Drawing.Size(120, 23);
            this.cboPlanStatus.TabIndex = 1;
            //
            // btnApplyStatus
            //
            this.btnApplyStatus.Location = new System.Drawing.Point(222, 10);
            this.btnApplyStatus.Name = "btnApplyStatus";
            this.btnApplyStatus.Size = new System.Drawing.Size(80, 30);
            this.btnApplyStatus.TabIndex = 2;
            this.btnApplyStatus.Text = "Aplică";
            this.btnApplyStatus.UseVisualStyleBackColor = true;
            this.btnApplyStatus.Click += new System.EventHandler(this.btnApplyStatus_Click);
            //
            // btnDeleteTreatment
            //
            this.btnDeleteTreatment.Location = new System.Drawing.Point(312, 10);
            this.btnDeleteTreatment.Name = "btnDeleteTreatment";
            this.btnDeleteTreatment.Size = new System.Drawing.Size(80, 30);
            this.btnDeleteTreatment.TabIndex = 3;
            this.btnDeleteTreatment.Text = "Șterge";
            this.btnDeleteTreatment.UseVisualStyleBackColor = true;
            this.btnDeleteTreatment.Click += new System.EventHandler(this.btnDeleteTreatment_Click);
            //
            // btnPrintEstimate
            //
            this.btnPrintEstimate.Location = new System.Drawing.Point(410, 10);
            this.btnPrintEstimate.Name = "btnPrintEstimate";
            this.btnPrintEstimate.Size = new System.Drawing.Size(130, 30);
            this.btnPrintEstimate.TabIndex = 4;
            this.btnPrintEstimate.Text = "Tipărește deviz";
            this.btnPrintEstimate.UseVisualStyleBackColor = true;
            this.btnPrintEstimate.Click += new System.EventHandler(this.btnPrintEstimate_Click);
            //
            // btnSaveEstimate
            //
            this.btnSaveEstimate.Location = new System.Drawing.Point(546, 10);
            this.btnSaveEstimate.Name = "btnSaveEstimate";
            this.btnSaveEstimate.Size = new System.Drawing.Size(150, 30);
            this.btnSaveEstimate.TabIndex = 5;
            this.btnSaveEstimate.Text = "Deviz ca imagine...";
            this.btnSaveEstimate.UseVisualStyleBackColor = true;
            this.btnSaveEstimate.Click += new System.EventHandler(this.btnSaveEstimate_Click);
            //
            // lblPlanTotals
            //
            this.lblPlanTotals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPlanTotals.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlanTotals.Location = new System.Drawing.Point(704, 13);
            this.lblPlanTotals.Name = "lblPlanTotals";
            this.lblPlanTotals.Size = new System.Drawing.Size(338, 23);
            this.lblPlanTotals.TabIndex = 6;
            this.lblPlanTotals.Text = "";
            this.lblPlanTotals.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // tabAppointments
            //
            this.tabAppointments.Controls.Add(this.gridAppointments);
            this.tabAppointments.Controls.Add(this.panelAppointmentActions);
            this.tabAppointments.Location = new System.Drawing.Point(4, 30);
            this.tabAppointments.Name = "tabAppointments";
            this.tabAppointments.Padding = new System.Windows.Forms.Padding(6);
            this.tabAppointments.Size = new System.Drawing.Size(1056, 581);
            this.tabAppointments.TabIndex = 2;
            this.tabAppointments.Text = "Programări";
            this.tabAppointments.UseVisualStyleBackColor = true;
            //
            // gridAppointments
            //
            this.gridAppointments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAppointments.Location = new System.Drawing.Point(6, 56);
            this.gridAppointments.Name = "gridAppointments";
            this.gridAppointments.Size = new System.Drawing.Size(1044, 519);
            this.gridAppointments.TabIndex = 1;
            this.gridAppointments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridAppointments_CellDoubleClick);
            //
            // panelAppointmentActions
            //
            this.panelAppointmentActions.Controls.Add(this.btnNewAppointment);
            this.panelAppointmentActions.Controls.Add(this.btnEditAppointment);
            this.panelAppointmentActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAppointmentActions.Location = new System.Drawing.Point(6, 6);
            this.panelAppointmentActions.Name = "panelAppointmentActions";
            this.panelAppointmentActions.Size = new System.Drawing.Size(1044, 50);
            this.panelAppointmentActions.TabIndex = 0;
            //
            // btnNewAppointment
            //
            this.btnNewAppointment.Location = new System.Drawing.Point(0, 10);
            this.btnNewAppointment.Name = "btnNewAppointment";
            this.btnNewAppointment.Size = new System.Drawing.Size(150, 30);
            this.btnNewAppointment.TabIndex = 0;
            this.btnNewAppointment.Text = "Programare nouă";
            this.btnNewAppointment.UseVisualStyleBackColor = true;
            this.btnNewAppointment.Click += new System.EventHandler(this.btnNewAppointment_Click);
            //
            // btnEditAppointment
            //
            this.btnEditAppointment.Location = new System.Drawing.Point(156, 10);
            this.btnEditAppointment.Name = "btnEditAppointment";
            this.btnEditAppointment.Size = new System.Drawing.Size(110, 30);
            this.btnEditAppointment.TabIndex = 1;
            this.btnEditAppointment.Text = "Deschide";
            this.btnEditAppointment.UseVisualStyleBackColor = true;
            this.btnEditAppointment.Click += new System.EventHandler(this.btnEditAppointment_Click);
            //
            // tabPayments
            //
            this.tabPayments.Controls.Add(this.gridPayments);
            this.tabPayments.Controls.Add(this.lblPaymentTotals);
            this.tabPayments.Controls.Add(this.panelPaymentAdd);
            this.tabPayments.Location = new System.Drawing.Point(4, 30);
            this.tabPayments.Name = "tabPayments";
            this.tabPayments.Padding = new System.Windows.Forms.Padding(6);
            this.tabPayments.Size = new System.Drawing.Size(1056, 581);
            this.tabPayments.TabIndex = 3;
            this.tabPayments.Text = "Încasări";
            this.tabPayments.UseVisualStyleBackColor = true;
            //
            // gridPayments
            //
            this.gridPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPayments.Location = new System.Drawing.Point(6, 56);
            this.gridPayments.Name = "gridPayments";
            this.gridPayments.Size = new System.Drawing.Size(1044, 485);
            this.gridPayments.TabIndex = 1;
            //
            // panelPaymentAdd
            //
            this.panelPaymentAdd.Controls.Add(this.lblAmount);
            this.panelPaymentAdd.Controls.Add(this.numAmount);
            this.panelPaymentAdd.Controls.Add(this.lblMethod);
            this.panelPaymentAdd.Controls.Add(this.cboMethod);
            this.panelPaymentAdd.Controls.Add(this.lblPaymentNote);
            this.panelPaymentAdd.Controls.Add(this.txtPaymentNote);
            this.panelPaymentAdd.Controls.Add(this.btnAddPayment);
            this.panelPaymentAdd.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPaymentAdd.Location = new System.Drawing.Point(6, 6);
            this.panelPaymentAdd.Name = "panelPaymentAdd";
            this.panelPaymentAdd.Size = new System.Drawing.Size(1044, 50);
            this.panelPaymentAdd.TabIndex = 0;
            //
            // lblAmount
            //
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(0, 17);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(64, 15);
            this.lblAmount.TabIndex = 0;
            this.lblAmount.Text = "Sumă (lei)";
            //
            // numAmount
            //
            this.numAmount.DecimalPlaces = 2;
            this.numAmount.Location = new System.Drawing.Point(72, 13);
            this.numAmount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numAmount.Name = "numAmount";
            this.numAmount.Size = new System.Drawing.Size(110, 23);
            this.numAmount.TabIndex = 1;
            this.numAmount.ThousandsSeparator = true;
            //
            // lblMethod
            //
            this.lblMethod.AutoSize = true;
            this.lblMethod.Location = new System.Drawing.Point(196, 17);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(46, 15);
            this.lblMethod.TabIndex = 2;
            this.lblMethod.Text = "Metodă";
            //
            // cboMethod
            //
            this.cboMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMethod.Location = new System.Drawing.Point(250, 13);
            this.cboMethod.Name = "cboMethod";
            this.cboMethod.Size = new System.Drawing.Size(140, 23);
            this.cboMethod.TabIndex = 3;
            //
            // lblPaymentNote
            //
            this.lblPaymentNote.AutoSize = true;
            this.lblPaymentNote.Location = new System.Drawing.Point(404, 17);
            this.lblPaymentNote.Name = "lblPaymentNote";
            this.lblPaymentNote.Size = new System.Drawing.Size(65, 15);
            this.lblPaymentNote.TabIndex = 4;
            this.lblPaymentNote.Text = "Observații";
            //
            // txtPaymentNote
            //
            this.txtPaymentNote.Location = new System.Drawing.Point(476, 13);
            this.txtPaymentNote.MaxLength = 200;
            this.txtPaymentNote.Name = "txtPaymentNote";
            this.txtPaymentNote.Size = new System.Drawing.Size(280, 23);
            this.txtPaymentNote.TabIndex = 5;
            //
            // btnAddPayment
            //
            this.btnAddPayment.Location = new System.Drawing.Point(768, 10);
            this.btnAddPayment.Name = "btnAddPayment";
            this.btnAddPayment.Size = new System.Drawing.Size(140, 30);
            this.btnAddPayment.TabIndex = 6;
            this.btnAddPayment.Text = "Înregistrează";
            this.btnAddPayment.UseVisualStyleBackColor = true;
            this.btnAddPayment.Click += new System.EventHandler(this.btnAddPayment_Click);
            //
            // lblPaymentTotals
            //
            this.lblPaymentTotals.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblPaymentTotals.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaymentTotals.Location = new System.Drawing.Point(6, 541);
            this.lblPaymentTotals.Name = "lblPaymentTotals";
            this.lblPaymentTotals.Size = new System.Drawing.Size(1044, 34);
            this.lblPaymentTotals.TabIndex = 2;
            this.lblPaymentTotals.Text = "";
            this.lblPaymentTotals.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // PatientCardForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 701);
            this.Controls.Add(this.tabCard);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 640);
            this.Name = "PatientCardForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Fișa pacientului";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.tabCard.ResumeLayout(false);
            this.tabOdontogram.ResumeLayout(false);
            this.panelTooth.ResumeLayout(false);
            this.panelTooth.PerformLayout();
            this.tabPlan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridPlan)).EndInit();
            this.panelPlanAdd.ResumeLayout(false);
            this.panelPlanAdd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDiscount)).EndInit();
            this.panelPlanActions.ResumeLayout(false);
            this.panelPlanActions.PerformLayout();
            this.tabAppointments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridAppointments)).EndInit();
            this.panelAppointmentActions.ResumeLayout(false);
            this.tabPayments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridPayments)).EndInit();
            this.panelPaymentAdd.ResumeLayout(false);
            this.panelPaymentAdd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Label lblAllergies;
        private System.Windows.Forms.Label lblBalance;
        private System.Windows.Forms.Button btnEditPatient;
        private System.Windows.Forms.TabControl tabCard;
        private System.Windows.Forms.TabPage tabOdontogram;
        private StomaDesk.Controls.OdontogramControl odontogram;
        private System.Windows.Forms.Panel panelTooth;
        private System.Windows.Forms.Label lblTooth;
        private System.Windows.Forms.ComboBox cboToothState;
        private System.Windows.Forms.Label lblToothNote;
        private System.Windows.Forms.TextBox txtToothNote;
        private System.Windows.Forms.Button btnApplyTooth;
        private System.Windows.Forms.Button btnPlanTooth;
        private System.Windows.Forms.Label lblToothHint;
        private System.Windows.Forms.TabPage tabPlan;
        private System.Windows.Forms.DataGridView gridPlan;
        private System.Windows.Forms.Panel panelPlanAdd;
        private System.Windows.Forms.Label lblProcedure;
        private System.Windows.Forms.ComboBox cboProcedure;
        private System.Windows.Forms.Label lblPlanTooth;
        private System.Windows.Forms.ComboBox cboPlanTooth;
        private System.Windows.Forms.Label lblPlanDoctor;
        private System.Windows.Forms.ComboBox cboPlanDoctor;
        private System.Windows.Forms.Label lblDiscount;
        private System.Windows.Forms.NumericUpDown numDiscount;
        private System.Windows.Forms.Button btnAddProcedure;
        private System.Windows.Forms.Panel panelPlanActions;
        private System.Windows.Forms.Label lblSetStatus;
        private System.Windows.Forms.ComboBox cboPlanStatus;
        private System.Windows.Forms.Button btnApplyStatus;
        private System.Windows.Forms.Button btnDeleteTreatment;
        private System.Windows.Forms.Button btnPrintEstimate;
        private System.Windows.Forms.Button btnSaveEstimate;
        private System.Windows.Forms.Label lblPlanTotals;
        private System.Windows.Forms.TabPage tabAppointments;
        private System.Windows.Forms.DataGridView gridAppointments;
        private System.Windows.Forms.Panel panelAppointmentActions;
        private System.Windows.Forms.Button btnNewAppointment;
        private System.Windows.Forms.Button btnEditAppointment;
        private System.Windows.Forms.TabPage tabPayments;
        private System.Windows.Forms.DataGridView gridPayments;
        private System.Windows.Forms.Panel panelPaymentAdd;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.NumericUpDown numAmount;
        private System.Windows.Forms.Label lblMethod;
        private System.Windows.Forms.ComboBox cboMethod;
        private System.Windows.Forms.Label lblPaymentNote;
        private System.Windows.Forms.TextBox txtPaymentNote;
        private System.Windows.Forms.Button btnAddPayment;
        private System.Windows.Forms.Label lblPaymentTotals;
    }
}
