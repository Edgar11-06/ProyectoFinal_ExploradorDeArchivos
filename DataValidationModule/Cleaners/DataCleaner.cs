using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DataValidationModule.Common;

namespace DataValidationModule.Cleaners
{
    /// <summary>
    /// Clase encargada de aplicar correcciones automáticas al DataTable.
    /// Mantiene un historial de cambios para permitir la reversión de correcciones.
    /// 
    /// Uso típico:
    ///   var cleaner = new DataCleaner(dataTable);
    ///   int fixed = cleaner.TrimSpaces();
    ///   cleaner.NormalizeText();
    ///   cleaner.FixEmails();
    /// </summary>
    public class DataCleaner
    {
        // -------------------------------------------------------------------
        // Campos internos
        // -------------------------------------------------------------------

        private DataTable _dataTable;

        /// <summary>
        /// Snapshot del DataTable antes de cualquier corrección.
        /// Se utiliza para revertir todos los cambios.
        /// </summary>
        private DataTable _originalSnapshot;

        /// <summary>Número total de correcciones aplicadas en la sesión actual.</summary>
        public int TotalCorrections { get; private set; }

        /// <summary>Log detallado de cada cambio realizado (para el reporte).</summary>
        public List<string> ChangeLog { get; private set; } = new List<string>();

        // Mapa de caracteres especiales comunes → reemplazo
        private static readonly Dictionary<string, string> SpecialCharMap = new Dictionary<string, string>
        {
            { "á", "á" }, { "é", "é" }, { "í", "í" }, { "ó", "ó" }, { "ú", "ú" },
            { "Á", "Á" }, { "É", "É" }, { "Í", "Í" }, { "Ó", "Ó" }, { "Ú", "Ú" },
            { "&amp;",  "&"  },
            { "&lt;",   "<"  },
            { "&gt;",   ">"  },
            { "&quot;", "\"" },
            { "&#39;",  "'"  },
            { "\r\n",   " "  },
            { "\n",     " "  },
            { "\t",     " "  },
        };

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Inicializa el limpiador con el DataTable a limpiar.
        /// Toma un snapshot inicial para permitir revertir cambios.
        /// </summary>
        public DataTable DataTable => _dataTable;

        public DataCleaner(DataTable dataTable)
        {
            _dataTable       = dataTable ?? throw new ArgumentNullException(nameof(dataTable));
            _originalSnapshot = dataTable.Copy(); // snapshot para revert
        }

        /// <summary>
        /// Intenta parsear fechas aplicando heurísticas simples:
        /// - Reemplaza separadores distintos por - o /
        /// - Reordena partes si parece YYYY-DD-MM u otras combinaciones
        /// - Descarta valores con componentes fuera de rango
        /// </summary>
        private bool TryHeuristicParseDate(string raw, out DateTime parsed)
        {
            parsed = default;
            if (string.IsNullOrWhiteSpace(raw)) return false;

            // Normalizar separadores
            string s = raw.Trim().Replace('.', '/').Replace('-', '/').Replace('\\', '/');

            // Eliminar textos no numéricos comunes
            s = Regex.Replace(s, @"[^0-9/]", "");

            var parts = s.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return false;

            // Intentar combinaciones: dd/MM/yyyy, yyyy/MM/dd, MM/dd/yyyy
            DateTime tmp;
            if (DateTime.TryParseExact(s, DataFormats.AcceptedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp))
            {
                parsed = tmp;
                return true;
            }

            // Intentar interpretar cuando hay año primero pero día/mes fuera de orden
            if (parts[0].Length == 4)
            {
                // yyyy/MM/dd -> yyyy/dd/MM? intentar permutaciones
                string p1 = parts[2] + "/" + parts[1] + "/" + parts[0];
                if (DateTime.TryParseExact(p1, DataFormats.AcceptedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp))
                {
                    parsed = tmp;
                    return true;
                }
            }

            // Intentar parsear componentes numéricos si están en rango razonable
            int a, b, c;
            if (parts.Length >= 3 && int.TryParse(parts[0], out a) && int.TryParse(parts[1], out b) && int.TryParse(parts[2], out c))
            {
                // Probar algunos ordenes comunes
                var combos = new[] {
                    new[]{a,b,c}, // d M y
                    new[]{c,b,a}, // y M d
                    new[]{b,a,c}  // M d y
                };

                foreach (var combo in combos)
                {
                    try
                    {
                        int day = combo[0];
                        int month = combo[1];
                        int year = combo[2];
                        if (year < 100) year += 2000;
                        if (year < 1900 || month < 1 || month > 12 || day < 1 || day > 31) continue;
                        if (DateTime.TryParseExact($"{day:00}/{month:00}/{year}", DataFormats.AcceptedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp))
                        {
                            parsed = tmp;
                            return true;
                        }
                    }
                    catch { }
                }
            }

            return false;
        }

        // -------------------------------------------------------------------
        // Método principal
        // -------------------------------------------------------------------

        /// <summary>
        /// Aplica todas las correcciones automáticas disponibles en secuencia.
        /// Retorna el número total de correcciones realizadas.
        /// </summary>
        public int FixAll(string nullReplacement = null)
        {
            TotalCorrections  = 0;
            TotalCorrections += TrimSpaces();
            TotalCorrections += FixEmails();
            TotalCorrections += FixDates();
            TotalCorrections += FixNumericFields();
            TotalCorrections += NormalizeText();
            TotalCorrections += FixSpecialChars();
            TotalCorrections += ReplaceNullValues(nullReplacement);
            return TotalCorrections;
        }

        // -------------------------------------------------------------------
        // Correcciones individuales
        // -------------------------------------------------------------------

        /// <summary>
        /// Elimina espacios en blanco al inicio y final de todos los valores de texto.
        /// Retorna el número de celdas corregidas.
        /// </summary>
        public int TrimSpaces()
        {
            int count = 0;
            for (int row = 0; row < _dataTable.Rows.Count; row++)
            {
                foreach (DataColumn col in _dataTable.Columns)
                {
                    string value = _dataTable.Rows[row][col]?.ToString() ?? "";
                    string trimmed = value.Trim();

                    if (value != trimmed)
                    {
                        Log(row, col.ColumnName, value, trimmed, "Espacios eliminados");
                        _dataTable.Rows[row][col] = trimmed;
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Convierte el texto de columnas de tipo texto al formato Título
        /// (primera letra de cada palabra en mayúscula).
        /// Solo aplica a columnas no numéricas, no de fecha y no de email.
        /// Retorna el número de celdas modificadas.
        /// </summary>
        public int NormalizeText()
        {
            int count  = 0;
            var ti     = new CultureInfo("es-MX", false).TextInfo;

            // Identificar columnas "de nombre" para aplicar TitleCase
            var textColumns = _dataTable.Columns
                                        .Cast<DataColumn>()
                                        .Where(c => IsTextNameColumn(c.ColumnName))
                                        .ToList();

            foreach (var col in textColumns)
            {
                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    string value = _dataTable.Rows[row][col]?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(value)) continue;

                    string titleCase = ti.ToTitleCase(value.ToLower());
                    if (value != titleCase)
                    {
                        Log(row, col.ColumnName, value, titleCase, "Formato Título aplicado");
                        _dataTable.Rows[row][col] = titleCase;
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Convierte correos electrónicos a minúsculas y elimina espacios.
        /// Aplica a columnas cuyo nombre contenga "email", "correo" o "mail".
        /// </summary>
        public int FixEmails()
        {
            int count    = 0;
            var emailCols = GetColumnsByKeyword("email", "correo", "mail");

            foreach (var col in emailCols)
            {
                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    string value = _dataTable.Rows[row][col]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(value.Trim())) continue;

                    string fixed_ = value.Trim().ToLowerInvariant();
                    if (value != fixed_)
                    {
                        Log(row, col.ColumnName, value, fixed_, "Correo normalizado");
                        _dataTable.Rows[row][col] = fixed_;
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Normaliza fechas al formato estándar dd/MM/yyyy.
        /// Aplica a columnas cuyo nombre contenga "fecha", "date", "nacimiento", etc.
        /// </summary>
        public int FixDates()
        {
            int count = 0;
            var dateColumnNames = GetColumnsByKeyword("fecha", "date", "nacimiento", "inicio", "fin", "birth")
                .Select(c => c.ColumnName)
                .ToList();

            foreach (string columnName in dateColumnNames)
            {
                if (!_dataTable.Columns.Contains(columnName))
                    continue;

                ConvertColumnToString(_dataTable.Columns[columnName]);

                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    var cell = _dataTable.Rows[row][columnName];
                    string value = cell == DBNull.Value ? "" : cell?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(value)) continue;

                    DateTime parsed;
                    bool ok = DateTime.TryParseExact(value, DataFormats.AcceptedDateFormats,
                                  CultureInfo.InvariantCulture,
                                  DateTimeStyles.None, out parsed)
                           || DateTime.TryParse(value,
                                  CultureInfo.InvariantCulture,
                                  DateTimeStyles.None, out parsed);

                    if (ok)
                    {
                        string normalized = parsed.ToString("dd/MM/yyyy");
                        if (value != normalized)
                        {
                            Log(row, columnName, value, normalized, "Fecha normalizada");
                            _dataTable.Rows[row][columnName] = normalized;
                            count++;
                        }
                    }
                    else if (TryHeuristicParseDate(value, out parsed))
                    {
                        string normalized = parsed.ToString("dd/MM/yyyy");
                        Log(row, columnName, value, normalized, "Fecha heurística aplicada");
                        _dataTable.Rows[row][columnName] = normalized;
                        count++;
                    }
                    else
                    {
                        Log(row, columnName, value, "(nulo)", "Fecha inválida convertida a nulo");
                        _dataTable.Rows[row][columnName] = DBNull.Value;
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Convierte físicamente una columna a tipo string preservando valores:
        /// - Si el valor es una fecha válida, la formatea a dd/MM/yyyy
        /// - Si es DBNull, se mantiene DBNull
        /// - Resto se convierte a su ToString()
        /// </summary>
        private void ConvertColumnToString(DataColumn col)
        {
            if (col.DataType == typeof(string))
                return;

            string columnName = col.ColumnName;
            string tmpName = columnName + "_str_tmp";
            var newCol = new DataColumn(tmpName, typeof(string));
            int ordinal = col.Ordinal;
            _dataTable.Columns.Add(newCol);

            for (int r = 0; r < _dataTable.Rows.Count; r++)
            {
                var v = _dataTable.Rows[r][columnName];
                if (v == null || v == DBNull.Value)
                {
                    _dataTable.Rows[r][tmpName] = DBNull.Value;
                    continue;
                }

                DateTime parsed;
                string s = v.ToString().Trim();
                if (DateTime.TryParseExact(s, DataFormats.AcceptedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    || DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                {
                    _dataTable.Rows[r][tmpName] = parsed.ToString("dd/MM/yyyy");
                }
                else
                {
                    _dataTable.Rows[r][tmpName] = s;
                }
            }

            _dataTable.Columns.Remove(columnName);
            newCol.ColumnName = columnName;
            newCol.SetOrdinal(ordinal);
        }

        /// <summary>
        /// Limpia campos numéricos eliminando texto no numérico adyacente
        /// (ej. "45 kg" → "45", "$1,200.50" → "1200.50").
        /// </summary>
        public int FixNumericFields()
        {
            int count = 0;

            foreach (DataColumn col in _dataTable.Columns)
            {
                // Solo actuar sobre columnas identificadas como numéricas
                if (!IsLikelyNumericColumn(col)) continue;

                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    string value = _dataTable.Rows[row][col]?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(value)) continue;

                    // Si ya es un número válido, saltar
                    string normalized = value.Replace(",", ".");
                    if (double.TryParse(normalized, NumberStyles.Any,
                            CultureInfo.InvariantCulture, out _)) continue;

                    // Extraer la parte numérica
                    string extracted = Regex.Match(value, @"-?\d+[\.,]?\d*").Value;
                    if (string.IsNullOrEmpty(extracted))
                    {
                        // No hay parte numérica: convertir a nulo para que se trate como vacío
                        Log(row, col.ColumnName, value, "(nulo)", "Valor numérico inválido convertido a nulo");
                        _dataTable.Rows[row][col] = DBNull.Value;
                        count++;
                    }
                    else if (extracted != value)
                    {
                        // Si existe una parte numérica, usarla (normalizada)
                        string normalizedExtracted = extracted.Replace(',', '.');
                        Log(row, col.ColumnName, value, normalizedExtracted, "Texto no numérico eliminado");
                        _dataTable.Rows[row][col] = normalizedExtracted;
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Reemplaza valores nulos o vacíos con un valor predeterminado.
        /// Si <paramref name="defaultValue"/> es null, usa valores según tipo de columna.
        /// </summary>
        public int ReplaceNullValues(string defaultValue = null)
        {
            int count = 0;

            for (int row = 0; row < _dataTable.Rows.Count; row++)
            {
                foreach (DataColumn col in _dataTable.Columns)
                {
                    object cellValue = _dataTable.Rows[row][col];
                    bool isEmpty = cellValue == null
                                || cellValue == DBNull.Value
                                || cellValue.ToString().Trim() == string.Empty;

                    if (isEmpty)
                    {
                        if (defaultValue == null)
                        {
                            // Dejar como nulo explícito
                            Log(row, col.ColumnName, "(vacío)", "(nulo)", "Se mantiene nulo");
                            _dataTable.Rows[row][col] = DBNull.Value;
                            count++;
                        }
                        else
                        {
                            string replacement = defaultValue ?? GetSmartDefault(col.ColumnName);
                            Log(row, col.ColumnName, "(vacío)", replacement, "Nulo reemplazado");
                            _dataTable.Rows[row][col] = replacement;
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Elimina filas completamente duplicadas del DataTable.
        /// Retorna el número de filas eliminadas.
        /// </summary>
        public int RemoveDuplicates()
        {
            var seenKeys = new HashSet<string>();
            var toDelete = new List<DataRow>();

            foreach (DataRow row in _dataTable.Rows)
            {
                string key = string.Join("|", row.ItemArray
                                             .Select(v => v?.ToString()?.Trim()?.ToLowerInvariant() ?? ""));
                if (seenKeys.Contains(key))
                    toDelete.Add(row);
                else
                    seenKeys.Add(key);
            }

            foreach (var row in toDelete)
            {
                ChangeLog.Add($"[Fila eliminada] Duplicado removido.");
                _dataTable.Rows.Remove(row);
            }

            return toDelete.Count;
        }

        /// <summary>
        /// Corrige caracteres HTML/XML codificados y caracteres de control comunes.
        /// </summary>
        public int FixSpecialChars()
        {
            int count = 0;

            for (int row = 0; row < _dataTable.Rows.Count; row++)
            {
                foreach (DataColumn col in _dataTable.Columns)
                {
                    string value = _dataTable.Rows[row][col]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(value)) continue;

                    string fixed_ = value;
                    foreach (var kvp in SpecialCharMap)
                        fixed_ = fixed_.Replace(kvp.Key, kvp.Value);

                    // Normalizar múltiples espacios consecutivos en uno
                    fixed_ = Regex.Replace(fixed_, @" {2,}", " ").Trim();

                    if (fixed_ != value)
                    {
                        Log(row, col.ColumnName, value, fixed_, "Caracteres especiales corregidos");
                        _dataTable.Rows[row][col] = fixed_;
                        count++;
                    }
                }
            }
            return count;
        }

        // -------------------------------------------------------------------
        // Revert / Reset
        // -------------------------------------------------------------------

        /// <summary>
        /// Revierte todos los cambios restaurando el snapshot original.
        /// Limpia el log y reinicia el contador de correcciones.
        /// </summary>
        public void RevertAllChanges()
        {
            // Limpiar el DataTable actual y recargar desde el snapshot
            _dataTable.Clear();
            foreach (DataRow row in _originalSnapshot.Rows)
                _dataTable.ImportRow(row);

            TotalCorrections = 0;
            ChangeLog.Clear();
        }

        /// <summary>
        /// Expone el DataTable actual (después de correcciones).
        /// </summary>
        public DataTable GetCurrentData() => _dataTable;

        // -------------------------------------------------------------------
        // Métodos auxiliares privados
        // -------------------------------------------------------------------

        private void Log(int row, string col, string oldVal, string newVal, string action)
        {
            ChangeLog.Add($"[Fila {row + 1}][{col}] {action}: \"{oldVal}\" → \"{newVal}\"");
        }

        private IEnumerable<DataColumn> GetColumnsByKeyword(params string[] keywords)
        {
            return _dataTable.Columns
                             .Cast<DataColumn>()
                             .Where(c => keywords.Any(k =>
                                 c.ColumnName.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        private bool IsTextNameColumn(string columnName)
        {
            string[] nameKeywords = { "nombre", "name", "apellido", "surname", "ciudad", "city",
                                      "pais", "country", "estado", "state", "direccion", "address",
                                      "cargo", "puesto", "descripcion", "description" };
            return nameKeywords.Any(k => columnName.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool IsLikelyNumericColumn(DataColumn col)
        {
            string[] numKeywords = { "edad", "age", "precio", "price", "monto", "amount",
                                     "cantidad", "quantity", "total", "salario", "salary",
                                     "numero", "number", "num", "id", "telefono", "phone" };
            return numKeywords.Any(k => col.ColumnName.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private string GetSmartDefault(string columnName)
        {
            string lower = columnName.ToLower();
            if (lower.Contains("fecha") || lower.Contains("date"))
                return DateTime.Today.ToString("dd/MM/yyyy");
            if (lower.Contains("email") || lower.Contains("correo"))
                return "sin-correo@ejemplo.com";
            if (lower.Contains("edad") || lower.Contains("age") ||
                lower.Contains("precio") || lower.Contains("price") ||
                lower.Contains("total") || lower.Contains("id"))
                return "0";
            return "N/A";
        }
    }
}
