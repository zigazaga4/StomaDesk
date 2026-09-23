namespace StomaDesk.Forms
{
    partial class MainForm
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
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenDataFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewPatients = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewAgenda = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewReports = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPatients = new System.Windows.Forms.TabPage();
            this.patientsView = new StomaDesk.Forms.PatientsView();
            this.tabAgenda = new System.Windows.Forms.TabPage();
            this.agendaView = new StomaDesk.Forms.AgendaView();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.reportsView = new StomaDesk.Forms.ReportsView();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.settingsView = new StomaDesk.Forms.SettingsView();
            this.statusMain = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblToday = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabPatients.SuspendLayout();
            this.tabAgenda.SuspendLayout();
            this.tabReports.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.statusMain.SuspendLayout();
            this.SuspendLayout();
            //
            // menuMain
            //
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuView,
            this.mnuHelp});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(1184, 24);
            this.menuMain.TabIndex = 0;
            //
            // mnuFile
            //
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuBackup,
            this.mnuOpenDataFolder,
            this.mnuFileSeparator,
            this.mnuExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(51, 20);
            this.mnuFile.Text = "&Fișier";
            //
            // mnuBackup
            //
            this.mnuBackup.Name = "mnuBackup";
            this.mnuBackup.Size = new System.Drawing.Size(240, 22);
            this.mnuBackup.Text = "Copie de &siguranță...";
            this.mnuBackup.Click += new System.EventHandler(this.mnuBackup_Click);
            //
            // mnuOpenDataFolder
            //
            this.mnuOpenDataFolder.Name = "mnuOpenDataFolder";
            this.mnuOpenDataFolder.Size = new System.Drawing.Size(240, 22);
            this.mnuOpenDataFolder.Text = "Deschide folderul cu &date";
            this.mnuOpenDataFolder.Click += new System.EventHandler(this.mnuOpenDataFolder_Click);
            //
            // mnuFileSeparator
            //
            this.mnuFileSeparator.Name = "mnuFileSeparator";
            this.mnuFileSeparator.Size = new System.Drawing.Size(237, 6);
            //
            // mnuExit
            //
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuExit.Size = new System.Drawing.Size(240, 22);
            this.mnuExit.Text = "&Ieșire";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            //
            // mnuView
            //
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewPatients,
            this.mnuViewAgenda,
            this.mnuViewReports,
            this.mnuViewSettings});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(84, 20);
            this.mnuView.Text = "&Vizualizare";
            //
            // mnuViewPatients
            //
            this.mnuViewPatients.Name = "mnuViewPatients";
            this.mnuViewPatients.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.mnuViewPatients.Size = new System.Drawing.Size(200, 22);
            this.mnuViewPatients.Text = "Pacienți";
            this.mnuViewPatients.Click += new System.EventHandler(this.mnuView_Click);
            //
            // mnuViewAgenda
            //
            this.mnuViewAgenda.Name = "mnuViewAgenda";
            this.mnuViewAgenda.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.mnuViewAgenda.Size = new System.Drawing.Size(200, 22);
            this.mnuViewAgenda.Text = "Agendă";
            this.mnuViewAgenda.Click += new System.EventHandler(this.mnuView_Click);
            //
            // mnuViewReports
            //
            this.mnuViewReports.Name = "mnuViewReports";
            this.mnuViewReports.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.mnuViewReports.Size = new System.Drawing.Size(200, 22);
            this.mnuViewReports.Text = "Rapoarte";
            this.mnuViewReports.Click += new System.EventHandler(this.mnuView_Click);
            //
            // mnuViewSettings
            //
            this.mnuViewSettings.Name = "mnuViewSettings";
            this.mnuViewSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
            this.mnuViewSettings.Size = new System.Drawing.Size(200, 22);
            this.mnuViewSettings.Text = "Nomenclatoare";
            this.mnuViewSettings.Click += new System.EventHandler(this.mnuView_Click);
            //
            // mnuHelp
            //
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(56, 20);
            this.mnuHelp.Text = "&Ajutor";
            //
            // mnuAbout
            //
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(200, 22);
            this.mnuAbout.Text = "Despre StomaDesk";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            //
            // tabMain
            //
            this.tabMain.Controls.Add(this.tabPatients);
            this.tabMain.Controls.Add(this.tabAgenda);
            this.tabMain.Controls.Add(this.tabReports);
            this.tabMain.Controls.Add(this.tabSettings);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 24);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Drawing.Point(14, 6);
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1184, 715);
            this.tabMain.TabIndex = 1;
            //
            // tabPatients
            //
            this.tabPatients.Controls.Add(this.patientsView);
            this.tabPatients.Location = new System.Drawing.Point(4, 30);
            this.tabPatients.Name = "tabPatients";
            this.tabPatients.Padding = new System.Windows.Forms.Padding(6);
            this.tabPatients.Size = new System.Drawing.Size(1176, 681);
            this.tabPatients.TabIndex = 0;
            this.tabPatients.Text = "Pacienți";
            this.tabPatients.UseVisualStyleBackColor = true;
            //
            // patientsView
            //
            this.patientsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientsView.Location = new System.Drawing.Point(6, 6);
            this.patientsView.Name = "patientsView";
            this.patientsView.Size = new System.Drawing.Size(1164, 669);
            this.patientsView.TabIndex = 0;
            //
            // tabAgenda
            //
            this.tabAgenda.Controls.Add(this.agendaView);
            this.tabAgenda.Location = new System.Drawing.Point(4, 30);
            this.tabAgenda.Name = "tabAgenda";
            this.tabAgenda.Padding = new System.Windows.Forms.Padding(6);
            this.tabAgenda.Size = new System.Drawing.Size(1176, 681);
            this.tabAgenda.TabIndex = 1;
            this.tabAgenda.Text = "Agendă";
            this.tabAgenda.UseVisualStyleBackColor = true;
            //
            // agendaView
            //
            this.agendaView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.agendaView.Location = new System.Drawing.Point(6, 6);
            this.agendaView.Name = "agendaView";
            this.agendaView.Size = new System.Drawing.Size(1164, 669);
            this.agendaView.TabIndex = 0;
            //
            // tabReports
            //
            this.tabReports.Controls.Add(this.reportsView);
            this.tabReports.Location = new System.Drawing.Point(4, 30);
            this.tabReports.Name = "tabReports";
            this.tabReports.Padding = new System.Windows.Forms.Padding(6);
            this.tabReports.Size = new System.Drawing.Size(1176, 681);
            this.tabReports.TabIndex = 2;
            this.tabReports.Text = "Rapoarte";
            this.tabReports.UseVisualStyleBackColor = true;
            //
            // reportsView
            //
            this.reportsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsView.Location = new System.Drawing.Point(6, 6);
            this.reportsView.Name = "reportsView";
            this.reportsView.Size = new System.Drawing.Size(1164, 669);
            this.reportsView.TabIndex = 0;
            //
            // tabSettings
            //
            this.tabSettings.Controls.Add(this.settingsView);
            this.tabSettings.Location = new System.Drawing.Point(4, 30);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(6);
            this.tabSettings.Size = new System.Drawing.Size(1176, 681);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Nomenclatoare";
            this.tabSettings.UseVisualStyleBackColor = true;
            //
            // settingsView
            //
            this.settingsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsView.Location = new System.Drawing.Point(6, 6);
            this.settingsView.Name = "settingsView";
            this.settingsView.Size = new System.Drawing.Size(1164, 669);
            this.settingsView.TabIndex = 0;
            //
            // statusMain
            //
            this.statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblToday});
            this.statusMain.Location = new System.Drawing.Point(0, 739);
            this.statusMain.Name = "statusMain";
            this.statusMain.Size = new System.Drawing.Size(1184, 22);
            this.statusMain.TabIndex = 2;
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1000, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Gata";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblToday
            //
            this.lblToday.Name = "lblToday";
            this.lblToday.Size = new System.Drawing.Size(169, 17);
            this.lblToday.Text = "Azi";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.statusMain);
            this.Controls.Add(this.menuMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuMain;
            this.MinimumSize = new System.Drawing.Size(980, 620);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StomaDesk";
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabPatients.ResumeLayout(false);
            this.tabAgenda.ResumeLayout(false);
            this.tabReports.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.statusMain.ResumeLayout(false);
            this.statusMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuBackup;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenDataFolder;
        private System.Windows.Forms.ToolStripSeparator mnuFileSeparator;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewPatients;
        private System.Windows.Forms.ToolStripMenuItem mnuViewAgenda;
        private System.Windows.Forms.ToolStripMenuItem mnuViewReports;
        private System.Windows.Forms.ToolStripMenuItem mnuViewSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPatients;
        private StomaDesk.Forms.PatientsView patientsView;
        private System.Windows.Forms.TabPage tabAgenda;
        private StomaDesk.Forms.AgendaView agendaView;
        private System.Windows.Forms.TabPage tabReports;
        private StomaDesk.Forms.ReportsView reportsView;
        private System.Windows.Forms.TabPage tabSettings;
        private StomaDesk.Forms.SettingsView settingsView;
        private System.Windows.Forms.StatusStrip statusMain;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblToday;
    }
}
