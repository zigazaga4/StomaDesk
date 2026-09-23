using System;

namespace StomaDesk.Models
{
    /// <summary>
    /// One procedure in a patient's treatment plan. Name and price are copied from the price list
    /// when planned, so a later price change does not rewrite what the patient already accepted.
    /// </summary>
    public class TreatmentItem
    {
        public TreatmentItem()
        {
            CreatedAt = DateTime.Now;
        }

        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ProcedureId { get; set; }
        public string ProcedureName { get; set; }
        public int? Tooth { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercent { get; set; }
        public TreatmentStatus Status { get; set; }
        public int? DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        /// <summary>Price after discount, rounded to bani.</summary>
        public decimal Total
        {
            get { return Math.Round(Price * (100m - DiscountPercent) / 100m, 2, MidpointRounding.AwayFromZero); }
        }
    }
}
