using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StomaDesk.Data;
using StomaDesk.Models;

namespace StomaDesk.Services
{
    /// <summary>A finished report: column headers, raw cell values (decimal, int, string) and a summary line.</summary>
    public class ReportTable
    {
        public ReportTable(string title, params string[] headers)
        {
            Title = title;
            Headers = headers;
            Rows = new List<object[]>();
        }

        public string Title { get; private set; }
        public string[] Headers { get; private set; }
        public List<object[]> Rows { get; private set; }
        public string Summary { get; set; }

        public void Add(params object[] cells)
        {
            Rows.Add(cells);
        }
    }

    public class ReportDefinition
    {
        private readonly Func<ClinicStore, DateTime, DateTime, ReportTable> _build;

        public ReportDefinition(string name, bool usesPeriod, Func<ClinicStore, DateTime, DateTime, ReportTable> build)
        {
            Name = name;
            UsesPeriod = usesPeriod;
            _build = build;
        }

        public string Name { get; private set; }
        public bool UsesPeriod { get; private set; }

        public ReportTable Build(ClinicStore store, DateTime from, DateTime to)
        {
            return _build(store, from, to);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>Every report the Reports tab offers. A new report is one more method and one more line in <see cref="All"/>.</summary>
    public static class Reports
    {
        public static IList<ReportDefinition> All()
        {
            return new List<ReportDefinition>
            {
                new ReportDefinition("Venit pe medic (proceduri finalizate)", true, RevenueByDoctor),
                new ReportDefinition("Încasări pe metodă de plată", true, PaymentsByMethod),
                new ReportDefinition("Cele mai frecvente proceduri", true, TopProcedures),
                new ReportDefinition("Programări pe status", true, AppointmentsByStatus),
                new ReportDefinition("Pacienți cu sold de plată", false, Debtors)
            };
        }

        private static ReportTable RevenueByDoctor(ClinicStore store, DateTime from, DateTime to)
        {
            var table = new ReportTable("Venit pe medic", "Medic", "Proceduri", "Total (lei)");
            var groups = store.TreatmentsCompletedBetween(from, to)
                .GroupBy(t => t.DoctorId)
                .Select(g => new { Doctor = store.DoctorName(g.Key), Count = g.Count(), Total = g.Sum(t => t.Total) })
                .OrderByDescending(g => g.Total)
                .ToList();

            foreach (var g in groups)
                table.Add(g.Doctor.Length == 0 ? "(fără medic)" : g.Doctor, g.Count, g.Total);
            table.Summary = "Total proceduri finalizate: " + Fmt.Money(groups.Sum(g => g.Total));
            return table;
        }

        private static ReportTable PaymentsByMethod(ClinicStore store, DateTime from, DateTime to)
        {
            var table = new ReportTable("Încasări pe metodă", "Metodă", "Încasări", "Total (lei)");
            var groups = store.PaymentsBetween(from, to)
                .GroupBy(p => p.Method)
                .Select(g => new { Method = g.Key, Count = g.Count(), Total = g.Sum(p => p.Amount) })
                .OrderByDescending(g => g.Total)
                .ToList();

            foreach (var g in groups)
                table.Add(Labels.For(g.Method), g.Count, g.Total);
            table.Summary = "Total încasat: " + Fmt.Money(groups.Sum(g => g.Total));
            return table;
        }

        private static ReportTable TopProcedures(ClinicStore store, DateTime from, DateTime to)
        {
            var table = new ReportTable("Proceduri frecvente", "Procedură", "Efectuate", "Total (lei)");
            var groups = store.TreatmentsCompletedBetween(from, to)
                .GroupBy(t => t.ProcedureName)
                .Select(g => new { Name = g.Key, Count = g.Count(), Total = g.Sum(t => t.Total) })
                .OrderByDescending(g => g.Count)
                .ThenByDescending(g => g.Total)
                .ToList();

            foreach (var g in groups)
                table.Add(g.Name, g.Count, g.Total);
            table.Summary = string.Format("{0} proceduri diferite, {1} efectuate în total", groups.Count, groups.Sum(g => g.Count));
            return table;
        }

        private static ReportTable AppointmentsByStatus(ClinicStore store, DateTime from, DateTime to)
        {
            var table = new ReportTable("Programări pe status", "Status", "Programări", "Procent (%)");
            List<Appointment> appointments = store.AppointmentsBetween(from, to);
            int total = appointments.Count;

            foreach (var g in appointments.GroupBy(a => a.Status).OrderBy(g => g.Key))
                table.Add(Labels.For(g.Key), g.Count(), total == 0 ? 0m : Math.Round(100m * g.Count() / total, 1));

            int past = appointments.Count(a => a.Status == AppointmentStatus.Done || a.Status == AppointmentStatus.NoShow);
            int noShow = appointments.Count(a => a.Status == AppointmentStatus.NoShow);
            table.Summary = past == 0
                ? total + " programări"
                : string.Format("{0} programări. Rata de neprezentare: {1} %", total, (100m * noShow / past).ToString("0.0", Fmt.RoNumbers));
            return table;
        }

        private static ReportTable Debtors(ClinicStore store, DateTime from, DateTime to)
        {
            var table = new ReportTable("Pacienți cu sold", "Pacient", "Telefon", "Sold (lei)");
            Dictionary<int, decimal> due = store.DueByPatient();
            var rows = store.FindPatients("")
                .Where(p => due.ContainsKey(p.Id) && due[p.Id] > 0m)
                .OrderByDescending(p => due[p.Id])
                .ToList();

            foreach (Patient p in rows)
                table.Add(p.FullName, p.Phone, due[p.Id]);
            table.Summary = string.Format("{0} pacienți, total de încasat: {1}", rows.Count, Fmt.Money(rows.Sum(p => due[p.Id])));
            return table;
        }

        /// <summary>
        /// CSV the way Romanian Excel opens it with a double click: semicolon separators,
        /// decimal comma, and a UTF-8 BOM so ș and ț are not garbled.
        /// </summary>
        public static void ExportCsv(ReportTable table, string path)
        {
            var csv = new StringBuilder();
            csv.AppendLine(string.Join(";", table.Headers.Select(h => Quote(h))));
            foreach (object[] row in table.Rows)
                csv.AppendLine(string.Join(";", row.Select(cell => Quote(Fmt.CsvCell(cell)))));
            File.WriteAllText(path, csv.ToString(), new UTF8Encoding(true));
        }

        private static string Quote(string value)
        {
            if (value.IndexOfAny(new[] { ';', '"', '\n', '\r' }) < 0)
                return value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
