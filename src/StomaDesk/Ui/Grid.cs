using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace StomaDesk.Ui
{
    /// <summary>DataGridView setup shared by every grid in the app, so they all look and behave the same.</summary>
    public static class Grid
    {
        private static readonly Color HeaderColor = Color.FromArgb(236, 240, 245);
        private static readonly Color StripeColor = Color.FromArgb(246, 248, 251);

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

        private static void ApplyLook(DataGridView grid)
        {
            grid.AutoGenerateColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.BackgroundColor = SystemColors.Window;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(225, 228, 232);
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 30;
            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderColor;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.Font, FontStyle.Bold);
            // Mono paints the header of the current column in the selection colour; keep headers plain everywhere.
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.BackColor = StripeColor;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 224, 247);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.RowTemplate.Height = 26;
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
