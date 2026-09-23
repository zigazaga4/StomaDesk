using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using StomaDesk.Ui;

namespace StomaDesk.Controls
{
    public enum NavBarStyle
    {
        /// <summary>Dark vertical bar with the logo, for the main window.</summary>
        Sidebar,

        /// <summary>Light horizontal strip with an underlined current item, for dialogs.</summary>
        Tabs
    }

    public sealed class NavItem
    {
        internal NavItem(string text, Glyph glyph, Control page, string description)
        {
            Text = text;
            Glyph = glyph;
            Page = page;
            Description = description ?? "";
        }

        public string Text { get; private set; }
        public Glyph Glyph { get; private set; }
        public Control Page { get; private set; }
        public string Description { get; private set; }

        /// <summary>Key hint shown on the right in the sidebar, for example "Ctrl+1".</summary>
        public string Shortcut { get; set; }

        internal RectangleF Bounds { get; set; }
    }

    /// <summary>
    /// Navigation drawn with GDI+. Each item owns a page (any control docked in a shared container);
    /// selecting an item shows its page and hides the others. It takes the place of TabControl,
    /// whose header strip cannot be restyled under Mono.
    /// </summary>
    public class NavBar : Control
    {
        private const float SidebarItemsTop = 104f;
        private const float SidebarItemHeight = 44f;

        private readonly List<NavItem> _items = new List<NavItem>();
        private NavBarStyle _barStyle = NavBarStyle.Sidebar;
        private string _title = "StomaDesk";
        private string _subtitle = "";
        private string _footer = "";
        private int _selected = -1;
        private int _hover = -1;

        public event EventHandler SelectedIndexChanged;

        public NavBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            Size = new Size(210, 400);
        }

        [DefaultValue(NavBarStyle.Sidebar)]
        public NavBarStyle BarStyle
        {
            get { return _barStyle; }
            set { _barStyle = value; LayoutItems(); Invalidate(); }
        }

        [DefaultValue("StomaDesk")]
        public string Title
        {
            get { return _title; }
            set { _title = value ?? ""; Invalidate(); }
        }

        [DefaultValue("")]
        public string Subtitle
        {
            get { return _subtitle; }
            set { _subtitle = value ?? ""; Invalidate(); }
        }

        [DefaultValue("")]
        public string Footer
        {
            get { return _footer; }
            set { _footer = value ?? ""; Invalidate(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ReadOnlyCollection<NavItem> Items
        {
            get { return _items.AsReadOnly(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get { return _selected; }
            set { SelectItem(value); }
        }

        [Browsable(false)]
        public NavItem SelectedItem
        {
            get { return _selected >= 0 ? _items[_selected] : null; }
        }

        public NavItem AddPage(string text, Glyph glyph, Control page, string description)
        {
            var item = new NavItem(text, glyph, page, description);
            _items.Add(item);
            page.Visible = false;
            LayoutItems();
            if (_selected < 0)
                SelectItem(0);
            Invalidate();
            return item;
        }

        public void ShowPage(Control page)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Page == page)
                {
                    SelectItem(i);
                    return;
                }
            }
        }

        public void SelectItem(int index)
        {
            if (index < 0 || index >= _items.Count || index == _selected)
                return;

            _selected = index;
            // Show the new page before hiding the others, so the container never shows empty.
            _items[index].Page.Visible = true;
            for (int i = 0; i < _items.Count; i++)
            {
                if (i != index)
                    _items[i].Page.Visible = false;
            }

            Invalidate();
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, EventArgs.Empty);
        }

        // ---------------------------------------------------------------- layout and mouse

        private Font ItemFont
        {
            get { return Theme.UiFont(10f); }
        }

        private Font SelectedFont
        {
            get { return Theme.UiFont(10f, FontStyle.Bold); }
        }

        private void LayoutItems()
        {
            if (_barStyle == NavBarStyle.Sidebar)
            {
                float y = SidebarItemsTop;
                foreach (NavItem item in _items)
                {
                    item.Bounds = new RectangleF(10f, y, Width - 20f, SidebarItemHeight - 4f);
                    y += SidebarItemHeight;
                }
                return;
            }

            // Tabs are measured in bold, so the current one does not change width when it turns bold.
            float x = 12f;
            foreach (NavItem item in _items)
            {
                float width = 42f + TextRenderer.MeasureText(item.Text, SelectedFont).Width + 16f;
                item.Bounds = new RectangleF(x, 0f, width, Height);
                x += width + 4f;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutItems();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int hit = HitTest(e.Location);
            if (hit == _hover)
                return;
            _hover = hit;
            Cursor = hit >= 0 ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = -1;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left)
                return;
            int hit = HitTest(e.Location);
            if (hit >= 0)
                SelectItem(hit);
        }

        private int HitTest(Point point)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Bounds.Contains(point))
                    return i;
            }
            return -1;
        }

        // ---------------------------------------------------------------- painting

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            if (_barStyle == NavBarStyle.Sidebar)
                PaintSidebar(g);
            else
                PaintTabs(g);
        }

        private void PaintSidebar(Graphics g)
        {
            Rectangle area = ClientRectangle;
            using (var back = new LinearGradientBrush(area, Theme.SidebarTop, Theme.SidebarBottom, 90f))
                g.FillRectangle(back, area);

            Glyphs.DrawLogo(g, new RectangleF(18f, 20f, 38f, 38f));
            GdiKit.DrawText(g, _title, Theme.UiFont(13f, FontStyle.Bold), Color.White, new RectangleF(64f, 17f, Width - 72f, 24f));
            GdiKit.DrawText(g, _subtitle, Theme.UiFont(8.25f), White(175), new RectangleF(65f, 39f, Width - 73f, 20f));

            using (var divider = new Pen(White(30)))
                g.DrawLine(divider, 16f, 74f, Width - 16f, 74f);
            GdiKit.DrawText(g, "MENIU", Theme.UiFont(7.5f, FontStyle.Bold), White(115), new RectangleF(21f, 80f, Width - 40f, 20f));

            for (int i = 0; i < _items.Count; i++)
            {
                NavItem item = _items[i];
                RectangleF b = item.Bounds;
                bool selected = i == _selected;

                if (selected || i == _hover)
                {
                    using (GraphicsPath back = GdiKit.RoundedRect(b, 8f))
                    using (var fill = new SolidBrush(White(selected ? 36 : 16)))
                        g.FillPath(fill, back);
                }
                if (selected)
                {
                    using (GraphicsPath bar = GdiKit.RoundedRect(new RectangleF(b.X, b.Y + 10f, 3.5f, b.Height - 20f), 1.75f))
                    using (var accent = new SolidBrush(Theme.Accent))
                        g.FillPath(accent, bar);
                }

                Color text = selected ? Color.White : White(210);
                Glyphs.Draw(g, item.Glyph, new RectangleF(b.X + 16f, b.Y + (b.Height - 19f) / 2f, 19f, 19f), selected ? Theme.Accent : text);
                GdiKit.DrawText(g, item.Text, selected ? SelectedFont : ItemFont, text, new RectangleF(b.X + 46f, b.Y, b.Width - 98f, b.Height));
                if (!string.IsNullOrEmpty(item.Shortcut))
                    GdiKit.DrawText(g, item.Shortcut, Theme.UiFont(7.5f), White(105), new RectangleF(b.Right - 56f, b.Y, 46f, b.Height), StringAlignment.Far);
            }

            GdiKit.DrawText(g, _footer, Theme.UiFont(8.25f), White(150), new RectangleF(20f, Height - 42f, Width - 40f, 22f));
        }

        private void PaintTabs(Graphics g)
        {
            g.Clear(Theme.Surface);
            using (var line = new Pen(Theme.Line))
                g.DrawLine(line, 0, Height - 1, Width, Height - 1);

            for (int i = 0; i < _items.Count; i++)
            {
                NavItem item = _items[i];
                RectangleF b = item.Bounds;
                bool selected = i == _selected;
                Color color = selected ? Theme.Brand : i == _hover ? Theme.Ink : Theme.Muted;

                Glyphs.Draw(g, item.Glyph, new RectangleF(b.X + 14f, (Height - 18f) / 2f, 18f, 18f), color);
                GdiKit.DrawText(g, item.Text, selected ? SelectedFont : ItemFont, color, new RectangleF(b.X + 40f, 0f, b.Width - 48f, Height));
                if (selected)
                {
                    using (GraphicsPath bar = GdiKit.RoundedRect(new RectangleF(b.X + 8f, Height - 3f, b.Width - 16f, 3f), 1.5f))
                    using (var fill = new SolidBrush(Theme.Brand))
                        g.FillPath(fill, bar);
                }
            }
        }

        private static Color White(int alpha)
        {
            return Color.FromArgb(alpha, 255, 255, 255);
        }
    }
}
