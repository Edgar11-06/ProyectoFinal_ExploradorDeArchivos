using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SistemaMultimedia.Utilities;

namespace SistemaMultimedia.Forms
{
    public partial class GroupSummaryForm : Form
    {
        public string? SelectedCategory { get; private set; }
        private readonly Dictionary<string, DataTable> _groups;
        private readonly List<CategorySummary> _summaryList;
        private readonly List<ChartMetric> _chartMetrics;
        private ComboBox? _cmbMetric;
        private ComboBox? _cmbChartType;
        private NumericUpDown? _numTopN;
        private Chart? _chartAdvanced;

        public GroupSummaryForm(Dictionary<string, DataTable> groups)
        {
            InitializeComponent();
            _groups = groups ?? new Dictionary<string, DataTable>();

            BackColor = Color.FromArgb(30, 30, 30);

            _summaryList = _groups.Select(kv =>
            {
                string display;
                if (string.Equals(kv.Key, "(Todos)", StringComparison.OrdinalIgnoreCase))
                {
                    display = "(Todos)";
                }
                else
                {
                    var parts = Regex.Split(kv.Key ?? string.Empty, @"\s*\|\s*");
                    var vals = parts.Select(p =>
                    {
                        var t = p?.Trim() ?? string.Empty;
                        var idx = t.IndexOf('=');
                        return idx >= 0 ? t.Substring(idx + 1).Trim() : t;
                    });
                    display = string.Join(" | ", vals.Where(v => !string.IsNullOrEmpty(v)));
                }

                return new CategorySummary
                {
                    Key = kv.Key ?? string.Empty,
                    Categoria = display,
                    Cantidad = kv.Value.Rows.Count,
                    SumaValor = SumValorSafe(kv.Value),
                    PromedioPrecioUnitario = AverageSafe(kv.Value, "PrecioUnitario")
                };
            }).OrderByDescending(x => x.SumaValor).ToList();

            _chartMetrics = BuildChartMetrics(_groups);

            ddvSummary.AutoGenerateColumns = true;
            ddvSummary.DataSource = _summaryList;
            ddvSummary.ReadOnly = true;
            ddvSummary.AllowUserToAddRows = false;
            ddvSummary.AllowUserToDeleteRows = false;
            ddvSummary.BackgroundColor = Color.White;
            ddvSummary.ForeColor = Color.Black;
            ddvSummary.DefaultCellStyle.ForeColor = Color.Black;
            ddvSummary.DefaultCellStyle.BackColor = Color.White;
            ddvSummary.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            ddvSummary.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            ddvSummary.RowHeadersDefaultCellStyle.ForeColor = Color.Black;
            ddvSummary.RowHeadersDefaultCellStyle.BackColor = Color.White;

            ddvSummary.DataBindingComplete += (s, e) =>
            {
                if (ddvSummary.Columns.Contains("Key"))
                    ddvSummary.Columns["Key"].Visible = false;
                if (ddvSummary.Columns.Contains("Categoria"))
                {
                    ddvSummary.Columns["Categoria"].HeaderText = "Categoria";
                    ddvSummary.Columns["Categoria"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            };

            ddvSummary.DoubleClick += (s, e) => SelectCurrentAndClose();
            btnCerrar.Click += (s, e) => Close();
            btnMostrarTodos.Click += (s, e) =>
            {
                SelectedCategory = null;
                DialogResult = DialogResult.OK;
                Close();
            };

            BuildAdvancedChartUi();
        }

        private void BuildAdvancedChartUi()
        {
            _chartAdvanced = new Chart { Dock = DockStyle.Fill, BackColor = Color.White };
            var area = new ChartArea("AreaAvanzada");
            _chartAdvanced.ChartAreas.Add(area);
            _chartAdvanced.Series.Add(new Series("Datos") { ChartArea = "AreaAvanzada" });

            var tabAdvanced = new TabPage("Grafica");
            tabAdvanced.Controls.Add(_chartAdvanced);
            tabMain.TabPages.Add(tabAdvanced);

            btnMostrarTodos.Location = new Point(8, 18);
            btnMostrarTodos.Size = new Size(120, 34);
            btnMostrar.Location = new Point(586, 18);
            btnMostrar.Size = new Size(104, 34);
            btnCerrar.Location = new Point(698, 18);
            btnCerrar.Size = new Size(94, 34);

            var lblMetric = new Label { AutoSize = true, ForeColor = Color.White, Location = new Point(140, 4), Text = "Metrica" };
            _cmbMetric = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(140, 24),
                Size = new Size(220, 33)
            };
            _cmbMetric.DataSource = _chartMetrics;
            _cmbMetric.DisplayMember = nameof(ChartMetric.Label);

            var lblType = new Label { AutoSize = true, ForeColor = Color.White, Location = new Point(370, 4), Text = "Tipo" };
            _cmbChartType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(370, 24),
                Size = new Size(130, 33)
            };
            _cmbChartType.Items.AddRange(new object[] { "Columnas", "Barras", "Linea", "Area", "Pastel", "Dona", "Spline" });
            _cmbChartType.SelectedIndex = 0;

            var lblTop = new Label { AutoSize = true, ForeColor = Color.White, Location = new Point(510, 4), Text = "Top" };
            _numTopN = new NumericUpDown
            {
                Location = new Point(510, 25),
                Minimum = 1,
                Maximum = 50,
                Value = 10,
                Size = new Size(70, 31)
            };

            panelButtons.Height = 70;
            panelButtons.Controls.Add(lblMetric);
            panelButtons.Controls.Add(_cmbMetric);
            panelButtons.Controls.Add(lblType);
            panelButtons.Controls.Add(_cmbChartType);
            panelButtons.Controls.Add(lblTop);
            panelButtons.Controls.Add(_numTopN);
        }

        private static decimal SumValorSafe(DataTable dt)
        {
            var valCol = FindBestValueColumn(dt);
            return valCol == null ? 0m : GetNumericValues(dt, valCol.ColumnName).Sum();
        }

        private static decimal AverageSafe(DataTable dt, string columnName)
        {
            if (dt == null) return 0m;

            var priceCandidates = new[] { "precio_unitario", "preciounitario", "precio_unit", "unit_price", "price", "precio", "unitprice", "importe", "cost" };
            var col = dt.Columns.Cast<DataColumn>()
                .FirstOrDefault(c =>
                    c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase) ||
                    priceCandidates.Any(s => c.ColumnName.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0));

            if (col == null) return 0m;

            var decimals = GetNumericValues(dt, col.ColumnName).ToList();
            return decimals.Count == 0 ? 0m : decimals.Average();
        }

        private void GroupSummaryForm_Load(object sender, EventArgs e)
        {
            ddvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ConfigureChartArea(chartSummary.ChartAreas.FirstOrDefault());
            ConfigureChartArea(chartTotal.ChartAreas.FirstOrDefault());
            ConfigureChartArea(_chartAdvanced?.ChartAreas.FirstOrDefault());
        }

        private void SelectCurrentAndClose()
        {
            if (ddvSummary.CurrentRow?.DataBoundItem is CategorySummary cs)
            {
                SelectedCategory = cs.Key;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            using var _wait = new WaitCursorScope();
            var topCount = (int)(_numTopN?.Value ?? 10);
            var toDisplay = _summaryList.OrderByDescending(x => x.Cantidad).Take(topCount).ToList();

            if (!toDisplay.Any())
            {
                chartSummary.Series.Clear();
                chartTotal.Series.Clear();
                _chartAdvanced?.Series.Clear();
                return;
            }

            var categories = toDisplay.Select(s => s.Categoria).ToArray();
            var cantidades = toDisplay.Select(s => s.Cantidad).ToArray();
            var totalsByValue = _summaryList.OrderByDescending(x => x.SumaValor).Take(topCount).ToList();
            var totalCategories = totalsByValue.Select(s => s.Categoria).ToArray();
            var totales = totalsByValue.Select(s => (double)s.SumaValor).ToArray();
            var culture = CultureInfo.CurrentCulture;

            chartSummary.Series.Clear();
            var ca1 = chartSummary.ChartAreas[0];
            ConfigureChartArea(ca1);
            var sCantidad = new Series("Cantidad")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Int32,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F),
                LabelForeColor = Color.Black,
                IsXValueIndexed = true
            };
            sCantidad["PointWidth"] = "0.45";
            chartSummary.Series.Add(sCantidad);
            ca1.AxisX.LabelStyle.Angle = -45;
            sCantidad.Points.DataBindXY(categories, cantidades);
            foreach (var point in sCantidad.Points)
                point.Label = ((int)point.YValues[0]).ToString("N0", culture);

            chartTotal.Series.Clear();
            var ca2 = chartTotal.ChartAreas[0];
            ConfigureChartArea(ca2);
            var sTotal = new Series("Total")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F),
                LabelForeColor = Color.Black,
                IsXValueIndexed = true
            };
            sTotal["PointWidth"] = "0.45";
            chartTotal.Series.Add(sTotal);
            ca2.AxisX.LabelStyle.Angle = -45;
            sTotal.Points.DataBindXY(totalCategories, totales);
            foreach (var point in sTotal.Points)
                point.Label = point.YValues[0].ToString("N2", culture);

            AdjustAxisY(ca1, sCantidad, "N0");
            AdjustAxisY(ca2, sTotal, "N2");

            chartSummary.Titles.Clear();
            chartTotal.Titles.Clear();
            chartSummary.Titles.Add($"Top {Math.Min(topCount, _summaryList.Count)} por cantidad");
            chartTotal.Titles.Add($"Top {Math.Min(topCount, _summaryList.Count)} por total");

            chartSummary.Invalidate();
            chartTotal.Invalidate();
            RenderAdvancedChart();
        }

        private void RenderAdvancedChart()
        {
            if (_chartAdvanced == null || _cmbMetric?.SelectedItem is not ChartMetric metric) return;

            var chartType = GetSelectedChartType();
            var topCount = (int)(_numTopN?.Value ?? 10);
            var values = _summaryList
                .Select(summary => new { summary.Categoria, Value = metric.GetValue(summary.Key) })
                .OrderByDescending(x => x.Value)
                .Take(topCount)
                .ToList();

            _chartAdvanced.Series.Clear();
            _chartAdvanced.Titles.Clear();
            _chartAdvanced.Legends.Clear();

            var series = new Series(metric.Label)
            {
                ChartArea = "AreaAvanzada",
                ChartType = chartType,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F),
                LabelForeColor = Color.Black,
                IsXValueIndexed = true
            };
            _chartAdvanced.Series.Add(series);
            series.Points.DataBindXY(values.Select(x => x.Categoria).ToArray(), values.Select(x => (double)x.Value).ToArray());

            var circular = chartType == SeriesChartType.Pie || chartType == SeriesChartType.Doughnut;
            if (circular)
            {
                _chartAdvanced.Legends.Add(new Legend("Leyenda"));
                series.Legend = "Leyenda";
                series.Label = "#PERCENT{P1}";
                series.LegendText = "#VALX: #VALY{N2}";
            }
            else
            {
                var area = _chartAdvanced.ChartAreas[0];
                ConfigureChartArea(area);
                area.AxisX.LabelStyle.Angle = chartType == SeriesChartType.Bar ? 0 : -45;
                AdjustAxisY(area, series, metric.IsWholeNumber ? "N0" : "N2");
                foreach (var point in series.Points)
                    point.Label = point.YValues[0].ToString(metric.IsWholeNumber ? "N0" : "N2", CultureInfo.CurrentCulture);
            }

            _chartAdvanced.Titles.Add($"{metric.Label} - Top {Math.Min(topCount, _summaryList.Count)}");
            _chartAdvanced.Invalidate();
            tabMain.SelectedTab = tabMain.TabPages.Cast<TabPage>().FirstOrDefault(t => t.Text == "Grafica") ?? tabMain.SelectedTab;
        }

        private SeriesChartType GetSelectedChartType()
        {
            return (_cmbChartType?.SelectedItem?.ToString() ?? string.Empty) switch
            {
                "Barras" => SeriesChartType.Bar,
                "Linea" => SeriesChartType.Line,
                "Area" => SeriesChartType.Area,
                "Pastel" => SeriesChartType.Pie,
                "Dona" => SeriesChartType.Doughnut,
                "Spline" => SeriesChartType.Spline,
                _ => SeriesChartType.Column
            };
        }

        private static void ConfigureChartArea(ChartArea? area)
        {
            if (area == null) return;
            area.AxisX.Interval = 1;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisY.MajorGrid.Enabled = true;
            area.AxisY.MinorGrid.Enabled = false;
            area.InnerPlotPosition = new ElementPosition(8, 8, 84, 82);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9F);
        }

        private static void AdjustAxisY(ChartArea area, Series series, string format)
        {
            if (series.Points.Count == 0) return;
            var maxVal = series.Points.Max(p => p.YValues[0]);
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = maxVal <= 0 ? 1 : Math.Ceiling(maxVal * 1.12);
            area.AxisY.Interval = Math.Max(1.0, Math.Ceiling(area.AxisY.Maximum / 5.0));
            area.AxisY.LabelStyle.Format = format;
        }

        private static List<ChartMetric> BuildChartMetrics(Dictionary<string, DataTable> groups)
        {
            var metrics = new List<ChartMetric>
            {
                new("Cantidad", key => groups.TryGetValue(key, out var dt) ? dt.Rows.Count : 0m, true)
            };

            var numericColumns = groups.Values
                .SelectMany(GetNumericColumns)
                .Where(c => !string.Equals(c, "__original_index", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            foreach (var col in numericColumns)
            {
                metrics.Add(new ChartMetric($"Suma de {col}", key => SumColumn(groups, key, col), false));
                metrics.Add(new ChartMetric($"Promedio de {col}", key => AverageColumn(groups, key, col), false));
            }

            return metrics;
        }

        private static IEnumerable<string> GetNumericColumns(DataTable dt)
        {
            if (dt == null) yield break;

            foreach (DataColumn col in dt.Columns)
            {
                if (col.DataType == typeof(byte) || col.DataType == typeof(short) || col.DataType == typeof(int) ||
                    col.DataType == typeof(long) || col.DataType == typeof(float) || col.DataType == typeof(double) ||
                    col.DataType == typeof(decimal) || GetNumericValues(dt, col.ColumnName).Any())
                {
                    yield return col.ColumnName;
                }
            }
        }

        private static decimal SumColumn(Dictionary<string, DataTable> groups, string key, string column)
        {
            return groups.TryGetValue(key, out var dt) ? GetNumericValues(dt, column).Sum() : 0m;
        }

        private static decimal AverageColumn(Dictionary<string, DataTable> groups, string key, string column)
        {
            if (!groups.TryGetValue(key, out var dt)) return 0m;
            var values = GetNumericValues(dt, column).ToList();
            return values.Count == 0 ? 0m : values.Average();
        }

        private static DataColumn? FindBestValueColumn(DataTable dt)
        {
            if (dt == null) return null;

            var candidates = new[] { "valor", "total", "amount", "value", "price", "precio", "importe", "subtotal", "salario", "salary" };
            return dt.Columns.Cast<DataColumn>()
                .Where(c => !string.Equals(c.ColumnName, "__original_index", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c =>
                {
                    var nameScore = candidates.Any(s => c.ColumnName.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0) ? 10 : 0;
                    var typeScore = c.DataType == typeof(decimal) || c.DataType == typeof(double) || c.DataType == typeof(int) ? 5 : 0;
                    var numericScore = GetNumericValues(dt, c.ColumnName).Any() ? 3 : 0;
                    return nameScore + typeScore + numericScore;
                })
                .FirstOrDefault(c => GetNumericValues(dt, c.ColumnName).Any());
        }

        private static IEnumerable<decimal> GetNumericValues(DataTable dt, string columnName)
        {
            if (dt == null || !dt.Columns.Contains(columnName)) yield break;

            foreach (DataRow row in dt.Rows)
            {
                if (row.IsNull(columnName)) continue;
                if (TryConvertDecimal(row[columnName], out var value))
                    yield return value;
            }
        }

        private static bool TryConvertDecimal(object? value, out decimal result)
        {
            result = 0m;
            if (value == null || value == DBNull.Value) return false;
            if (value is decimal d) { result = d; return true; }
            if (value is double db && !double.IsNaN(db)) { result = (decimal)db; return true; }
            if (value is float f && !float.IsNaN(f)) { result = (decimal)f; return true; }
            if (value is int or long or short or byte)
            {
                result = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                return true;
            }

            var text = Convert.ToString(value, CultureInfo.CurrentCulture)?.Trim();
            if (string.IsNullOrWhiteSpace(text)) return false;

            text = Regex.Replace(text, @"[^\d\-\+\,\.]", string.Empty);
            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out result)) return true;
            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out result)) return true;

            var lastComma = text.LastIndexOf(',');
            var lastDot = text.LastIndexOf('.');
            if (lastComma > lastDot)
            {
                var normalized = text.Replace(".", string.Empty).Replace(',', '.');
                return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
            }

            if (lastDot > lastComma)
            {
                var normalized = text.Replace(",", string.Empty);
                return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
            }

            return false;
        }
    }

    internal class ChartMetric
    {
        private readonly Func<string, decimal> _valueFactory;

        public ChartMetric(string label, Func<string, decimal> valueFactory, bool isWholeNumber)
        {
            Label = label;
            _valueFactory = valueFactory;
            IsWholeNumber = isWholeNumber;
        }

        public string Label { get; }
        public bool IsWholeNumber { get; }
        public decimal GetValue(string key) => _valueFactory(key);
    }

    public class CategorySummary
    {
        public string Key { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal SumaValor { get; set; }
        public decimal PromedioPrecioUnitario { get; set; }
    }
}
