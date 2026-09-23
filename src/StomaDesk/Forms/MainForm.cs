using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using StomaDesk.Controls;
using StomaDesk.Data;
using StomaDesk.Services;
using StomaDesk.Ui;

namespace StomaDesk.Forms
{
    public partial class MainForm : Form
    {
        private readonly ClinicStore _store;

        /// <summary>Used by the Visual Studio designer only.</summary>
        public MainForm()
        {
            InitializeComponent();

            navMain.AddPage("Pacienți", Glyph.Patients, patientsView,
                "Caută după nume, CNP sau telefon. Enter sau dublu click deschide fișa pacientului.").Shortcut = "Ctrl+1";
            navMain.AddPage("Agendă", Glyph.Agenda, agendaView,
                "Dublu click pe un loc liber face o programare; click dreapta pe o programare îi schimbă statusul.").Shortcut = "Ctrl+2";
            navMain.AddPage("Rapoarte", Glyph.Reports, reportsView,
                "Venituri, încasări și activitate pe perioada aleasă, cu export CSV pentru Excel.").Shortcut = "Ctrl+3";
            navMain.AddPage("Nomenclatoare", Glyph.Settings, settingsView,
                "Medicii, lista de prețuri și datele clinicii care apar pe deviz.").Shortcut = "Ctrl+4";
            navMain.Footer = "Versiunea " + Application.ProductVersion;
            ShowPageTitle();
            Theme.Apply(this);
        }

        public MainForm(ClinicStore store)
            : this()
        {
            _store = store;
            patientsView.Bind(store);
            agendaView.Bind(store);
            reportsView.Bind(store);
            settingsView.Bind(store);

            store.Changed += Store_Changed;
            lblToday.Text = Fmt.DayTitle(DateTime.Today);
            lblStatus.Text = "Date: " + store.FilePath;
            UpdateTitle();
        }

        /// <summary>Lets the UI smoke test walk through the pages.</summary>
        internal NavBar Navigation
        {
            get { return navMain; }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_store != null)
                _store.Changed -= Store_Changed;
            base.OnFormClosed(e);
        }

        private void Store_Changed(object sender, EventArgs e)
        {
            lblStatus.Text = string.Format("Salvat la {0:HH:mm:ss} în {1}", DateTime.Now, _store.FilePath);
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Text = "StomaDesk   " + _store.Info.Name;
            navMain.Subtitle = _store.Info.Name;
        }

        private void navMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPageTitle();
        }

        private void ShowPageTitle()
        {
            NavItem item = navMain.SelectedItem;
            lblPageTitle.Text = item == null ? "" : item.Text;
            lblPageHint.Text = item == null ? "" : item.Description;
        }

        private void mnuBackup_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Date StomaDesk (*.xml)|*.xml";
                dialog.FileName = "StomaDesk copie " + DateTime.Now.ToString("yyyy.MM.dd HHmm") + ".xml";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                File.Copy(_store.FilePath, dialog.FileName, true);
                Dialogs.Info(this, "Copia de siguranță a fost salvată în:\n" + dialog.FileName);
            }
        }

        private void mnuOpenDataFolder_Click(object sender, EventArgs e)
        {
            string folder = Path.GetDirectoryName(_store.FilePath);
            try
            {
                Process.Start(folder);
            }
            catch (Exception)
            {
                // No file manager registered (a bare Linux session, for example): show the path instead.
                Dialogs.Info(this, "Folderul cu date:\n" + folder);
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuView_Click(object sender, EventArgs e)
        {
            if (sender == mnuViewPatients)
                navMain.ShowPage(patientsView);
            else if (sender == mnuViewAgenda)
                navMain.ShowPage(agendaView);
            else if (sender == mnuViewReports)
                navMain.ShowPage(reportsView);
            else if (sender == mnuViewSettings)
                navMain.ShowPage(settingsView);
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            bool mono = Type.GetType("Mono.Runtime") != null;
            string runtime = (mono ? "Mono, CLR " : ".NET Framework, CLR ") + Environment.Version;
            Dialogs.Info(this, string.Format(
                "StomaDesk {0}\nGestiune cabinet stomatologic, aplicație demonstrativă.\n\n" +
                "Windows Forms, .NET Framework 4.5, C# 5\nRulează pe: {1}\nSistem: {2}\n\nDate: {3}",
                Application.ProductVersion, runtime, Environment.OSVersion, _store.FilePath));
        }
    }
}
