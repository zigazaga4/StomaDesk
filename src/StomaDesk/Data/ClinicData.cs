using System.Collections.Generic;
using System.Xml.Serialization;
using StomaDesk.Models;

namespace StomaDesk.Data
{
    /// <summary>
    /// Everything the clinic stores: what ClinicStore keeps in memory after reading PostgreSQL,
    /// and the shape of the XML backup file (File > Copie de siguranță, loaded back with --import).
    /// </summary>
    [XmlRoot("Clinic")]
    public class ClinicData
    {
        public const int CurrentVersion = 1;

        public ClinicData()
        {
            Version = CurrentVersion;
            NextId = 1;
            NextReceiptNo = 1;
            Info = new ClinicInfo();
            Doctors = new List<Doctor>();
            Procedures = new List<Procedure>();
            Patients = new List<Patient>();
            Appointments = new List<Appointment>();
            Treatments = new List<TreatmentItem>();
            Payments = new List<Payment>();
        }

        [XmlAttribute]
        public int Version { get; set; }

        public int NextId { get; set; }
        public int NextReceiptNo { get; set; }
        public ClinicInfo Info { get; set; }
        public List<Doctor> Doctors { get; set; }
        public List<Procedure> Procedures { get; set; }
        public List<Patient> Patients { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<TreatmentItem> Treatments { get; set; }
        public List<Payment> Payments { get; set; }

        public int NewId()
        {
            return NextId++;
        }

        public string NewReceiptNo()
        {
            return FormatReceiptNo(NextReceiptNo++);
        }

        /// <summary>CH-000042. Used for demo data here and for the database's receipt sequence in ClinicDatabase.</summary>
        public static string FormatReceiptNo(long number)
        {
            return string.Format("CH-{0:D6}", number);
        }
    }

    public class ClinicInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string FiscalCode { get; set; }
    }
}
