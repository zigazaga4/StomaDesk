using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using StomaDesk.Ui;

namespace StomaDesk.Controls
{
    /// <summary>
    /// White rounded panel with an optional title, used to group fields; it replaces the grey GroupBox.
    /// Docked children start below the title strip.
    /// </summary>
    public class Card : Panel
    {
        private const float Radius = 8f;
        private const int TitleHeight = 34;
        private Padding _gap = Padding.Empty;

        public Card()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Surface;
        }

        /// <summary>Empty space kept inside the bounds around the card, so docked cards do not touch.</summary>
        [DefaultValue(typeof(Padding), "0, 0, 0, 0")]
        public Padding Gap
        {
            get { return _gap; }
            set { _gap = value; PerformLayout(); Invalidate(); }
        }

        /// <summary>The title drawn in the top strip.</summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; PerformLayout(); Invalidate(); }
        }

        /// <summary>
        /// Starts from the base rectangle (which Mono already shrinks by Padding, while .NET applies Padding
        /// later in the layout engine), so the card works the same on both.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle r = base.DisplayRectangle;
                int top = _gap.Top + (string.IsNullOrEmpty(Text) ? 0 : TitleHeight);
                return new Rectangle(r.X + _gap.Left + 1, r.Y + top, r.Width - _gap.Horizontal - 2, r.Height - top - _gap.Bottom - 1);
            }
        }

        private RectangleF Frame
        {
            get
            {
                return new RectangleF(_gap.Left, _gap.Top, Width - _gap.Horizontal - 1f, Height - _gap.Vertical - 1f);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent == null ? Theme.Canvas : Parent.BackColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            RectangleF frame = Frame;
            if (frame.Width <= 0f || frame.Height <= 0f)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = GdiKit.RoundedRect(frame, Radius))
            using (var fill = new SolidBrush(BackColor))
            using (var border = new Pen(Theme.Line))
            {
                g.FillPath(fill, path);
                g.DrawPath(border, path);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                GdiKit.DrawText(g, Text, Theme.UiFont(10f, FontStyle.Bold), Theme.Ink,
                    new RectangleF(frame.X + 14f, frame.Y + 4f, frame.Width - 28f, TitleHeight - 6f));
            }

            base.OnPaint(e);   // raises Paint, which draws the frames of the text boxes inside
        }
    }
}
