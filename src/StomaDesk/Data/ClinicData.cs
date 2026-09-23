using System.Collections.Generic;
using System.Xml.Serialization;
using StomaDesk.Models;

namespace StomaDesk.Data
{
    /// <summary>Everything the clinic stores, saved as one XML document.</summary>
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
            return string.Format("CH-{0:D6}", NextReceiptNo++);
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
