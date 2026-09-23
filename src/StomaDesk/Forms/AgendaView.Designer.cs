namespace StomaDesk.Forms
{
    partial class AgendaView
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnPrev = new System.Windows.Forms.Button();
            this.dtpDay = new System.Windows.Forms.DateTimePicker();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnToday = new System.Windows.Forms.Button();
            this.lblDay = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnReminders = new System.Windows.Forms.Button();
            this.progressReminders = new System.Windows.Forms.ProgressBar();
            this.btnCancelReminders = new System.Windows.Forms.Button();
            this.lblSummary = new System.Windows.Forms.Label();
            this.gridDay = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDay)).BeginInit();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.btnPrev);
            this.panelTop.Controls.Add(this.dtpDay);
            this.panelTop.Controls.Add(this.btnNext);
            this.panelTop.Controls.Add(this.btnToday);
            this.panelTop.Controls.Add(this.lblDay);
            this.panelTop.Controls.Add(this.btnNew);
            this.panelTop.Controls.Add(this.btnReminders);
            this.panelTop.Controls.Add(this.progressReminders);
            this.panelTop.Controls.Add(this.btnCancelReminders);
            this.panelTop.Controls.Add(this.lblSummary);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1164, 88);
            this.panelTop.TabIndex = 0;
            //
            // btnPrev
            //
            this.btnPrev.Location = new System.Drawing.Point(0, 8);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(40, 30);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.Text = "‹";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            //
            // dtpDay
            //
            this.dtpDay.CustomFormat = "dd.MM.yyyy";
            this.dtpDay.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDay.Location = new System.Drawing.Point(46, 11);
            this.dtpDay.Name = "dtpDay";
            this.dtpDay.Size = new System.Drawing.Size(130, 23);
            this.dtpDay.TabIndex = 1;
            this.dtpDay.ValueChanged += new System.EventHandler(this.dtpDay_ValueChanged);
            //
            // btnNext
            //
            this.btnNext.Location = new System.Drawing.Point(182, 8);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(40, 30);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "›";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            //
            // btnToday
            //
            this.btnToday.Location = new System.Drawing.Point(228, 8);
            this.btnToday.Name = "btnToday";
            this.btnToday.Size = new System.Drawing.Size(64, 30);
            this.btnToday.TabIndex = 3;
            this.btnToday.Text = "Azi";
            this.btnToday.UseVisualStyleBackColor = true;
            this.btnToday.Click += new System.EventHandler(this.btnToday_Click);
            //
            // lblDay
            //
            this.lblDay.AutoSize = true;
            this.lblDay.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDay.Location = new System.Drawing.Point(306, 11);
            this.lblDay.Name = "lblDay";
            this.lblDay.Size = new System.Drawing.Size(60, 21);
            this.lblDay.TabIndex = 4;
            this.lblDay.Text = "Ziua";
            //
            // btnNew
            //
            this.btnNew.Location = new System.Drawing.Point(0, 48);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(150, 30);
            this.btnNew.TabIndex = 5;
            this.btnNew.Text = "Programare nouă";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            //
            // btnReminders
            //
            this.btnReminders.Location = new System.Drawing.Point(156, 48);
            this.btnReminders.Name = "btnReminders";
            this.btnReminders.Size = new System.Drawing.Size(230, 30);
            this.btnReminders.TabIndex = 6;
            this.btnReminders.Text = "Trimite remindere SMS pentru mâine";
            this.btnReminders.UseVisualStyleBackColor = true;
            this.btnReminders.Click += new System.EventHandler(this.btnReminders_Click);
            //
            // progressReminders
            //
            this.progressReminders.Location = new System.Drawing.Point(396, 53);
            this.progressReminders.Name = "progressReminders";
            this.progressReminders.Size = new System.Drawing.Size(160, 20);
            this.progressReminders.TabIndex = 7;
            this.progressReminders.Visible = false;
            //
            // btnCancelReminders
            //
            this.btnCancelReminders.Location = new System.Drawing.Point(562, 48);
            this.btnCancelReminders.Name = "btnCancelReminders";
            this.btnCancelReminders.Size = new System.Drawing.Size(80, 30);
            this.btnCancelReminders.TabIndex = 8;
            this.btnCancelReminders.Text = "Oprește";
            this.btnCancelReminders.UseVisualStyleBackColor = true;
            this.btnCancelReminders.Visible = false;
            this.btnCancelReminders.Click += new System.EventHandler(this.btnCancelReminders_Click);
            //
            // lblSummary
            //
            this.lblSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSummary.ForeColor = System.Drawing.Color.DimGray;
            this.lblSummary.Location = new System.Drawing.Point(654, 51);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(506, 23);
            this.lblSummary.TabIndex = 9;
            this.lblSummary.Text = "";
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridDay
            //
            this.gridDay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDay.Location = new System.Drawing.Point(0, 88);
            this.gridDay.Name = "gridDay";
            this.gridDay.Size = new System.Drawing.Size(1164, 581);
            this.gridDay.TabIndex = 1;
            this.gridDay.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridDay_CellDoubleClick);
            this.gridDay.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridDay_CellMouseDown);
            //
            // AgendaView
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.gridDay);
            this.Controls.Add(this.panelTop);
            this.Name = "AgendaView";
            this.Size = new System.Drawing.Size(1164, 669);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.DateTimePicker dtpDay;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnToday;
        private System.Windows.Forms.Label lblDay;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnReminders;
        private System.Windows.Forms.ProgressBar progressReminders;
        private System.Windows.Forms.Button btnCancelReminders;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.DataGridView gridDay;
    }
}
