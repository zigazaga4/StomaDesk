using System;
using System.Globalization;

namespace StomaDesk.Services
{
    /// <summary>What a valid CNP says about its owner.</summary>
    public class CnpInfo
    {
        public DateTime BirthDate { get; set; }

        /// <summary>Null when the first digit is 9 (foreign citizen, sex not encoded).</summary>
        public bool? Male { get; set; }

        public int County { get; set; }
    }

    /// <summary>
    /// Romanian personal numeric code: S YY MM DD JJ NNN C.
    /// S = sex and century, JJ = county, NNN = sequence, C = control digit:
    /// the first 12 digits times 2,7,9,1,4,6,3,5,8,2,7,9, summed, modulo 11 (a result of 10 becomes 1).
    /// </summary>
    public static class Cnp
    {
        private const string Weights = "279146358279";

        public static bool IsValid(string cnp)
        {
            CnpInfo info;
            string error;
            return TryParse(cnp, out info, out error);
        }

        public static bool TryParse(string cnp, out CnpInfo info, out string error)
        {
            info = null;
            error = null;
            cnp = (cnp ?? "").Trim();

            if (cnp.Length != 13)
            {
                error = "CNP-ul are 13 cifre.";
                return false;
            }
            foreach (char c in cnp)
            {
                if (c < '0' || c > '9')
                {
                    error = "CNP-ul conține doar cifre.";
                    return false;
                }
            }

            int s = cnp[0] - '0';
            if (s == 0)
            {
                error = "Prima cifră a CNP-ului nu poate fi 0.";
                return false;
            }

            int yy = Number(cnp, 1, 2);
            int mm = Number(cnp, 3, 2);
            int dd = Number(cnp, 5, 2);
            int year = CenturyFor(s, yy) + yy;
            if (mm < 1 || mm > 12 || dd < 1 || dd > DateTime.DaysInMonth(year, mm))
            {
                error = "Data nașterii din CNP nu există.";
                return false;
            }

            int county = Number(cnp, 7, 2);
            if (!IsCounty(county))
            {
                error = "Codul de județ din CNP nu există.";
                return false;
            }
            if (Number(cnp, 9, 3) == 0)
            {
                error = "Numărul de ordine din CNP nu poate fi 000.";
                return false;
            }
            if (cnp[12] != ControlDigit(cnp.Substring(0, 12)))
            {
                error = "Cifra de control nu se potrivește: CNP-ul este tastat greșit.";
                return false;
            }

            info = new CnpInfo
            {
                BirthDate = new DateTime(year, mm, dd),
                Male = s == 9 ? (bool?)null : s % 2 == 1,
                County = county
            };
            return true;
        }

        public static char ControlDigit(string first12)
        {
            int sum = 0;
            for (int i = 0; i < 12; i++)
                sum += (first12[i] - '0') * (Weights[i] - '0');

            int rest = sum % 11;
            return (char)('0' + (rest == 10 ? 1 : rest));
        }

        /// <summary>Builds a valid CNP (used for the demo patients).</summary>
        public static string Build(bool male, DateTime birth, int county, int sequence)
        {
            int s = birth.Year >= 2000 ? (male ? 5 : 6)
                : birth.Year >= 1900 ? (male ? 1 : 2)
                : (male ? 3 : 4);
            string first12 = string.Format(CultureInfo.InvariantCulture, "{0}{1:yyMMdd}{2:D2}{3:D3}", s, birth, county, sequence);
            return first12 + ControlDigit(first12);
        }

        private static int CenturyFor(int s, int yy)
        {
            switch (s)
            {
                case 1:
                case 2:
                    return 1900;
                case 3:
                case 4:
                    return 1800;
                case 5:
                case 6:
                    return 2000;
                default:
                    // 7, 8 (residents) and 9 (foreigners): the century is not encoded, so take the most recent one that fits.
                    return 2000 + yy <= DateTime.Today.Year ? 2000 : 1900;
            }
        }

        private static bool IsCounty(int code)
        {
            // 01..40 counties, 41..46 Bucharest sectors 1..6, 47..48 former sectors 7..8, 51 Călărași, 52 Giurgiu, 70 any county.
            return (code >= 1 && code <= 48) || code == 51 || code == 52 || code == 70;
        }

        private static int Number(string text, int start, int length)
        {
            return int.Parse(text.Substring(start, length), CultureInfo.InvariantCulture);
        }
    }
}
