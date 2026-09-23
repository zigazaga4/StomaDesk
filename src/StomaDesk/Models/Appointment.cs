using System;

namespace StomaDesk.Models
{
    public class Appointment
    {
        public Appointment()
        {
            DurationMinutes = 30;
        }

        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime Start { get; set; }
        public int DurationMinutes { get; set; }
        public string Reason { get; set; }
        public AppointmentStatus Status { get; set; }
        public bool ReminderSent { get; set; }

        public DateTime End
        {
            get { return Start.AddMinutes(DurationMinutes); }
        }

        /// <summary>A cancelled appointment frees its slot.</summary>
        public bool IsActive
        {
            get { return Status != AppointmentStatus.Cancelled; }
        }

        public bool Overlaps(Appointment other)
        {
            return Start < other.End && other.Start < End;
        }

        public Appointment Clone()
        {
            return (Appointment)MemberwiseClone();
        }

        /// <summary>Copies everything the user edits; the Id and reminder flag stay.</summary>
        public void CopyDetailsFrom(Appointment other)
        {
            PatientId = other.PatientId;
            DoctorId = other.DoctorId;
            Start = other.Start;
            DurationMinutes = other.DurationMinutes;
            Reason = other.Reason;
            Status = other.Status;
        }
    }
}
