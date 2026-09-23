using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using StomaDesk.Models;

namespace StomaDesk.Ui
{
    public enum ButtonKind
    {
        Secondary,
        Primary,
        Danger
    }

    /// <summary>
    /// The look of the whole app in one place: palette, fonts, and how the standard controls are styled.
    /// Each form calls <see cref="Apply"/> after InitializeComponent, so the Designer files keep plain
    /// WinForms defaults and a colour is changed here once, not in forty property grids.
    /// </summary>
    public static class Theme
    {
        // Brand: a clinical teal, with a mint accent for the dark sidebar.
        public static readonly Color Brand = Color.FromArgb(15, 118, 110);
        public static readonly Color BrandHover = Color.FromArgb(17, 94, 89);
        public static readonly Color BrandPressed = Color.FromArgb(19, 78, 74);
        public static readonly Color BrandLight = Color.FromArgb(218, 241, 237);
        public static readonly Color Accent = Color.FromArgb(94, 234, 212);
        public static readonly Color SidebarTop = Color.FromArgb(17, 78, 75);
        public static readonly Color SidebarBottom = Color.FromArgb(10, 45, 44);

        // Neutrals.
        public static readonly Color Canvas = Color.FromArgb(242, 245, 246);
        public static readonly Color Surface = Color.White;
        public static readonly Color SurfaceAlt = Color.FromArgb(248, 250, 251);
        public static readonly Color Line = Color.FromArgb(223, 229, 232);
        public static readonly Color LineStrong = Color.FromArgb(200, 210, 215);
        public static readonly Color Ink = Color.FromArgb(28, 39, 48);
        public static readonly Color Muted = Color.FromArgb(99, 113, 124);

        // Meaning.
        public static readonly Color Danger = Color.FromArgb(192, 57, 43);
        public static readonly Color Success = Color.FromArgb(22, 128, 80);
        public static readonly Color Warning = Color.FromArgb(180, 110, 10);
        public static readonly Color Info = Color.FromArgb(37, 99, 184);

        private const string FontName = "Segoe UI";
        private static readonly Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
        private static readonly ConditionalWeakTable<Button, StrongBox<ButtonKind>> ButtonKinds =
            new ConditionalWeakTable<Button, StrongBox<ButtonKind>>();
        private static Icon _appIcon;

        /// <summary>Renderer for every menu, context menu and status bar (set once on ToolStripManager).</summary>
        public static readonly ToolStripRenderer MenuRenderer = new ToolStripProfessionalRenderer(new MenuColors()) { RoundedEdges = false };

        /// <summary>The tooth logo as a window icon, drawn in code, so the repository needs no .ico file.</summary>
        public static Icon AppIcon
        {
            get
            {
                if (_appIcon == null)
                    _appIcon = IconMaker.Create(Glyphs.DrawLogo, 16, 24, 32, 48, 64);
                return _appIcon;
            }
        }

        /// <summary>A shared font in the app's typeface. Fonts are cached for the life of the app: do not dispose them.</summary>
        public static Font UiFont(float size, FontStyle style = FontStyle.Regular)
        {
            string key = size.ToString(CultureInfo.InvariantCulture) + "/" + (int)style;
            Font font;
            if (!Fonts.TryGetValue(key, out font))
            {
                font = new Font(FontName, size, style);
                Fonts.Add(key, font);
            }
            return font;
        }

        public static Color Tone(AppointmentStatus status)
        {
            switch (status)
            {
                case AppointmentStatus.Confirmed: return Info;
                case AppointmentStatus.Arrived: return Warning;
                case AppointmentStatus.Done: return Success;
                case AppointmentStatus.NoShow: return Danger;
                default: return Muted;
            }
        }

        public static Color Tone(TreatmentStatus status)
        {
            switch (status)
            {
                case TreatmentStatus.Accepted: return Info;
                case TreatmentStatus.InProgress: return Warning;
                case TreatmentStatus.Done: return Success;
                default: return Muted;
            }
        }

        // ---------------------------------------------------------------- styling the control tree

        /// <summary>Styles a form or control and everything inside it. Buttons already given a kind keep it.</summary>
        public static void Apply(Control root)
        {
            StyleControl(root);
            foreach (Control child in root.Controls)
                Apply(child);
        }

        public static void Primary(params Button[] buttons)
        {
            Style(ButtonKind.Primary, buttons);
        }

        /// <summary>Red text and border, for buttons that delete something.</summary>
        public static void Destructive(params Button[] buttons)
        {
            Style(ButtonKind.Danger, buttons);
        }

        public static void Style(ButtonKind kind, params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                StrongBox<ButtonKind> box;
                if (ButtonKinds.TryGetValue(button, out box))
                {
                    box.Value = kind;
                }
                else
                {
                    ButtonKinds.Add(button, new StrongBox<ButtonKind>(kind));
                    button.EnabledChanged += Button_EnabledChanged;
                }

                button.FlatStyle = FlatStyle.Flat;
                button.UseVisualStyleBackColor = false;
                button.Cursor = Cursors.Hand;
                button.FlatAppearance.BorderSize = 1;
                Recolor(button, kind);
            }
        }

        private static void StyleControl(Control control)
        {
            var form = control as Form;
            if (form != null)
            {
                form.BackColor = Canvas;
                form.ForeColor = Ink;
                form.Icon = AppIcon;
                return;
            }

            var button = control as Button;
            if (button != null)
            {
                StrongBox<ButtonKind> box;
                if (!ButtonKinds.TryGetValue(button, out box))
                    Style(ButtonKind.Secondary, button);
                return;
            }

            var label = control as Label;
            if (label != null)
            {
                if (label.ForeColor == Color.DimGray)
                    label.ForeColor = Muted;
                return;
            }

            var combo = control as ComboBox;
            if (combo != null)
            {
                combo.FlatStyle = FlatStyle.Flat;
                return;
            }

            var upDown = control as UpDownBase;
            if (upDown != null)
            {
                if (upDown.BorderStyle != BorderStyle.None)
                {
                    upDown.BorderStyle = BorderStyle.None;
                    Frame(upDown, false);
                }
                return;
            }

            var text = control as TextBox;
            if (text != null)
            {
                if (text.BorderStyle != BorderStyle.None)
                {
                    text.BorderStyle = BorderStyle.None;
                    Frame(text, text.Multiline);
                }
                return;
            }

            var strip = control as ToolStrip;
            if (strip != null)
            {
                bool status = strip is StatusStrip;
                strip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                strip.BackColor = Surface;
                strip.Font = UiFont(status ? 8.25f : 9f);
                if (status)
                    strip.ForeColor = Muted;
            }
        }

        /// <summary>
        /// A soft rounded frame instead of the black (FixedSingle) or sunken (Fixed3D) border, the same on
        /// Windows and Mono. The box, already without a border, is moved inward, and its parent draws the frame
        /// in the box's old bounds: grey normally, teal while the box has focus.
        /// </summary>
        private static void Frame(Control box, bool multiline)
        {
            Control parent = box.Parent;
            if (parent == null)
                return;

            Rectangle outer = box.Bounds;
            const int dx = 6;
            int dy = multiline ? 4 : Math.Max(2, (outer.Height - box.Height) / 2);
            box.SetBounds(outer.X + dx, outer.Y + dy, outer.Width - 2 * dx, multiline ? outer.Height - 2 * dy : box.Height);
            box.BackColor = Surface;

            parent.Paint += (sender, e) =>
            {
                if (!box.Visible)
                    return;
                var frame = new RectangleF(box.Left - dx, box.Top - dy, box.Width + 2 * dx - 1, box.Height + 2 * dy - 1);
                bool focused = box.ContainsFocus;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GdiKit.RoundedRect(frame, 4f))
                using (var fill = new SolidBrush(box.BackColor))
                using (var border = new Pen(focused ? Brand : LineStrong, focused ? 1.6f : 1f))
                {
                    e.Graphics.FillPath(fill, path);
                    e.Graphics.DrawPath(border, path);
                }
            };

            EventHandler repaint = (sender, e) => parent.Invalidate();
            box.Enter += repaint;
            box.Leave += repaint;
            box.LocationChanged += repaint;
            box.SizeChanged += repaint;
            box.VisibleChanged += repaint;
            box.EnabledChanged += (sender, e) =>
            {
                box.BackColor = box.Enabled ? Surface : SurfaceAlt;
                parent.Invalidate();
            };
        }

        private static void Button_EnabledChanged(object sender, EventArgs e)
        {
            var button = (Button)sender;
            StrongBox<ButtonKind> box;
            if (ButtonKinds.TryGetValue(button, out box))
                Recolor(button, box.Value);
        }

        private static void Recolor(Button button, ButtonKind kind)
        {
            bool on = button.Enabled;
            switch (kind)
            {
                case ButtonKind.Primary:
                    button.BackColor = on ? Brand : GdiKit.Tint(Brand, 0.55f);
                    button.ForeColor = Color.White;
                    button.FlatAppearance.BorderColor = button.BackColor;
                    button.FlatAppearance.MouseOverBackColor = BrandHover;
                    button.FlatAppearance.MouseDownBackColor = BrandPressed;
                    break;

                case ButtonKind.Danger:
                    button.BackColor = Surface;
                    button.ForeColor = on ? Danger : Muted;
                    button.FlatAppearance.BorderColor = on ? GdiKit.Tint(Danger, 0.6f) : Line;
                    button.FlatAppearance.MouseOverBackColor = GdiKit.Tint(Danger, 0.92f);
                    button.FlatAppearance.MouseDownBackColor = GdiKit.Tint(Danger, 0.84f);
                    break;

                default:
                    button.BackColor = on ? Surface : SurfaceAlt;
                    button.ForeColor = on ? Ink : Muted;
                    button.FlatAppearance.BorderColor = on ? LineStrong : Line;
                    button.FlatAppearance.MouseOverBackColor = GdiKit.Tint(Brand, 0.93f);
                    button.FlatAppearance.MouseDownBackColor = BrandLight;
                    break;
            }
        }

        /// <summary>White menus with a teal highlight, instead of the default blue-grey gradients.</summary>
        private sealed class MenuColors : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin { get { return Surface; } }
            public override Color MenuStripGradientEnd { get { return Surface; } }
            public override Color ToolStripDropDownBackground { get { return Surface; } }
            public override Color ImageMarginGradientBegin { get { return SurfaceAlt; } }
            public override Color ImageMarginGradientMiddle { get { return SurfaceAlt; } }
            public override Color ImageMarginGradientEnd { get { return SurfaceAlt; } }
            public override Color MenuBorder { get { return LineStrong; } }
            public override Color MenuItemBorder { get { return GdiKit.Tint(Brand, 0.55f); } }
            public override Color MenuItemSelected { get { return BrandLight; } }
            public override Color MenuItemSelectedGradientBegin { get { return BrandLight; } }
            public override Color MenuItemSelectedGradientEnd { get { return BrandLight; } }
            public override Color MenuItemPressedGradientBegin { get { return BrandLight; } }
            public override Color MenuItemPressedGradientMiddle { get { return BrandLight; } }
            public override Color MenuItemPressedGradientEnd { get { return BrandLight; } }
            public override Color SeparatorDark { get { return Line; } }
            public override Color SeparatorLight { get { return Surface; } }
            public override Color StatusStripGradientBegin { get { return Surface; } }
            public override Color StatusStripGradientEnd { get { return Surface; } }
            public override Color ToolStripBorder { get { return Line; } }
            public override Color CheckBackground { get { return BrandLight; } }
            public override Color CheckSelectedBackground { get { return BrandLight; } }
            public override Color CheckPressedBackground { get { return BrandLight; } }
            public override Color ButtonSelectedBorder { get { return Brand; } }
        }
    }
}
