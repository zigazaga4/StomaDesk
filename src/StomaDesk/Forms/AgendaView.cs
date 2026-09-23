using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
        private readonly System.Windows.Forms.Timer _clock = new System.Windows.Forms.Timer { Interval = 60000 };
        private Dictionary<int, int> _countByDoctor = new Dictionary<int, int>();
        private Appointment _menuAppointment;
        private string _layoutKey;
        private CancellationTokenSource _reminderCancel;

        public AgendaView()
        {
            InitializeComponent();
            Grid.SetupList(gridDay);
            gridDay.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridDay.CellBorderStyle = DataGridViewCellBorderStyle.None;   // every cell is drawn in gridDay_CellPainting
            gridDay.ColumnHeadersHeight = 42;
            gridDay.RowTemplate.Height = 30;
            gridDay.ShowCellToolTips = true;
            BuildMenu();
            Theme.Primary(btnNew);

            // Moves the red "now" line once a minute.
            _clock.Tick += (s, e) => gridDay.Invalidate();
            _clock.Start();
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
            ClearCells();

            List<Appointment> appointments = Store.AppointmentsOn(day).Where(a => a.IsActive).ToList();
            foreach (Appointment appointment in appointments)
                Place(appointment, doctors);
            _countByDoctor = appointments.GroupBy(a => a.DoctorId).ToDictionary(g => g.Key, g => g.Count());
            gridDay.Invalidate();

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

            foreach (Doctor doctor in doctors)
            {
                DataGridViewTextBoxColumn column = Grid.AddColumn(gridDay, doctor.Name, 200, fill: true);
                column.Tag = doctor;
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

        /// <summary>The cells hold no text: each slot keeps its appointment in Tag and gridDay_CellPainting draws it.</summary>
        private void ClearCells()
        {
            foreach (DataGridViewRow row in gridDay.Rows)
            {
                for (int c = 1; c < row.Cells.Count; c++)
                {
                    DataGridViewCell cell = row.Cells[c];
                    cell.Tag = null;
                    cell.ToolTipText = "";
                }
            }
        }

        private void Place(Appointment appointment, IList<Doctor> doctors)
        {
            int column = IndexOfDoctor(doctors, appointment.DoctorId) + 1;
            if (column <= 0)
                return;

            int first = (int)((appointment.Start.TimeOfDay.TotalMinutes - FirstHour * 60) / SlotMinutes);
            int slots = Math.Max(1, (int)Math.Ceiling(appointment.DurationMinutes / (double)SlotMinutes));
            string tooltip = string.Format("{0}\nora {1}, {2} min\n{3}\nStatus: {4}{5}",
                PatientName(appointment), Fmt.Time(appointment.Start), appointment.DurationMinutes, appointment.Reason,
                Labels.For(appointment.Status), appointment.ReminderSent ? "\nSMS de reamintire trimis" : "");

            for (int i = 0; i < slots; i++)
            {
                int rowIndex = first + i;
                if (rowIndex < 0 || rowIndex >= gridDay.Rows.Count)
                    continue;

                DataGridViewCell cell = gridDay.Rows[rowIndex].Cells[column];
                cell.Tag = appointment;
                cell.ToolTipText = tooltip;
            }
        }

        private string PatientName(Appointment appointment)
        {
            Patient patient = Store.GetPatient(appointment.PatientId);
            return patient == null ? "(pacient șters)" : patient.FullName;
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

        // ---------------------------------------------------------------- painting

        private void gridDay_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            if (e.RowIndex < 0)
                PaintHeader(g, e.CellBounds, e.ColumnIndex);
            else if (e.ColumnIndex == 0)
                PaintHour(g, e.CellBounds, e.RowIndex);
            else
                PaintSlot(g, e.CellBounds, e.RowIndex, e.ColumnIndex, (e.State & DataGridViewElementStates.Selected) != 0);
            e.Handled = true;
        }

        /// <summary>Doctor columns: a colour dot, the name, the day's count and a stripe in the doctor's colour.</summary>
        private void PaintHeader(Graphics g, Rectangle b, int column)
        {
            using (var back = new SolidBrush(Theme.Surface))
                g.FillRectangle(back, b);
            using (var line = new Pen(Theme.Line))
            {
                g.DrawLine(line, b.Left, b.Bottom - 1, b.Right, b.Bottom - 1);
                g.DrawLine(line, b.Right - 1, b.Top, b.Right - 1, b.Bottom);
            }

            var doctor = gridDay.Columns[column].Tag as Doctor;
            if (doctor == null)
            {
                GdiKit.DrawText(g, "ORA", Theme.UiFont(7.75f, FontStyle.Bold), Theme.Muted, new RectangleF(b.X, b.Y, b.Width - 10f, b.Height), StringAlignment.Far);
                return;
            }

            Color color = Color.FromArgb(doctor.ColorArgb);
            using (var stripe = new SolidBrush(color))
            {
                g.FillRectangle(stripe, b.X, b.Bottom - 3, b.Width - 1, 3);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(stripe, b.X + 12f, b.Y + (b.Height - 10f) / 2f - 1f, 10f, 10f);
            }

            int count;
            _countByDoctor.TryGetValue(doctor.Id, out count);
            string info = count == 0 ? "liber" : count == 1 ? "1 programare" : count + " programări";
            Font small = Theme.UiFont(8f);
            float infoWidth = g.MeasureString(info, small).Width + 4f;
            GdiKit.DrawText(g, doctor.Name, Theme.UiFont(9.5f, FontStyle.Bold), Theme.Ink, new RectangleF(b.X + 28f, b.Y, b.Width - 40f - infoWidth, b.Height - 2f));
            GdiKit.DrawText(g, info, small, Theme.Muted, new RectangleF(b.Right - infoWidth - 10f, b.Y, infoWidth, b.Height - 2f), StringAlignment.Far);
        }

        private void PaintHour(Graphics g, Rectangle b, int row)
        {
            var time = (TimeSpan)gridDay.Rows[row].Tag;
            using (var back = new SolidBrush(Theme.Surface))
                g.FillRectangle(back, b);
            using (var line = new Pen(Theme.Line))
                g.DrawLine(line, b.Right - 1, b.Top, b.Right - 1, b.Bottom);

            bool fullHour = time.Minutes == 0;
            GdiKit.DrawText(g, time.ToString(@"hh\:mm"), Theme.UiFont(fullHour ? 9f : 8.25f, fullHour ? FontStyle.Bold : FontStyle.Regular),
                fullHour ? Theme.Ink : Theme.Muted, new RectangleF(b.X, b.Y, b.Width - 12f, b.Height), StringAlignment.Far);
            PaintNowLine(g, b, time, true);
        }

        private void PaintSlot(Graphics g, Rectangle b, int row, int column, bool selected)
        {
            DateTime day = dtpDay.Value.Date;
            var time = (TimeSpan)gridDay.Rows[row].Tag;
            Color back = day.DayOfWeek == DayOfWeek.Sunday ? Color.FromArgb(240, 242, 243)
                : time.Minutes == 0 ? Theme.Surface
                : Theme.SurfaceAlt;
            using (var brush = new SolidBrush(back))
                g.FillRectangle(brush, b);
            // A stronger line where a new hour starts, a faint one at the half hour.
            using (var slotLine = new Pen(time.Minutes == 30 ? Theme.Line : Color.FromArgb(238, 242, 244)))
                g.DrawLine(slotLine, b.Left, b.Bottom - 1, b.Right, b.Bottom - 1);
            using (var columnLine = new Pen(Theme.Line))
                g.DrawLine(columnLine, b.Right - 1, b.Top, b.Right - 1, b.Bottom);

            var appointment = gridDay.Rows[row].Cells[column].Tag as Appointment;
            if (appointment != null)
            {
                PaintAppointment(g, b, row, column, appointment);
            }
            else if (selected)
            {
                // The free slot "Programare nouă" would use.
                var hint = new RectangleF(b.X + 4f, b.Y + 2f, b.Width - 9f, b.Height - 5f);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GdiKit.RoundedRect(hint, 5f))
                using (var fill = new SolidBrush(Theme.BrandLight))
                using (var border = new Pen(GdiKit.Tint(Theme.Brand, 0.45f)))
                {
                    g.FillPath(fill, path);
                    g.DrawPath(border, path);
                }
                GdiKit.DrawText(g, "+ " + time.ToString(@"hh\:mm") + "   programare nouă", Theme.UiFont(8.25f, FontStyle.Bold), Theme.Brand,
                    new RectangleF(hint.X + 10f, hint.Y, hint.Width - 14f, hint.Height));
            }
            PaintNowLine(g, b, time, false);
        }

        /// <summary>
        /// One slot of an appointment card. A booking over several slots is drawn slot by slot: the first rounds the
        /// top corners and carries the time, name and status, the second the reason, the last rounds the bottom.
        /// </summary>
        private void PaintAppointment(Graphics g, Rectangle b, int row, int column, Appointment appointment)
        {
            int offset = 0;
            while (row - offset - 1 >= 0 && ReferenceEquals(gridDay.Rows[row - offset - 1].Cells[column].Tag, appointment))
                offset++;
            bool first = offset == 0;
            bool last = row + 1 >= gridDay.Rows.Count || !ReferenceEquals(gridDay.Rows[row + 1].Cells[column].Tag, appointment);

            Color color = Color.FromArgb(((Doctor)gridDay.Columns[column].Tag).ColorArgb);
            bool closed = appointment.Status == AppointmentStatus.Done || appointment.Status == AppointmentStatus.NoShow;
            Color fill = closed ? Color.FromArgb(242, 244, 245) : GdiKit.Tint(color, 0.78f);
            Color bar = closed ? Theme.LineStrong : color;
            Color ink = closed ? Theme.Muted : Theme.Ink;

            var card = new RectangleF(b.X + 4f, b.Y + (first ? 3f : 0f), b.Width - 9f, b.Height - (first ? 3f : 0f) - (last ? 4f : 0f));
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = GdiKit.RoundedRect(card, 6f, first, last))
            using (var cardBrush = new SolidBrush(fill))
            using (var barBrush = new SolidBrush(bar))
            {
                g.FillPath(cardBrush, path);
                g.SetClip(path, CombineMode.Intersect);
                g.FillRectangle(barBrush, card.X, card.Y, 4f, card.Height);
            }
            g.Restore(state);

            float x = card.X + 12f;
            float width = card.Width - 16f;
            if (first)
            {
                string time = Fmt.Time(appointment.Start);
                Font timeFont = Theme.UiFont(8.25f, FontStyle.Bold);
                float timeWidth = g.MeasureString(time, timeFont).Width;
                GdiKit.DrawText(g, time, timeFont, closed ? Theme.Muted : GdiKit.Shade(color, 0.45f), new RectangleF(x, card.Y, timeWidth, card.Height));

                float pillWidth = 0f;
                if (appointment.Status != AppointmentStatus.Scheduled && card.Width > 200f)
                {
                    string status = Labels.For(appointment.Status);
                    Font pillFont = Theme.UiFont(7.5f, FontStyle.Bold);
                    SizeF pill = GdiKit.PillSize(g, status, pillFont);
                    pillWidth = pill.Width + 8f;
                    GdiKit.DrawPill(g, status, pillFont, Theme.Tone(appointment.Status),
                        new RectangleF(card.Right - pill.Width - 6f, card.Y + (card.Height - pill.Height) / 2f, pill.Width, pill.Height));
                }

                string name = PatientName(appointment);
                Font nameFont = Theme.UiFont(9f, FontStyle.Bold);
                var nameBox = new RectangleF(x + timeWidth + 5f, card.Y, width - timeWidth - 5f - pillWidth, card.Height);
                GdiKit.DrawText(g, name, nameFont, ink, nameBox);

                // A single-slot booking has no second line, so its reason follows the name.
                if (last && !string.IsNullOrEmpty(appointment.Reason))
                {
                    float nameWidth = g.MeasureString(name, nameFont).Width + 6f;
                    GdiKit.DrawText(g, appointment.Reason, Theme.UiFont(8.25f), Theme.Muted,
                        new RectangleF(nameBox.X + nameWidth, card.Y, nameBox.Width - nameWidth, card.Height));
                }
            }
            else if (offset == 1)
            {
                string detail = string.IsNullOrEmpty(appointment.Reason) ? appointment.DurationMinutes + " min" : appointment.Reason;
                GdiKit.DrawText(g, detail, Theme.UiFont(8.5f), closed ? Theme.Muted : GdiKit.Shade(color, 0.55f), new RectangleF(x, card.Y, width, card.Height - (last ? 1f : 0f)));
            }
            else if (offset == 2 && appointment.ReminderSent)
            {
                GdiKit.DrawText(g, "SMS de reamintire trimis", Theme.UiFont(8f), Theme.Muted, new RectangleF(x, card.Y, width, card.Height));
            }
        }

        /// <summary>Red line at the current time, on today's page only.</summary>
        private void PaintNowLine(Graphics g, Rectangle b, TimeSpan slot, bool hourColumn)
        {
            DateTime now = DateTime.Now;
            if (dtpDay.Value.Date != now.Date)
                return;

            double minutes = (now.TimeOfDay - slot).TotalMinutes;
            if (minutes < 0 || minutes >= SlotMinutes)
                return;

            float y = b.Y + (float)(minutes / SlotMinutes) * b.Height;
            using (var pen = new Pen(Theme.Danger, 2f))
                g.DrawLine(pen, b.Left, y, b.Right, y);
            if (hourColumn)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var dot = new SolidBrush(Theme.Danger))
                    g.FillEllipse(dot, b.Right - 10f, y - 4f, 8f, 8f);
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
            _clock.Stop();
            base.OnHandleDestroyed(e);
        }
    }
}
