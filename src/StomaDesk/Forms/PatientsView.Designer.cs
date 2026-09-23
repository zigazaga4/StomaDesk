namespace StomaDesk.Forms
{
    partial class PatientsView
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
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnCard = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.gridPatients = new System.Windows.Forms.DataGridView();
            this.searchTimer = new System.Windows.Forms.Timer(this.components);
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPatients)).BeginInit();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblSearch);
            this.panelTop.Controls.Add(this.txtSearch);
            this.panelTop.Controls.Add(this.btnNew);
            this.panelTop.Controls.Add(this.btnEdit);
            this.panelTop.Controls.Add(this.btnCard);
            this.panelTop.Controls.Add(this.btnDelete);
            this.panelTop.Controls.Add(this.lblCount);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1164, 50);
            this.panelTop.TabIndex = 0;
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(4, 17);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(40, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Caută:";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(56, 13);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(300, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // btnNew
            //
            this.btnNew.Location = new System.Drawing.Point(372, 10);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(120, 30);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "Pacient nou";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            //
            // btnEdit
            //
            this.btnEdit.Location = new System.Drawing.Point(498, 10);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(110, 30);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Editează";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // btnCard
            //
            this.btnCard.Location = new System.Drawing.Point(614, 10);
            this.btnCard.Name = "btnCard";
            this.btnCard.Size = new System.Drawing.Size(140, 30);
            this.btnCard.TabIndex = 4;
            this.btnCard.Text = "Fișa pacientului";
            this.btnCard.UseVisualStyleBackColor = true;
            this.btnCard.Click += new System.EventHandler(this.btnCard_Click);
            //
            // btnDelete
            //
            this.btnDelete.Location = new System.Drawing.Point(760, 10);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 30);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Șterge";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // lblCount
            //
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.ForeColor = System.Drawing.Color.DimGray;
            this.lblCount.Location = new System.Drawing.Point(964, 13);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(196, 23);
            this.lblCount.TabIndex = 6;
            this.lblCount.Text = "0 pacienți";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridPatients
            //
            this.gridPatients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPatients.Location = new System.Drawing.Point(0, 50);
            this.gridPatients.Name = "gridPatients";
            this.gridPatients.Size = new System.Drawing.Size(1164, 619);
            this.gridPatients.TabIndex = 1;
            this.gridPatients.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridPatients_CellDoubleClick);
            this.gridPatients.SelectionChanged += new System.EventHandler(this.gridPatients_SelectionChanged);
            this.gridPatients.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridPatients_KeyDown);
            //
            // searchTimer
            //
            this.searchTimer.Interval = 250;
            this.searchTimer.Tick += new System.EventHandler(this.searchTimer_Tick);
            //
            // PatientsView
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.gridPatients);
            this.Controls.Add(this.panelTop);
            this.Name = "PatientsView";
            this.Size = new System.Drawing.Size(1164, 669);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPatients)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnCard;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.DataGridView gridPatients;
        private System.Windows.Forms.Timer searchTimer;
    }
}
