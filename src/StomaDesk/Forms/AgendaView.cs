using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using StomaDesk.Models;
using StomaDesk.Services;
using StomaDesk.Ui;

namespace StomaDesk.Forms
{
    /// <summary>
    /// Day view: one row per half hour, one column per active doctor.
    /// Double click an empty cell to book, double click a booking to edit it, right click for status.
    /// </summary>
    public partial class AgendaView : StoreView
    {
        private const int FirstHour = 8;
        private const int LastHour = 20;
        private const int SlotMinutes = 30;

        private readonly ContextMenuStrip _menu = new ContextMenuStrip();
        private readonly Font _boldFont;
        private Appointment _menuAppointment;
        private string _layoutKey;
        private CancellationTokenSource _reminderCancel;

        public AgendaView()
        {
            InitializeComponent();
            _boldFont = new Font(Font, FontStyle.Bold);
            Grid.SetupList(gridDay);
            gridDay.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridDay.AlternatingRowsDefaultCellStyle.BackColor = Color.Empty;
            gridDay.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            gridDay.RowTemplate.Height = 30;
            gridDay.ShowCellToolTips = true;
            BuildMenu();
        }

        protected override void OnBound()
        {
            dtpDay.Value = DateTime.Today;
        }

        public override void RefreshView()
        {
            if (Store == null)
                return;

            DateTime day = dtpDay.Value.Date;
            lblDay.Text = Fmt.DayTitle(day) + (day == DateTime.Today ? "   (azi)" : "");

            IList<Doctor> doctors = Store.ActiveDoctors;
            EnsureLayout(doctors);
            ClearCells(day);

            List<Appointment> appointments = Store.AppointmentsOn(day).Where(a => a.IsActive).ToList();
            foreach (Appointment appointment in appointments)
                Place(appointment, doctors);

            if (_reminderCancel == null)
                lblSummary.Text = Summary(appointments);
        }

        private static string Summary(List<Appointment> appointments)
        {
            if (appointments.Count == 0)
                return "Nicio programare în această zi.";

            int confirmed = appointments.Count(a => a.Status != AppointmentStatus.Scheduled);
            int reminded = appointments.Count(a => a.ReminderSent);
            return string.Format("{0} programări: {1} confirmate sau prezente, {2} cu SMS trimis",
                appointments.Count, confirmed, reminded);
        }

        // ---------------------------------------------------------------- grid layout

        /// <summary>Columns and rows are rebuilt only when the doctor list changes, so the scroll position survives refreshes.</summary>
        private void EnsureLayout(IList<Doctor> doctors)
        {
            string key = string.Join("|", doctors.Select(d => d.Id + ":" + d.Name + ":" + d.ColorArgb));
            if (key == _layoutKey)
                return;
            _layoutKey = key;

            gridDay.Rows.Clear();
            gridDay.Columns.Clear();

            DataGridViewTextBoxColumn hour = Grid.AddColumn(gridDay, "Ora", 64);
            hour.Frozen = true;
            hour.DefaultCellStyle.ForeColor = Color.DimGray;
            hour.DefaultCellStyle.BackColor = Color.FromArgb(244, 246, 249);

            foreach (Doctor doctor in doctors)
            {
                DataGridViewTextBoxColumn column = Grid.AddColumn(gridDay, doctor.Name, 200, fill: true);
                column.Tag = doctor;
                column.HeaderCell.Style.BackColor = GdiKit.Tint(Color.FromArgb(doctor.ColorArgb), 0.55f);
            }

            for (int minutes = FirstHour * 60; minutes < LastHour * 60; minutes += SlotMinutes)
            {
                TimeSpan time = TimeSpan.FromMinutes(minutes);
                int index = gridDay.Rows.Add();
                DataGridViewRow row = gridDay.Rows[index];
                row.Tag = time;
                row.Cells[0].Value = time.ToString(@"hh\:mm");
            }
        }

        private void ClearCells(DateTime day)
        {
            bool closed = day.DayOfWeek == DayOfWeek.Sunday;
            foreach (DataGridViewRow row in gridDay.Rows)
            {
                var time = (TimeSpan)row.Tag;
                Color background = closed ? Color.FromArgb(242, 242, 242)
                    : time.Minutes == 0 ? Color.White
                    : Color.FromArgb(250, 251, 253);

                for (int c = 1; c < row.Cells.Count; c++)
                {
                    DataGridViewCell cell = row.Cells[c];
                    cell.Value = null;
                    cell.Tag = null;
                    cell.ToolTipText = "";
                    cell.Style.BackColor = background;
                    cell.Style.Font = null;
                    cell.Style.ForeColor = Color.Empty;
                }
            }
        }

        private void Place(Appointment appointment, IList<Doctor> doctors)
        {
            int column = IndexOfDoctor(doctors, appointment.DoctorId) + 1;
            if (column <= 0)
                return;

            Doctor doctor = doctors[column - 1];
            Patient patient = Store.GetPatient(appointment.PatientId);
            string name = patient == null ? "(pacient șters)" : patient.FullName;
            string status = appointment.Status == AppointmentStatus.Scheduled ? "" : "   [" + Labels.For(appointment.Status) + "]";

            int first = (int)((appointment.Start.TimeOfDay.TotalMinutes - FirstHour * 60) / SlotMinutes);
            int slots = Math.Max(1, (int)Math.Ceiling(appointment.DurationMinutes / (double)SlotMinutes));
            string tooltip = string.Format("{0}\nora {1}, {2} min\n{3}\nStatus: {4}{5}",
                name, Fmt.Time(appointment.Start), appointment.DurationMinutes, appointment.Reason,
                Labels.For(appointment.Status), appointment.ReminderSent ? "\nSMS de reamintire trimis" : "");

            for (int i = 0; i < slots; i++)
            {
                int rowIndex = first + i;
                if (rowIndex < 0 || rowIndex >= gridDay.Rows.Count)
                    continue;

                DataGridViewCell cell = gridDay.Rows[rowIndex].Cells[column];
                cell.Tag = appointment;
                cell.ToolTipText = tooltip;
                cell.Style.BackColor = ColorFor(appointment, doctor);

                if (i == 0)
                {
                    cell.Value = Fmt.Time(appointment.Start) + "  " + name + (slots == 1 ? "   " + appointment.Reason : "") + status;
                    cell.Style.Font = _boldFont;
                }
                else if (i == 1)
                {
                    cell.Value = "      " + appointment.Reason;
                    cell.Style.ForeColor = Color.FromArgb(70, 70, 70);
                }
            }
        }

        private static int IndexOfDoctor(IList<Doctor> doctors, int doctorId)
        {
            for (int i = 0; i < doctors.Count; i++)
            {
                if (doctors[i].Id == doctorId)
                    return i;
            }
            return -1;
        }

        private static Color ColorFor(Appointment appointment, Doctor doctor)
        {
            Color color = Color.FromArgb(doctor.ColorArgb);
            switch (appointment.Status)
            {
                case AppointmentStatus.Done: return Color.FromArgb(226, 229, 233);
                case AppointmentStatus.NoShow: return Color.FromArgb(247, 214, 210);
                case AppointmentStatus.Arrived: return GdiKit.Tint(color, 0.35f);
                case AppointmentStatus.Confirmed: return GdiKit.Tint(color, 0.55f);
                default: return GdiKit.Tint(color, 0.75f);
            }
        }

        // ---------------------------------------------------------------- booking and status

        private void gridDay_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1)
                return;

            var existing = gridDay.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag as Appointment;
            if (existing != null)
            {
                AppointmentForm.Edit(FindForm(), Store, existing, false);
                return;
            }

            var doctor = (Doctor)gridDay.Columns[e.ColumnIndex].Tag;
            var time = (TimeSpan)gridDay.Rows[e.RowIndex].Tag;
            NewAppointment(doctor, dtpDay.Value.Date + time);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            DataGridViewCell cell = gridDay.CurrentCell;
            if (cell != null && cell.ColumnIndex > 0 && cell.Tag == null)
            {
                NewAppointment((Doctor)gridDay.Columns[cell.ColumnIndex].Tag, dtpDay.Value.Date + (TimeSpan)gridDay.Rows[cell.RowIndex].Tag);
                return;
            }

            IList<Doctor> doctors = Store.ActiveDoctors;
            NewAppointment(doctors.Count > 0 ? doctors[0] : null, dtpDay.Value.Date.AddHours(9));
        }

        private void NewAppointment(Doctor doctor, DateTime start)
        {
            var appointment = new Appointment
            {
                DoctorId = doctor == null ? 0 : doctor.Id,
                Start = start,
                DurationMinutes = SlotMinutes
            };
            AppointmentForm.Edit(FindForm(), Store, appointment, false);
        }

        private void BuildMenu()
        {
            foreach (AppointmentStatus status in Enum.GetValues(typeof(AppointmentStatus)))
            {
                var item = new ToolStripMenuItem(Labels.For(status)) { Tag = status };
                item.Click += StatusItem_Click;
                _menu.Items.Add(item);
            }
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(new ToolStripMenuItem("Deschide programarea...", null, (s, e) => AppointmentForm.Edit(FindForm(), Store, _menuAppointment, false)));
            _menu.Items.Add(new ToolStripMenuItem("Fișa pacientului...", null, (s, e) => OpenPatientCard()));
            _menu.Items.Add(new ToolStripMenuItem("Șterge programarea", null, (s, e) => DeleteMenuAppointment()));
        }

        private void gridDay_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 1)
                return;

            gridDay.CurrentCell = gridDay.Rows[e.RowIndex].Cells[e.ColumnIndex];
            _menuAppointment = gridDay.CurrentCell.Tag as Appointment;
            if (_menuAppointment == null)
                return;

            foreach (ToolStripItem item in _menu.Items)
            {
                var menuItem = item as ToolStripMenuItem;
                if (menuItem != null && menuItem.Tag is AppointmentStatus)
                    menuItem.Checked = (AppointmentStatus)menuItem.Tag == _menuAppointment.Status;
            }
            _menu.Show(Cursor.Position);
        }

        private void StatusItem_Click(object sender, EventArgs e)
        {
            if (_menuAppointment == null)
                return;

            var status = (AppointmentStatus)((ToolStripMenuItem)sender).Tag;
            try
            {
                Store.SetAppointmentStatus(_menuAppointment, status);
            }
            catch (InvalidOperationException ex)
            {
                Dialogs.Error(FindForm(), ex.Message);
            }
        }

        private void OpenPatientCard()
        {
            Patient patient = _menuAppointment == null ? null : Store.GetPatient(_menuAppointment.PatientId);
            if (patient != null)
                PatientCardForm.Open(FindForm(), Store, patient);
        }

        private void DeleteMenuAppointment()
        {
            if (_menuAppointment == null)
                return;
            if (Dialogs.Confirm(FindForm(), "Ștergeți programarea? Pentru istoric, „Anulat” este de obicei mai potrivit."))
                Store.DeleteAppointment(_menuAppointment);
        }

        // ---------------------------------------------------------------- day navigation

        private void dtpDay_ValueChanged(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            dtpDay.Value = dtpDay.Value.Date.AddDays(-1);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            dtpDay.Value = dtpDay.Value.Date.AddDays(1);
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpDay.Value = DateTime.Today;
        }

        // ---------------------------------------------------------------- SMS reminders (async)

        /// <summary>
        /// The SMS calls run on the thread pool, so the window stays responsive. Thanks to async/await the code
        /// after "await" runs back on the UI thread, which is the only thread allowed to touch controls and the store.
        /// </summary>
        private async void btnReminders_Click(object sender, EventArgs e)
        {
            Form owner = FindForm();
            DateTime tomorrow = DateTime.Today.AddDays(1);
            List<Reminder> reminders = Reminders.Build(Store, tomorrow);
            if (reminders.Count == 0)
            {
                Dialogs.Info(owner, "Nu există remindere de trimis: programările de mâine au primit deja SMS sau nu există.");
                return;
            }
            if (!Dialogs.Confirm(owner, string.Format("Se trimit {0} SMS-uri pentru programările de mâine ({1}). Continuați?",
                reminders.Count, Fmt.Date(tomorrow))))
                return;

            _reminderCancel = new CancellationTokenSource();
            SetSending(true, reminders.Count);
            try
            {
                var progress = new Progress<int>(done =>
                {
                    progressReminders.Value = done;
                    lblSummary.Text = string.Format("Se trimit remindere: {0} din {1}...", done, reminders.Count);
                });

                ReminderRun run = await Reminders.SendAsync(reminders, progress, _reminderCancel.Token);
                if (IsDisposed)
                    return;

                Store.MarkRemindersSent(run.SentIds);
                string message = string.Format("SMS-uri trimise: {0} din {1}.", run.SentIds.Count, reminders.Count);
                if (run.Cancelled)
                    message += " Trimiterea a fost oprită.";
                if (run.Failures.Count > 0)
                    message += "\n\nNetrimise:\n" + string.Join("\n", run.Failures);
                Dialogs.Info(owner, message);
            }
            finally
            {
                _reminderCancel.Dispose();
                _reminderCancel = null;
                if (!IsDisposed)
                {
                    SetSending(false, 0);
                    RefreshView();
                }
            }
        }

        private void btnCancelReminders_Click(object sender, EventArgs e)
        {
            if (_reminderCancel != null)
                _reminderCancel.Cancel();
        }

        private void SetSending(bool sending, int total)
        {
            btnReminders.Enabled = !sending;
            progressReminders.Visible = sending;
            btnCancelReminders.Visible = sending;
            progressReminders.Maximum = Math.Max(1, total);
            progressReminders.Value = 0;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_reminderCancel != null)
                _reminderCancel.Cancel();
            base.OnHandleDestroyed(e);
        }
    }
}
