using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StomaDesk.Data;
using StomaDesk.Models;

namespace StomaDesk.Services
{
    /// <summary>One SMS ready to send. Plain data, safe to hand to a worker thread.</summary>
    public class Reminder
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
    }

    public class ReminderRun
    {
        public ReminderRun()
        {
            SentIds = new List<int>();
            Failures = new List<string>();
        }

        public List<int> SentIds { get; private set; }
        public List<string> Failures { get; private set; }
        public bool Cancelled { get; set; }
    }

    public static class Reminders
    {
        /// <summary>
        /// Builds tomorrow's (or any day's) SMS list. It reads the store, so call it on the UI thread;
        /// the result holds only strings and numbers, which the worker thread can use safely.
        /// </summary>
        public static List<Reminder> Build(ClinicStore store, DateTime day)
        {
            var reminders = new List<Reminder>();
            foreach (Appointment a in store.AppointmentsOn(day))
            {
                if (a.ReminderSent)
                    continue;
                if (a.Status != AppointmentStatus.Scheduled && a.Status != AppointmentStatus.Confirmed)
                    continue;

                Patient patient = store.GetPatient(a.PatientId);
                if (patient == null)
                    continue;

                reminders.Add(new Reminder
                {
                    AppointmentId = a.Id,
                    PatientName = patient.FullName,
                    Phone = patient.Phone,
                    Message = MessageFor(store.Info, patient, a)
                });
            }
            return reminders;
        }

        /// <summary>
        /// SMS text without diacritics on purpose: a single ș or ț switches the whole SMS to UCS-2,
        /// which cuts one part from 160 characters to 70 and can double the cost.
        /// </summary>
        public static string MessageFor(ClinicInfo clinic, Patient patient, Appointment appointment)
        {
            string text = string.Format(
                "Buna ziua, {0}! Va reamintim programarea din {1}, ora {2}, la {3}. Pentru reprogramare: {4}.",
                patient.FirstName, Fmt.Date(appointment.Start), Fmt.Time(appointment.Start), clinic.Name, clinic.Phone);
            return Search.StripDiacritics(text);
        }

        /// <summary>
        /// Sends one SMS at a time on a thread-pool thread. Because the caller awaits this from a UI event,
        /// progress callbacks and the code after the await run back on the UI thread.
        /// </summary>
        public static async Task<ReminderRun> SendAsync(IList<Reminder> reminders, IProgress<int> progress, CancellationToken token)
        {
            var run = new ReminderRun();
            for (int i = 0; i < reminders.Count; i++)
            {
                if (token.IsCancellationRequested)
                {
                    run.Cancelled = true;
                    break;
                }

                Reminder reminder = reminders[i];
                string error = await Task.Run(() => SendOne(reminder));
                if (error == null)
                    run.SentIds.Add(reminder.AppointmentId);
                else
                    run.Failures.Add(reminder.PatientName + ": " + error);

                if (progress != null)
                    progress.Report(i + 1);
            }
            return run;
        }

        /// <summary>Stand-in for the SMS provider's HTTP call. Runs off the UI thread and never touches a control.</summary>
        private static string SendOne(Reminder reminder)
        {
            Thread.Sleep(350);
            if (!PhoneNumber.IsMobile(reminder.Phone))
                return string.Format("numărul „{0}” nu este de mobil", reminder.Phone ?? "");
            return null;
        }
    }
}
