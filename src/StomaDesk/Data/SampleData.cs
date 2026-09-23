using System;
using System.Collections.Generic;
using System.Linq;
using StomaDesk.Models;
using StomaDesk.Services;

namespace StomaDesk.Data
{
    /// <summary>
    /// Demo clinic created on first run: 3 doctors, a price list, 13 patients, three weeks of agenda
    /// around today, treatment plans and payments. A fixed random seed gives the same data every time.
    /// </summary>
    internal static class SampleData
    {
        private const int FirstSlotHour = 9;
        private const int SlotsPerDay = 18;   // 09:00 to 18:00, 30 minutes each

        public static ClinicData Create(DateTime today)
        {
            var data = new ClinicData();
            var random = new Random(2012);

            data.Info = new ClinicInfo
            {
                Name = "Clinica Dentară Zâmbet",
                Address = "Str. Mircea cel Bătrân 12, Constanța",
                Phone = "0241 555 012",
                FiscalCode = "RO0000000 (demo)"
            };

            AddDoctors(data);
            AddProcedures(data);
            AddPatients(data, random);
            AddTreatmentsAndPayments(data, random, today);
            AddAppointments(data, random, today);
            return data;
        }

        private static void AddDoctors(ClinicData data)
        {
            data.Doctors.Add(NewDoctor(data, "Dr. Ana Marinescu", "Stomatologie generală", 0x4A90D9));
            data.Doctors.Add(NewDoctor(data, "Dr. Radu Ionescu", "Endodonție", 0x2E9E6A));
            data.Doctors.Add(NewDoctor(data, "Dr. Ioana Stan", "Chirurgie și implantologie", 0xC9782B));
        }

        private static Doctor NewDoctor(ClinicData data, string name, string specialty, int rgb)
        {
            return new Doctor { Id = data.NewId(), Name = name, Specialty = specialty, ColorArgb = unchecked((int)0xFF000000) | rgb };
        }

        private static void AddProcedures(ClinicData data)
        {
            AddProcedure(data, "C01", "Consultație și plan de tratament", 150m, 30, false);
            AddProcedure(data, "C02", "Radiografie retroalveolară", 50m, 15, true);
            AddProcedure(data, "C03", "Radiografie panoramică (OPG)", 120m, 15, false);
            AddProcedure(data, "P01", "Detartraj și periaj profesional", 250m, 45, false);
            AddProcedure(data, "S01", "Sigilare", 120m, 20, true);
            AddProcedure(data, "O01", "Obturație compozit, o suprafață", 250m, 45, true);
            AddProcedure(data, "O02", "Obturație compozit, două sau trei suprafețe", 350m, 60, true);
            AddProcedure(data, "E01", "Tratament endodontic monoradicular", 600m, 60, true);
            AddProcedure(data, "E02", "Tratament endodontic pluriradicular", 900m, 90, true);
            AddProcedure(data, "X01", "Extracție simplă", 250m, 30, true);
            AddProcedure(data, "X02", "Extracție molar de minte", 600m, 60, true);
            AddProcedure(data, "R01", "Coroană metalo-ceramică", 900m, 60, true);
            AddProcedure(data, "R02", "Coroană zirconiu", 1400m, 60, true);
            AddProcedure(data, "I01", "Implant dentar (fără coroană)", 3200m, 90, true);
            AddProcedure(data, "A01", "Albire profesională", 1200m, 90, false);
        }

        private static void AddProcedure(ClinicData data, string code, string name, decimal price, int minutes, bool perTooth)
        {
            data.Procedures.Add(new Procedure
            {
                Id = data.NewId(),
                Code = code,
                Name = name,
                Price = price,
                DurationMinutes = minutes,
                PerTooth = perTooth
            });
        }

        private static void AddPatients(ClinicData data, Random random)
        {
            var people = new[]
            {
                new { Last = "Popescu", First = "Ion", Male = true, Born = new DateTime(1978, 3, 14), Phone = "0722 314 578", Allergies = "" },
                new { Last = "Ionescu", First = "Maria", Male = false, Born = new DateTime(1985, 7, 2), Phone = "0744 201 330", Allergies = "Penicilină" },
                new { Last = "Dumitru", First = "Andrei", Male = true, Born = new DateTime(1992, 11, 23), Phone = "0755 612 904", Allergies = "" },
                new { Last = "Stoica", First = "Elena", Male = false, Born = new DateTime(1969, 5, 30), Phone = "0723 118 245", Allergies = "" },
                new { Last = "Constantin", First = "Mihai", Male = true, Born = new DateTime(2001, 9, 9), Phone = "0766 402 117", Allergies = "" },
                new { Last = "Georgescu", First = "Ana", Male = false, Born = new DateTime(1958, 1, 17), Phone = "0241 612 330", Allergies = "Latex" },
                new { Last = "Radu", First = "Alexandru", Male = true, Born = new DateTime(2010, 6, 21), Phone = "0734 555 219", Allergies = "" },
                new { Last = "Matei", First = "Cristina", Male = false, Born = new DateTime(1990, 12, 5), Phone = "0727 903 611", Allergies = "" },
                new { Last = "Dobre", First = "Florin", Male = true, Born = new DateTime(1975, 8, 12), Phone = "0745 330 872", Allergies = "Lidocaină" },
                new { Last = "Nistor", First = "Ioana", Male = false, Born = new DateTime(1996, 2, 28), Phone = "0768 214 390", Allergies = "" },
                new { Last = "Barbu", First = "Gabriel", Male = true, Born = new DateTime(1983, 4, 16), Phone = "0729 818 042", Allergies = "" },
                new { Last = "Lungu", First = "Raluca", Male = false, Born = new DateTime(1988, 10, 3), Phone = "0751 406 713", Allergies = "" },
                new { Last = "Șerban", First = "Ștefan", Male = true, Born = new DateTime(1972, 7, 19), Phone = "0722 690 154", Allergies = "" }
            };
            string[] towns = { "Constanța", "Năvodari", "Mangalia", "Ovidiu", "Eforie Nord", "Medgidia" };

            for (int i = 0; i < people.Length; i++)
            {
                var person = people[i];
                var patient = new Patient
                {
                    Id = data.NewId(),
                    LastName = person.Last,
                    FirstName = person.First,
                    Cnp = Cnp.Build(person.Male, person.Born, 13, 101 + i * 53),
                    BirthDate = person.Born,
                    Phone = person.Phone,
                    Email = Search.Fold(person.First + "." + person.Last) + "@example.com",
                    Address = towns[random.Next(towns.Length)],
                    Allergies = person.Allergies,
                    CreatedAt = new DateTime(2019 + random.Next(6), 1 + random.Next(12), 1 + random.Next(28))
                };
                AddTeeth(patient, random);
                data.Patients.Add(patient);
            }
        }

        private static void AddTeeth(Patient patient, Random random)
        {
            ToothState[] weighted =
            {
                ToothState.Filling, ToothState.Filling, ToothState.Filling, ToothState.Filling,
                ToothState.Caries, ToothState.Caries, ToothState.Caries,
                ToothState.Crown, ToothState.RootCanal, ToothState.RootCanal,
                ToothState.Extracted, ToothState.Implant
            };
            List<int> teeth = Dentition.All.ToList();
            int count = 2 + random.Next(5);
            for (int i = 0; i < count; i++)
            {
                int tooth = teeth[random.Next(teeth.Count)];
                teeth.Remove(tooth);
                patient.SetTooth(tooth, weighted[random.Next(weighted.Length)], null);
            }
        }

        private static void AddTreatmentsAndPayments(ClinicData data, Random random, DateTime today)
        {
            Procedure consultation = data.Procedures[0];
            List<Procedure> perTooth = data.Procedures.Where(p => p.PerTooth).ToList();

            foreach (Patient patient in data.Patients)
            {
                DateTime firstVisit = today.AddDays(-20 - random.Next(25));
                var items = new List<TreatmentItem>();
                items.Add(NewItem(data, patient, consultation, null, RandomDoctor(data, random), TreatmentStatus.Done, firstVisit));

                int extra = 1 + random.Next(4);
                for (int i = 0; i < extra; i++)
                {
                    Procedure procedure = perTooth[random.Next(perTooth.Count)];
                    int tooth = PickTooth(patient, random);
                    int roll = random.Next(10);
                    TreatmentStatus status = roll < 5 ? TreatmentStatus.Done
                        : roll < 7 ? TreatmentStatus.Accepted
                        : roll < 8 ? TreatmentStatus.InProgress
                        : TreatmentStatus.Proposed;
                    DateTime? done = status == TreatmentStatus.Done ? (DateTime?)firstVisit.AddDays(random.Next(1, 18)) : null;
                    TreatmentItem item = NewItem(data, patient, procedure, tooth, RandomDoctor(data, random), status, done);
                    if (random.Next(5) == 0)
                        item.DiscountPercent = 10m;
                    items.Add(item);
                }
                data.Treatments.AddRange(items);

                decimal billed = items.Where(t => t.Status == TreatmentStatus.Done).Sum(t => t.Total);
                DateTime lastDone = items.Where(t => t.CompletedAt.HasValue).Max(t => t.CompletedAt.Value);
                int payRoll = random.Next(20);
                decimal paid = payRoll < 12 ? billed
                    : payRoll < 17 ? Math.Floor(billed / 2m / 50m) * 50m
                    : 0m;
                if (paid > 0m)
                {
                    data.Payments.Add(new Payment
                    {
                        Id = data.NewId(),
                        PatientId = patient.Id,
                        Amount = paid,
                        Date = lastDone.Date.AddHours(12 + random.Next(6)),
                        Method = (PaymentMethod)random.Next(3),
                        ReceiptNo = data.NewReceiptNo()
                    });
                }
            }
        }

        private static TreatmentItem NewItem(ClinicData data, Patient patient, Procedure procedure, int? tooth, int doctorId,
            TreatmentStatus status, DateTime? completedAt)
        {
            return new TreatmentItem
            {
                Id = data.NewId(),
                PatientId = patient.Id,
                ProcedureId = procedure.Id,
                ProcedureName = procedure.Name,
                Price = procedure.Price,
                Tooth = tooth,
                DoctorId = doctorId,
                Status = status,
                CreatedAt = (completedAt ?? DateTime.Today).AddDays(-7),
                CompletedAt = completedAt
            };
        }

        /// <summary>Prefers a tooth already marked with a problem, as a real plan would.</summary>
        private static int PickTooth(Patient patient, Random random)
        {
            List<ToothRecord> problems = patient.Teeth.Where(t => t.State == ToothState.Caries || t.State == ToothState.Extracted).ToList();
            if (problems.Count > 0 && random.Next(3) > 0)
                return problems[random.Next(problems.Count)].Tooth;

            List<int> all = Dentition.All.ToList();
            return all[random.Next(all.Count)];
        }

        private static int RandomDoctor(ClinicData data, Random random)
        {
            return data.Doctors[random.Next(data.Doctors.Count)].Id;
        }

        private static void AddAppointments(ClinicData data, Random random, DateTime today)
        {
            List<Procedure> procedures = data.Procedures;
            for (int offset = -14; offset <= 7; offset++)
            {
                DateTime day = today.AddDays(offset);
                if (day.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                bool saturday = day.DayOfWeek == DayOfWeek.Saturday;
                foreach (Doctor doctor in data.Doctors)
                {
                    var busy = new bool[SlotsPerDay];
                    int count = saturday ? 1 + random.Next(2) : 3 + random.Next(3);
                    for (int i = 0; i < count; i++)
                    {
                        Procedure procedure = procedures[random.Next(procedures.Count)];
                        int slots = Math.Max(1, (int)Math.Ceiling(procedure.DurationMinutes / 30.0));
                        int start = FindFreeSlot(busy, slots, random, saturday ? 10 : SlotsPerDay);
                        if (start < 0)
                            continue;
                        for (int s = start; s < start + slots; s++)
                            busy[s] = true;

                        Patient patient = data.Patients[random.Next(data.Patients.Count)];
                        data.Appointments.Add(new Appointment
                        {
                            Id = data.NewId(),
                            PatientId = patient.Id,
                            DoctorId = doctor.Id,
                            Start = day.AddHours(FirstSlotHour).AddMinutes(start * 30),
                            DurationMinutes = slots * 30,
                            Reason = procedure.Name,
                            Status = StatusFor(offset, random),
                            ReminderSent = offset < 1
                        });
                    }
                }
            }
        }

        private static int FindFreeSlot(bool[] busy, int slots, Random random, int lastSlot)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                int start = random.Next(0, lastSlot - slots + 1);
                bool free = true;
                for (int s = start; s < start + slots; s++)
                {
                    if (busy[s])
                    {
                        free = false;
                        break;
                    }
                }
                if (free)
                    return start;
            }
            return -1;
        }

        private static AppointmentStatus StatusFor(int dayOffset, Random random)
        {
            int roll = random.Next(100);
            if (dayOffset < 0)
                return roll < 88 ? AppointmentStatus.Done : roll < 96 ? AppointmentStatus.NoShow : AppointmentStatus.Cancelled;
            if (dayOffset == 0)
                return roll < 40 ? AppointmentStatus.Arrived : roll < 80 ? AppointmentStatus.Confirmed : AppointmentStatus.Scheduled;
            return roll < 30 ? AppointmentStatus.Confirmed : AppointmentStatus.Scheduled;
        }
    }
}
