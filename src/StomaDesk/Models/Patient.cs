using System;
using System.Collections.Generic;

namespace StomaDesk.Models
{
    public class Patient
    {
        public Patient()
        {
            Teeth = new List<ToothRecord>();
            CreatedAt = DateTime.Now;
        }

        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Cnp { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Allergies { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Only teeth that are not healthy are stored; no record means "Sănătos".</summary>
        public List<ToothRecord> Teeth { get; set; }

        public string FullName
        {
            get { return ((LastName ?? "") + " " + (FirstName ?? "")).Trim(); }
        }

        public int? Age
        {
            get
            {
                if (!BirthDate.HasValue)
                    return null;

                DateTime today = DateTime.Today;
                int age = today.Year - BirthDate.Value.Year;
                if (BirthDate.Value.Date > today.AddYears(-age))
                    age--;
                return age;
            }
        }

        public ToothRecord FindTooth(int tooth)
        {
            foreach (ToothRecord record in Teeth)
            {
                if (record.Tooth == tooth)
                    return record;
            }
            return null;
        }

        public void SetTooth(int tooth, ToothState state, string note)
        {
            ToothRecord record = FindTooth(tooth);

            if (state == ToothState.Healthy && string.IsNullOrWhiteSpace(note))
            {
                if (record != null)
                    Teeth.Remove(record);
                return;
            }

            if (record == null)
            {
                record = new ToothRecord { Tooth = tooth };
                Teeth.Add(record);
            }
            record.State = state;
            record.Note = note;
        }

        public override string ToString()
        {
            return FullName;
        }
    }

    public class ToothRecord
    {
        /// <summary>FDI number: 11..18, 21..28, 31..38, 41..48.</summary>
        public int Tooth { get; set; }
        public ToothState State { get; set; }
        public string Note { get; set; }
    }
}
