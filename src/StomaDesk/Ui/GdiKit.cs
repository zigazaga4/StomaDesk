using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace StomaDesk.Ui
{
    /// <summary>Small GDI+ helpers shared by the custom-drawn controls, the agenda and the grids.</summary>
    public static class GdiKit
    {
        /// <summary>Mixes two colours: 0 gives <paramref name="from"/>, 1 gives <paramref name="to"/>.</summary>
        public static Color Blend(Color from, Color to, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            return Color.FromArgb(Mix(from.R, to.R, amount), Mix(from.G, to.G, amount), Mix(from.B, to.B, amount));
        }

        /// <summary>A lighter version of the colour: 0 keeps it, 1 gives white.</summary>
        public static Color Tint(Color color, float amount)
        {
            return Blend(color, Color.White, amount);
        }

        /// <summary>A darker version of the colour: 0 keeps it, 1 gives black.</summary>
        public static Color Shade(Color color, float amount)
        {
            return Blend(color, Color.Black, amount);
        }

        /// <summary>
        /// Rectangle with rounded corners. Turning off the top or bottom corners lets a shape continue
        /// across several cells (an appointment spanning several agenda slots).
        /// </summary>
        public static GraphicsPath RoundedRect(RectangleF r, float radius, bool roundTop = true, bool roundBottom = true)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2f, Math.Min(r.Width, r.Height));
            if (d <= 0f)
            {
                path.AddRectangle(r);
                return path;
            }

            if (roundTop)
            {
                path.AddArc(r.X, r.Y, d, d, 180, 90);
                path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            }
            else
            {
                path.AddLine(r.X, r.Y, r.Right, r.Y);
            }

            if (roundBottom)
            {
                path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            }
            else
            {
                path.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
            }

            path.CloseFigure();
            return path;
        }

        /// <summary>One line of text, vertically centred in the box, cut with "…" when it does not fit.</summary>
        public static void DrawText(Graphics g, string text, Font font, Color color, RectangleF box,
            StringAlignment alignment = StringAlignment.Near)
        {
            if (string.IsNullOrEmpty(text) || box.Width <= 0f || box.Height <= 0f)
                return;

            using (var format = new StringFormat(StringFormatFlags.NoWrap))
            using (var brush = new SolidBrush(color))
            {
                format.Alignment = alignment;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(text, font, brush, box, format);
            }
        }

        /// <summary>Size of a pill drawn by <see cref="DrawPill"/> around the text.</summary>
        public static SizeF PillSize(Graphics g, string text, Font font)
        {
            SizeF size = g.MeasureString(text, font);
            return new SizeF(size.Width + 12f, size.Height + 3f);
        }

        /// <summary>Status pill: a light tint of the colour behind text in a darker shade of it.</summary>
        public static void DrawPill(Graphics g, string text, Font font, Color tone, RectangleF pill)
        {
            SmoothingMode smoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = RoundedRect(pill, pill.Height / 2f))
            using (var fill = new SolidBrush(Tint(tone, 0.86f)))
                g.FillPath(fill, path);
            g.SmoothingMode = smoothing;
            DrawText(g, text, font, Shade(tone, 0.12f), pill, StringAlignment.Center);
        }

        private static int Mix(int from, int to, float amount)
        {
            return (int)Math.Round(from + (to - from) * amount);
        }
    }
}
