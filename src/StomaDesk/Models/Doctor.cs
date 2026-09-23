namespace StomaDesk.Models
{
    public class Doctor
    {
        /// <summary>Steel blue, used until the clinic picks a colour for the agenda.</summary>
        public const int DefaultColor = unchecked((int)0xFF4682B4);

        public Doctor()
        {
            Active = true;
            ColorArgb = DefaultColor;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }

        /// <summary>A doctor who left stays in the list as inactive, so old appointments keep their name.</summary>
        public bool Active { get; set; }

        /// <summary>Stored as ARGB because System.Drawing.Color does not serialize to XML.</summary>
        public int ColorArgb { get; set; }

        public Doctor Clone()
        {
            return (Doctor)MemberwiseClone();
        }

        public override string ToString()
        {
            return Name ?? "";
        }
    }
}
