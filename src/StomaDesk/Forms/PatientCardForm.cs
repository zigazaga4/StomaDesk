using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using StomaDesk.Controls;
using StomaDesk.Data;
using StomaDesk.Models;
using StomaDesk.Services;
using StomaDesk.Ui;

namespace StomaDesk.Forms
{
    /// <summary>
    /// "Fișa pacientului": dental chart, treatment plan with estimate, appointments and payments.
    /// Every change goes through the store; its Changed event redraws the whole card,
    /// so no button has to remember which parts of the screen it affects.
    /// </summary>
    public partial class PatientCardForm : Form
    {
        private const string NoTooth = "(fără)";

        private readonly ClinicStore _store;
        private readonly Patient _patient;
        private readonly ContextMenuStrip _toothMenu = new ContextMenuStrip();

        /// <summary>Used by the Visual Studio designer only.</summary>
        public PatientCardForm()
        {
            InitializeComponent();

            navCard.AddPage("Odontogramă", Glyph.Tooth, pageOdontogram, null);
            navCard.AddPage("Plan de tratament", Glyph.Plan, pagePlan, null);
            navCard.AddPage("Programări", Glyph.Agenda, pageAppointments, null);
            navCard.AddPage("Încasări", Glyph.Payments, pagePayments, null);

            Theme.Primary(btnApplyTooth, btnAddProcedure, btnNewAppointment, btnAddPayment);
            Theme.Destructive(btnDeleteTreatment);
            Theme.Apply(this);
        }

        public PatientCardForm(ClinicStore store, Patient patient)
            : this()
        {
            _store = store;
            _patient = patient;

            SetupGrids();
            FillChoices();
            BuildToothMenu();

            _store.Changed += Store_Changed;
            RefreshAll();
        }

        public static void Open(IWin32Window owner, ClinicStore store, Patient patient)
        {
            using (var form = new PatientCardForm(store, patient))
            {
                form.ShowDialog(owner);
            }
        }

        /// <summary>Lets the UI smoke test walk through the pages.</summary>
        internal NavBar Navigation
        {
            get { return navCard; }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_store != null)
                _store.Changed -= Store_Changed;
            _toothMenu.Dispose();
            base.OnFormClosed(e);
        }

        private void Store_Changed(object sender, EventArgs e)
        {
            RefreshAll();
        }

        // ---------------------------------------------------------------- setup

        private void SetupGrids()
        {
            Grid.SetupList(gridPlan, true);
            Grid.AddColumn(gridPlan, "Dinte", 60);
            Grid.AddColumn(gridPlan, "Procedură", 260, fill: true);
            Grid.AddColumn(gridPlan, "Medic", 160);
            Grid.AddColumn(gridPlan, "Preț", 90, alignRight: true);
            Grid.AddColumn(gridPlan, "Discount", 70, alignRight: true);
            Grid.AddColumn(gridPlan, "Total", 90, alignRight: true);
            Grid.AddColumn(gridPlan, "Status", 100);
            Grid.AddColumn(gridPlan, "Finalizat la", 100);
            Grid.BadgeColumn<TreatmentItem>(gridPlan, 6, t => Theme.Tone(t.Status));

            Grid.SetupList(gridAppointments);
            Grid.AddColumn(gridAppointments, "Data", 100);
            Grid.AddColumn(gridAppointments, "Ora", 60);
            Grid.AddColumn(gridAppointments, "Medic", 170);
            Grid.AddColumn(gridAppointments, "Durată", 70, alignRight: true);
            Grid.AddColumn(gridAppointments, "Motiv", 260, fill: true);
            Grid.AddColumn(gridAppointments, "Status", 110);
            Grid.AddColumn(gridAppointments, "SMS", 60);
            Grid.BadgeColumn<Appointment>(gridAppointments, 5, a => Theme.Tone(a.Status));

            Grid.SetupList(gridPayments);
            Grid.AddColumn(gridPayments, "Data", 100);
            Grid.AddColumn(gridPayments, "Chitanța", 110);
            Grid.AddColumn(gridPayments, "Sumă (lei)", 110, alignRight: true);
            Grid.AddColumn(gridPayments, "Metodă", 130);
            Grid.AddColumn(gridPayments, "Observații", 260, fill: true);
        }

        private void FillChoices()
        {
            Combo.Fill(cboProcedure, _store.Procedures, null);

            var teeth = new List<object> { NoTooth };
            teeth.AddRange(Dentition.All.Cast<object>());
            Combo.Fill(cboPlanTooth, teeth, NoTooth);

            Combo.Fill(cboPlanDoctor, _store.ActiveDoctors, null);
            Combo.FillEnum<TreatmentStatus>(cboPlanStatus, Labels.For, TreatmentStatus.Done);
            Combo.FillEnum<PaymentMethod>(cboMethod, Labels.For, PaymentMethod.Card);
            Combo.FillEnum<ToothState>(cboToothState, Labels.For, ToothState.Healthy);
        }

        private void BuildToothMenu()
        {
            foreach (ToothState state in Enum.GetValues(typeof(ToothState)))
            {
                var item = new ToolStripMenuItem(Labels.For(state)) { Tag = state };
                item.Click += ToothStateItem_Click;
                _toothMenu.Items.Add(item);
            }
            _toothMenu.Items.Add(new ToolStripSeparator());
            _toothMenu.Items.Add(new ToolStripMenuItem("Adaugă procedură pe acest dinte...", null, btnPlanTooth_Click));
        }

        // ---------------------------------------------------------------- refresh

        private void RefreshAll()
        {
            RefreshHeader();
            RefreshTeeth();
            RefreshPlan();
            RefreshAppointments();
            RefreshPayments();
        }

        private void RefreshHeader()
        {
            Text = "Fișa pacientului: " + _patient.FullName;
            lblName.Text = _patient.FullName;
            avatarPatient.Text = _patient.FullName;

            var details = new List<string>();
            if (!string.IsNullOrEmpty(_patient.Cnp))
                details.Add("CNP " + _patient.Cnp);
            if (_patient.Age.HasValue)
                details.Add(_patient.Age.Value + " ani");
            if (!string.IsNullOrEmpty(_patient.Phone))
                details.Add("Tel. " + _patient.Phone);
            if (!string.IsNullOrEmpty(_patient.Email))
                details.Add(_patient.Email);
            if (!string.IsNullOrEmpty(_patient.Address))
                details.Add(_patient.Address);
            lblDetails.Text = string.Join("     ", details);

            bool allergic = !string.IsNullOrWhiteSpace(_patient.Allergies);
            lblAllergies.Text = allergic ? "ALERGII: " + _patient.Allergies : "Fără alergii cunoscute";
            lblAllergies.ForeColor = allergic ? Theme.Danger : Theme.Muted;

            Balance balance = _store.BalanceFor(_patient.Id);
            if (balance.Due > 0m)
                lblBalance.Text = "De plată: " + Fmt.Money(balance.Due);
            else if (balance.Due < 0m)
                lblBalance.Text = "Avans: " + Fmt.Money(-balance.Due);
            else
                lblBalance.Text = "Sold la zi";
            lblBalance.ForeColor = balance.Due > 0m ? Theme.Danger : Theme.Success;
        }

        private void RefreshTeeth()
        {
            IEnumerable<int> planned = _store.TreatmentsFor(_patient.Id)
                .Where(t => t.Status != TreatmentStatus.Done && t.Tooth.HasValue)
                .Select(t => t.Tooth.Value);
            odontogram.SetData(_patient.Teeth, planned);
            ShowSelectedTooth();
        }

        private void ShowSelectedTooth()
        {
            int tooth = odontogram.SelectedTooth;
            bool selected = tooth != 0;
            lblTooth.Text = selected ? "Dinte selectat: " + tooth : "Alegeți un dinte din schemă";
            cboToothState.Enabled = selected;
            txtToothNote.Enabled = selected;
            btnApplyTooth.Enabled = selected;
            btnPlanTooth.Enabled = selected;
            if (!selected)
                return;

            ToothRecord record = _patient.FindTooth(tooth);
            Combo.SelectValue(cboToothState, record == null ? ToothState.Healthy : record.State);
            txtToothNote.Text = record == null ? "" : record.Note;
        }

        private void RefreshPlan()
        {
            List<TreatmentItem> items = _store.TreatmentsFor(_patient.Id);
            Grid.Fill(gridPlan, items,
                t => new object[]
                {
                    t.Tooth.HasValue ? t.Tooth.Value.ToString() : "",
                    t.ProcedureName,
                    _store.DoctorName(t.DoctorId),
                    Fmt.Number(t.Price),
                    t.DiscountPercent > 0m ? t.DiscountPercent.ToString("0.#", Fmt.RoNumbers) + " %" : "",
                    Fmt.Number(t.Total),
                    Labels.For(t.Status),
                    Fmt.Date(t.CompletedAt)
                });

            decimal open = items.Where(t => t.Status != TreatmentStatus.Done).Sum(t => t.Total);
            decimal done = items.Where(t => t.Status == TreatmentStatus.Done).Sum(t => t.Total);
            lblPlanTotals.Text = string.Format("De efectuat: {0}      Finalizat: {1}", Fmt.Money(open), Fmt.Money(done));
        }

        private void RefreshAppointments()
        {
            Grid.Fill(gridAppointments, _store.AppointmentsFor(_patient.Id),
                a => new object[]
                {
                    Fmt.Date(a.Start),
                    Fmt.Time(a.Start),
                    _store.DoctorName(a.DoctorId),
                    a.DurationMinutes + " min",
                    a.Reason,
                    Labels.For(a.Status),
                    a.ReminderSent ? "trimis" : ""
                },
                a => a.Start.Date >= DateTime.Today && a.IsActive ? GdiKit.Tint(Theme.Info, 0.94f) : Color.Empty);
        }

        private void RefreshPayments()
        {
            Grid.Fill(gridPayments, _store.PaymentsFor(_patient.Id),
                p => new object[] { Fmt.Date(p.Date), p.ReceiptNo, Fmt.Number(p.Amount), Labels.For(p.Method), p.Note });

            Balance balance = _store.BalanceFor(_patient.Id);
            lblPaymentTotals.Text = string.Format("Proceduri finalizate: {0}       Încasat: {1}       Sold: {2}",
                Fmt.Money(balance.Billed), Fmt.Money(balance.Paid), Fmt.Money(balance.Due));
            numAmount.Value = Math.Max(0m, Math.Min(numAmount.Maximum, balance.Due));
        }

        // ---------------------------------------------------------------- header

        private void btnEditPatient_Click(object sender, EventArgs e)
        {
            PatientForm.Edit(this, _store, _patient);
        }

        // ---------------------------------------------------------------- dental chart

        private void odontogram_SelectedToothChanged(object sender, EventArgs e)
        {
            ShowSelectedTooth();
        }

        private void odontogram_ToothClicked(object sender, ToothEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            ToothState current = odontogram.StateOf(e.Tooth);
            foreach (ToolStripItem item in _toothMenu.Items)
            {
                var menuItem = item as ToolStripMenuItem;
                if (menuItem != null && menuItem.Tag is ToothState)
                    menuItem.Checked = (ToothState)menuItem.Tag == current;
            }
            _toothMenu.Show(odontogram, e.Location);
        }

        private void ToothStateItem_Click(object sender, EventArgs e)
        {
            int tooth = odontogram.SelectedTooth;
            if (tooth == 0)
                return;

            var state = (ToothState)((ToolStripMenuItem)sender).Tag;
            ToothRecord record = _patient.FindTooth(tooth);
            _store.SetTooth(_patient, tooth, state, record == null ? null : record.Note);
        }

        private void btnApplyTooth_Click(object sender, EventArgs e)
        {
            int tooth = odontogram.SelectedTooth;
            if (tooth != 0)
                _store.SetTooth(_patient, tooth, Combo.Value<ToothState>(cboToothState), txtToothNote.Text.Trim());
        }

        private void btnPlanTooth_Click(object sender, EventArgs e)
        {
            int tooth = odontogram.SelectedTooth;
            if (tooth == 0)
                return;

            navCard.ShowPage(pagePlan);
            cboPlanTooth.SelectedItem = tooth;
            cboProcedure.Focus();
        }

        // ---------------------------------------------------------------- treatment plan

        private int? SelectedPlanTooth
        {
            get { return cboPlanTooth.SelectedItem is int ? (int?)(int)cboPlanTooth.SelectedItem : null; }
        }

        private void cboProcedure_SelectedIndexChanged(object sender, EventArgs e)
        {
            var procedure = Combo.Selected<Procedure>(cboProcedure);
            cboPlanTooth.Enabled = procedure == null || procedure.PerTooth;
        }

        private void btnAddProcedure_Click(object sender, EventArgs e)
        {
            var procedure = Combo.Selected<Procedure>(cboProcedure);
            if (procedure == null)
            {
                Dialogs.Error(this, "Alegeți procedura din listă.");
                return;
            }

            int? tooth = SelectedPlanTooth;
            if (procedure.PerTooth && !tooth.HasValue)
            {
                Dialogs.Error(this, string.Format("„{0}” se face pe un dinte anume. Alegeți dintele.", procedure.Name));
                cboPlanTooth.Focus();
                return;
            }

            var doctor = Combo.Selected<Doctor>(cboPlanDoctor);
            _store.PlanProcedure(_patient.Id, procedure, tooth, doctor == null ? (int?)null : doctor.Id, numDiscount.Value);
        }

        private void btnApplyStatus_Click(object sender, EventArgs e)
        {
            List<TreatmentItem> items = Grid.SelectedItems<TreatmentItem>(gridPlan);
            if (items.Count > 0)
                _store.SetTreatmentStatus(items, Combo.Value<TreatmentStatus>(cboPlanStatus));
        }

        private void btnDeleteTreatment_Click(object sender, EventArgs e)
        {
            var item = Grid.Selected<TreatmentItem>(gridPlan);
            if (item == null)
                return;
            if (!Dialogs.Confirm(this, "Ștergeți „" + item.ProcedureName + "” din plan?"))
                return;

            string reason;
            if (!_store.TryDeleteTreatment(item, out reason))
                Dialogs.Error(this, reason);
        }

        /// <summary>The estimate lists what is still to be done: proposed, accepted and in progress.</summary>
        private Estimate BuildEstimate()
        {
            List<TreatmentItem> open = _store.TreatmentsFor(_patient.Id).Where(t => t.Status != TreatmentStatus.Done).ToList();
            if (open.Count == 0)
            {
                Dialogs.Info(this, "Planul nu mai are proceduri de efectuat, deci devizul ar fi gol.");
                return null;
            }
            return Estimate.For(_store, _patient, open);
        }

        private void btnPrintEstimate_Click(object sender, EventArgs e)
        {
            Estimate estimate = BuildEstimate();
            if (estimate == null)
                return;

            try
            {
                using (var document = new EstimateDocument(estimate))
                using (var preview = new PrintPreviewDialog())
                {
                    preview.Document = document;
                    preview.Width = 900;
                    preview.Height = 1000;
                    preview.ShowDialog(this);
                }
            }
            catch (InvalidPrinterException)
            {
                Dialogs.Error(this, "Nu este instalată nicio imprimantă. Folosiți „Deviz ca imagine”.");
            }
        }

        private void btnSaveEstimate_Click(object sender, EventArgs e)
        {
            Estimate estimate = BuildEstimate();
            if (estimate == null)
                return;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Imagine PNG (*.png)|*.png";
                dialog.FileName = "Deviz " + Search.StripDiacritics(_patient.FullName) + ".png";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                EstimateRenderer.SavePng(estimate, dialog.FileName);
                Dialogs.Info(this, "Devizul a fost salvat în:\n" + dialog.FileName);
            }
        }

        // ---------------------------------------------------------------- appointments

        private void btnNewAppointment_Click(object sender, EventArgs e)
        {
            IList<Doctor> doctors = _store.ActiveDoctors;
            var appointment = new Appointment
            {
                PatientId = _patient.Id,
                DoctorId = doctors.Count > 0 ? doctors[0].Id : 0,
                Start = DateTime.Today.AddDays(1).AddHours(10)
            };
            AppointmentForm.Edit(this, _store, appointment, true);
        }

        private void btnEditAppointment_Click(object sender, EventArgs e)
        {
            var appointment = Grid.Selected<Appointment>(gridAppointments);
            if (appointment != null)
                AppointmentForm.Edit(this, _store, appointment, true);
        }

        private void gridAppointments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                btnEditAppointment_Click(sender, e);
        }

        // ---------------------------------------------------------------- payments

        private void btnAddPayment_Click(object sender, EventArgs e)
        {
            if (numAmount.Value <= 0m)
            {
                Dialogs.Error(this, "Introduceți suma încasată.");
                numAmount.Focus();
                return;
            }

            var payment = new Payment
            {
                PatientId = _patient.Id,
                Amount = numAmount.Value,
                Method = Combo.Value<PaymentMethod>(cboMethod),
                Note = txtPaymentNote.Text.Trim()
            };
            _store.AddPayment(payment);
            txtPaymentNote.Clear();
            Dialogs.Info(this, string.Format("Încasare înregistrată: chitanța {0}, {1}.", payment.ReceiptNo, Fmt.Money(payment.Amount)));
        }
    }
}
