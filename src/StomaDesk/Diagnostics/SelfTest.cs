using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using StomaDesk.Controls;
using StomaDesk.Data;
using StomaDesk.Forms;
using StomaDesk.Models;
using StomaDesk.Services;

namespace StomaDesk.Diagnostics
{
    /// <summary>
    /// Checks that run from the command line, so the app can be verified on a machine without Visual Studio
    /// (for example with Mono on Linux). They always work on a temporary copy of the demo data.
    /// </summary>
    internal static class SelfTest
    {
        public static bool Run(TextWriter log, string outputFolder)
        {
            var check = new Checker(log);
            log.WriteLine("StomaDesk self-test");

            CheckCnp(check);
            CheckText(check);

            string path = TempDataPath();
            try
            {
                ClinicStore store = ClinicStore.Open(path);
                CheckStore(check, store, path);
                CheckRules(check, store);
                CheckReminders(check, store);
                CheckReports(check, store, outputFolder);
                CheckDrawing(check, store, outputFolder);
            }
            catch (Exception ex)
            {
                check.Fail("excepție neprevăzută", ex);
            }
            finally
            {
                DeleteTemp(path);
            }
            return check.Report();
        }

        /// <summary>
        /// Opens every window and tab once. Needs a display (Windows, or X11 on Linux).
        /// With an output folder it also saves a screenshot of each screen there.
        /// </summary>
        public static bool RunUi(TextWriter log, string outputFolder)
        {
            var check = new Checker(log);
            log.WriteLine("StomaDesk UI smoke test");
            string path = TempDataPath();
            try
            {
                ClinicStore store = ClinicStore.Open(path);
                using (var main = new MainForm(store))
                {
                    main.StartPosition = FormStartPosition.Manual;
                    main.Location = new Point(20, 20);
                    main.Show();
                    Pump();
                    int index = 0;
                    foreach (TabPage page in main.Tabs.TabPages)
                    {
                        main.Tabs.SelectedTab = page;
                        Pump();
                        check.Check("fereastra principală, tab " + page.Text, main.Tabs.SelectedTab == page);
                        Capture(main, outputFolder, "1" + index++ + "-" + page.Name);
                    }

                    Patient patient = store.FindPatients("").OrderByDescending(p => p.Teeth.Count).First();
                    using (var card = new PatientCardForm(store, patient))
                    {
                        card.StartPosition = FormStartPosition.Manual;
                        card.Location = new Point(40, 40);
                        card.Show(main);
                        Pump();
                        index = 0;
                        foreach (TabPage page in card.Tabs.TabPages)
                        {
                            card.Tabs.SelectedTab = page;
                            Pump();
                            check.Check("fișa pacientului, tab " + page.Text, card.Tabs.SelectedTab == page);
                            Capture(card, outputFolder, "2" + index++ + "-" + page.Name);
                        }
                        card.Close();
                    }

                    using (var form = new PatientForm(store, patient))
                    {
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = new Point(60, 60);
                        form.Show(main);
                        Pump();
                        check.Check("formular pacient", form.Visible);
                        Capture(form, outputFolder, "30-pacient");
                        form.Close();
                    }

                    Appointment appointment = store.AppointmentsFor(patient.Id).FirstOrDefault()
                        ?? new Appointment { PatientId = patient.Id, DoctorId = store.ActiveDoctors[0].Id, Start = DateTime.Today.AddHours(9) };
                    using (var form = new AppointmentForm(store, appointment, false))
                    {
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = new Point(60, 60);
                        form.Show(main);
                        Pump();
                        check.Check("formular programare", form.Visible);
                        Capture(form, outputFolder, "31-programare");
                        form.Close();
                    }

                    main.Close();
                }
            }
            catch (Exception ex)
            {
                check.Fail("interfață", ex);
            }
            finally
            {
                DeleteTemp(path);
            }
            return check.Report();
        }

        private static void CheckCnp(Checker check)
        {
            check.Check("CNP valid 1800101221144", Cnp.IsValid("1800101221144"));
            check.Check("CNP cu cifra de control greșită", !Cnp.IsValid("1800101221145"));
            check.Check("CNP cu luna 13", !Cnp.IsValid("1801301221144"));
            check.Check("CNP prea scurt", !Cnp.IsValid("180010122114"));
            check.Check("CNP cu litere", !Cnp.IsValid("18001012211A4"));

            CnpInfo info;
            string error;
            Cnp.TryParse("1800101221144", out info, out error);
            check.Check("CNP: data nașterii 01.01.1980", info != null && info.BirthDate == new DateTime(1980, 1, 1));
            check.Check("CNP: bărbat, județul 22", info != null && info.Male == true && info.County == 22);

            bool allBuilt = true;
            for (int year = 1930; year <= 2025; year += 7)
            {
                string female = Cnp.Build(false, new DateTime(year, 2, 28), 13, 250);
                allBuilt &= Cnp.IsValid(female) && (year < 2000 ? female[0] == '2' : female[0] == '6');
            }
            check.Check("CNP generate pentru 1930 până în 2025 sunt valide", allBuilt);
        }

        private static void CheckText(Checker check)
        {
            check.Check("diacritice eliminate (ș cu virgulă)", Search.Fold("Șerban Ștefăniță") == "serban stefanita");
            check.Check("diacritice eliminate (ş cu sedilă)", Search.Fold("Şerban") == "serban");
            check.Check("telefon +40 normalizat", PhoneNumber.National("+40 722 123 456") == "0722123456");
            check.Check("telefon 0040 normalizat", PhoneNumber.National("0040722123456") == "0722123456");
            check.Check("fix nu este mobil", !PhoneNumber.IsMobile("0241 612 330") && PhoneNumber.IsValid("0241 612 330"));
            check.Check("format bani 1.250,50 lei", Fmt.Money(1250.5m) == "1.250,50 lei");
        }

        private static void CheckStore(Checker check, ClinicStore store, string path)
        {
            List<Patient> patients = store.FindPatients("");
            check.Check("date demo: 13 pacienți", patients.Count == 13);
            check.Check("date demo: 3 medici, 15 proceduri", store.Doctors.Count == 3 && store.Procedures.Count == 15);
            check.Check("date demo: programări mâine", store.AppointmentsOn(DateTime.Today.AddDays(1)).Count > 0
                || DateTime.Today.AddDays(1).DayOfWeek == DayOfWeek.Sunday);

            ClinicStore reopened = ClinicStore.Open(path);
            check.Check("XML salvat și recitit: aceiași pacienți", reopened.FindPatients("").Count == patients.Count);
            check.Check("XML salvat și recitit: aceleași programări",
                reopened.AppointmentsBetween(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(30)).Count
                == store.AppointmentsBetween(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(30)).Count);
            check.Check("XML salvat și recitit: diacritice intacte", reopened.FindPatients("serban").Any(p => p.LastName == "Șerban"));

            check.Check("căutare fără diacritice (serban)", store.FindPatients("serban").Count == 1);
            check.Check("căutare prenume înainte de nume (ion popescu)", store.FindPatients("ion popescu").Count == 1);
            check.Check("căutare după telefon (0722 314)", store.FindPatients("0722 314").Count == 1);
            check.Check("căutare după CNP parțial", store.FindPatients(patients[0].Cnp.Substring(0, 7)).Count >= 1);
        }

        private static void CheckRules(Checker check, ClinicStore store)
        {
            Appointment existing = store.AppointmentsBetween(DateTime.Today.AddDays(1), DateTime.Today.AddDays(7)).First(a => a.IsActive);
            var clash = new Appointment { PatientId = existing.PatientId, DoctorId = existing.DoctorId, Start = existing.Start.AddMinutes(10), DurationMinutes = 30 };
            check.Check("suprapunere la același medic detectată", store.FindConflict(clash) == existing);

            clash.Start = existing.End;
            check.Check("programare lipită de cealaltă nu e suprapunere", store.FindConflict(clash) != existing);

            Doctor other = store.Doctors.First(d => d.Id != existing.DoctorId);
            clash.Start = existing.Start;
            clash.DoctorId = other.Id;
            check.Check("alt medic la aceeași oră nu e suprapunere", store.FindConflict(clash) != existing);

            bool refused = false;
            try
            {
                store.SaveAppointment(new Appointment { PatientId = existing.PatientId, DoctorId = existing.DoctorId, Start = existing.Start, DurationMinutes = 30 });
            }
            catch (InvalidOperationException)
            {
                refused = true;
            }
            check.Check("store refuză salvarea unei suprapuneri", refused);

            var patient = new Patient { LastName = "Test", FirstName = "Sold" };
            store.SavePatient(patient);
            Procedure consultation = store.Procedures.First(p => !p.PerTooth);
            Procedure filling = store.Procedures.First(p => p.PerTooth);

            refused = false;
            try
            {
                store.PlanProcedure(patient.Id, filling, null, null, 0m);
            }
            catch (ArgumentException)
            {
                refused = true;
            }
            check.Check("procedura pe dinte cere dintele", refused);

            TreatmentItem item = store.PlanProcedure(patient.Id, consultation, null, null, 10m);
            check.Check("discount 10% aplicat", item.Total == Math.Round(consultation.Price * 0.9m, 2));
            check.Check("procedura nouă intră ca propusă", item.Status == TreatmentStatus.Proposed);

            store.SetTreatmentStatus(new[] { item }, TreatmentStatus.Done);
            check.Check("finalizarea pune data", item.CompletedAt.HasValue);

            var payment = new Payment { PatientId = patient.Id, Amount = 50m, Method = PaymentMethod.Cash };
            store.AddPayment(payment);
            Balance balance = store.BalanceFor(patient.Id);
            check.Check("sold = finalizat minus încasat", balance.Due == item.Total - 50m);
            check.Check("chitanță numerotată CH-", payment.ReceiptNo != null && payment.ReceiptNo.StartsWith("CH-"));

            string reason;
            check.Check("procedura finalizată nu se șterge", !store.TryDeleteTreatment(item, out reason));
            check.Check("pacient cu încasări nu se șterge", !store.TryDeletePatient(patient, out reason));

            store.SetTooth(patient, 26, ToothState.Caries, "ocluzal");
            check.Check("starea dintelui 26 salvată", patient.FindTooth(26) != null && patient.FindTooth(26).State == ToothState.Caries);
            store.SetTooth(patient, 26, ToothState.Healthy, "");
            check.Check("dinte sănătos fără notă nu se mai stochează", patient.FindTooth(26) == null);
        }

        private static void CheckReminders(Checker check, ClinicStore store)
        {
            DateTime tomorrow = DateTime.Today.AddDays(1);
            List<Reminder> reminders = Reminders.Build(store, tomorrow);
            check.Check("SMS-urile nu conțin diacritice", reminders.All(r => r.Message == Search.StripDiacritics(r.Message)));

            List<Reminder> batch = reminders.Take(3).ToList();
            batch.Add(new Reminder { AppointmentId = -1, PatientName = "Fix", Phone = "0241 612 330", Message = "test" });
            ReminderRun run = Reminders.SendAsync(batch, null, CancellationToken.None).Result;
            check.Check("trimitere asincronă: mobilele trimise, fixul refuzat",
                run.SentIds.Count == batch.Count - 1 && run.Failures.Count == 1);

            store.MarkRemindersSent(run.SentIds);
            check.Check("programările trimise nu mai primesc SMS", Reminders.Build(store, tomorrow).Count == reminders.Count - (batch.Count - 1));

            var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            ReminderRun stopped = Reminders.SendAsync(batch, null, cancelled.Token).Result;
            check.Check("trimiterea se poate opri", stopped.Cancelled && stopped.SentIds.Count == 0);
        }

        private static void CheckReports(Checker check, ClinicStore store, string outputFolder)
        {
            DateTime from = DateTime.Today.AddDays(-60);
            DateTime to = DateTime.Today.AddDays(7);
            foreach (ReportDefinition definition in Reports.All())
            {
                ReportTable table = definition.Build(store, from, to);
                check.Check("raport „" + definition.Name + "” are date", table.Rows.Count > 0 && table.Rows.All(r => r.Length == table.Headers.Length));
            }

            ReportTable debtors = Reports.All().Last().Build(store, from, to);
            string csv = Path.Combine(outputFolder ?? Path.GetTempPath(), "stomadesk-raport-" + Guid.NewGuid().ToString("N") + ".csv");
            Reports.ExportCsv(debtors, csv);
            byte[] bytes = File.ReadAllBytes(csv);
            string text = Encoding.UTF8.GetString(bytes);
            check.Check("CSV cu BOM UTF-8 și separator ;", bytes.Length > 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF && text.Contains(";"));
            if (outputFolder == null)
                File.Delete(csv);
        }

        private static void CheckDrawing(Checker check, ClinicStore store, string outputFolder)
        {
            Patient patient = store.FindPatients("").First(p => p.Teeth.Count > 0);
            using (var chart = new OdontogramControl())
            using (var image = new Bitmap(chart.Width, chart.Height))
            {
                chart.SetData(patient.Teeth, new[] { 36, 46 });
                chart.SelectedTooth = patient.Teeth[0].Tooth;
                using (Graphics g = Graphics.FromImage(image))
                    chart.RenderTo(g);

                check.Check("odontograma se desenează", CountInk(image) > 2000);
                if (outputFolder != null)
                    image.Save(Path.Combine(outputFolder, "odontograma.png"), ImageFormat.Png);
            }

            List<TreatmentItem> items = store.FindPatients("").SelectMany(p => store.TreatmentsFor(p.Id)).Take(40).ToList();
            Estimate estimate = Estimate.For(store, patient, items);
            List<Bitmap> pages = EstimateRenderer.RenderPages(estimate);
            try
            {
                check.Check("devizul cu 40 de rânduri are mai multe pagini", pages.Count >= 2);
                check.Check("pagina de deviz are conținut", CountInk(pages[0]) > 2000);
                if (outputFolder != null)
                    EstimateRenderer.SavePng(estimate, Path.Combine(outputFolder, "deviz.png"));
            }
            finally
            {
                foreach (Bitmap page in pages)
                    page.Dispose();
            }
        }

        /// <summary>Counts pixels that are not (almost) white, sampled every 2 px.</summary>
        private static int CountInk(Bitmap image)
        {
            int ink = 0;
            for (int y = 0; y < image.Height; y += 2)
            {
                for (int x = 0; x < image.Width; x += 2)
                {
                    Color c = image.GetPixel(x, y);
                    if (c.R + c.G + c.B < 720)
                        ink++;
                }
            }
            return ink;
        }

        /// <summary>
        /// Saves what the user sees in the window. It copies from the window's own device context, because
        /// copying from the screen returns a black image on Wayland desktops. Mono on Linux has no user32.dll,
        /// so there it falls back to a screen copy.
        /// </summary>
        private static void Capture(Form form, string outputFolder, string name)
        {
            if (outputFolder == null)
                return;

            form.Activate();
            Pump();
            Rectangle bounds = form.Bounds;
            using (var image = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    try
                    {
                        IntPtr target = g.GetHdc();
                        IntPtr source = NativeMethods.GetWindowDC(form.Handle);
                        NativeMethods.BitBlt(target, 0, 0, bounds.Width, bounds.Height, source, 0, 0, NativeMethods.SrcCopy);
                        NativeMethods.ReleaseDC(form.Handle, source);
                        g.ReleaseHdc(target);
                    }
                    catch (DllNotFoundException)
                    {
                        g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    }
                    catch (EntryPointNotFoundException)
                    {
                        g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    }
                }
                image.Save(Path.Combine(outputFolder, name + ".png"), ImageFormat.Png);
            }
        }

        private static class NativeMethods
        {
            public const int SrcCopy = 0x00CC0020;

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr window);

            [DllImport("user32.dll")]
            public static extern int ReleaseDC(IntPtr window, IntPtr dc);

            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool BitBlt(IntPtr target, int x, int y, int width, int height, IntPtr source, int sourceX, int sourceY, int operation);
        }

        private static void Pump()
        {
            for (int i = 0; i < 6; i++)
            {
                Application.DoEvents();
                Thread.Sleep(40);
            }
        }

        private static string TempDataPath()
        {
            return Path.Combine(Path.GetTempPath(), "stomadesk-test-" + Guid.NewGuid().ToString("N") + ".xml");
        }

        private static void DeleteTemp(string path)
        {
            foreach (string file in new[] { path, path + ".bak", path + ".tmp" })
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        private sealed class Checker
        {
            private readonly TextWriter _log;
            private int _passed;
            private int _failed;

            public Checker(TextWriter log)
            {
                _log = log;
            }

            public void Check(string name, bool ok)
            {
                if (ok)
                    _passed++;
                else
                    _failed++;
                _log.WriteLine((ok ? "  ok     " : "  EȘUAT  ") + name);
            }

            public void Fail(string name, Exception ex)
            {
                _failed++;
                _log.WriteLine("  EȘUAT  " + name + ": " + ex);
            }

            public bool Report()
            {
                _log.WriteLine();
                _log.WriteLine("{0} verificări trecute, {1} eșuate.", _passed, _failed);
                _log.Flush();
                return _failed == 0;
            }
        }
    }
}
