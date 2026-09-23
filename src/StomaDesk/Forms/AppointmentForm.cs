using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using StomaDesk.Data;
using StomaDesk.Models;
using StomaDesk.Services;
using StomaDesk.Ui;

namespace StomaDesk.Forms
{
    /// <summary>
    /// Book or edit an appointment. Every change of doctor, day, hour or duration re-checks
    /// the doctor's agenda and shows a clash before the user presses Salvează.
    /// </summary>
    public partial class AppointmentForm : Form
    {
        private readonly ClinicStore _store;
        private readonly Appointment _appointment;

        /// <summary>Used by the Visual Studio designer only.</summary>
        public AppointmentForm()
        {
            InitializeComponent();
        }

        public AppointmentForm(ClinicStore store, Appointment appointment, bool lockPatient)
            : this()
        {
            _appointment = appointment;
            Text = appointment.Id == 0 ? "Programare nouă" : "Programare";

            Combo.Fill(cboPatient, store.FindPatients(""), store.GetPatient(appointment.PatientId));
            cboPatient.Enabled = !lockPatient;

            List<Doctor> doctors = store.ActiveDoctors.ToList();
            Doctor current = store.GetDoctor(appointment.DoctorId);
            if (current != null && !doctors.Contains(current))
                doctors.Add(current);   // an old appointment keeps the doctor who has since left
            Combo.Fill(cboDoctor, doctors, current);

            DateTime start = appointment.Start.Year < 2000 ? DateTime.Today.AddHours(9) : appointment.Start;
            dtpDate.Value = start.Date;
            FillTimes(start.TimeOfDay);
            numDuration.Value = Math.Max(numDuration.Minimum, Math.Min(numDuration.Maximum, appointment.DurationMinutes));
            Combo.FillEnum<AppointmentStatus>(cboStatus, Labels.For, appointment.Status);

            txtReason.AutoCompleteCustomSource.AddRange(store.Procedures.Select(p => p.Name).ToArray());
            txtReason.Text = appointment.Reason;

            // Set last: from here on the input events may query the store.
            _store = store;
            UpdateConflict();
        }

        /// <summary>Shows the dialog and saves the appointment if the user confirms. Returns true when saved.</summary>
        public static bool Edit(IWin32Window owner, ClinicStore store, Appointment appointment, bool lockPatient)
        {
            using (var form = new AppointmentForm(store, appointment, lockPatient))
            {
                if (form.ShowDialog(owner) != DialogResult.OK)
                    return false;
            }
            store.SaveAppointment(appointment);
            return true;
        }

        private void FillTimes(TimeSpan selected)
        {
            var times = new List<Choice<TimeSpan>>();
            for (int minutes = 7 * 60; minutes <= 20 * 60; minutes += 15)
                times.Add(TimeChoice(TimeSpan.FromMinutes(minutes)));
            if (!times.Any(t => t.Value == selected))
                times.Add(TimeChoice(selected));
            times.Sort((a, b) => a.Value.CompareTo(b.Value));

            Combo.FillChoices(cboTime, times, selected);
        }

        private static Choice<TimeSpan> TimeChoice(TimeSpan time)
        {
            return new Choice<TimeSpan>(time, time.ToString(@"hh\:mm"));
        }

        private Patient SelectedPatient()
        {
            var patient = cboPatient.SelectedItem as Patient;
            if (patient != null)
                return patient;

            // Typed but not picked from the list: accept an exact name match.
            string typed = Search.Fold(cboPatient.Text);
            return cboPatient.Items.OfType<Patient>().FirstOrDefault(p => Search.Fold(p.FullName) == typed);
        }

        private Appointment BuildCandidate()
        {
            Appointment candidate = _appointment.Clone();
            Patient patient = SelectedPatient();
            Doctor doctor = Combo.Selected<Doctor>(cboDoctor);

            candidate.PatientId = patient == null ? 0 : patient.Id;
            candidate.DoctorId = doctor == null ? 0 : doctor.Id;
            candidate.Start = dtpDate.Value.Date + Combo.Value<TimeSpan>(cboTime);
            candidate.DurationMinutes = (int)numDuration.Value;
            candidate.Reason = txtReason.Text.Trim();
            candidate.Status = Combo.Value<AppointmentStatus>(cboStatus);
            return candidate;
        }

        private void Inputs_Changed(object sender, EventArgs e)
        {
            UpdateConflict();
        }

        private void UpdateConflict()
        {
            if (_store == null)
                return;

            Appointment clash = _store.FindConflict(BuildCandidate());
            lblConflict.Text = clash == null ? "" : DescribeConflict(clash);
        }

        private string DescribeConflict(Appointment clash)
        {
            Patient patient = _store.GetPatient(clash.PatientId);
            return string.Format("Medicul are deja o programare la {0} ({1} min){2}. Alegeți altă oră.",
                Fmt.Time(clash.Start), clash.DurationMinutes, patient == null ? "" : ", " + patient.FullName);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Appointment candidate = BuildCandidate();
            if (candidate.PatientId == 0)
            {
                Dialogs.Error(this, "Alegeți pacientul din listă.");
                cboPatient.Focus();
                return;
            }
            if (candidate.DoctorId == 0)
            {
                Dialogs.Error(this, "Alegeți medicul.");
                cboDoctor.Focus();
                return;
            }

            Appointment clash = _store.FindConflict(candidate);
            if (clash != null)
            {
                lblConflict.Text = DescribeConflict(clash);
                cboTime.Focus();
                return;
            }

            _appointment.CopyDetailsFrom(candidate);
            DialogResult = DialogResult.OK;
        }
    }
}
