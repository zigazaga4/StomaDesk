using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StomaDesk.Data;
using StomaDesk.Models;
using StomaDesk.Services;
using StomaDesk.Ui;

namespace StomaDesk.Forms
{
    /// <summary>
    /// Doctors, price list and clinic details. The grids edit copies held in BindingLists;
    /// nothing reaches the store until "Salvează", so "Renunță" simply reloads.
    /// </summary>
    public partial class SettingsView : StoreView
    {
        private BindingList<Doctor> _doctors;
        private BindingList<Procedure> _procedures;
        private bool _dirty;
        private bool _loading;

        public SettingsView()
        {
            InitializeComponent();

            Grid.SetupEditable(gridDoctors);
            Grid.AddColumn(gridDoctors, "Nume", 190, fill: true, dataProperty: "Name");
            Grid.AddColumn(gridDoctors, "Specialitate", 170, dataProperty: "Specialty");
            Grid.AddCheckColumn(gridDoctors, "Activ", 50, "Active");

            Grid.SetupEditable(gridProcedures);
            Grid.AddColumn(gridProcedures, "Cod", 60, dataProperty: "Code");
            Grid.AddColumn(gridProcedures, "Procedură", 260, fill: true, dataProperty: "Name");
            DataGridViewTextBoxColumn price = Grid.AddColumn(gridProcedures, "Preț (lei)", 90, alignRight: true, dataProperty: "Price");
            price.DefaultCellStyle.Format = "N2";
            price.DefaultCellStyle.FormatProvider = Fmt.RoNumbers;   // 1.250,50 shown and typed, whatever the Windows region
            Grid.AddColumn(gridProcedures, "Durată (min)", 90, alignRight: true, dataProperty: "DurationMinutes");
            Grid.AddCheckColumn(gridProcedures, "Pe dinte", 70, "PerTooth");
        }

        public override void RefreshView()
        {
            // Unsaved edits win over a refresh triggered by changes elsewhere in the app.
            if (Store == null || _dirty)
                return;

            _loading = true;
            _doctors = new BindingList<Doctor>(Store.Doctors.Select(d => d.Clone()).ToList());
            _procedures = new BindingList<Procedure>(Store.Procedures.Select(p => p.Clone()).ToList());
            gridDoctors.DataSource = _doctors;
            gridProcedures.DataSource = _procedures;

            ClinicInfo info = Store.Info;
            txtClinicName.Text = info.Name;
            txtClinicAddress.Text = info.Address;
            txtClinicPhone.Text = info.Phone;
            txtClinicFiscal.Text = info.FiscalCode;
            lblDataFile.Text = "Fișier de date: " + Store.FilePath;
            _loading = false;
            SetDirty(false);
        }

        private void SetDirty(bool dirty)
        {
            _dirty = dirty;
            btnSave.Enabled = dirty;
            btnRevert.Enabled = dirty;
        }

        private void MarkEdited()
        {
            if (!_loading)
                SetDirty(true);
        }

        private void Grid_Edited(object sender, DataGridViewCellEventArgs e)
        {
            MarkEdited();
        }

        private void Grid_RowsEdited(object sender, DataGridViewRowEventArgs e)
        {
            MarkEdited();
        }

        private void Field_Edited(object sender, EventArgs e)
        {
            MarkEdited();
        }

        private void gridDoctors_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0)
                return;
            var doctor = gridDoctors.Rows[e.RowIndex].DataBoundItem as Doctor;
            if (doctor != null)
                e.CellStyle.BackColor = GdiKit.Tint(Color.FromArgb(doctor.ColorArgb), 0.6f);
        }

        private void gridDoctors_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var doctor = e.Row.DataBoundItem as Doctor;
            if (doctor == null || doctor.Id == 0 || !Store.IsDoctorUsed(doctor.Id))
                return;

            e.Cancel = true;
            Dialogs.Error(FindForm(), doctor.Name + " are programări sau tratamente în istoric. Debifați „Activ” în loc să ștergeți medicul.");
        }

        private void btnDoctorColor_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = gridDoctors.CurrentRow;
            var doctor = row == null ? null : row.DataBoundItem as Doctor;
            if (doctor == null)
            {
                Dialogs.Info(FindForm(), "Selectați întâi un medic din listă.");
                return;
            }

            using (var dialog = new ColorDialog())
            {
                dialog.Color = Color.FromArgb(doctor.ColorArgb);
                dialog.FullOpen = true;
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                    return;

                doctor.ColorArgb = dialog.Color.ToArgb();
                gridDoctors.InvalidateRow(row.Index);
                SetDirty(true);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            gridDoctors.EndEdit();
            gridProcedures.EndEdit();

            string error = FindSettingsError();
            if (error != null)
            {
                Dialogs.Error(FindForm(), error);
                return;
            }

            var info = new ClinicInfo
            {
                Name = txtClinicName.Text.Trim(),
                Address = txtClinicAddress.Text.Trim(),
                Phone = txtClinicPhone.Text.Trim(),
                FiscalCode = txtClinicFiscal.Text.Trim()
            };

            // Clear the flag first so the store's Changed event reloads the saved values.
            SetDirty(false);
            try
            {
                Store.SaveSettings(info, _doctors.ToList(), _procedures.ToList());
            }
            catch (InvalidOperationException ex)
            {
                SetDirty(true);
                Dialogs.Error(FindForm(), ex.Message);
            }
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            SetDirty(false);
            RefreshView();
        }

        private string FindSettingsError()
        {
            if (txtClinicName.Text.Trim().Length == 0)
                return "Completați numele clinicii.";
            if (!_doctors.Any(d => d.Active))
                return "Trebuie să rămână cel puțin un medic activ.";
            if (_doctors.Any(d => string.IsNullOrWhiteSpace(d.Name)))
                return "Fiecare medic are nevoie de un nume.";

            foreach (Procedure procedure in _procedures)
            {
                if (string.IsNullOrWhiteSpace(procedure.Name))
                    return "Fiecare procedură are nevoie de o denumire.";
                if (procedure.Price < 0m)
                    return string.Format("Prețul pentru „{0}” nu poate fi negativ.", procedure.Name);
                if (procedure.DurationMinutes <= 0)
                    return string.Format("Durata pentru „{0}” trebuie să fie de cel puțin un minut.", procedure.Name);
            }

            var duplicate = _procedures
                .Where(p => !string.IsNullOrWhiteSpace(p.Code))
                .GroupBy(p => p.Code.Trim().ToUpperInvariant())
                .FirstOrDefault(g => g.Count() > 1);
            if (duplicate != null)
                return string.Format("Codul {0} apare de două ori în lista de prețuri.", duplicate.Key);

            return null;
        }
    }
}
