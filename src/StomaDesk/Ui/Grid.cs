using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace StomaDesk.Ui
{
    /// <summary>DataGridView setup shared by every grid in the app, so they all look and behave the same.</summary>
    public static class Grid
    {
        private static readonly Color StripeColor = Color.FromArgb(250, 251, 252);

        /// <summary>Read-only list: whole-row selection, no row headers, striped rows.</summary>
        public static void SetupList(DataGridView grid, bool multiSelect = false)
        {
            ApplyLook(grid);
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = multiSelect;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
        }

        /// <summary>Editable table bound to a BindingList: users add rows at the bottom and delete with the Delete key.</summary>
        public static void SetupEditable(DataGridView grid)
        {
            ApplyLook(grid);
            grid.ReadOnly = false;
            grid.AllowUserToAddRows = true;
            grid.AllowUserToDeleteRows = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            grid.RowHeadersVisible = true;
            grid.RowHeadersWidth = 28;
            grid.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            grid.CurrentCellDirtyStateChanged += CommitCheckBoxAtOnce;
            grid.DataError += ShowDataError;
        }

        public static DataGridViewTextBoxColumn AddColumn(DataGridView grid, string header, int width,
            bool alignRight = false, bool fill = false, string dataProperty = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                Width = width,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            if (dataProperty != null)
                column.DataPropertyName = dataProperty;
            if (fill)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                column.MinimumWidth = width;
                // The columns are sized to fit the width; Mono would still add a horizontal scroll bar.
                grid.ScrollBars = ScrollBars.Vertical;
            }
            if (alignRight)
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns.Add(column);
            return column;
        }

        public static DataGridViewCheckBoxColumn AddCheckColumn(DataGridView grid, string header, int width, string dataProperty)
        {
            var column = new DataGridViewCheckBoxColumn
            {
                HeaderText = header,
                Width = width,
                DataPropertyName = dataProperty,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            grid.Columns.Add(column);
            return column;
        }

        /// <summary>
        /// Replaces all rows. Each row's Tag keeps its item, so a selected row maps straight back to the data.
        /// The item that was selected before stays selected.
        /// </summary>
        public static void Fill<T>(DataGridView grid, IEnumerable<T> items, Func<T, object[]> cells, Func<T, Color> rowColor = null)
            where T : class
        {
            T previous = Selected<T>(grid);
            grid.Rows.Clear();

            foreach (T item in items)
            {
                int index = grid.Rows.Add(cells(item));
                DataGridViewRow row = grid.Rows[index];
                row.Tag = item;
                if (rowColor != null)
                {
                    Color color = rowColor(item);
                    if (!color.IsEmpty)
                        row.DefaultCellStyle.BackColor = color;
                }
            }

            if (previous != null)
                SelectItem(grid, previous);
        }

        public static void SelectItem(DataGridView grid, object item)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (!ReferenceEquals(row.Tag, item))
                    continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Visible)
                    {
                        grid.CurrentCell = cell;
                        return;
                    }
                }
            }
        }

        public static T Selected<T>(DataGridView grid) where T : class
        {
            DataGridViewRow row = grid.CurrentRow;
            return row == null ? null : row.Tag as T;
        }

        /// <summary>All selected items; falls back to the current row when nothing is highlighted.</summary>
        public static List<T> SelectedItems<T>(DataGridView grid) where T : class
        {
            var items = new List<T>();
            foreach (DataGridViewRow row in grid.SelectedRows)
            {
                T item = row.Tag as T;
                if (item != null)
                    items.Add(item);
            }
            if (items.Count == 0)
            {
                T current = Selected<T>(grid);
                if (current != null)
                    items.Add(current);
            }
            return items;
        }

        /// <summary>
        /// Draws one column's cells as coloured pills (a status, a balance). <paramref name="tone"/> picks the
        /// colour from the row's item; an empty colour or empty text leaves the cell to the normal painting.
        /// </summary>
        public static void BadgeColumn<T>(DataGridView grid, int column, Func<T, Color> tone) where T : class
        {
            grid.CellPainting += (sender, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != column || e.Handled)
                    return;

                var item = grid.Rows[e.RowIndex].Tag as T;
                string text = Convert.ToString(e.FormattedValue);
                Color color = item == null ? Color.Empty : tone(item);
                if (text.Length == 0 || color.IsEmpty)
                    return;

                PaintCellBackground(grid, e);
                Font font = Theme.UiFont(8.25f, FontStyle.Bold);
                SizeF size = GdiKit.PillSize(e.Graphics, text, font);
                Rectangle b = e.CellBounds;
                bool right = e.CellStyle.Alignment == DataGridViewContentAlignment.MiddleRight;
                float x = right ? b.Right - size.Width - 8f : b.X + 6f;
                e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                GdiKit.DrawPill(e.Graphics, text, font, color, new RectangleF(x, b.Y + (b.Height - size.Height) / 2f, size.Width, size.Height));
                e.Handled = true;
            };
        }

        /// <summary>Background and bottom line of a cell, for CellPainting handlers that draw the content themselves.</summary>
        public static void PaintCellBackground(DataGridView grid, DataGridViewCellPaintingEventArgs e)
        {
            bool selected = (e.State & DataGridViewElementStates.Selected) != 0;
            using (var back = new SolidBrush(selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor))
                e.Graphics.FillRectangle(back, e.CellBounds);
            if (grid.CellBorderStyle != DataGridViewCellBorderStyle.None)
            {
                using (var line = new Pen(grid.GridColor))
                    e.Graphics.DrawLine(line, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }
        }

        private static void ApplyLook(DataGridView grid)
        {
            grid.AutoGenerateColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.BackgroundColor = Theme.Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Theme.Line;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 34;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Theme.SurfaceAlt;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Theme.Muted;
            grid.ColumnHeadersDefaultCellStyle.Font = Theme.UiFont(8f, FontStyle.Bold);
            // Mono paints the header of the current column in the selection colour; keep headers plain everywhere.
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Theme.SurfaceAlt;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Theme.Muted;
            grid.RowHeadersDefaultCellStyle.BackColor = Theme.SurfaceAlt;
            grid.RowHeadersDefaultCellStyle.SelectionBackColor = Theme.BrandLight;
            grid.DefaultCellStyle.BackColor = Theme.Surface;
            grid.DefaultCellStyle.ForeColor = Theme.Ink;
            grid.DefaultCellStyle.SelectionBackColor = Theme.BrandLight;
            grid.DefaultCellStyle.SelectionForeColor = Theme.Ink;
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = StripeColor;
            grid.RowTemplate.Height = 30;
            grid.CellPainting += PaintColumnHeader;
        }

        /// <summary>Column headers in small muted capitals on a flat background, with one line underneath.</summary>
        private static void PaintColumnHeader(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1 || e.ColumnIndex < 0 || e.Handled)
                return;

            var grid = (DataGridView)sender;
            Rectangle b = e.CellBounds;
            using (var back = new SolidBrush(Theme.SurfaceAlt))
                e.Graphics.FillRectangle(back, b);
            using (var line = new Pen(Theme.Line))
                e.Graphics.DrawLine(line, b.Left, b.Bottom - 1, b.Right, b.Bottom - 1);

            DataGridViewColumn column = grid.Columns[e.ColumnIndex];
            bool right = column.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            GdiKit.DrawText(e.Graphics, column.HeaderText.ToUpperInvariant(), Theme.UiFont(7.75f, FontStyle.Bold), Theme.Muted,
                new RectangleF(b.X + 8f, b.Y, b.Width - 16f, b.Height), right ? StringAlignment.Far : StringAlignment.Near);
            e.Handled = true;
        }

        /// <summary>A check box normally commits only when the cell loses focus; this saves the click at once.</summary>
        private static void CommitCheckBoxAtOnce(object sender, EventArgs e)
        {
            var grid = (DataGridView)sender;
            if (grid.IsCurrentCellDirty && grid.CurrentCell is DataGridViewCheckBoxCell)
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// <summary>Replaces the default DataGridView error dialog (a stack trace) with a plain message.</summary>
        private static void ShowDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var grid = (DataGridView)sender;
            e.ThrowException = false;
            string column = e.ColumnIndex >= 0 ? grid.Columns[e.ColumnIndex].HeaderText : "";
            Dialogs.Error(grid.FindForm(), string.Format("Valoare invalidă în coloana „{0}”. Pentru sume folosiți doar cifre și virgulă.", column));
        }
    }
}
