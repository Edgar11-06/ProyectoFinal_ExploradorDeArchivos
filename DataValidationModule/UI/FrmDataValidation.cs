using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataValidationModule.Cleaners;
using DataValidationModule.Models;
using DataValidationModule.Validators;

namespace DataValidationModule.UI
{
    public partial class FrmDataValidation : Form
    {
        private DataTable _originalData;
        private DataTable _workingData;
        private DataTable _displayedData;
        private DataCleaner _cleaner;
        private DataValidator _validator;
        private List<ValidationResult> _validationResults = new List<ValidationResult>();
        private DatasetStatistics _stats = new DatasetStatistics();
        private string _loadedFilePath = "";
        private bool _hasPendingChanges = false;
        private bool _isLongOperationRunning = false;

        public FrmDataValidation()
        {
            InitializeComponent();
            WireEvents();
            UpdateButtonStates(fileLoaded: false);
        }

        private void BtnSaveFile_Click(object sender, EventArgs e)
        {
            var dtToSave = _hasPendingChanges ? _workingData : _originalData;
            if (dtToSave == null)
            {
                MessageBox.Show("No hay datos para guardar.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Title = "Guardar dataset";
                dlg.FileName = Path.GetFileNameWithoutExtension(_loadedFilePath) + "_modified";
                dlg.Filter = "CSV|*.csv|Texto (TXT)|*.txt|HTML|*.html|XML|*.xml|Todos|*.*";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string ext = Path.GetExtension(dlg.FileName).ToLower();
                    if (ext == ".html" || ext == ".htm")
                        ReportExporter.ExportHtml(dlg.FileName, _stats, _validationResults);
                    else
                        DatasetFileService.ExportToFile(dtToSave, dlg.FileName);

                    MessageBox.Show($"Archivo guardado: {dlg.FileName}", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar archivo:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void WireEvents()
        {
            this.btnImport.Click += BtnImport_Click;
            this.btnAnalyze.Click += BtnAnalyze_Click;
            this.btnAutoFix.Click += BtnAutoFix_Click;
            this.btnRemoveDups.Click += BtnRemoveDups_Click;
            this.btnApplyChanges.Click += BtnApplyChanges_Click;
            this.btnRevert.Click += BtnRevert_Click;
            this.btnExportReport.Click += BtnExportReport_Click;
            this.btnSaveFile.Click += BtnSaveFile_Click;
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Seleccionar dataset",
                Filter = "Archivos de datos|*.csv;*.json;*.xml;*.txt;*.xlsx;*.xls" +
                         "|CSV|*.csv|JSON|*.json|XML|*.xml|Texto|*.txt|Excel|*.xlsx;*.xls|Todos|*.*"
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;
            LoadFromFile(dlg.FileName);
        }

        public void LoadFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("No se encontró el archivo indicado.", "Archivo no encontrado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SetProgress(10, "Leyendo archivo…");
                Application.DoEvents();

                _loadedFilePath = filePath;
                _originalData = ImportDataset(filePath);
                _workingData = _originalData?.Copy();

                if (_originalData == null || _originalData.Rows.Count == 0)
                {
                    MessageBox.Show("El archivo está vacío o no contiene datos.",
                        "Archivo vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetProgress(0, "Listo");
                    return;
                }

                _cleaner = null;
                _validator = null;
                _validationResults.Clear();
                _stats = new DatasetStatistics();
                _hasPendingChanges = false;
                BindGridData(_originalData);
                lblFile.Text = Path.GetFileName(_loadedFilePath);
                UpdateStatsDisplay();
                ClearReport();
                AppendReport($"Archivo cargado: {Path.GetFileName(_loadedFilePath)}");
                AppendReport($"   {_originalData.Rows.Count} filas × {_originalData.Columns.Count} columnas");

                if (_workingData != null)
                {
                    var tmpValidator = new DataValidator(_workingData.Copy());
                    tmpValidator.InferColumnTypes();
                    AppendReport("   Tipos inferidos por columna:");
                    foreach (var kvp in tmpValidator.InferredColumnTypes)
                        AppendReport($"     • {kvp.Key}: {kvp.Value}");
                }

                UpdateButtonStates(fileLoaded: true);
                SetProgress(100, "Archivo cargado");
                SetStatus($"Dataset cargado — {_originalData.Rows.Count} registros, {_originalData.Columns.Count} columnas");
            }
            catch (NotImplementedException niEx)
            {
                MessageBox.Show(niEx.Message, "Formato no implementado",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetProgress(0, "Listo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el archivo:\n{ex.Message}",
                    "Error de importación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetProgress(0, "Error");
            }
        }

        private static DataTable ImportDataset(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext is ".txt")
                return DatasetFileService.ImportCsv(filePath);
            return DatasetFileService.ImportFromFile(filePath);
        }

        private async void BtnAnalyze_Click(object sender, EventArgs e)
        {
            if (_workingData == null || _isLongOperationRunning) return;

            _isLongOperationRunning = true;
            SetProgress(0, "Iniciando análisis…");
            EnableButtons(false);
            ClearReport();

            try
            {
                _validationResults = await Task.Run(() =>
                {
                    _validator = new DataValidator(_workingData.Copy());
                    return _validator.ValidateAll();
                });

                SetProgress(70, "Calculando estadísticas…");
                _stats = new DatasetStatistics
                {
                    TotalRecords = _workingData.Rows.Count,
                    TotalColumns = _workingData.Columns.Count,
                    ErrorsFound = _validationResults.Count,
                    DuplicatesFound = _validationResults.Count(r => r.ErrorType == ErrorType.Duplicate),
                    NullValues = _validationResults.Count(r => r.ErrorType == ErrorType.NullOrEmpty ||
                                                                     r.ErrorType == ErrorType.RequiredFieldEmpty),
                    CorrectionsApplied = _cleaner?.ChangeLog?.Count ?? 0
                };
                _stats.CalculateQuality();

                HighlightErrorRows();
                UpdateStatsDisplay();

                BuildReport();

                SetProgress(100, "Análisis completado");
                SetStatus($"Análisis completado — {_validationResults.Count} errores encontrados");
            }
            catch (Exception ex)
            {
                AppendReport($"❌ Error durante el análisis: {ex.Message}", Color.Red);
                SetProgress(0, "Error");
            }
            finally
            {
                _isLongOperationRunning = false;
                EnableButtons(true);
            }
        }

        private async void BtnAutoFix_Click(object sender, EventArgs e)
        {
            if (_workingData == null || _isLongOperationRunning) return;

            var confirm = MessageBox.Show(
                "Se aplicarán correcciones automáticas al dataset:\n\n" +
                "• Eliminar espacios innecesarios\n" +
                "• Normalizar formato Título en nombres\n" +
                "• Corregir correos (minúsculas)\n" +
                "• Normalizar fechas (dd/MM/yyyy)\n" +
                "• Limpiar campos numéricos\n" +
                "• Corregir caracteres HTML/especiales\n\n" +
                "¿Desea continuar?",
                "Confirmar corrección automática",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            _isLongOperationRunning = true;
            SetProgress(0, "Aplicando correcciones…");
            EnableButtons(false);

            try
            {
                if (_cleaner == null) _cleaner = new DataCleaner(_workingData);

                int corrected = await Task.Run(() => _cleaner.FixAll());

                _hasPendingChanges = corrected > 0;
                _stats.CorrectionsApplied = _cleaner.ChangeLog.Count;

                // Fuerza el re-enlace del DataGridView al DataTable corregido
                // para asegurar que los cambios se reflejen inmediatamente.
                if (_workingData != null)
                {
                    // Guardamos referencia mostrada
                    _displayedData = _workingData;

                    // Forzar desenlace y volver a enlazar directamente al DataTable
                    if (InvokeRequired)
                    {
                        Invoke((Action)(() => { dataGridView.DataSource = null; dataGridView.DataSource = _workingData; }));
                    }
                    else
                    {
                        dataGridView.DataSource = null;
                        dataGridView.DataSource = _workingData;
                    }

                    // Asegurar formato de columnas de fecha y refrescar visualmente
                    try { ApplyDateColumnFormatting(_workingData); } catch { }

                    // Normalizar valores visibles y forzar actualización completa
                    RefreshDataGrid(_workingData);
                    dataGridView.Refresh();
                    dataGridView.Invalidate();
                    dataGridView.Update();

                    // Depuración: comparar algunos valores entre DataTable y Grid
                    CompareDataTableAndGrid(_workingData, 5);
                }

                UpdateStatsDisplay();

                ClearReport();
                AppendReport($"⚙️ Correcciones automáticas aplicadas: {corrected} cambios", Color.FromArgb(39, 174, 96));
                AppendReport("");

                if (_cleaner.ChangeLog.Count > 0)
                {
                    AppendReport("── Log de cambios ──", Color.FromArgb(100, 149, 237));
                    foreach (var entry in _cleaner.ChangeLog.Take(200))
                        AppendReport("  " + entry);

                    if (_cleaner.ChangeLog.Count > 200)
                        AppendReport($"  ... y {_cleaner.ChangeLog.Count - 200} cambios más.");
                }

                SetProgress(100, $"{corrected} correcciones aplicadas");
                SetStatus($"Corrección automática completada — {corrected} cambios realizados");
            }
            catch (Exception ex)
            {
                AppendReport($"❌ Error durante la corrección: {ex.Message}", Color.Red);
                SetProgress(0, "Error");
            }
            finally
            {
                _isLongOperationRunning = false;
                EnableButtons(true);
            }
        }


        private void BtnRemoveDups_Click(object sender, EventArgs e)
        {
            if (_workingData == null) return;

            int dupCount = _validationResults.Count(r => r.ErrorType == ErrorType.Duplicate && r.SuggestedValue == "Eliminar esta fila");

            if (dupCount == 0)
            {
                MessageBox.Show("No se han detectado duplicados en el análisis previo.\n" +
                                "Ejecute primero 'Analizar Dataset'.",
                                "Sin duplicados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Se encontraron {dupCount} filas duplicadas.\n¿Desea eliminarlas?",
                "Eliminar duplicados",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                if (_cleaner == null) _cleaner = new DataCleaner(_workingData);
                int removed = _cleaner.RemoveDuplicates();
                _hasPendingChanges = removed > 0;
                _stats.DuplicatesFound = 0;
                _stats.CorrectionsApplied += removed;
                _stats.TotalRecords = _workingData.Rows.Count;
                _stats.CalculateQuality();

                BindGridData(_cleaner.GetCurrentData());
                UpdateStatsDisplay();

                AppendReport($"🗑️ {removed} filas duplicadas eliminadas.", Color.FromArgb(230, 126, 34));
                SetProgress(100, $"{removed} duplicados eliminados");
                SetStatus($"{removed} filas duplicadas eliminadas del dataset");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar duplicados:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BtnApplyChanges_Click(object sender, EventArgs e)
        {
            if (!_hasPendingChanges)
            {
                MessageBox.Show("No hay cambios pendientes para aplicar.",
                    "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show(
                $"Los cambios se han aplicado al dataset en memoria.\n\n" +
                $"Total de correcciones realizadas: {_cleaner.ChangeLog.Count}\n\n" +
                "Use 'Exportar Reporte' para guardar el resultado.",
                "Cambios aplicados", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _originalData = _workingData.Copy();
            _hasPendingChanges = false;
            SetStatus("Cambios aplicados. Listo para exportar.");
            BindGridData(_originalData);
        }


        private void BtnRevert_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Se revertirán TODOS los cambios y el dataset volverá al estado original.\n¿Continuar?",
                "Revertir cambios",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _workingData = _originalData.Copy();
                _cleaner = new DataCleaner(_workingData);
                _hasPendingChanges = false;
                _validationResults.Clear();
                _stats.CorrectionsApplied = 0;

                BindGridData(_originalData);
                UpdateStatsDisplay();

                ClearReport();
                AppendReport("↩️ Dataset revertido al estado original.", Color.FromArgb(127, 140, 141));
                SetProgress(0, "Cambios revertidos");
                SetStatus("Dataset revertido al estado original.");

                foreach (DataGridViewRow row in dataGridView.Rows)
                    row.DefaultCellStyle.BackColor = Color.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al revertir:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BtnExportReport_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                string baseName = Path.GetFileNameWithoutExtension(_loadedFilePath) + "_reporte";
                dlg.Title = "Exportar reporte";
                dlg.FileName = baseName;
                dlg.Filter = "Reporte HTML|*.html|Reporte de texto|*.txt|Todos|*.*";

                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string ext = Path.GetExtension(dlg.FileName).ToLower();

                    if (ext == ".html" || ext == ".htm")
                        ReportExporter.ExportHtml(dlg.FileName, _stats, _validationResults);
                    else
                        ReportExporter.ExportTxt(dlg.FileName, _stats, _validationResults, _cleaner?.ChangeLog);

                    MessageBox.Show($"Reporte exportado exitosamente:\n{dlg.FileName}",
                        "Exportación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (MessageBox.Show("¿Desea abrir el reporte ahora?", "Abrir reporte",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = dlg.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al exportar el reporte:\n{ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void BindGridData(DataTable dt)
        {
            if (InvokeRequired) { Invoke((Action<DataTable>)BindGridData, dt); return; }
            _displayedData = dt;

            // Enlazar directamente al DataTable para que los cambios en dt se reflejen
            // sin crear una vista intermedia que pueda ocultar actualizaciones.
            dataGridView.DataSource = null;
            dataGridView.DataSource = dt;

            try
            {
                if (dt.Columns.Contains("Vigencia"))
                {
                    var c = dataGridView.Columns["Vigencia"];
                    c.FillWeight = 18; 
                    c.MinimumWidth = 80;
                    c.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                    c.DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                if (dt.Columns.Contains("Zona A"))
                {
                    var c = dataGridView.Columns["Zona A"];
                    c.FillWeight = 22;
                    c.MinimumWidth = 80;
                }
                if (dt.Columns.Contains("Zona B"))
                {
                    var c = dataGridView.Columns["Zona B"];
                    c.FillWeight = 20;
                    c.MinimumWidth = 80;
                }
                if (dt.Columns.Contains("Zona C"))
                {
                    var c = dataGridView.Columns["Zona C"];
                    c.FillWeight = 20;
                    c.MinimumWidth = 80;
                }
                if (dt.Columns.Contains("Zona Fronteriza"))
                {
                    var c = dataGridView.Columns["Zona Fronteriza"];
                    c.FillWeight = 20;
                    c.MinimumWidth = 90;
                }

            }
            catch { }
            if (btnSaveFile != null) btnSaveFile.Enabled = dt != null && dt.Rows.Count > 0;
        }

        private void ApplyDateColumnFormatting(DataTable dt)
        {
            if (dt == null) return;
            if (InvokeRequired) { Invoke((Action<DataTable>)ApplyDateColumnFormatting, dt); return; }

            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                try
                {
                    if (col.Name.IndexOf("fecha", StringComparison.OrdinalIgnoreCase) >= 0
                        || col.Name.IndexOf("date", StringComparison.OrdinalIgnoreCase) >= 0
                        || col.Name.IndexOf("vigencia", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        col.DefaultCellStyle.Format = "dd/MM/yyyy";
                        col.DefaultCellStyle.NullValue = "(null)";
                        col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
                    }
                }
                catch { }
            }
        }

        private void RefreshDataGrid(DataTable dt)
        {
            if (InvokeRequired) { Invoke((Action<DataTable>)RefreshDataGrid, dt); return; }
            // No reasignar a DataView: respetar el DataTable ya enlazado.
            try
            {
                bool needAssign = false;
                if (dataGridView.DataSource == null) needAssign = true;
                else if (dataGridView.DataSource is DataTable dtSrc && !object.ReferenceEquals(dtSrc, dt)) needAssign = true;
                else if (dataGridView.DataSource is DataView dv && !object.ReferenceEquals(dv.Table, dt)) needAssign = true;

                if (needAssign)
                {
                    dataGridView.DataSource = null;
                    dataGridView.DataSource = dt;
                }
            }
            catch { }

            // Aplicar formato a columnas de fecha
            try { ApplyDateColumnFormatting(dt); } catch { }

            dataGridView.Refresh();
            dataGridView.Invalidate();
            dataGridView.Update();

            var dateCols = dataGridView.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Name.IndexOf("fecha", StringComparison.OrdinalIgnoreCase) >= 0
                         || c.Name.IndexOf("date", StringComparison.OrdinalIgnoreCase) >= 0
                         || c.Name.IndexOf("vigencia", StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (dateCols.Count > 0 && dt != null)
            {
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    foreach (var col in dateCols)
                    {
                        string colName = col.Name;
                        if (!dt.Columns.Contains(colName)) continue;
                        var val = dt.Rows[r][colName];
                        if (val == null || val == DBNull.Value) continue;
                        string s = val.ToString().Trim();
                        if (string.IsNullOrEmpty(s)) continue;

                        DateTime parsed;
                        bool parsedOk = DateTime.TryParse(s, out parsed)
                                        || DateTime.TryParseExact(s, new[] { "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy" },
                                            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsed);

                        if (parsedOk)
                        {
                            string normalized = parsed.ToString("dd/MM/yyyy");
                            if (dt.Rows[r][colName].ToString() != normalized)
                            {
                                dt.Rows[r][colName] = normalized;
                                System.Diagnostics.Debug.WriteLine($"[RefreshDataGrid] Updated DT row {r + 1} col {colName}: {normalized}");
                            }

                            try
                            {
                                var cell = dataGridView.Rows[r].Cells[colName];
                                if (cell != null && cell.Value?.ToString() != normalized)
                                {
                                    cell.Value = normalized;
                                    System.Diagnostics.Debug.WriteLine($"[RefreshDataGrid] Updated Grid row {r + 1} col {colName}: {normalized}");
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private void CompareDataTableAndGrid(DataTable dt, int takeRows = 5)
        {
            int rows = Math.Min(takeRows, dt.Rows.Count);
            for (int r = 0; r < rows; r++)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    var valDt = dt.Rows[r][c];
                    string sDt = valDt == DBNull.Value ? "(NULL)" : valDt.ToString();
                    string sGrid = "(no mostrado)";
                    try
                    {
                        var cell = dataGridView.Rows[r].Cells[c.ColumnName];
                        sGrid = cell?.Value == null ? "(NULL)" : cell.Value.ToString();
                    }
                    catch { }
                    System.Diagnostics.Debug.WriteLine($"[Compare] Fila {r + 1} Col {c.ColumnName} -> DataTable: '{sDt}' | Grid: '{sGrid}'");
                }
            }
        }

        private void HighlightErrorRows()
        {
            if (_validationResults.Count == 0) return;

            var errorRows = new HashSet<int>(_validationResults.Select(r => r.RowIndex));
            var dupRows = new HashSet<int>(_validationResults
                .Where(r => r.ErrorType == ErrorType.Duplicate).Select(r => r.RowIndex));

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                int rowIdx = row.Index;
                if (dupRows.Contains(rowIdx))
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 180);
                else if (errorRows.Contains(rowIdx))
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 200);
            }
        }


        private void BuildReport()
        {
            richTextBoxReport.Clear();
            AppendReport("═══════════════════════════════════════════════════════════", Color.FromArgb(100, 149, 237));
            AppendReport($"  ANÁLISIS DE CALIDAD  •  {DateTime.Now:dd/MM/yyyy HH:mm:ss}", Color.White);
            AppendReport("═══════════════════════════════════════════════════════════", Color.FromArgb(100, 149, 237));
            AppendReport($"  Errores totales: {_stats.ErrorsFound}   |   " +
                         $"Duplicados: {_stats.DuplicatesFound}   |   " +
                         $"Nulos: {_stats.NullValues}   |   " +
                         $"Calidad: {_stats.QualityPercent:F1}%", Color.FromArgb(200, 200, 200));
            AppendReport("");

            if (_validationResults.Count == 0)
            {
                AppendReport("✅ ¡Excelente! No se encontraron errores en el dataset.",
                    Color.FromArgb(39, 174, 96));
                return;
            }

            var groups = _validationResults.GroupBy(r => r.ErrorType).OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                AppendReport($"\n── {group.First().ErrorTypeName} ({group.Count()}) ──",
                    Color.FromArgb(100, 149, 237));

                foreach (var err in group.Take(50))
                {
                    AppendReport($"  [F{err.RowIndex + 1}][{err.ColumnName}]  {err.CurrentValue}",
                        Color.FromArgb(255, 160, 122));
                    AppendReport($"    → {err.Description}", Color.FromArgb(200, 200, 200));
                    if (!string.IsNullOrEmpty(err.SuggestedValue))
                        AppendReport($"    💡 {err.SuggestedValue}", Color.FromArgb(144, 238, 144));
                }

                if (group.Count() > 50)
                    AppendReport($"  ... y {group.Count() - 50} errores más del mismo tipo.",
                        Color.FromArgb(150, 150, 150));
            }
        }

        private void AppendReport(string text, Color? color = null)
        {
            richTextBoxReport.SelectionColor = color ?? Color.FromArgb(230, 230, 230);
            richTextBoxReport.AppendText(text + "\n");
            richTextBoxReport.ScrollToCaret();
        }

        private void ClearReport() => richTextBoxReport.Clear();

        private void UpdateStatsDisplay()
        {
            var dt = _displayedData ?? (_originalData ?? _workingData);
            _stats.TotalRecords = dt?.Rows.Count ?? 0;
            _stats.TotalColumns = dt?.Columns.Count ?? 0;

            lblStatRecords.Text = $"📝 Registros:      {_stats.TotalRecords}";
            lblStatColumns.Text = $"📋 Columnas:       {_stats.TotalColumns}";
            lblStatErrors.Text = $"❌ Errores:        {_stats.ErrorsFound}";
            lblStatDups.Text = $"♻️  Duplicados:     {_stats.DuplicatesFound}";
            lblStatNulls.Text = $"⬜ Nulos:          {_stats.NullValues}";
            lblStatFixed.Text = $"✅ Correcciones:  {_stats.CorrectionsApplied}";
            lblStatQuality.Text = $"📊 Calidad:        {_stats.QualityPercent:F1}%";

            lblStatQuality.ForeColor = _stats.QualityPercent >= 80 ? Color.FromArgb(39, 174, 96)
                                     : _stats.QualityPercent >= 50 ? Color.FromArgb(230, 126, 34)
                                     : Color.FromArgb(192, 57, 43);
        }

        private void SetProgress(int value, string message)
        {
            progressBar.Value = Math.Min(Math.Max(value, 0), 100);
            lblProgress.Text = message;
            Application.DoEvents();
        }

        private void SetStatus(string message)
            => toolStripStatus.Text = message;

        private void EnableButtons(bool enabled)
        {
            var hasData = _originalData != null;
            btnAnalyze.Enabled = enabled && hasData;
            btnAutoFix.Enabled = enabled && hasData;
            btnRemoveDups.Enabled = enabled && hasData;
            btnApplyChanges.Enabled = enabled && hasData;
            btnRevert.Enabled = enabled && hasData;
            btnExportReport.Enabled = enabled && hasData;
            if (btnSaveFile != null)
                btnSaveFile.Enabled = enabled && hasData;
            btnImport.Enabled = enabled;
        }

        private void UpdateButtonStates(bool fileLoaded)
        {
            btnAnalyze.Enabled = fileLoaded;
            btnAutoFix.Enabled = fileLoaded;
            btnRemoveDups.Enabled = fileLoaded;
            btnApplyChanges.Enabled = fileLoaded;
            btnRevert.Enabled = fileLoaded;
            btnExportReport.Enabled = fileLoaded;
            if (btnSaveFile != null) btnSaveFile.Enabled = fileLoaded;
        }

    }
}
