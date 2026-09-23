using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using StomaDesk.Models;
using StomaDesk.Ui;

namespace StomaDesk.Controls
{
    public class ToothEventArgs : EventArgs
    {
        public ToothEventArgs(int tooth, MouseButtons button, Point location)
        {
            Tooth = tooth;
            Button = button;
            Location = location;
        }

        public int Tooth { get; private set; }
        public MouseButtons Button { get; private set; }
        public Point Location { get; private set; }
    }

    /// <summary>
    /// Dental chart painted with GDI+ (FDI numbering). The control only shows data and raises events;
    /// the form that hosts it decides what a click changes and saves it.
    /// Arrow keys move the selection, a right click raises <see cref="ToothClicked"/> for a context menu.
    /// </summary>
    public class OdontogramControl : Control
    {
        private const float Pad = 14f;
        private const float MidGap = 18f;
        private const float NumberBand = 20f;
        private const float ArchGap = 40f;
        private const float LegendHeight = 30f;

        private static readonly Color PlannedColor = Color.FromArgb(242, 140, 40);
        private static readonly Color SelectedColor = Color.FromArgb(20, 60, 140);

        private readonly Dictionary<int, ToothState> _states = new Dictionary<int, ToothState>();
        private readonly HashSet<int> _planned = new HashSet<int>();
        private readonly Dictionary<int, RectangleF> _boxes = new Dictionary<int, RectangleF>();
        private readonly ToolTip _toolTip = new ToolTip();
        private int _hoverTooth;
        private int _selectedTooth;
        private float _chartTop;
        private float _chartBottom;

        public OdontogramControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            BackColor = Color.White;
            Size = new Size(900, 330);
        }

        public event EventHandler<ToothEventArgs> ToothClicked;
        public event EventHandler SelectedToothChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedTooth
        {
            get { return _selectedTooth; }
            set
            {
                if (_selectedTooth == value)
                    return;
                _selectedTooth = value;
                Invalidate();
                EventHandler handler = SelectedToothChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
        }

        /// <param name="records">Tooth states of the patient (missing tooth = healthy).</param>
        /// <param name="plannedTeeth">Teeth with unfinished procedures in the plan; they get an orange dot.</param>
        public void SetData(IEnumerable<ToothRecord> records, IEnumerable<int> plannedTeeth)
        {
            _states.Clear();
            foreach (ToothRecord record in records)
                _states[record.Tooth] = record.State;

            _planned.Clear();
            foreach (int tooth in plannedTeeth)
                _planned.Add(tooth);

            Invalidate();
        }

        public ToothState StateOf(int tooth)
        {
            ToothState state;
            return _states.TryGetValue(tooth, out state) ? state : ToothState.Healthy;
        }

        public static Color ColorFor(ToothState state)
        {
            switch (state)
            {
                case ToothState.Caries: return Color.FromArgb(214, 69, 65);
                case ToothState.Filling: return Color.FromArgb(52, 120, 200);
                case ToothState.RootCanal: return Color.FromArgb(142, 68, 173);
                case ToothState.Crown: return Color.FromArgb(214, 160, 20);
                case ToothState.Implant: return Color.FromArgb(39, 150, 96);
                case ToothState.Extracted: return Color.FromArgb(140, 140, 140);
                default: return Color.FromArgb(160, 160, 160);
            }
        }

        /// <summary>The tooth under a point, or 0.</summary>
        public int HitTest(Point point)
        {
            foreach (KeyValuePair<int, RectangleF> box in _boxes)
            {
                if (box.Value.Contains(point))
                    return box.Key;
            }
            return 0;
        }

        /// <summary>Paints the whole chart on any Graphics: the screen, a bitmap or a printer page.</summary>
        public void RenderTo(Graphics g)
        {
            ComputeLayout();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            using (var numberFont = new Font(Font.FontFamily, 8.5f))
            using (var selectedFont = new Font(Font.FontFamily, 8.5f, FontStyle.Bold))
            using (var centered = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var midline = new Pen(Color.FromArgb(200, 200, 200), 1f) { DashStyle = DashStyle.Dash })
            {
                float centerX = Width / 2f;
                g.DrawLine(midline, centerX, _chartTop, centerX, _chartBottom);

                foreach (KeyValuePair<int, RectangleF> pair in _boxes)
                {
                    int tooth = pair.Key;
                    RectangleF box = pair.Value;
                    bool selected = tooth == _selectedTooth;

                    DrawTooth(g, box, StateOf(tooth), tooth == _hoverTooth, selected);
                    if (_planned.Contains(tooth))
                        DrawPlannedMark(g, box);

                    RectangleF numberRect = Dentition.IsUpper(tooth)
                        ? new RectangleF(box.X - 4f, box.Y - NumberBand, box.Width + 8f, NumberBand)
                        : new RectangleF(box.X - 4f, box.Bottom, box.Width + 8f, NumberBand);
                    g.DrawString(tooth.ToString(), selected ? selectedFont : numberFont,
                        selected ? Brushes.Navy : Brushes.DimGray, numberRect, centered);
                }

                DrawLegend(g, numberFont);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            RenderTo(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ComputeLayout();
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int tooth = HitTest(e.Location);
            if (tooth == _hoverTooth)
                return;

            _hoverTooth = tooth;
            Cursor = tooth == 0 ? Cursors.Default : Cursors.Hand;
            _toolTip.SetToolTip(this, tooth == 0 ? null : string.Format("Dinte {0}: {1}", tooth, Labels.For(StateOf(tooth))));
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverTooth == 0)
                return;
            _hoverTooth = 0;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            int tooth = HitTest(e.Location);
            if (tooth == 0)
                return;

            SelectedTooth = tooth;
            EventHandler<ToothEventArgs> handler = ToothClicked;
            if (handler != null)
                handler(this, new ToothEventArgs(tooth, e.Button, e.Location));
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            IList<int> row = Dentition.IsUpper(_selectedTooth) ? Dentition.Upper : Dentition.Lower;
            int index = row.IndexOf(_selectedTooth);
            if (index < 0)
            {
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                    SelectedTooth = Dentition.Upper[0];
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Left:
                    SelectedTooth = row[Math.Max(0, index - 1)];
                    break;
                case Keys.Right:
                    SelectedTooth = row[Math.Min(row.Count - 1, index + 1)];
                    break;
                case Keys.Up:
                    SelectedTooth = Dentition.Upper[index];
                    break;
                case Keys.Down:
                    SelectedTooth = Dentition.Lower[index];
                    break;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _toolTip.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>Sizes the teeth to the control and centres the chart in the space above the legend.</summary>
        private void ComputeLayout()
        {
            _boxes.Clear();
            float cell = Math.Max(12f, (Width - 2f * Pad - MidGap) / 16f);
            float toothWidth = cell - 6f;
            float area = Height - 2f * Pad - LegendHeight;
            float toothHeight = Math.Max(16f, Math.Min(toothWidth * 1.35f, (area - 2f * NumberBand - ArchGap) / 2f));

            float chartHeight = 2f * NumberBand + 2f * toothHeight + ArchGap;
            _chartTop = Pad + Math.Max(0f, (area - chartHeight) / 2f);
            _chartBottom = _chartTop + chartHeight;

            float left = (Width - (cell * 16f + MidGap)) / 2f;
            float upperTop = _chartTop + NumberBand;
            float lowerTop = upperTop + toothHeight + ArchGap;
            Place(Dentition.Upper, left, upperTop, cell, toothWidth, toothHeight);
            Place(Dentition.Lower, left, lowerTop, cell, toothWidth, toothHeight);
        }

        private void Place(IList<int> row, float left, float top, float cell, float toothWidth, float toothHeight)
        {
            for (int i = 0; i < row.Count; i++)
            {
                float x = left + i * cell + (i >= 8 ? MidGap : 0f) + (cell - toothWidth) / 2f;
                _boxes[row[i]] = new RectangleF(x, top, toothWidth, toothHeight);
            }
        }

        private static void DrawTooth(Graphics g, RectangleF box, ToothState state, bool hover, bool selected)
        {
            Color fill = state == ToothState.Healthy ? Color.FromArgb(252, 252, 249) : GdiKit.Tint(ColorFor(state), 0.72f);
            if (hover)
                fill = GdiKit.Blend(fill, Color.FromArgb(120, 170, 230), 0.25f);

            Color border = selected ? SelectedColor : ColorFor(state);
            float borderWidth = selected ? 3f : state == ToothState.Crown ? 3f : 1.4f;

            using (GraphicsPath path = GdiKit.RoundedRect(box, Math.Min(box.Width, box.Height) * 0.3f))
            using (var brush = new SolidBrush(fill))
            using (var pen = new Pen(border, borderWidth))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            DrawSymbol(g, box, state);
        }

        /// <summary>A shape per state, so the chart still reads on a black and white printout.</summary>
        private static void DrawSymbol(Graphics g, RectangleF box, ToothState state)
        {
            float cx = box.X + box.Width / 2f;
            float cy = box.Y + box.Height / 2f;
            float w = box.Width;
            float h = box.Height;
            Color color = ColorFor(state);

            switch (state)
            {
                case ToothState.Caries:
                    using (var brush = new SolidBrush(color))
                        g.FillEllipse(brush, cx - w * 0.2f, cy - w * 0.2f, w * 0.4f, w * 0.4f);
                    break;

                case ToothState.Filling:
                    using (var brush = new SolidBrush(color))
                        g.FillRectangle(brush, cx - w * 0.24f, cy - h * 0.16f, w * 0.48f, h * 0.32f);
                    break;

                case ToothState.RootCanal:
                    using (var pen = new Pen(color, 3f))
                    {
                        g.DrawLine(pen, cx, box.Y + h * 0.18f, cx - w * 0.12f, box.Bottom - h * 0.12f);
                        g.DrawLine(pen, cx, box.Y + h * 0.18f, cx + w * 0.12f, box.Bottom - h * 0.12f);
                    }
                    break;

                case ToothState.Crown:
                    using (var pen = new Pen(color, 2f))
                        g.DrawLine(pen, box.X + w * 0.15f, box.Y + h * 0.3f, box.Right - w * 0.15f, box.Y + h * 0.3f);
                    break;

                case ToothState.Implant:
                    using (var pen = new Pen(color, 2f))
                    {
                        g.DrawLine(pen, cx, box.Y + h * 0.2f, cx, box.Bottom - h * 0.12f);
                        for (int i = 0; i < 4; i++)
                        {
                            float y = box.Y + h * (0.3f + i * 0.15f);
                            g.DrawLine(pen, cx - w * 0.2f, y, cx + w * 0.2f, y + h * 0.04f);
                        }
                    }
                    break;

                case ToothState.Extracted:
                    using (var pen = new Pen(Color.FromArgb(200, 55, 45), 2.5f))
                    {
                        g.DrawLine(pen, box.X + 4f, box.Y + 4f, box.Right - 4f, box.Bottom - 4f);
                        g.DrawLine(pen, box.Right - 4f, box.Y + 4f, box.X + 4f, box.Bottom - 4f);
                    }
                    break;
            }
        }

        private static void DrawPlannedMark(Graphics g, RectangleF box)
        {
            using (var brush = new SolidBrush(PlannedColor))
                g.FillEllipse(brush, box.Right - 7f, box.Y - 4f, 10f, 10f);
        }

        private void DrawLegend(Graphics g, Font font)
        {
            float x = Pad;
            float y = Height - Pad - 14f;

            foreach (ToothState state in Enum.GetValues(typeof(ToothState)))
            {
                string label = Labels.For(state);
                Color fill = state == ToothState.Healthy ? Color.White : GdiKit.Tint(ColorFor(state), 0.72f);
                using (var brush = new SolidBrush(fill))
                using (var pen = new Pen(ColorFor(state), 1.4f))
                {
                    g.FillRectangle(brush, x, y, 13f, 13f);
                    g.DrawRectangle(pen, x, y, 13f, 13f);
                }
                g.DrawString(label, font, Brushes.DimGray, x + 17f, y - 1f);
                x += 17f + g.MeasureString(label, font).Width + 12f;
            }

            using (var brush = new SolidBrush(PlannedColor))
                g.FillEllipse(brush, x, y + 1f, 11f, 11f);
            g.DrawString("procedură planificată", font, Brushes.DimGray, x + 15f, y - 1f);
        }
    }
}
