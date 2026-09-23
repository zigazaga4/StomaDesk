using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StomaDesk.Models
{
    /// <summary>
    /// Permanent teeth in FDI numbering, in the order a dentist reads the chart while facing the patient:
    /// upper arch 18..11 | 21..28, lower arch 48..41 | 31..38.
    /// </summary>
    public static class Dentition
    {
        public static readonly ReadOnlyCollection<int> Upper = Array.AsReadOnly(new[]
        {
            18, 17, 16, 15, 14, 13, 12, 11, 21, 22, 23, 24, 25, 26, 27, 28
        });

        public static readonly ReadOnlyCollection<int> Lower = Array.AsReadOnly(new[]
        {
            48, 47, 46, 45, 44, 43, 42, 41, 31, 32, 33, 34, 35, 36, 37, 38
        });

        /// <summary>All 32 teeth sorted by number (11, 12 ... 48), for drop-down lists.</summary>
        public static IEnumerable<int> All
        {
            get { return Upper.Concat(Lower).OrderBy(t => t); }
        }

        public static bool IsUpper(int tooth)
        {
            return tooth >= 11 && tooth <= 28;
        }

        public static bool IsValid(int tooth)
        {
            return Upper.Contains(tooth) || Lower.Contains(tooth);
        }
    }
}
