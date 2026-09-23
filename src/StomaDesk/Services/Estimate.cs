using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using StomaDesk.Data;
using StomaDesk.Models;
using StomaDesk.Ui;

namespace StomaDesk.Services
{
    public class EstimateLine
    {
        public string Tooth { get; set; }
        public string Procedure { get; set; }
        public string Doctor { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>A "deviz": the priced list of procedures the patient still has to do.</summary>
    public class Estimate
    {
        public Estimate()
        {
            Lines = new List<EstimateLine>();
            Date = DateTime.Today;
        }

        public ClinicInfo Clinic { get; set; }
        public Patient Patient { get; set; }
        public DateTime Date { get; set; }
        public List<EstimateLine> Lines { get; private set; }

        public decimal Total
        {
            get { return Lines.Sum(l => l.Total); }
        }

        public static Estimate For(ClinicStore store, Patient patient, IEnumerable<TreatmentItem> items)
        {
            var estimate = new Estimate { Clinic = store.Info, Patient = patient };
            foreach (TreatmentItem item in items)
            {
                estimate.Lines.Add(new EstimateLine
                {
                    Tooth = item.Tooth.HasValue ? item.Tooth.Value.ToString() : "",
                    Procedure = item.ProcedureName,
                    Doctor = store.DoctorName(item.DoctorId),
                    Price = item.Price,
                    Discount = item.DiscountPercent,
                    Total = item.Total
                });
            }
            return estimate;
        }
    }

    /// <summary>
    /// Draws an estimate page with GDI+. The printer (<see cref="EstimateDocument"/>) and the PNG export
    /// both call the same code, so the preview, the paper and the image always match.
    /// </summary>
    public static class EstimateRenderer
    {
        private static readonly string[] Headers = { "Nr.", "Dinte", "Procedură", "Medic", "Preț", "Disc.", "Total" };
        private static readonly float[] Widths = { 0.06f, 0.07f, 0.39f, 0.19f, 0.11f, 0.07f, 0.11f };
        private static readonly bool[] AlignRight = { false, false, false, false, true, true, true };
        private static readonly Color Accent = Theme.Brand;

        /// <summary>Draws one page starting at <paramref name="firstLine"/> and returns the first line not drawn.</summary>
        public static int DrawPage(Graphics g, RectangleF area, Estimate estimate, int firstLine, int pageNumber)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var titleFont = new Font(FontFamily.GenericSansSerif, 15f, FontStyle.Bold))
            using (var headFont = new Font(FontFamily.GenericSansSerif, 9f, FontStyle.Bold))
            using (var bodyFont = new Font(FontFamily.GenericSansSerif, 9f))
            using (var smallFont = new Font(FontFamily.GenericSansSerif, 8f))
            using (var accentBrush = new SolidBrush(Accent))
            using (var grayBrush = new SolidBrush(Color.FromArgb(90, 90, 90)))
            using (var stripeBrush = new SolidBrush(GdiKit.Tint(Accent, 0.95f)))
            using (var headerBrush = new SolidBrush(GdiKit.Tint(Accent, 0.85f)))
            using (var accentPen = new Pen(Accent, 2f))
            using (var linePen = new Pen(Color.FromArgb(200, 200, 200), 1f))
            {
                float x = area.Left;
                float width = area.Width;
                float y = area.Top;

                // Logo and clinic on the left, document title on the right.
                float logo = titleFont.GetHeight(g) + smallFont.GetHeight(g) + 2f;
                Glyphs.DrawLogo(g, new RectangleF(x, y, logo, logo));
                float textX = x + logo + 10f;
                g.DrawString(estimate.Clinic.Name, titleFont, Brushes.Black, textX, y);
                DrawRight(g, "DEVIZ ESTIMATIV", titleFont, accentBrush, x + width, y);
                y += titleFont.GetHeight(g) + 2f;
                g.DrawString(estimate.Clinic.Address + "   Tel. " + estimate.Clinic.Phone, smallFont, grayBrush, textX, y);
                DrawRight(g, string.Format("Data: {0}   Pagina {1}", Fmt.Date(estimate.Date), pageNumber), smallFont, grayBrush, x + width, y);
                y += smallFont.GetHeight(g) + 8f;
                g.DrawLine(accentPen, x, y, x + width, y);
                y += 12f;

                // Patient block.
                Patient patient = estimate.Patient;
                g.DrawString("Pacient: " + patient.FullName, headFont, Brushes.Black, x, y);
                y += headFont.GetHeight(g) + 2f;
                string details = string.IsNullOrEmpty(patient.Cnp) ? "" : "CNP " + patient.Cnp + "    ";
                details += string.IsNullOrEmpty(patient.Phone) ? "" : "Tel. " + patient.Phone;
                g.DrawString(details, bodyFont, grayBrush, x, y);
                y += bodyFont.GetHeight(g) + 14f;

                // Table.
                float rowHeight = bodyFont.GetHeight(g) + 9f;
                float[] columns = ColumnPositions(x, width);
                g.FillRectangle(headerBrush, x, y, width, rowHeight);
                for (int c = 0; c < Headers.Length; c++)
                    DrawCell(g, Headers[c], headFont, Brushes.Black, columns[c], y, columns[c + 1] - columns[c], rowHeight, AlignRight[c]);
                y += rowHeight;

                float lastRowBottom = area.Bottom - rowHeight * 7f;   // room for totals and signatures
                int line = firstLine;
                while (line < estimate.Lines.Count && (y + rowHeight <= lastRowBottom || line == firstLine))
                {
                    EstimateLine l = estimate.Lines[line];
                    if (line % 2 == 1)
                        g.FillRectangle(stripeBrush, x, y + 1f, width, rowHeight - 1f);   // keep the line above visible

                    string[] cells =
                    {
                        (line + 1).ToString(), l.Tooth, l.Procedure, l.Doctor, Fmt.Number(l.Price),
                        l.Discount > 0m ? l.Discount.ToString("0.#", Fmt.RoNumbers) + "%" : "", Fmt.Number(l.Total)
                    };
                    for (int c = 0; c < cells.Length; c++)
                        DrawCell(g, cells[c], bodyFont, Brushes.Black, columns[c], y, columns[c + 1] - columns[c], rowHeight, AlignRight[c]);

                    y += rowHeight;
                    g.DrawLine(linePen, x, y, x + width, y);
                    line++;
                }

                if (line < estimate.Lines.Count)
                {
                    g.DrawString("Continuare pe pagina următoare", smallFont, grayBrush, x, y + 6f);
                    return line;
                }

                // Totals, validity and signatures on the last page.
                y += 8f;
                DrawRight(g, "Total deviz: " + Fmt.Money(estimate.Total), titleFont, Brushes.Black, x + width, y);
                y += titleFont.GetHeight(g) + 16f;
                g.DrawString("Devizul este valabil 30 de zile. Prețurile se pot modifica dacă planul de tratament se schimbă.", smallFont, grayBrush, x, y);
                y += rowHeight * 2.5f;
                g.DrawString("Medic curant: ______________________", bodyFont, Brushes.Black, x, y);
                DrawRight(g, "Pacient: ______________________", bodyFont, Brushes.Black, x + width, y);
                return line;
            }
        }

        /// <summary>Renders every page as an A4 image at 96 dpi.</summary>
        public static List<Bitmap> RenderPages(Estimate estimate)
        {
            const int PageWidth = 794, PageHeight = 1123, Margin = 57;
            var pages = new List<Bitmap>();
            int line = 0;
            do
            {
                var page = new Bitmap(PageWidth, PageHeight);
                page.SetResolution(96f, 96f);
                using (Graphics g = Graphics.FromImage(page))
                {
                    g.Clear(Color.White);
                    var area = new RectangleF(Margin, Margin, PageWidth - 2 * Margin, PageHeight - 2 * Margin);
                    line = DrawPage(g, area, estimate, line, pages.Count + 1);
                }
                pages.Add(page);
            }
            while (line < estimate.Lines.Count);
            return pages;
        }

        /// <summary>Saves page 1 to <paramref name="path"/>; further pages get "-2", "-3" before the extension.</summary>
        public static void SavePng(Estimate estimate, string path)
        {
            List<Bitmap> pages = RenderPages(estimate);
            try
            {
                string folder = Path.GetDirectoryName(path) ?? "";
                string name = Path.GetFileNameWithoutExtension(path);
                for (int i = 0; i < pages.Count; i++)
                {
                    string file = i == 0 ? path : Path.Combine(folder, name + "-" + (i + 1) + ".png");
                    pages[i].Save(file, ImageFormat.Png);
                }
            }
            finally
            {
                foreach (Bitmap page in pages)
                    page.Dispose();
            }
        }

        private static float[] ColumnPositions(float left, float width)
        {
            var positions = new float[Widths.Length + 1];
            positions[0] = left;
            for (int i = 0; i < Widths.Length; i++)
                positions[i + 1] = positions[i] + Widths[i] * width;
            return positions;
        }

        private static void DrawCell(Graphics g, string text, Font font, Brush brush, float x, float y, float width, float height, bool right)
        {
            using (var format = new StringFormat())
            {
                format.Alignment = right ? StringAlignment.Far : StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                g.DrawString(text, font, brush, new RectangleF(x + 4f, y, width - 8f, height), format);
            }
        }

        private static void DrawRight(Graphics g, string text, Font font, Brush brush, float right, float y)
        {
            SizeF size = g.MeasureString(text, font);
            g.DrawString(text, font, brush, right - size.Width, y);
        }
    }

    /// <summary>Prints the estimate page by page; used by PrintPreviewDialog and by the printer itself.</summary>
    public class EstimateDocument : PrintDocument
    {
        private readonly Estimate _estimate;
        private int _nextLine;
        private int _page;

        public EstimateDocument(Estimate estimate)
        {
            _estimate = estimate;
            DocumentName = "Deviz " + estimate.Patient.FullName;
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
            _nextLine = 0;
            _page = 0;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            _page++;
            _nextLine = EstimateRenderer.DrawPage(e.Graphics, e.MarginBounds, _estimate, _nextLine, _page);
            e.HasMorePages = _nextLine < _estimate.Lines.Count;
        }
    }
}
