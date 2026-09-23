using System;

namespace StomaDesk.Models
{
    public class Payment
    {
        public Payment()
        {
            Date = DateTime.Now;
            Method = PaymentMethod.Cash;
        }

        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public string Note { get; set; }

        /// <summary>Receipt number given by the store, e.g. CH-000042.</summary>
        public string ReceiptNo { get; set; }
    }
}
