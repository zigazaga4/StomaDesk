using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using StomaDesk.Ui;

namespace StomaDesk.Controls
{
    /// <summary>
    /// A circle with a person's initials (Text holds the full name). The colour comes from the name,
    /// so the same patient always gets the same colour, in the list and on the card.
    /// </summary>
    public class Avatar : Control
    {
        private static readonly Color[] Palette =
        {
            Color.FromArgb(15, 118, 110), Color.FromArgb(37, 99, 184), Color.FromArgb(124, 58, 237),
            Color.FromArgb(190, 24, 93), Color.FromArgb(180, 83, 9), Color.FromArgb(21, 128, 61),
            Color.FromArgb(71, 85, 105), Color.FromArgb(8, 145, 178)
        };

        public Avatar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Selectable, false);
            BackColor = Color.Transparent;
            Size = new Size(48, 48);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float side = Math.Min(Width, Height) - 1f;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            Draw(e.Graphics, new RectangleF((Width - side) / 2f, (Height - side) / 2f, side, side), Text, Font);
        }

        public static void Draw(Graphics g, RectangleF circle, string name, Font font)
        {
            Color color = ColorFor(name);
            SmoothingMode smoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var fill = new SolidBrush(GdiKit.Tint(color, 0.82f)))
                g.FillEllipse(fill, circle);
            g.SmoothingMode = smoothing;
            GdiKit.DrawText(g, Initials(name), font, GdiKit.Shade(color, 0.1f), circle, StringAlignment.Center);
        }

        /// <summary>"Barbu Gabriel" gives "BG"; a single word gives one letter.</summary>
        public static string Initials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "?";

            string[] parts = name.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            string initials = parts[0].Substring(0, 1);
            if (parts.Length > 1)
                initials += parts[1].Substring(0, 1);
            return initials.ToUpperInvariant();
        }

        public static Color ColorFor(string name)
        {
            int hash = 0;
            foreach (char c in name ?? "")
                hash = unchecked(hash * 31 + c);
            return Palette[(hash & 0x7fffffff) % Palette.Length];
        }
    }
}
