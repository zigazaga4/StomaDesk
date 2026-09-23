using StomaDesk.Services;

namespace StomaDesk.Models
{
    /// <summary>One line of the clinic's price list.</summary>
    public class Procedure
    {
        public Procedure()
        {
            DurationMinutes = 30;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }

        /// <summary>True when the procedure is done on one tooth (filling, extraction) and needs a tooth number.</summary>
        public bool PerTooth { get; set; }

        public Procedure Clone()
        {
            return (Procedure)MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("{0}   ({1})", Name, Fmt.Money(Price));
        }
    }
}
