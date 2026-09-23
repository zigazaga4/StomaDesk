using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using StomaDesk.Models;
using StomaDesk.Services;
using StomaDesk.Ui;

namespace StomaDesk.Forms
{
    public partial class PatientsView : StoreView
    {
        private static readonly Color OwesColor = Color.FromArgb(255, 238, 235);

        public PatientsView()
        {
            InitializeComponent();
            Grid.SetupList(gridPatients);
            Grid.AddColumn(gridPatients, "Nume și prenume", 220, fill: true);
            Grid.AddColumn(gridPatients, "CNP", 130);
            Grid.AddColumn(gridPatients, "Vârstă", 60, alignRight: true);
            Grid.AddColumn(gridPatients, "Telefon", 120);
            Grid.AddColumn(gridPatients, "Localitate", 130);
            Grid.AddColumn(gridPatients, "Ultima vizită", 100);
            Grid.AddColumn(gridPatients, "Sold (lei)", 100, alignRight: true);
        }

        public override void RefreshView()
        {
            if (Store == null)
                return;

            Dictionary<int, decimal> due = Store.DueByPatient();
            Dictionary<int, DateTime> lastVisit = Store.LastVisitByPatient();
            List<Patient> patients = Store.FindPatients(txtSearch.Text);

            Grid.Fill(gridPatients, patients,
                p => new object[]
                {
                    p.FullName,
                    p.Cnp,
                    p.Age.HasValue ? p.Age.Value.ToString() : "",
                    p.Phone,
                    p.Address,
                    lastVisit.ContainsKey(p.Id) ? Fmt.Date(lastVisit[p.Id]) : "",
                    DueOf(due, p.Id) != 0m ? Fmt.Number(DueOf(due, p.Id)) : ""
                },
                p => DueOf(due, p.Id) > 0m ? OwesColor : Color.Empty);

            lblCount.Text = patients.Count == 1 ? "1 pacient" : patients.Count + " pacienți";
            UpdateButtons();
        }

        private static decimal DueOf(Dictionary<int, decimal> due, int patientId)
        {
            decimal value;
            return due.TryGetValue(patientId, out value) ? value : 0m;
        }

        private Patient SelectedPatient
        {
            get { return Grid.Selected<Patient>(gridPatients); }
        }

        private void UpdateButtons()
        {
            bool any = SelectedPatient != null;
            btnEdit.Enabled = any;
            btnCard.Enabled = any;
            btnDelete.Enabled = any;
        }

        private void OpenSelected()
        {
            Patient patient = SelectedPatient;
            if (patient != null)
                PatientCardForm.Open(FindForm(), Store, patient);
        }

        // Typing restarts a short timer; the list refreshes once the user pauses, not on every key.
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            searchTimer.Stop();
            searchTimer.Start();
        }

        private void searchTimer_Tick(object sender, EventArgs e)
        {
            searchTimer.Stop();
            RefreshView();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchTimer.Stop();
                RefreshView();
                OpenSelected();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                gridPatients.Focus();
                e.Handled = true;
            }
        }

        private void gridPatients_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            OpenSelected();
            e.Handled = true;
        }

        private void gridPatients_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                OpenSelected();
        }

        private void gridPatients_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            var patient = new Patient();
            if (PatientForm.Edit(FindForm(), Store, patient))
                PatientCardForm.Open(FindForm(), Store, patient);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Patient patient = SelectedPatient;
            if (patient != null)
                PatientForm.Edit(FindForm(), Store, patient);
        }

        private void btnCard_Click(object sender, EventArgs e)
        {
            OpenSelected();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Patient patient = SelectedPatient;
            if (patient == null)
                return;
            if (!Dialogs.Confirm(FindForm(), "Ștergeți pacientul " + patient.FullName + " și programările lui?"))
                return;

            string reason;
            if (!Store.TryDeletePatient(patient, out reason))
                Dialogs.Error(FindForm(), reason);
        }
    }
}
