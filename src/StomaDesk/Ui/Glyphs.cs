using System.Drawing;
using System.Drawing.Drawing2D;

namespace StomaDesk.Ui
{
    public enum Glyph
    {
        None,
        Tooth,
        Patients,
        Agenda,
        Reports,
        Settings,
        Plan,
        Clock,
        Payments
    }

    /// <summary>
    /// Line icons drawn with GDI+ on a 24 x 24 grid (the grid most icon sets use), so they stay sharp at
    /// any size, take any colour and need no image files.
    /// </summary>
    public static class Glyphs
    {
        public static void Draw(Graphics g, Glyph glyph, RectangleF box, Color color)
        {
            if (glyph == Glyph.None)
                return;

            GraphicsState state = g.Save();
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TranslateTransform(box.X, box.Y);
                g.ScaleTransform(box.Width / 24f, box.Height / 24f);
                using (var pen = new Pen(color, 1.9f) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                using (GraphicsPath path = Outline(glyph))
                    g.DrawPath(pen, path);
            }
            finally
            {
                g.Restore(state);
            }
        }

        /// <summary>The app logo: a white tooth on a rounded teal tile.</summary>
        public static void DrawLogo(Graphics g, RectangleF r)
        {
            GraphicsState state = g.Save();
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath tile = GdiKit.RoundedRect(r, r.Width * 0.24f))
                using (var fill = new LinearGradientBrush(r, Color.FromArgb(38, 184, 164), Theme.Brand, 60f))
                    g.FillPath(fill, tile);

                float inset = r.Width * 0.2f;
                using (var tooth = new GraphicsPath())
                {
                    AddTooth(tooth, new RectangleF(r.X + inset, r.Y + inset * 0.9f, r.Width - 2f * inset, r.Height - 2f * inset));
                    g.FillPath(Brushes.White, tooth);
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        /// <summary>Outline of a molar (two cusps, two roots) fitted to <paramref name="r"/>.</summary>
        public static void AddTooth(GraphicsPath path, RectangleF r)
        {
            path.StartFigure();
            path.AddBezier(P(r, .50f, .17f), P(r, .40f, .05f), P(r, .18f, .03f), P(r, .12f, .24f));
            path.AddBezier(P(r, .12f, .24f), P(r, .07f, .43f), P(r, .20f, .55f), P(r, .24f, .70f));
            path.AddBezier(P(r, .24f, .70f), P(r, .27f, .84f), P(r, .28f, .97f), P(r, .36f, .97f));
            path.AddBezier(P(r, .36f, .97f), P(r, .43f, .97f), P(r, .42f, .72f), P(r, .50f, .68f));
            path.AddBezier(P(r, .50f, .68f), P(r, .58f, .72f), P(r, .57f, .97f), P(r, .64f, .97f));
            path.AddBezier(P(r, .64f, .97f), P(r, .72f, .97f), P(r, .73f, .84f), P(r, .76f, .70f));
            path.AddBezier(P(r, .76f, .70f), P(r, .80f, .55f), P(r, .93f, .43f), P(r, .88f, .24f));
            path.AddBezier(P(r, .88f, .24f), P(r, .82f, .03f), P(r, .60f, .05f), P(r, .50f, .17f));
            path.CloseFigure();
        }

        private static GraphicsPath Outline(Glyph glyph)
        {
            var p = new GraphicsPath();
            switch (glyph)
            {
                case Glyph.Tooth:
                    AddTooth(p, new RectangleF(3.5f, 2.5f, 17f, 19f));
                    break;

                case Glyph.Patients:
                    p.AddEllipse(8f, 3f, 8f, 8f);
                    p.StartFigure();
                    p.AddLine(5f, 21f, 5f, 19f);
                    p.AddArc(5f, 15f, 8f, 8f, 180f, 90f);
                    p.AddLine(9f, 15f, 15f, 15f);
                    p.AddArc(11f, 15f, 8f, 8f, 270f, 90f);
                    p.AddLine(19f, 19f, 19f, 21f);
                    break;

                case Glyph.Agenda:
                    AddRounded(p, 3f, 4.5f, 18f, 17f, 2.5f);
                    Segment(p, 3f, 10f, 21f, 10f);
                    Segment(p, 8f, 2.5f, 8f, 6.5f);
                    Segment(p, 16f, 2.5f, 16f, 6.5f);
                    Segment(p, 7.6f, 14.8f, 8.4f, 14.8f);
                    Segment(p, 11.6f, 14.8f, 12.4f, 14.8f);
                    Segment(p, 15.6f, 14.8f, 16.4f, 14.8f);
                    break;

                case Glyph.Reports:
                    p.AddLine(3f, 3f, 3f, 21f);
                    p.AddLine(3f, 21f, 21f, 21f);
                    Segment(p, 8f, 17f, 8f, 13f);
                    Segment(p, 13f, 17f, 13f, 7f);
                    Segment(p, 18f, 17f, 18f, 10f);
                    break;

                case Glyph.Settings:
                    Segment(p, 3f, 6f, 10f, 6f);
                    p.AddEllipse(10f, 4f, 4f, 4f);
                    Segment(p, 14f, 6f, 21f, 6f);
                    Segment(p, 3f, 12f, 5f, 12f);
                    p.AddEllipse(5f, 10f, 4f, 4f);
                    Segment(p, 9f, 12f, 21f, 12f);
                    Segment(p, 3f, 18f, 14f, 18f);
                    p.AddEllipse(14f, 16f, 4f, 4f);
                    Segment(p, 18f, 18f, 21f, 18f);
                    break;

                case Glyph.Plan:
                    p.StartFigure();
                    p.AddLines(new[] { new PointF(3f, 6.5f), new PointF(5f, 8.5f), new PointF(9f, 4.5f) });
                    p.StartFigure();
                    p.AddLines(new[] { new PointF(3f, 16.5f), new PointF(5f, 18.5f), new PointF(9f, 14.5f) });
                    Segment(p, 13f, 6.5f, 21f, 6.5f);
                    Segment(p, 13f, 12f, 21f, 12f);
                    Segment(p, 13f, 17.5f, 21f, 17.5f);
                    break;

                case Glyph.Clock:
                    p.AddEllipse(2.5f, 2.5f, 19f, 19f);
                    p.StartFigure();
                    p.AddLine(12f, 6.5f, 12f, 12f);
                    p.AddLine(12f, 12f, 15.5f, 14f);
                    break;

                case Glyph.Payments:
                    AddRounded(p, 2f, 5f, 20f, 14f, 2.5f);
                    Segment(p, 2f, 10f, 22f, 10f);
                    Segment(p, 6f, 15f, 10f, 15f);
                    break;
            }
            return p;
        }

        private static void Segment(GraphicsPath p, float x1, float y1, float x2, float y2)
        {
            p.StartFigure();
            p.AddLine(x1, y1, x2, y2);
        }

        private static void AddRounded(GraphicsPath p, float x, float y, float width, float height, float radius)
        {
            p.StartFigure();
            using (GraphicsPath rounded = GdiKit.RoundedRect(new RectangleF(x, y, width, height), radius))
                p.AddPath(rounded, false);
        }

        private static PointF P(RectangleF r, float x, float y)
        {
            return new PointF(r.X + r.Width * x, r.Y + r.Height * y);
        }
    }
}
