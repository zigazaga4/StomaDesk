using System;
using System.Linq;
using System.Windows.Forms;
using StomaDesk.Services;
using StomaDesk.Ui;

namespace StomaDesk.Forms
{
    public partial class ReportsView : StoreView
    {
        private ReportTable _current;
        private bool _loading;

        public ReportsView()
        {
            InitializeComponent();
            Grid.SetupList(gridReport);
        }

        protected override void OnBound()
        {
            _loading = true;
            Combo.Fill(cboReport, Reports.All(), null);
            dtpTo.Value = DateTime.Today;
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _loading = false;
        }

        public override void RefreshView()
        {
            if (Store == null || _loading)
                return;

            var definition = Combo.Selected<ReportDefinition>(cboReport);
            if (definition == null)
                return;

            dtpFrom.Enabled = definition.UsesPeriod;
            dtpTo.Enabled = definition.UsesPeriod;

            DateTime from = dtpFrom.Value.Date;
            DateTime to = dtpTo.Value.Date;
            if (definition.UsesPeriod && from > to)
            {
                _current = null;
                gridReport.Rows.Clear();
                btnExport.Enabled = false;
                lblSummary.Text = "Perioada nu este validă: data de început este după data de sfârșit.";
                return;
            }

            _current = definition.Build(Store, from, to);
            ShowTable(_current);
        }

        private void ShowTable(ReportTable table)
        {
            gridReport.Rows.Clear();
            gridReport.Columns.Clear();
            for (int i = 0; i < table.Headers.Length; i++)
            {
                bool numeric = table.Rows.Count > 0 && IsNumber(table.Rows[0][i]);
                Grid.AddColumn(gridReport, table.Headers[i], i == 0 ? 280 : 140, alignRight: numeric, fill: i == 0);
            }

            Grid.Fill(gridReport, table.Rows, row => row.Select(cell => (object)Fmt.Cell(cell)).ToArray());
            lblSummary.Text = table.Rows.Count == 0 ? "Nu există date pentru perioada aleasă." : table.Summary;
            btnExport.Enabled = table.Rows.Count > 0;
        }

        private static bool IsNumber(object value)
        {
            return value is decimal || value is int || value is double;
        }

        private void cboReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void Period_ValueChanged(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_current == null)
                return;

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV pentru Excel (*.csv)|*.csv";
                dialog.FileName = Search.StripDiacritics(_current.Title) + " " + DateTime.Today.ToString("yyyy.MM.dd") + ".csv";
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                    return;

                Reports.ExportCsv(_current, dialog.FileName);
                lblSummary.Text = "Exportat în " + dialog.FileName;
            }
        }
    }
}
