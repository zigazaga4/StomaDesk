using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using Npgsql;
using StomaDesk.Models;

namespace StomaDesk.Data
{
    /// <summary>
    /// The PostgreSQL side of <see cref="ClinicStore"/>: creates the tables (Data/Schema.sql), reads the whole clinic
    /// in one round trip and writes each change as it happens, one transaction per change.
    /// Methods that add rows put the new id on the object only after the commit, so a failed save never leaves
    /// an object holding an id that does not exist. A rule in the schema broken because another workstation changed
    /// the data meanwhile comes back as an <see cref="InvalidOperationException"/> with a message for the user,
    /// the same kind of exception the store throws for its own checks.
    /// </summary>
    public sealed class ClinicDatabase
    {
        /// <summary>Name of the connection string in StomaDesk.exe.config (App.config in the project).</summary>
        public const string ConnectionName = "StomaDesk";

        private const string ScratchPrefix = "stomadesk_test_";

        /// <summary>Any fixed number, the same on every workstation: two of them starting together set up the database once.</summary>
        private const long SetupLock = 2012;

        private const string ChangedElsewhere =
            "Datele au fost schimbate între timp de pe alt calculator, iar modificarea nu se mai poate salva. " +
            "Redeschideți aplicația ca să vedeți datele actuale.";

        /// <summary>Schema constraints whose meaning the user already knows from the store's own checks.</summary>
        private static readonly Dictionary<string, string> ConstraintMessages = new Dictionary<string, string>
        {
            { "appointments_no_overlap", "Intervalul se suprapune cu altă programare a medicului." },
            { "patients_cnp_key", "Există deja un pacient cu acest CNP." }
        };

        /// <summary>Tables whose id comes from an identity column.</summary>
        private static readonly string[] NumberedTables = { "doctors", "procedures", "patients", "appointments", "treatments", "payments" };

        private readonly string _connectionString;

        public ClinicDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Lipsește șirul de conexiune la PostgreSQL.", "connectionString");
            _connectionString = connectionString;
        }

        /// <summary>The connection string named "StomaDesk" in StomaDesk.exe.config.</summary>
        public static ClinicDatabase FromConfig()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionName];
            if (settings == null)
                throw new ConfigurationErrorsException(string.Format(
                    "În StomaDesk.exe.config lipsește conexiunea „{0}” din <connectionStrings>.", ConnectionName));
            return new ClinicDatabase(settings.ConnectionString);
        }

        /// <summary>Where the data lives, for the status bar and the About box. Never includes the password.</summary>
        public string Description
        {
            get
            {
                var builder = new NpgsqlConnectionStringBuilder(_connectionString);
                string text = string.Format("PostgreSQL {0}@{1}:{2}/{3}", builder.Username, builder.Host, builder.Port, builder.Database);
                return string.IsNullOrEmpty(builder.SearchPath) ? text : text + ", schema " + builder.SearchPath;
            }
        }

        // ---------------------------------------------------------------- setup

        /// <summary>
        /// Creates whatever tables are missing and, when the database holds no clinic yet, fills it with
        /// <paramref name="initialData"/> (the demo clinic, or an imported XML file), keeping its ids.
        /// Returns true when it filled the database, false when a clinic was already there (nothing is written then).
        /// </summary>
        public bool EnsureCreated(Func<ClinicData> initialData)
        {
            return Run((connection, transaction) =>
            {
                Execute(connection, transaction, "SELECT pg_advisory_xact_lock(" + SetupLock + ")");
                Execute(connection, transaction, SchemaScript());
                if ((bool)Scalar(connection, transaction, "SELECT EXISTS (SELECT 1 FROM clinic_info)"))
                    return false;

                InsertAll(connection, transaction, initialData());
                return true;
            });
        }

        /// <summary>
        /// The same server and database, in a new empty schema of its own. The self-test works there, so it never
        /// sees real patients; remove it afterwards with <see cref="DropScratchSchema"/>.
        /// </summary>
        public ClinicDatabase CreateScratchSchema()
        {
            string schema = ScratchPrefix + Guid.NewGuid().ToString("N");
            Run((connection, transaction) => Execute(connection, transaction, "CREATE SCHEMA " + schema));
            var builder = new NpgsqlConnectionStringBuilder(_connectionString) { SearchPath = schema };
            return new ClinicDatabase(builder.ConnectionString);
        }

        public void DropScratchSchema()
        {
            string schema = new NpgsqlConnectionStringBuilder(_connectionString).SearchPath;
            if (schema == null || !schema.StartsWith(ScratchPrefix, StringComparison.Ordinal))
                throw new InvalidOperationException("Se pot șterge doar schemele create de CreateScratchSchema.");
            Run((connection, transaction) => Execute(connection, transaction, "DROP SCHEMA " + schema + " CASCADE"));
        }

        // ---------------------------------------------------------------- reading

        /// <summary>The whole clinic, read in one round trip from one snapshot, so a change saved meanwhile on
        /// another workstation is either all there or not there at all.</summary>
        public ClinicData Load()
        {
            const string sql =
                "SELECT * FROM clinic_info;" +
                "SELECT * FROM doctors ORDER BY id;" +
                "SELECT * FROM procedures ORDER BY id;" +
                "SELECT * FROM patients ORDER BY id;" +
                "SELECT * FROM patient_teeth ORDER BY patient_id, tooth;" +
                "SELECT * FROM appointments ORDER BY id;" +
                "SELECT * FROM treatments ORDER BY id;" +
                "SELECT * FROM payments ORDER BY id;";

            return Run((connection, transaction) =>
            {
                var data = new ClinicData();
                var teeth = new List<KeyValuePair<int, ToothRecord>>();
                using (var command = new NpgsqlCommand(sql, connection, transaction))
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        data.Info = ReadInfo(reader);
                    ReadAll(reader, data.Doctors, ReadDoctor);
                    ReadAll(reader, data.Procedures, ReadProcedure);
                    ReadAll(reader, data.Patients, ReadPatient);
                    ReadAll(reader, teeth, ReadTooth);
                    ReadAll(reader, data.Appointments, ReadAppointment);
                    ReadAll(reader, data.Treatments, ReadTreatment);
                    ReadAll(reader, data.Payments, ReadPayment);
                }

                Dictionary<int, Patient> patients = data.Patients.ToDictionary(p => p.Id);
                foreach (KeyValuePair<int, ToothRecord> tooth in teeth)
                    patients[tooth.Key].Teeth.Add(tooth.Value);
                return data;
            }, IsolationLevel.RepeatableRead);
        }

        // ---------------------------------------------------------------- writing, one method per change in ClinicStore

        /// <summary>Clinic details, doctors and price list in one transaction, as the Settings tab saves them.</summary>
        public void SaveSettings(ClinicInfo info, IList<Doctor> doctors, IEnumerable<int> removedDoctorIds,
            IList<Procedure> procedures, IEnumerable<int> removedProcedureIds)
        {
            int[] doctorIds = null;
            int[] procedureIds = null;
            Run((connection, transaction) =>
            {
                SaveInfo(connection, transaction, info);
                Execute(connection, transaction, "DELETE FROM doctors WHERE id = ANY(@ids)", new Values { { "ids", removedDoctorIds.ToArray() } });
                Execute(connection, transaction, "DELETE FROM procedures WHERE id = ANY(@ids)", new Values { { "ids", removedProcedureIds.ToArray() } });
                doctorIds = doctors.Select(d => Save(connection, transaction, "doctors", d.Id, DoctorValues(d))).ToArray();
                procedureIds = procedures.Select(p => Save(connection, transaction, "procedures", p.Id, ProcedureValues(p))).ToArray();
            });

            for (int i = 0; i < doctors.Count; i++)
                doctors[i].Id = doctorIds[i];
            for (int i = 0; i < procedures.Count; i++)
                procedures[i].Id = procedureIds[i];
        }

        /// <summary>Adds a new patient (Id 0) together with any teeth already marked, or updates an existing one's details.</summary>
        public void SavePatient(Patient patient)
        {
            patient.Id = Run((connection, transaction) => patient.Id == 0
                ? InsertPatient(connection, transaction, patient)
                : Save(connection, transaction, "patients", patient.Id, PatientValues(patient)));
        }

        /// <summary>Appointments and teeth go with the patient; the schema refuses if treatments or payments exist.</summary>
        public void DeletePatient(int patientId)
        {
            Delete("patients", patientId);
        }

        /// <summary>Stores one tooth of the odontogram; a null record means healthy, so the row is removed.</summary>
        public void SaveTooth(int patientId, int tooth, ToothRecord record)
        {
            Run((connection, transaction) => WriteTooth(connection, transaction, patientId, tooth, record));
        }

        public void SaveAppointment(Appointment appointment)
        {
            appointment.Id = Run((connection, transaction) =>
                Save(connection, transaction, "appointments", appointment.Id, AppointmentValues(appointment)));
        }

        public void SetAppointmentStatus(int appointmentId, AppointmentStatus status)
        {
            Run((connection, transaction) =>
                Update(connection, transaction, "appointments", appointmentId, new Values { { "status", status.ToString() } }));
        }

        public void DeleteAppointment(int appointmentId)
        {
            Delete("appointments", appointmentId);
        }

        public void MarkRemindersSent(IEnumerable<int> appointmentIds)
        {
            Run((connection, transaction) => Execute(connection, transaction,
                "UPDATE appointments SET reminder_sent = true WHERE id = ANY(@ids)", new Values { { "ids", appointmentIds.ToArray() } }));
        }

        public void AddTreatment(TreatmentItem item)
        {
            item.Id = Run((connection, transaction) => Insert(connection, transaction, "treatments", 0, TreatmentValues(item)));
        }

        /// <summary>New status for several plan items in one transaction; <paramref name="completedAt"/>[i] belongs to <paramref name="ids"/>[i].</summary>
        public void SetTreatmentStatus(IList<int> ids, TreatmentStatus status, IList<DateTime?> completedAt)
        {
            Run((connection, transaction) =>
            {
                for (int i = 0; i < ids.Count; i++)
                    Update(connection, transaction, "treatments", ids[i],
                        new Values { { "status", status.ToString() }, { "completed_at", completedAt[i] } });
            });
        }

        public void DeleteTreatment(int itemId)
        {
            Delete("treatments", itemId);
        }

        /// <summary>Saves the payment with the next receipt number from the database's sequence, then sets Id and ReceiptNo.</summary>
        public void AddPayment(Payment payment)
        {
            string receiptNo = null;
            int id = Run((connection, transaction) =>
            {
                receiptNo = ClinicData.FormatReceiptNo((long)Scalar(connection, transaction, "SELECT nextval('receipt_numbers')"));
                return Insert(connection, transaction, "payments", 0, PaymentValues(payment, receiptNo));
            });
            payment.Id = id;
            payment.ReceiptNo = receiptNo;
        }

        // ---------------------------------------------------------------- rows <-> objects

        private static Values DoctorValues(Doctor d)
        {
            return new Values { { "name", d.Name }, { "specialty", d.Specialty }, { "active", d.Active }, { "color_argb", d.ColorArgb } };
        }

        private static Values ProcedureValues(Procedure p)
        {
            return new Values
            {
                { "code", p.Code }, { "name", p.Name }, { "price", p.Price },
                { "duration_minutes", p.DurationMinutes }, { "per_tooth", p.PerTooth }
            };
        }

        private static Values PatientValues(Patient p)
        {
            return new Values
            {
                { "last_name", p.LastName }, { "first_name", p.FirstName }, { "cnp", p.Cnp }, { "birth_date", p.BirthDate },
                { "phone", p.Phone }, { "email", p.Email }, { "address", p.Address }, { "allergies", p.Allergies },
                { "notes", p.Notes }, { "created_at", p.CreatedAt }
            };
        }

        private static Values AppointmentValues(Appointment a)
        {
            return new Values
            {
                { "patient_id", a.PatientId }, { "doctor_id", a.DoctorId }, { "start_at", a.Start },
                { "duration_minutes", a.DurationMinutes }, { "reason", a.Reason }, { "status", a.Status.ToString() },
                { "reminder_sent", a.ReminderSent }
            };
        }

        private static Values TreatmentValues(TreatmentItem t)
        {
            return new Values
            {
                { "patient_id", t.PatientId }, { "procedure_id", t.ProcedureId }, { "procedure_name", t.ProcedureName },
                { "tooth", t.Tooth }, { "price", t.Price }, { "discount_percent", t.DiscountPercent },
                { "status", t.Status.ToString() }, { "doctor_id", t.DoctorId }, { "created_at", t.CreatedAt },
                { "completed_at", t.CompletedAt }
            };
        }

        private static Values PaymentValues(Payment p, string receiptNo)
        {
            return new Values
            {
                { "patient_id", p.PatientId }, { "paid_at", p.Date }, { "amount", p.Amount },
                { "method", p.Method.ToString() }, { "note", p.Note }, { "receipt_no", receiptNo }
            };
        }

        private static ClinicInfo ReadInfo(NpgsqlDataReader r)
        {
            return new ClinicInfo
            {
                Name = Field<string>(r, "name"),
                Address = Field<string>(r, "address"),
                Phone = Field<string>(r, "phone"),
                FiscalCode = Field<string>(r, "fiscal_code")
            };
        }

        private static Doctor ReadDoctor(NpgsqlDataReader r)
        {
            return new Doctor
            {
                Id = Field<int>(r, "id"),
                Name = Field<string>(r, "name"),
                Specialty = Field<string>(r, "specialty"),
                Active = Field<bool>(r, "active"),
                ColorArgb = Field<int>(r, "color_argb")
            };
        }

        private static Procedure ReadProcedure(NpgsqlDataReader r)
        {
            return new Procedure
            {
                Id = Field<int>(r, "id"),
                Code = Field<string>(r, "code"),
                Name = Field<string>(r, "name"),
                Price = Field<decimal>(r, "price"),
                DurationMinutes = Field<int>(r, "duration_minutes"),
                PerTooth = Field<bool>(r, "per_tooth")
            };
        }

        private static Patient ReadPatient(NpgsqlDataReader r)
        {
            return new Patient
            {
                Id = Field<int>(r, "id"),
                LastName = Field<string>(r, "last_name"),
                FirstName = Field<string>(r, "first_name"),
                Cnp = Field<string>(r, "cnp"),
                BirthDate = Field<DateTime?>(r, "birth_date"),
                Phone = Field<string>(r, "phone"),
                Email = Field<string>(r, "email"),
                Address = Field<string>(r, "address"),
                Allergies = Field<string>(r, "allergies"),
                Notes = Field<string>(r, "notes"),
                CreatedAt = Field<DateTime>(r, "created_at")
            };
        }

        /// <summary>A tooth row, with the id of the patient it belongs to.</summary>
        private static KeyValuePair<int, ToothRecord> ReadTooth(NpgsqlDataReader r)
        {
            var record = new ToothRecord
            {
                Tooth = Field<int>(r, "tooth"),
                State = EnumField<ToothState>(r, "state"),
                Note = Field<string>(r, "note")
            };
            return new KeyValuePair<int, ToothRecord>(Field<int>(r, "patient_id"), record);
        }

        private static Appointment ReadAppointment(NpgsqlDataReader r)
        {
            return new Appointment
            {
                Id = Field<int>(r, "id"),
                PatientId = Field<int>(r, "patient_id"),
                DoctorId = Field<int>(r, "doctor_id"),
                Start = Field<DateTime>(r, "start_at"),
                DurationMinutes = Field<int>(r, "duration_minutes"),
                Reason = Field<string>(r, "reason"),
                Status = EnumField<AppointmentStatus>(r, "status"),
                ReminderSent = Field<bool>(r, "reminder_sent")
            };
        }

        private static TreatmentItem ReadTreatment(NpgsqlDataReader r)
        {
            return new TreatmentItem
            {
                Id = Field<int>(r, "id"),
                PatientId = Field<int>(r, "patient_id"),
                ProcedureId = Field<int>(r, "procedure_id"),
                ProcedureName = Field<string>(r, "procedure_name"),
                Tooth = Field<int?>(r, "tooth"),
                Price = Field<decimal>(r, "price"),
                DiscountPercent = Field<decimal>(r, "discount_percent"),
                Status = EnumField<TreatmentStatus>(r, "status"),
                DoctorId = Field<int?>(r, "doctor_id"),
                CreatedAt = Field<DateTime>(r, "created_at"),
                CompletedAt = Field<DateTime?>(r, "completed_at")
            };
        }

        private static Payment ReadPayment(NpgsqlDataReader r)
        {
            return new Payment
            {
                Id = Field<int>(r, "id"),
                PatientId = Field<int>(r, "patient_id"),
                Date = Field<DateTime>(r, "paid_at"),
                Amount = Field<decimal>(r, "amount"),
                Method = EnumField<PaymentMethod>(r, "method"),
                Note = Field<string>(r, "note"),
                ReceiptNo = Field<string>(r, "receipt_no")
            };
        }

        /// <summary>Moves to the next result set of a multi-statement command and reads all its rows.</summary>
        private static void ReadAll<T>(NpgsqlDataReader reader, List<T> into, Func<NpgsqlDataReader, T> read)
        {
            reader.NextResult();
            while (reader.Read())
                into.Add(read(reader));
        }

        /// <summary>A column value, with SQL NULL as the type's default (null for strings and nullable types).</summary>
        private static T Field<T>(NpgsqlDataReader reader, string column)
        {
            object value = reader[column];
            return value is DBNull ? default(T) : (T)value;
        }

        private static TEnum EnumField<TEnum>(NpgsqlDataReader reader, string column) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), (string)reader[column]);
        }

        // ---------------------------------------------------------------- statements

        /// <summary>Everything in <paramref name="data"/>, ids included, then every counter moved past the highest id.</summary>
        private static void InsertAll(NpgsqlConnection connection, NpgsqlTransaction transaction, ClinicData data)
        {
            SaveInfo(connection, transaction, data.Info);
            foreach (Doctor doctor in data.Doctors)
                Insert(connection, transaction, "doctors", doctor.Id, DoctorValues(doctor));
            foreach (Procedure procedure in data.Procedures)
                Insert(connection, transaction, "procedures", procedure.Id, ProcedureValues(procedure));
            foreach (Patient patient in data.Patients)
                InsertPatient(connection, transaction, patient);
            foreach (Appointment appointment in data.Appointments)
                Insert(connection, transaction, "appointments", appointment.Id, AppointmentValues(appointment));
            foreach (TreatmentItem item in data.Treatments)
                Insert(connection, transaction, "treatments", item.Id, TreatmentValues(item));
            foreach (Payment payment in data.Payments)
                Insert(connection, transaction, "payments", payment.Id, PaymentValues(payment, payment.ReceiptNo));

            foreach (string table in NumberedTables)
                Execute(connection, transaction, string.Format(
                    "SELECT setval(pg_get_serial_sequence('{0}', 'id'), COALESCE(MAX(id), 0) + 1, false) FROM {0}", table));
            Execute(connection, transaction,
                "SELECT setval('receipt_numbers', COALESCE(MAX(substring(receipt_no FROM '[0-9]+$')::bigint), 0) + 1, false) FROM payments");
        }

        private static int InsertPatient(NpgsqlConnection connection, NpgsqlTransaction transaction, Patient patient)
        {
            int id = Insert(connection, transaction, "patients", patient.Id, PatientValues(patient));
            foreach (ToothRecord record in patient.Teeth)
                WriteTooth(connection, transaction, id, record.Tooth, record);
            return id;
        }

        private static void SaveInfo(NpgsqlConnection connection, NpgsqlTransaction transaction, ClinicInfo info)
        {
            Upsert(connection, transaction, "clinic_info", new[] { "id" }, new Values
            {
                { "id", 1 }, { "name", info.Name }, { "address", info.Address }, { "phone", info.Phone }, { "fiscal_code", info.FiscalCode }
            });
        }

        private static void WriteTooth(NpgsqlConnection connection, NpgsqlTransaction transaction, int patientId, int tooth, ToothRecord record)
        {
            if (record == null)
                Execute(connection, transaction, "DELETE FROM patient_teeth WHERE patient_id = @patient_id AND tooth = @tooth",
                    new Values { { "patient_id", patientId }, { "tooth", tooth } });
            else
                Upsert(connection, transaction, "patient_teeth", new[] { "patient_id", "tooth" }, new Values
                {
                    { "patient_id", patientId }, { "tooth", tooth }, { "state", record.State.ToString() }, { "note", record.Note }
                });
        }

        /// <summary>
        /// Adds one row and returns its id. Id 0 lets the database number the row; any other id is kept
        /// (demo data and imports, whose rows already point at each other).
        /// </summary>
        private static int Insert(NpgsqlConnection connection, NpgsqlTransaction transaction, string table, int id, Values values)
        {
            if (id != 0)
                values.Add("id", id);
            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2}) RETURNING id",
                table, string.Join(", ", values.Columns), values.Placeholders);
            return (int)Scalar(connection, transaction, sql, values);
        }

        /// <summary>Updates one row; if another workstation deleted it meanwhile, says so instead of saving nothing.</summary>
        private static void Update(NpgsqlConnection connection, NpgsqlTransaction transaction, string table, int id, Values values)
        {
            string assignments = string.Join(", ", values.Columns.Select(c => c + " = @" + c));
            values.Add("id", id);
            if (Execute(connection, transaction, string.Format("UPDATE {0} SET {1} WHERE id = @id", table, assignments), values) != 1)
                throw new InvalidOperationException(ChangedElsewhere);
        }

        /// <summary>Id 0: adds the row. Otherwise updates it. Returns the row's id either way.</summary>
        private static int Save(NpgsqlConnection connection, NpgsqlTransaction transaction, string table, int id, Values values)
        {
            if (id == 0)
                return Insert(connection, transaction, table, 0, values);
            Update(connection, transaction, table, id, values);
            return id;
        }

        /// <summary>INSERT, or UPDATE of the other columns when a row with the same key already exists.</summary>
        private static void Upsert(NpgsqlConnection connection, NpgsqlTransaction transaction, string table, string[] key, Values values)
        {
            string updates = string.Join(", ", values.Columns.Where(c => !key.Contains(c)).Select(c => c + " = EXCLUDED." + c));
            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2}) ON CONFLICT ({3}) DO UPDATE SET {4}",
                table, string.Join(", ", values.Columns), values.Placeholders, string.Join(", ", key), updates);
            Execute(connection, transaction, sql, values);
        }

        private void Delete(string table, int id)
        {
            Run((connection, transaction) =>
                Execute(connection, transaction, "DELETE FROM " + table + " WHERE id = @id", new Values { { "id", id } }));
        }

        private static int Execute(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, Values values = null)
        {
            using (NpgsqlCommand command = Command(connection, transaction, sql, values))
                return command.ExecuteNonQuery();
        }

        private static object Scalar(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, Values values = null)
        {
            using (NpgsqlCommand command = Command(connection, transaction, sql, values))
                return command.ExecuteScalar();
        }

        private static NpgsqlCommand Command(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, Values values)
        {
            var command = new NpgsqlCommand(sql, connection, transaction);
            if (values != null)
                values.AddTo(command);
            return command;
        }

        /// <summary>
        /// Opens a pooled connection, runs <paramref name="work"/> in one transaction and commits.
        /// Anything thrown rolls the whole change back.
        /// </summary>
        private T Run<T>(Func<NpgsqlConnection, NpgsqlTransaction, T> work, IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (NpgsqlTransaction transaction = connection.BeginTransaction(isolation))
                    {
                        T result = work(connection, transaction);
                        transaction.Commit();
                        return result;
                    }
                }
            }
            catch (PostgresException ex)
            {
                // Class 23 is "integrity constraint violation": a rule in Schema.sql refused the change.
                if (!ex.SqlState.StartsWith("23", StringComparison.Ordinal))
                    throw;
                string message;
                if (ex.ConstraintName == null || !ConstraintMessages.TryGetValue(ex.ConstraintName, out message))
                    message = ChangedElsewhere;
                throw new InvalidOperationException(message, ex);
            }
        }

        private void Run(Action<NpgsqlConnection, NpgsqlTransaction> work)
        {
            Run((connection, transaction) =>
            {
                work(connection, transaction);
                return true;
            });
        }

        private static string SchemaScript()
        {
            using (Stream stream = typeof(ClinicDatabase).Assembly.GetManifestResourceStream("StomaDesk.Data.Schema.sql"))
            {
                if (stream == null)
                    throw new InvalidOperationException("Schema.sql lipsește din resursele aplicației.");
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Column values for one statement, written as a collection initializer: { "column", value }.
        /// Each value travels as a parameter named after its column, never inside the SQL text.
        /// </summary>
        private sealed class Values : IEnumerable
        {
            private readonly List<string> _columns = new List<string>();
            private readonly List<object> _values = new List<object>();

            public IList<string> Columns
            {
                get { return _columns; }
            }

            public string Placeholders
            {
                get { return string.Join(", ", _columns.Select(c => "@" + c)); }
            }

            public void Add(string column, object value)
            {
                _columns.Add(column);
                _values.Add(value ?? DBNull.Value);
            }

            public void AddTo(NpgsqlCommand command)
            {
                for (int i = 0; i < _columns.Count; i++)
                    command.Parameters.AddWithValue(_columns[i], _values[i]);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _columns.GetEnumerator();
            }
        }
    }
}
