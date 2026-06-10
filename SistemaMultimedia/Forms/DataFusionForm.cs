using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SistemaMultimedia.DataFusion;
using SistemaMultimedia.Integration;
using SistemaMultimedia.Utilities;
using VisorEditorDocumentos.Services;

namespace SistemaMultimedia.Forms
{
    public partial class DataFusionForm : Form
    {
        private DataTable _dataTable = new();
        private DataTable _originalTable = new();
        private string? _loadedFilePath;

        public DataFusionForm()
        {
            InitializeComponent();

            ddvDatos.AutoGenerateColumns = true;
            ddvDatos.DataBindingComplete += DdvDatos_DataBindingComplete;

            if (this.btnGroup != null)
                this.btnGroup.Click += BtnGroup_Click;

            if (this.cmbDB != null)
            {
                cmbDB.Items.Clear();
                cmbDB.Items.Add("SQL Server");
                cmbDB.Items.Add("MariaDB");
                cmbDB.Items.Add("PostgreSQL");
                cmbDB.SelectedIndex = 0;
            }

            UpdateSummaryLabel(null);

            if (this.btnLoadFromDb != null)
                this.btnLoadFromDb.Click += btnLoadFromDb_Click;

            if (btnValidate != null)
                btnValidate.Click += BtnValidate_Click;
        }

        public DataFusionForm(string filePath) : this()
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                Shown += (_, _) => CargarArchivoDesdeRuta(filePath);
            }
        }

        public void CargarArchivoDesdeRuta(string filePath)
        {
            _loadedFilePath = filePath;
            try
            {
                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                DataTable dt = ext switch
                {
                    ".json" => LoadJsonToDataTable(filePath),
                    ".xml" => LoadXmlToDataTable(filePath),
                    _ => LoadCsvToDataTable(filePath)
                };

                EnsureOriginalIndexColumn(dt);
                _dataTable = dt;
                _originalTable = dt.Copy();
                ddvDatos.DataSource = _dataTable.DefaultView;

                ApplySortFromCombo();
                ConfigureGridColumns();
                UpdateSummaryLabel();
                Text = $"DataFusion - {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error al leer archivo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -------------------------
        // CARGA DE ARCHIVOS -> DataTable
        // -------------------------
        private async void btnLoad_Click(object? sender, EventArgs e)
        {
            using var _wait = new WaitCursorScope();
            using var dlg = new OpenFileDialog
            {
                Filter = "Archivos (*.csv;*.txt;*.json;*.xml)|*.csv;*.txt;*.json;*.xml|CSV (*.csv)|*.csv|Texto (*.txt)|*.txt|JSON (*.json)|*.json|XML (*.xml)|*.xml|Todos (*.*)|*.*",
                Title = "Seleccionar archivo"
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            _loadedFilePath = dlg.FileName;
            try
            {
                var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
                DataTable dt;

                if (ext == ".json")
                    dt = LoadJsonToDataTable(dlg.FileName);
                else if (ext == ".xml")
                    dt = LoadXmlToDataTable(dlg.FileName);
                else
                    dt = LoadCsvToDataTable(dlg.FileName);

                // asegurar columna de índice original para poder "restaurar orden original"
                EnsureOriginalIndexColumn(dt);

                _dataTable = dt;
                _originalTable = dt.Copy();

                // Asignar DataSource (orden inicial gestionado por ApplySortFromCombo / __original_index)
                ddvDatos.DataSource = _dataTable.DefaultView;

                ApplySortFromCombo();
                ConfigureGridColumns();
                UpdateSummaryLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error al leer archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnValidate_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_loadedFilePath) || !File.Exists(_loadedFilePath))
            {
                MessageBox.Show(this, "Cargue un archivo de datos antes de abrir la validación.",
                    "Sin archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataValidationLauncher.Open(_loadedFilePath, this);
        }

        private static void EnsureOriginalIndexColumn(DataTable dt)
        {
            DataTableHelpers.EnsureOriginalIndexColumn(dt);
        }

        private DataTable LoadCsvToDataTable(string path, Encoding? encoding = null)
        {
            // intentar CsvLoader primero (manejo de encoding y comillas)
            try
            {
                var doc = CsvLoader.ReadRaw(path, hasHeader: true, encoding: encoding);
                if (doc != null && (doc.Headers.Count > 0 || doc.Rows.Count > 0))
                {
                    var mapping = CsvLoader.AutoMapHeadersToDataItem(doc.Headers);
                    DataTable dt;
                    if (mapping != null)
                        dt = CsvLoader.MapRowsToDataTable(doc, mapping);
                    else
                    {
                        dt = new DataTable("Imported");
                        if (doc.Headers.Count > 0)
                        {
                            foreach (var h in doc.Headers)
                            {
                                var name = string.IsNullOrWhiteSpace(h) ? $"Col{dt.Columns.Count + 1}" : h;
                                var final = name; int k = 1;
                                while (dt.Columns.Contains(final)) { final = name + "_" + k; k++; }
                                dt.Columns.Add(final, typeof(string));
                            }
                        }
                        else if (doc.Rows.Count > 0)
                        {
                            int maxCols = doc.Rows.Max(r => r?.Length ?? 0);
                            for (int i = 0; i < maxCols; i++) dt.Columns.Add($"Col{i + 1}", typeof(string));
                        }

                        foreach (var row in doc.Rows)
                        {
                            var dr = dt.NewRow();
                            for (int c = 0; c < dt.Columns.Count; c++)
                                dr[c] = (row != null && c < row.Length) ? (row[c] ?? string.Empty) : string.Empty;
                            dt.Rows.Add(dr);
                        }
                    }

                    InferColumnTypes(dt);
                    EnsureOriginalIndexColumn(dt);
                    return dt;
                }
            }
            catch { /* fallback abajo */ }

            // fallback: TxtToDataItems (detecta separador, unwrap, etc.)
            var fallback = TxtToDataItems.LoadBySeparator(path, separator: '|', hasHeader: true, encoding: encoding);
            InferColumnTypes(fallback);
            EnsureOriginalIndexColumn(fallback);
            return fallback;
        }

        // Intenta inferir tipos (int/decimal) y convertir columnas del DataTable
        private static void InferColumnTypes(DataTable dt)
        {
            DataTableHelpers.InferColumnTypes(dt);
        }

        // JSON -> DataTable. Soporta array JSON y NDJSON y objetos planos (flatten)
        private DataTable LoadJsonToDataTable(string path, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var text = File.ReadAllText(path, encoding);
            var dt = new DataTable("ImportedJson");

            var candidates = new List<Dictionary<string, string>>(); // cada item = diccionario plano name->value

            // intentar parsear como JSON array / objeto
            try
            {
                using var doc = JsonDocument.Parse(text);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in root.EnumerateArray())
                    {
                        if (el.ValueKind == JsonValueKind.Object)
                            candidates.Add(FlattenJsonObject(el));
                    }
                }
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    candidates.Add(FlattenJsonObject(root));
                }
            }
            catch
            {
                // intentar NDJSON: cada línea json
                using var sr = new StringReader(text);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        using var doc = JsonDocument.Parse(line);
                        var root = doc.RootElement;
                        if (root.ValueKind == JsonValueKind.Object)
                            candidates.Add(FlattenJsonObject(root));
                    }
                    catch { continue; }
                }
            }

            if (!candidates.Any()) return dt;

            // union de columnas
            var columns = candidates.SelectMany(d => d.Keys).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var col in columns)
                dt.Columns.Add(col, typeof(string)); // string por defecto

            // agregar filas
            foreach (var dic in candidates)
            {
                var row = dt.NewRow();
                foreach (var c in columns)
                {
                    if (dic.TryGetValue(c, out var v))
                        row[c] = v ?? string.Empty;
                    else
                        row[c] = string.Empty;
                }
                dt.Rows.Add(row);
            }

            InferColumnTypes(dt);
            return dt;
        }

        private void BtnGroup_Click(object? sender, EventArgs e)
        {
            if (_dataTable == null || _dataTable.Rows.Count == 0)
            {
                MessageBox.Show(this, "No hay datos cargados para agrupar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Mostrar diálogo para seleccionar columnas por las que agrupar
            var selected = GroupByDialog.ShowDialogAndGetColumns(this, _dataTable);
            if (selected == null) return; // usuario canceló

            // Crear grupos dinámicos usando las columnas seleccionadas (si no selecciona ninguna, devuelve "(Todos)")
            var groups = GroupingHelpers.GroupByColumns(_dataTable, selected);

            if (groups == null || groups.Count == 0)
            {
                MessageBox.Show(this, "No se obtuvieron grupos con las columnas seleccionadas.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frm = new GroupSummaryForm(groups);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(frm.SelectedCategory))
                    ddvDatos.DataSource = _dataTable.DefaultView;
                else if (groups.TryGetValue(frm.SelectedCategory, out var tbl))
                    ddvDatos.DataSource = tbl.DefaultView;

                // volver al comportamiento anterior: sólo repoblar combo de categoría


                ConfigureGridColumns();
                ApplySortFromCombo();
                UpdateSummaryLabel();
            }
        }
        // aplana objeto JSON (propiedad anidada se concatena con '_')
        private static Dictionary<string, string> FlattenJsonObject(JsonElement el, string prefix = "")
        {
            var result = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            foreach (var prop in el.EnumerateObject())
            {
                var name = string.IsNullOrWhiteSpace(prefix) ? prop.Name : prefix + "_" + prop.Name;
                var v = prop.Value;
                switch (v.ValueKind)
                {
                    case JsonValueKind.Object:
                        foreach (var kv in FlattenJsonObject(v, name))
                            result[kv.Key] = kv.Value;
                        break;
                    case JsonValueKind.Array:
                        // unir array como JSON compacto
                        result[name] = v.GetRawText();
                        break;
                    case JsonValueKind.String:
                        result[name] = v.GetString() ?? string.Empty;
                        break;
                    case JsonValueKind.Number:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                    default:
                        result[name] = v.GetRawText();
                        break;
                }
            }
            return result;
        }

        // XML -> DataTable: intenta tomar hijos de root como registros; aplanar elementos hoja
        private DataTable LoadXmlToDataTable(string path, Encoding? encoding = null)
        {
            var dt = XmlToDataItems.Load(path, encoding);
            InferColumnTypes(dt);
            EnsureOriginalIndexColumn(dt);
            return dt;
        }

        // Busca nombre de columna que represente 'categoria'
        private static string? FindCategoriaColumn(DataTable dt)
        {
            if (dt == null) return null;

            // buscar coincidencia exacta ignorando mayúsculas/minúsculas
            var exact = dt.Columns.Cast<DataColumn>()
                          .FirstOrDefault(c => string.Equals(c.ColumnName, "Categoria", StringComparison.OrdinalIgnoreCase));
            if (exact != null) return exact.ColumnName;

            // búsqueda por parecido (categoria / category / grupo)
            var c = dt.Columns.Cast<DataColumn>()
             .FirstOrDefault(x =>
                 x.ColumnName.IndexOf("categoria", StringComparison.OrdinalIgnoreCase) >= 0
                 || x.ColumnName.IndexOf("category", StringComparison.OrdinalIgnoreCase) >= 0
                 || x.ColumnName.IndexOf("grupo", StringComparison.OrdinalIgnoreCase) >= 0);
            return c?.ColumnName;
        }

        private static string? FindValorColumn(DataTable dt)
        {
            if (dt == null) return null;

            // búsqueda exacta case-insensitive
            var exact = dt.Columns.Cast<DataColumn>()
                          .FirstOrDefault(c => string.Equals(c.ColumnName, "Valor", StringComparison.OrdinalIgnoreCase));
            if (exact != null) return exact.ColumnName;

            // búsqueda por parecido (valor / amount / total)
            var c = dt.Columns.Cast<DataColumn>()
             .FirstOrDefault(x =>
                 x.ColumnName.IndexOf("valor", StringComparison.OrdinalIgnoreCase) >= 0
                 || x.ColumnName.IndexOf("amount", StringComparison.OrdinalIgnoreCase) >= 0
                 || x.ColumnName.IndexOf("total", StringComparison.OrdinalIgnoreCase) >= 0);
            return c?.ColumnName;
        }

        private static string SanitizeSqlColumnName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Col";
            var s = Regex.Replace(name.Trim(), @"[^\p{L}\p{Nd}_]+", "_");
            if (char.IsDigit(s[0])) s = "_" + s;
            return s;
        }

        private static DataTable BuildSanitizedTableForSql(DataTable src)
        {
            var dst = new DataTable(src.TableName ?? "Imported");
            var nameMap = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            foreach (DataColumn c in src.Columns)
            {
                var sanitized = SanitizeSqlColumnName(c.ColumnName);
                var baseName = sanitized; int k = 1;
                while (dst.Columns.Contains(sanitized)) { sanitized = baseName + "_" + k; k++; }
                nameMap[c.ColumnName] = sanitized;
                dst.Columns.Add(new DataColumn(sanitized, c.DataType) { AllowDBNull = c.AllowDBNull });
            }

            foreach (DataRow r in src.Rows)
            {
                var nr = dst.NewRow();
                foreach (DataColumn c in src.Columns)
                {
                    var targetName = nameMap[c.ColumnName];
                    nr[targetName] = r.IsNull(c) ? DBNull.Value : r[c];
                }
                dst.Rows.Add(nr);
            }

            return dst;
        }
        // Manejador para el botón "Guardar BD" (referenciado desde el diseñador).
        // Implementación mínima para evitar error CS0103; sustituye por la lógica real de migración/guardado.
        private async void btnMigrateToSql_Click(object? sender, EventArgs e)
        {
            DataTable tableToSave;
            if (ddvDatos.DataSource is DataView dv)
                tableToSave = dv.ToTable();
            else
                tableToSave = _dataTable ?? new DataTable();

            if (tableToSave.Rows.Count == 0)
            {
                MessageBox.Show(this, "No hay datos para guardar en la base de datos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedEngine = cmbDB?.SelectedItem?.ToString() ?? string.Empty;

            // Ruta MariaDB: reutilizar SqlConnectionDialog para pedir cadena/base/tabla
            if (selectedEngine.IndexOf("mariadb", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                using var dlg = new SqlConnectionDialog(
                    defaultConnection: "Server=localhost;User ID=root;Password=;",
                    defaultDatabase: "imported_data",
                    defaultTable: "ImportedData"
                );

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var connectionString = dlg.ConnectionString;
                var databaseName = string.IsNullOrWhiteSpace(dlg.DatabaseName) ? "imported_data" : dlg.DatabaseName;
                var tableName = string.IsNullOrWhiteSpace(dlg.TableName) ? "ImportedData" : dlg.TableName;

                var repo = new MariaDbRepository(connectionString, databaseName);

                try
                {
                    var (ok, msg) = await repo.TestConnectionAsync();
                    if (!ok)
                    {
                        MessageBox.Show(this, $"No se puede conectar a MariaDB: {msg}", "Error conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    await repo.InitializeAsync();

                    var sanitizedTable = BuildSanitizedTableForSql(tableToSave);
                    await repo.CreateTableFromDataTable(sanitizedTable, tableName);
                    await repo.SaveDataTableAsync(sanitizedTable, tableName);

                    try
                    {
                        var loaded = await repo.LoadItemsAsync(tableName);
                        int rowsInDb = loaded?.Rows?.Count ?? 0;
                        MessageBox.Show(this, $"Datos guardados en MariaDB. Filas en la tabla {tableName}: {rowsInDb}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception exVerify)
                    {
                        MessageBox.Show(this, $"Guardado OK pero fallo verificación: {exVerify.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Error al guardar en MariaDB: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            // Ruta PostgreSQL: reutilizar SqlConnectionDialog para pedir cadena/base/tabla
            if (selectedEngine.IndexOf("postgres", StringComparison.OrdinalIgnoreCase) >= 0
                || selectedEngine.IndexOf("postgresql", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                using var dlg = new SqlConnectionDialog(
                    defaultConnection: "Host=localhost;Username=postgres;Password=;",
                    defaultDatabase: "imported_data",
                    defaultTable: "ImportedData"
                );

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var connectionString = dlg.ConnectionString;
                var databaseName = string.IsNullOrWhiteSpace(dlg.DatabaseName) ? "imported_data" : dlg.DatabaseName;
                var tableName = string.IsNullOrWhiteSpace(dlg.TableName) ? "ImportedData" : dlg.TableName;

                var repo = new PostgreSqlRepository(connectionString, databaseName);

                try
                {
                    var (ok, msg) = await repo.TestConnectionAsync();
                    if (!ok)
                    {
                        MessageBox.Show(this, $"No se puede conectar a PostgreSQL: {msg}", "Error conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    await repo.InitializeAsync();

                    var sanitizedTable = BuildSanitizedTableForSql(tableToSave);
                    await repo.CreateTableFromDataTable(sanitizedTable, tableName, "public");
                    await repo.SaveDataTableAsync(sanitizedTable, tableName, "public");

                    try
                    {
                        var loaded = await repo.LoadItemsAsync(tableName, "public");
                        int rowsInDb = loaded?.Rows?.Count ?? 0;
                        MessageBox.Show(this, $"Datos guardados en PostgreSQL. Filas en la tabla {tableName}: {rowsInDb}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception exVerify)
                    {
                        MessageBox.Show(this, $"Guardado OK pero fallo verificación: {exVerify.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Error al guardar en PostgreSQL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            // Ruta SQL Server (comportamiento existente)
            using var sqlDlg = new SqlConnectionDialog(
                defaultConnection: "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;",
                defaultDatabase: "ImportedDataDb",
                defaultTable: "ImportedData"
            );

            if (sqlDlg.ShowDialog(this) != DialogResult.OK) return;

            var sqlConnectionString = sqlDlg.ConnectionString;
            var sqlDatabaseName = string.IsNullOrWhiteSpace(sqlDlg.DatabaseName) ? "ImportedDataDb" : sqlDlg.DatabaseName;
            var sqlTableName = string.IsNullOrWhiteSpace(sqlDlg.TableName) ? "ImportedData" : sqlDlg.TableName;

            var sqlRepo = new SqlServerRepository(sqlConnectionString, sqlDatabaseName);

            try
            {
                var (okSql, msgSql) = await sqlRepo.TestConnectionAsync();
                if (!okSql)
                {
                    MessageBox.Show(this, $"No se puede conectar a SQL Server: {msgSql}", "Error conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await sqlRepo.InitializeAsync();
                var sanitizedTableSql = BuildSanitizedTableForSql(tableToSave);
                await sqlRepo.CreateTableFromDataTable(sanitizedTableSql, sqlTableName, "dbo");
                await sqlRepo.SaveDataTableAsync(sanitizedTableSql, sqlTableName, "dbo");

                try
                {
                    var loaded = await sqlRepo.LoadItemsAsync(sqlTableName, "dbo");
                    int rowsInDb = loaded?.Rows?.Count ?? 0;
                    MessageBox.Show(this, $"Datos guardados en SQL Server. Filas en la tabla {sqlTableName}: {rowsInDb}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception exVerify)
                {
                    MessageBox.Show(this, $"Guardado OK pero fallo verificación: {exVerify.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error al guardar en SQL Server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -------------------------
        // FILTROS / POPULATE COMBO
        // -------------------------
        public void PopulateCategoryComboBox(ComboBox combo)
        {
            if (combo == null) return;
            combo.Items.Clear();
            combo.Items.Add("(Todos)");
            if (_dataTable == null || _dataTable.Columns.Count == 0)
            {
                combo.SelectedIndex = 0;
                return;
            }

            var catCol = FindCategoriaColumn(_dataTable);
            if (catCol == null)
            {
                combo.SelectedIndex = 0;
                return;
            }

            var values = _dataTable.AsEnumerable()
                           .Select(r => r.Field<object>(catCol)?.ToString()?.Trim() ?? string.Empty)
                           .Where(s => !string.IsNullOrEmpty(s))
                           .Distinct(StringComparer.CurrentCultureIgnoreCase)
                           .OrderBy(s => s, StringComparer.CurrentCultureIgnoreCase)
                           .ToList();

            foreach (var v in values) combo.Items.Add(v);
            combo.SelectedIndex = 0;
        }

        private void PopulateSortOrderComboBox(ComboBox combo)
        {
            if (combo == null) return;
            combo.Items.Clear();
            combo.Items.Add("Orden original");

            // tabla actual (DataView si procede)
            DataTable table = null;
            if (ddvDatos.DataSource is DataView dv) table = dv.Table;
            else table = _dataTable;

            if (table == null || table.Columns.Count == 0)
            {
                combo.SelectedIndex = 0;
                return;
            }

            // Priorizar columnas numéricas o con nombres típicos de valor
            var valueNames = new[] { "valor", "total", "amount", "value", "price", "precio", "importe", "subtotal" };
            var candidates = table.Columns.Cast<DataColumn>()
                           .OrderByDescending(c =>
                           {
                               var vt = c.DataType;
                               int score = 0;
                               if (vt == typeof(decimal) || vt == typeof(double) || vt == typeof(int)) score += 4;
                               if (valueNames.Any(n => c.ColumnName.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0)) score += 8;
                               return score;
                           })
                           .Select(c => c.ColumnName)
                           .ToList();

            foreach (var col in candidates)
            {
                // añadir ambas direcciones
                combo.Items.Add($"{col} (Mayor a menor)");
                combo.Items.Add($"{col} (Menor a mayor)");
            }

            combo.SelectedIndex = 0;
        }

        public void FilterByCategoria(string? categoria)
        {
            if (ddvDatos.DataSource is not DataView dv) return;
            if (string.IsNullOrWhiteSpace(categoria) || categoria == "(Todos)")
            {
                dv.RowFilter = string.Empty;
            }
            else
            {
                // escapar comillas simples en el valor
                var val = categoria.Replace("'", "''");
                var catCol = FindCategoriaColumn(_dataTable);
                if (catCol == null)
                {
                    dv.RowFilter = string.Empty;
                }
                else
                {
                    // usar igualdad exacta (como antes)
                    dv.RowFilter = $"[{catCol}] = '{val.Replace("'", "''")}'";
                }
            }

            ConfigureGridColumns();
            UpdateSummaryLabel();
        }

        // -------------------------
        // ORDENAMIENTO
        // -------------------------
        private void ApplySortFromCombo()
        {
            // Como el combo de orden se eliminó, restaurar orden por índice original por defecto.
            var dv = ddvDatos.DataSource as DataView;
            if (dv == null) return;
            dv.Sort = "__original_index ASC";
        }

        // -------------------------
        // Configuración DataGridView
        // -------------------------
        private void DdvDatos_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            ConfigureGridColumns();
            UpdateSummaryLabel();
        }

        private void ConfigureGridColumns()
        {
            if (ddvDatos.Columns.Count == 0) return;

            ddvDatos.SuspendLayout();
            ddvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            DataGridViewColumn valorCol = null;
            foreach (DataGridViewColumn col in ddvDatos.Columns)
            {
                if (string.Equals(col.Name, "Valor", StringComparison.OrdinalIgnoreCase))
                {
                    valorCol = col;
                    continue;
                }

                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                int pref = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
                col.Width = Math.Min(Math.Max(pref + 8, 40), 1000);

                // detectar numéricos por tipo o por nombre
                var valueType = col.ValueType ?? typeof(string);
                if (valueType == typeof(decimal) || valueType == typeof(double) || valueType == typeof(int)
                    || col.Name.Equals("Cantidad", StringComparison.OrdinalIgnoreCase)
                    || col.Name.Equals("PrecioUnitario", StringComparison.OrdinalIgnoreCase)
                    || col.Name.IndexOf("valor", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    if (col.Name.Equals("PrecioUnitario", StringComparison.OrdinalIgnoreCase) || valueType == typeof(decimal))
                        col.DefaultCellStyle.Format = "N2";
                }
            }

            int idx = 0;
            foreach (DataGridViewColumn col in ddvDatos.Columns)
            {
                if (valorCol != null && col == valorCol) continue;
                col.DisplayIndex = idx++;
            }

            if (valorCol != null)
            {
                valorCol.DisplayIndex = ddvDatos.Columns.Count - 1;
                valorCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                valorCol.DefaultCellStyle.Format = "N2";
                valorCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                valorCol.MinimumWidth = 80;
            }

            ddvDatos.ResumeLayout();
        }

        private async Task SaveDataTableToFileAsync(DataTable table)
        {
            using var _wait = new WaitCursorScope();
            if (table == null || table.Rows.Count == 0)
            {
                MessageBox.Show(this, "No hay datos para guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Filter = "JSON (*.json)|*.json|CSV (*.csv)|*.csv|XML (*.xml)|*.xml|Texto tabulado (*.txt)|*.txt|Word (*.docx)|*.docx|PDF (*.pdf)|*.pdf|Excel (*.xlsx)|*.xlsx|Todos (*.*)|*.*",
                DefaultExt = "json",
                AddExtension = true,
                Title = "Guardar datos",
                FileName = "datos.json"
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var path = dlg.FileName;
            var ext = Path.GetExtension(path).ToLowerInvariant();

            try
            {
                var exportTable = BuildExportTable(table);
                if (ext == ".json")
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(DataTableToDictionaryList(exportTable), options), Encoding.UTF8);
                }
                else if (ext == ".csv" || ext == ".txt")
                {
                    char sep = ext == ".csv" ? ',' : '\t';
                    await File.WriteAllTextAsync(path, DataTableToDelimitedText(exportTable, sep), Encoding.UTF8);
                }
                else if (ext == ".xml")
                {
                    await File.WriteAllTextAsync(path, DataTableToXmlString(exportTable), Encoding.UTF8);
                }
                else if (ext == ".docx")
                {
                    var html = ExcelHtmlConverter.DataTableToBodyHtml(exportTable);
                    DocumentExportService.ExportHtmlAsDocx(html, path);
                }
                else if (ext == ".pdf")
                {
                    var html = ExcelHtmlConverter.DataTableToBodyHtml(exportTable);
                    DocumentExportService.ExportHtmlAsPdf(html, path, "DataFusion");
                }
                else if (ext == ".xlsx")
                {
                    var html = ExcelHtmlConverter.DataTableToBodyHtml(exportTable);
                    DocumentExportService.ExportHtmlAsXlsx(html, path);
                }
                else
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(DataTableToDictionaryList(exportTable), options), Encoding.UTF8);
                }

                MessageBox.Show(this, "Fichero guardado correctamente.", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error al guardar fichero: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string EscapeField(string value, char separator)
        {
            if (value == null) return string.Empty;
            bool mustQuote = value.Contains(separator) || value.Contains('"') || value.Contains('\r') || value.Contains('\n');
            if (!mustQuote) return value;
            var escaped = value.Replace("\"", "\"\"");
            return $"\"{escaped}\"";
        }

        // Handler guardado (diseñador puede apuntar aquí)
        private async void btnSaveJson_Click(object? sender, EventArgs e)
        {
            DataTable tableToSave;
            if (ddvDatos.DataSource is DataView dv)
                tableToSave = dv.ToTable();
            else
                tableToSave = _dataTable;

            await SaveDataTableToFileAsync(tableToSave);
        }

        // -------------------------
        // RESUMEN
        // -------------------------
        private void UpdateSummaryLabel(DataTable? source = null)
        {
            DataTable dt;
            if (source != null) dt = source;
            else if (ddvDatos.DataSource is DataView dv) dt = dv.ToTable();
            else dt = _dataTable ?? new DataTable();

            int count = dt.Rows.Count;
            decimal total = 0m;
            var valCol = FindValorColumn(dt);
            if (!string.IsNullOrEmpty(valCol))
            {
                try
                {
                    total = dt.AsEnumerable().Sum(r =>
                    {
                        if (r.IsNull(valCol)) return 0m;
                        var o = r[valCol];
                        if (o is decimal d) return d;
                        if (o is double db) return (decimal)db;
                        if (o is int i) return (decimal)i;
                        if (decimal.TryParse(Convert.ToString(o, CultureInfo.CurrentCulture), NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed)) return parsed;
                        if (decimal.TryParse(Convert.ToString(o, CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out parsed)) return parsed;
                        return 0m;
                    });
                }
                catch { total = 0m; }
            }

            if (count == 0)
            {
                lblSummary.Text = "No hay datos cargados.";
            }
            else
            {
                lblSummary.Text = $"Registros: {count}    Total Valor: {total.ToString("N2", CultureInfo.CurrentCulture)}";
            }
        }
        private async void btnLoadFromDb_Click(object? sender, EventArgs e)
        {
            using var _wait = new WaitCursorScope();
            var selectedEngine = cmbDB?.SelectedItem?.ToString() ?? string.Empty;

            string defaultConn = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;";
            string defaultDb = "ImportedDataDb";
            if (selectedEngine.IndexOf("mariadb", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                defaultConn = "Server=localhost;User ID=root;Password=;";
                defaultDb = "imported_data";
            }
            else if (selectedEngine.IndexOf("postgres", StringComparison.OrdinalIgnoreCase) >= 0
                     || selectedEngine.IndexOf("postgresql", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                defaultConn = "Host=localhost;Username=postgres;Password=;";
                defaultDb = "imported_data";
            }

            using var dlg = new SqlConnectionDialog(defaultConnection: defaultConn, defaultDatabase: defaultDb, defaultTable: "");
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var connStr = dlg.ConnectionString;
            var dbName = dlg.DatabaseName;
            var tableInput = dlg.TableName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tableInput))
            {
                MessageBox.Show(this, "Especifica el nombre de la tabla a cargar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataTable loaded = null;

                if (selectedEngine.IndexOf("mariadb", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var repo = new MariaDbRepository(connStr, dbName);
                    var (ok, msg) = await repo.TestConnectionAsync();
                    if (!ok) { MessageBox.Show(this, $"No se puede conectar a MariaDB: {msg}", "Error conexión", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    await repo.InitializeAsync();
                    loaded = await repo.LoadItemsAsync(tableInput);
                }
                else if (selectedEngine.IndexOf("postgres", StringComparison.OrdinalIgnoreCase) >= 0
                         || selectedEngine.IndexOf("postgresql", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var schema = "public";
                    var tableName = tableInput;
                    if (tableInput.Contains('.'))
                    {
                        var parts = tableInput.Split(new[] { '.' }, 2);
                        schema = parts[0].Trim('"', ' ');
                        tableName = parts[1].Trim('"', ' ');
                    }

                    var repo = new PostgreSqlRepository(connStr, dbName);
                    var (ok, msg) = await repo.TestConnectionAsync();
                    if (!ok) { MessageBox.Show(this, $"No se puede conectar a PostgreSQL: {msg}", "Error conexión", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    await repo.InitializeAsync();
                    loaded = await repo.LoadItemsAsync(tableName, schema);
                }
                else // SQL Server
                {
                    var schema = "dbo";
                    var tableName = tableInput;
                    if (tableInput.Contains('.'))
                    {
                        var parts = tableInput.Split(new[] { '.' }, 2);
                        schema = parts[0].Trim('[', ']', ' ');
                        tableName = parts[1].Trim('[', ']', ' ');
                    }

                    var repo = new SqlServerRepository(connStr, dbName);
                    var (ok, msg) = await repo.TestConnectionAsync();
                    if (!ok) { MessageBox.Show(this, $"No se puede conectar a SQL Server: {msg}", "Error conexión", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    await repo.InitializeAsync();
                    loaded = await repo.LoadItemsAsync(tableName, schema);
                }

                if (loaded == null)
                {
                    MessageBox.Show(this, "No se obtuvieron datos de la tabla solicitada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Asegurar columna índice original y actualizar UI/estado
                EnsureOriginalIndexColumn(loaded);
                _dataTable = loaded;
                _originalTable = loaded.Copy();
                ddvDatos.DataSource = _dataTable.DefaultView;

                // NOTA: quitar orden forzado por Id — dejar que ApplySortFromCombo o __original_index gestionen el orden
                ApplySortFromCombo();
                ConfigureGridColumns();
                UpdateSummaryLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error al cargar desde la base de datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helpers para exportación (implementación mínima para compilar y comportamiento razonable)
        private static DataTable BuildExportTable(DataTable src)
        {
            if (src == null) return new DataTable("Export");
            var outDt = new DataTable(src.TableName ?? "Export");
            // Todas las columnas como string para facilitar export
            foreach (DataColumn c in src.Columns)
                outDt.Columns.Add(c.ColumnName, typeof(string));

            foreach (DataRow r in src.Rows)
            {
                var nr = outDt.NewRow();
                foreach (DataColumn c in src.Columns)
                {
                    var v = r[c];
                    nr[c.ColumnName] = v == null || v == DBNull.Value ? string.Empty : Convert.ToString(v, CultureInfo.CurrentCulture) ?? string.Empty;
                }
                outDt.Rows.Add(nr);
            }

            return outDt;
        }

        private static List<Dictionary<string, object?>> DataTableToDictionaryList(DataTable dt)
        {
            var list = new List<Dictionary<string, object?>>();
            if (dt == null) return list;
            foreach (DataRow r in dt.Rows)
            {
                var dict = new Dictionary<string, object?>(StringComparer.CurrentCultureIgnoreCase);
                foreach (DataColumn c in dt.Columns)
                {
                    dict[c.ColumnName] = r.IsNull(c) ? null : r[c];
                }
                list.Add(dict);
            }
            return list;
        }

        private static string DataTableToDelimitedText(DataTable dt, char separator)
        {
            if (dt == null) return string.Empty;
            var sb = new StringBuilder();
            // encabezado
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0) sb.Append(separator);
                sb.Append(EscapeField(dt.Columns[i].ColumnName, separator));
            }
            sb.AppendLine();

            foreach (DataRow r in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(separator);
                    var val = r.IsNull(i) ? string.Empty : Convert.ToString(r[i], CultureInfo.CurrentCulture) ?? string.Empty;
                    sb.Append(EscapeField(val, separator));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string DataTableToXmlString(DataTable dt)
        {
            if (dt == null) return string.Empty;
            try
            {
                var ds = new DataSet(dt.TableName ?? "Export");
                ds.Tables.Add(dt.Copy());
                using var sw = new StringWriter();
                ds.WriteXml(sw, System.Data.XmlWriteMode.IgnoreSchema);
                return sw.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
