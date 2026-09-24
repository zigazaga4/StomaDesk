using System;
using System.Collections.Generic;
using System.Linq;
using StomaDesk.Models;
using StomaDesk.Services;

namespace StomaDesk.Data
{
    /// <summary>What a patient owes: finished work minus what was paid.</summary>
    public class Balance
    {
        private readonly decimal _billed;
        private readonly decimal _paid;
        private readonly decimal _planned;

        public Balance(decimal billed, decimal paid, decimal planned)
        {
            _billed = billed;
            _paid = paid;
            _planned = planned;
        }

        /// <summary>Total of finished procedures.</summary>
        public decimal Billed { get { return _billed; } }

        public decimal Paid { get { return _paid; } }

        /// <summary>Total of procedures still in the plan (not finished yet).</summary>
        public decimal Planned { get { return _planned; } }

        /// <summary>Positive: the patient owes money. Negative: the patient paid in advance.</summary>
        public decimal Due { get { return _billed - _paid; } }
    }

    /// <summary>
    /// The clinic's data in memory plus the rules that keep it consistent.
    /// Every change is written to PostgreSQL first, in one transaction, then applied in memory and announced
    /// through <see cref="Changed"/>; if the write fails, memory stays as it was.
    /// Use it from the UI thread only; background work receives plain copies (see Reminders).
    /// </summary>
    public class ClinicStore
    {
        private readonly ClinicDatabase _database;
        private readonly ClinicData _data;

        private ClinicStore(ClinicDatabase database, ClinicData data)
        {
            _database = database;
            _data = data;
        }

        public event EventHandler Changed;

        /// <summary>Opens the clinic database; the first time it creates the tables and fills them with demo data.</summary>
        public static ClinicStore Open(ClinicDatabase database)
        {
            DateTime today = DateTime.Today;
            database.EnsureCreated(() => SampleData.Create(today));
            return new ClinicStore(database, database.Load());
        }

        /// <summary>
        /// Loads a StomaDesk XML file (a backup from the File menu, or the data file of the XML-only version)
        /// into an empty database, keeping its ids. Returns false, writing nothing, when the database already has a clinic.
        /// </summary>
        public static bool Import(ClinicDatabase database, string xmlPath)
        {
            ClinicData data = XmlClinicFile.Load(xmlPath);
            return database.EnsureCreated(() => data);
        }

        /// <summary>Where the data lives, for the status bar and the About box.</summary>
        public string Location
        {
            get { return _database.Description; }
        }

        public ClinicInfo Info
        {
            get { return _data.Info; }
        }

        /// <summary>Writes the whole clinic to one XML file, which <see cref="Import"/> can load back.</summary>
        public void ExportXml(string path)
        {
            XmlClinicFile.Save(path, _data);
        }

        // ---------------------------------------------------------------- doctors and price list

        public IList<Doctor> Doctors
        {
            get { return _data.Doctors.AsReadOnly(); }
        }

        public IList<Doctor> ActiveDoctors
        {
            get { return _data.Doctors.Where(d => d.Active).ToList(); }
        }

        public Doctor GetDoctor(int? id)
        {
            if (!id.HasValue)
                return null;
            return _data.Doctors.FirstOrDefault(d => d.Id == id.Value);
        }

        public string DoctorName(int? id)
        {
            Doctor doctor = GetDoctor(id);
            return doctor == null ? "" : doctor.Name;
        }

        public IList<Procedure> Procedures
        {
            get { return _data.Procedures.OrderBy(p => p.Code).ToList(); }
        }

        public bool IsDoctorUsed(int doctorId)
        {
            return _data.Appointments.Any(a => a.DoctorId == doctorId)
                || _data.Treatments.Any(t => t.DoctorId == doctorId);
        }

        /// <summary>Saves the Settings tab in one write: clinic details, doctors and price list.</summary>
        public void SaveSettings(ClinicInfo info, IList<Doctor> doctors, IList<Procedure> procedures)
        {
            var removedDoctors = new List<int>();
            foreach (Doctor old in _data.Doctors)
            {
                if (doctors.Any(d => d.Id == old.Id))
                    continue;
                if (IsDoctorUsed(old.Id))
                    throw new InvalidOperationException(string.Format(
                        "{0} are programări sau tratamente în istoric și nu poate fi șters. Debifați „Activ”.", old.Name));
                removedDoctors.Add(old.Id);
            }
            List<int> removedProcedures = _data.Procedures
                .Where(old => !procedures.Any(p => p.Id == old.Id))
                .Select(p => p.Id)
                .ToList();

            // New doctors and procedures (Id 0) get their ids from the database here.
            _database.SaveSettings(info, doctors, removedDoctors, procedures, removedProcedures);

            _data.Info = info;
            _data.Doctors = new List<Doctor>(doctors);
            _data.Procedures = new List<Procedure>(procedures);
            OnChanged();
        }

        // ---------------------------------------------------------------- patients

        public Patient GetPatient(int id)
        {
            return _data.Patients.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Finds patients by name (with or without diacritics, either order), or by CNP / phone digits.
        /// An empty query returns everyone, sorted by name.
        /// </summary>
        public List<Patient> FindPatients(string query)
        {
            string text = Search.Fold(query);
            string digits = PhoneNumber.Digits(query);
            bool numeric = digits.Length > 0 && text.All(c => char.IsDigit(c) || c == ' ' || c == '+');
            string phoneDigits = digits.Length >= 10 ? PhoneNumber.National(query) : digits;

            return _data.Patients
                .Where(p => text.Length == 0
                    || (numeric
                        ? (p.Cnp ?? "").Contains(digits) || PhoneNumber.Digits(p.Phone).Contains(phoneDigits)
                        : Search.Fold(p.LastName + " " + p.FirstName).Contains(text)
                          || Search.Fold(p.FirstName + " " + p.LastName).Contains(text)))
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList();
        }

        public bool IsCnpTaken(string cnp, int exceptPatientId)
        {
            return _data.Patients.Any(p => p.Id != exceptPatientId && p.Cnp == cnp);
        }

        /// <summary>Adds a new patient (Id 0) or saves the edits made to an existing one.</summary>
        public void SavePatient(Patient patient)
        {
            bool isNew = patient.Id == 0;
            _database.SavePatient(patient);   // a new patient gets its id here
            if (isNew)
                _data.Patients.Add(patient);
            OnChanged();
        }

        public void SetTooth(Patient patient, int tooth, ToothState state, string note)
        {
            if (!Dentition.IsValid(tooth))
                throw new ArgumentOutOfRangeException("tooth", tooth, "Număr de dinte inexistent în sistemul FDI.");

            // Work out the stored record on a scratch patient first: the rule stays in Patient.SetTooth,
            // and a failed write leaves the chart as it was.
            var probe = new Patient();
            probe.SetTooth(tooth, state, note);
            _database.SaveTooth(patient.Id, tooth, probe.FindTooth(tooth));

            patient.SetTooth(tooth, state, note);
            OnChanged();
        }

        public bool TryDeletePatient(Patient patient, out string reason)
        {
            if (_data.Treatments.Any(t => t.PatientId == patient.Id) || _data.Payments.Any(p => p.PatientId == patient.Id))
            {
                reason = "Pacientul are tratamente sau încasări înregistrate, iar fișa medicală se păstrează.";
                return false;
            }

            _database.DeletePatient(patient.Id);
            _data.Appointments.RemoveAll(a => a.PatientId == patient.Id);
            _data.Patients.Remove(patient);
            reason = null;
            OnChanged();
            return true;
        }

        public Balance BalanceFor(int patientId)
        {
            decimal billed = 0m, planned = 0m, paid = 0m;
            foreach (TreatmentItem item in _data.Treatments.Where(t => t.PatientId == patientId))
            {
                if (item.Status == TreatmentStatus.Done)
                    billed += item.Total;
                else
                    planned += item.Total;
            }
            foreach (Payment payment in _data.Payments.Where(p => p.PatientId == patientId))
                paid += payment.Amount;
            return new Balance(billed, paid, planned);
        }

        /// <summary>Amount due for every patient in one pass (for lists, where a query per row would be slow).</summary>
        public Dictionary<int, decimal> DueByPatient()
        {
            var due = new Dictionary<int, decimal>();
            foreach (TreatmentItem item in _data.Treatments)
            {
                if (item.Status == TreatmentStatus.Done)
                    AddTo(due, item.PatientId, item.Total);
            }
            foreach (Payment payment in _data.Payments)
                AddTo(due, payment.PatientId, -payment.Amount);
            return due;
        }

        public Dictionary<int, DateTime> LastVisitByPatient()
        {
            var last = new Dictionary<int, DateTime>();
            foreach (Appointment a in _data.Appointments)
            {
                if (a.Status != AppointmentStatus.Done && a.Status != AppointmentStatus.Arrived)
                    continue;
                DateTime current;
                if (!last.TryGetValue(a.PatientId, out current) || a.Start > current)
                    last[a.PatientId] = a.Start;
            }
            return last;
        }

        // ---------------------------------------------------------------- appointments

        public List<Appointment> AppointmentsOn(DateTime day)
        {
            DateTime date = day.Date;
            return _data.Appointments.Where(a => a.Start.Date == date).OrderBy(a => a.Start).ToList();
        }

        public List<Appointment> AppointmentsBetween(DateTime from, DateTime to)
        {
            return _data.Appointments
                .Where(a => a.Start.Date >= from.Date && a.Start.Date <= to.Date)
                .OrderBy(a => a.Start)
                .ToList();
        }

        public List<Appointment> AppointmentsFor(int patientId)
        {
            return _data.Appointments.Where(a => a.PatientId == patientId).OrderByDescending(a => a.Start).ToList();
        }

        /// <summary>Returns an active appointment of the same doctor that overlaps the candidate, or null.</summary>
        public Appointment FindConflict(Appointment candidate)
        {
            if (!candidate.IsActive)
                return null;

            return _data.Appointments.FirstOrDefault(other =>
                other.Id != candidate.Id
                && other.IsActive
                && other.DoctorId == candidate.DoctorId
                && other.Overlaps(candidate));
        }

        public void SaveAppointment(Appointment appointment)
        {
            if (FindConflict(appointment) != null)
                throw new InvalidOperationException("Intervalul se suprapune cu altă programare a medicului.");

            // The database checks the overlap again, against what other workstations saved meanwhile.
            bool isNew = appointment.Id == 0;
            _database.SaveAppointment(appointment);
            if (isNew)
                _data.Appointments.Add(appointment);
            OnChanged();
        }

        public void SetAppointmentStatus(Appointment appointment, AppointmentStatus status)
        {
            // Check on a copy first: bringing back a cancelled appointment may collide with one made since.
            Appointment probe = appointment.Clone();
            probe.Status = status;
            if (FindConflict(probe) != null)
                throw new InvalidOperationException("Intervalul a fost ocupat între timp de altă programare a medicului.");

            _database.SetAppointmentStatus(appointment.Id, status);
            appointment.Status = status;
            OnChanged();
        }

        public void DeleteAppointment(Appointment appointment)
        {
            _database.DeleteAppointment(appointment.Id);
            _data.Appointments.Remove(appointment);
            OnChanged();
        }

        public void MarkRemindersSent(IEnumerable<int> appointmentIds)
        {
            var ids = new HashSet<int>(appointmentIds);
            if (ids.Count == 0)
                return;

            _database.MarkRemindersSent(ids);
            foreach (Appointment a in _data.Appointments)
            {
                if (ids.Contains(a.Id))
                    a.ReminderSent = true;
            }
            OnChanged();
        }

        // ---------------------------------------------------------------- treatment plan

        public List<TreatmentItem> TreatmentsFor(int patientId)
        {
            return _data.Treatments.Where(t => t.PatientId == patientId).OrderBy(t => t.CreatedAt).ToList();
        }

        public List<TreatmentItem> TreatmentsCompletedBetween(DateTime from, DateTime to)
        {
            return _data.Treatments
                .Where(t => t.Status == TreatmentStatus.Done && t.CompletedAt.HasValue
                    && t.CompletedAt.Value.Date >= from.Date && t.CompletedAt.Value.Date <= to.Date)
                .ToList();
        }

        public TreatmentItem PlanProcedure(int patientId, Procedure procedure, int? tooth, int? doctorId, decimal discountPercent)
        {
            if (procedure == null)
                throw new ArgumentNullException("procedure");
            if (procedure.PerTooth && !tooth.HasValue)
                throw new ArgumentException(string.Format("Procedura „{0}” se face pe dinte: alegeți dintele.", procedure.Name));
            if (discountPercent < 0m || discountPercent > 100m)
                throw new ArgumentOutOfRangeException("discountPercent");

            var item = new TreatmentItem
            {
                PatientId = patientId,
                ProcedureId = procedure.Id,
                ProcedureName = procedure.Name,
                Price = procedure.Price,
                Tooth = procedure.PerTooth ? tooth : null,
                DoctorId = doctorId,
                DiscountPercent = discountPercent,
                Status = TreatmentStatus.Proposed
            };
            _database.AddTreatment(item);   // sets item.Id
            _data.Treatments.Add(item);
            OnChanged();
            return item;
        }

        public void SetTreatmentStatus(IEnumerable<TreatmentItem> items, TreatmentStatus status)
        {
            List<TreatmentItem> list = items.ToList();
            List<DateTime?> completedAt = list
                .Select(item => status == TreatmentStatus.Done ? (DateTime?)(item.CompletedAt ?? DateTime.Now) : null)
                .ToList();

            _database.SetTreatmentStatus(list.Select(item => item.Id).ToList(), status, completedAt);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Status = status;
                list[i].CompletedAt = completedAt[i];
            }
            OnChanged();
        }

        public bool TryDeleteTreatment(TreatmentItem item, out string reason)
        {
            if (item.Status == TreatmentStatus.Done)
            {
                reason = "Procedurile finalizate intră în sold și nu se șterg. Schimbați întâi statusul.";
                return false;
            }

            _database.DeleteTreatment(item.Id);
            _data.Treatments.Remove(item);
            reason = null;
            OnChanged();
            return true;
        }

        // ---------------------------------------------------------------- payments

        public List<Payment> PaymentsFor(int patientId)
        {
            return _data.Payments.Where(p => p.PatientId == patientId).OrderByDescending(p => p.Date).ToList();
        }

        public List<Payment> PaymentsBetween(DateTime from, DateTime to)
        {
            return _data.Payments.Where(p => p.Date.Date >= from.Date && p.Date.Date <= to.Date).ToList();
        }

        public void AddPayment(Payment payment)
        {
            if (payment.Amount <= 0m)
                throw new ArgumentException("Suma încasată trebuie să fie mai mare decât zero.");

            _database.AddPayment(payment);   // sets Id and the next receipt number
            _data.Payments.Add(payment);
            OnChanged();
        }

        // ---------------------------------------------------------------- internals

        private void OnChanged()
        {
            EventHandler handler = Changed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private static void AddTo(Dictionary<int, decimal> totals, int key, decimal amount)
        {
            decimal current;
            totals.TryGetValue(key, out current);
            totals[key] = current + amount;
        }
    }
}
