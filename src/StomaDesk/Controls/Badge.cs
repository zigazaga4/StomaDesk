using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using StomaDesk.Ui;

namespace StomaDesk.Controls
{
    /// <summary>
    /// A label drawn as a coloured pill. ForeColor picks the colour; the background is a light tint of it.
    /// The pill hugs the text and follows TextAlign, so a wide right-aligned badge keeps its pill on the right.
    /// </summary>
    public class Badge : Label
    {
        public Badge()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Padding = new Padding(9, 3, 9, 3);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                return;

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            SizeF text = g.MeasureString(Text, Font);
            float width = Math.Min(ClientSize.Width, text.Width + Padding.Horizontal);
            float height = Math.Min(ClientSize.Height, text.Height + Padding.Vertical);

            float x;
            if (IsRight(TextAlign))
                x = ClientSize.Width - width;
            else if (IsCenter(TextAlign))
                x = (ClientSize.Width - width) / 2f;
            else
                x = 0f;

            GdiKit.DrawPill(g, Text, Font, ForeColor, new RectangleF(x, (ClientSize.Height - height) / 2f, width, height));
        }

        private static bool IsRight(ContentAlignment alignment)
        {
            return alignment == ContentAlignment.TopRight || alignment == ContentAlignment.MiddleRight || alignment == ContentAlignment.BottomRight;
        }

        private static bool IsCenter(ContentAlignment alignment)
        {
            return alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.MiddleCenter || alignment == ContentAlignment.BottomCenter;
        }
    }
}
