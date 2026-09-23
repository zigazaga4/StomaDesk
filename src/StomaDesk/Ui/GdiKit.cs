using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace StomaDesk.Ui
{
    /// <summary>Small GDI+ helpers shared by the dental chart and the agenda.</summary>
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

        public static GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2f, Math.Min(r.Width, r.Height));
            if (d <= 0f)
            {
                path.AddRectangle(r);
                return path;
            }

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static int Mix(int from, int to, float amount)
        {
            return (int)Math.Round(from + (to - from) * amount);
        }
    }
}
