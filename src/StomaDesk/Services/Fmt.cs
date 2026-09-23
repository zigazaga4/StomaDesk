using System;
using System.Globalization;

namespace StomaDesk.Services
{
    /// <summary>
    /// Romanian formatting (1.250,50 lei, 23.09.2026) built by hand, so it looks the same
    /// whatever regional settings the Windows or Linux machine has.
    /// </summary>
    public static class Fmt
    {
        private static readonly NumberFormatInfo Ro = CreateRoNumbers();

        private static readonly string[] DayNames =
        {
            "Duminică", "Luni", "Marți", "Miercuri", "Joi", "Vineri", "Sâmbătă"
        };

        private static readonly string[] MonthNames =
        {
            "ianuarie", "februarie", "martie", "aprilie", "mai", "iunie",
            "iulie", "august", "septembrie", "octombrie", "noiembrie", "decembrie"
        };

        public static NumberFormatInfo RoNumbers
        {
            get { return Ro; }
        }

        public static string Number(decimal value)
        {
            return value.ToString("N2", Ro);
        }

        public static string Money(decimal value)
        {
            return Number(value) + " lei";
        }

        public static string Date(DateTime value)
        {
            return value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        public static string Date(DateTime? value)
        {
            return value.HasValue ? Date(value.Value) : "";
        }

        public static string Time(DateTime value)
        {
            return value.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        /// <summary>"Miercuri, 23 septembrie 2026".</summary>
        public static string DayTitle(DateTime value)
        {
            return string.Format("{0}, {1} {2} {3}", DayNames[(int)value.DayOfWeek], value.Day, MonthNames[value.Month - 1], value.Year);
        }

        /// <summary>Text for a report cell: numbers with two decimals, dates as dd.MM.yyyy.</summary>
        public static string Cell(object value)
        {
            if (value == null)
                return "";
            if (value is decimal)
                return Number((decimal)value);
            if (value is DateTime)
                return Date((DateTime)value);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        /// <summary>Same as <see cref="Cell"/> but without thousands separators, so Excel reads numbers as numbers.</summary>
        public static string CsvCell(object value)
        {
            if (value is decimal)
                return ((decimal)value).ToString("0.00", Ro);
            return Cell(value);
        }

        private static NumberFormatInfo CreateRoNumbers()
        {
            var numbers = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            numbers.NumberDecimalSeparator = ",";
            numbers.NumberGroupSeparator = ".";
            return numbers;
        }
    }
}
