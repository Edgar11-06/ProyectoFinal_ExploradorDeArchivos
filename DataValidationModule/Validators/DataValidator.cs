using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using DataValidationModule.Common;
using DataValidationModule.Models;

namespace DataValidationModule.Validators
{
    /// <summary>
    /// Clase encargada de realizar todas las validaciones de calidad sobre un DataTable.
    /// Detecta errores, genera reportes detallados y estadísticas del dataset.
    /// 
    /// Uso típico:
    ///   var validator = new DataValidator(dataTable);
    ///   validator.RequiredFields = new List&lt;string&gt; { "Nombre", "Email" };
    ///   var results = validator.ValidateAll();
    /// </summary>
    public class DataValidator
    {
        // -------------------------------------------------------------------
        // Campos y propiedades
        // -------------------------------------------------------------------

        private readonly DataTable _dataTable;

        /// <summary>Lista de columnas que no pueden estar vacías.</summary>
        public List<string> RequiredFields { get; set; } = new List<string>();

        /// <summary>Longitudes máximas por columna. Clave = nombre columna, Valor = longitud.</summary>
        public Dictionary<string, int> MaxLengths { get; set; } = new Dictionary<string, int>();

        /// <summary>Tipos inferidos automáticamente por columna.</summary>
        public Dictionary<string, string> InferredColumnTypes { get; private set; }
            = new Dictionary<string, string>();

        // Regex de validación
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex SuspiciousCharsRegex = new Regex(
            @"[<>\{\}\[\]\\|`~^]",
            RegexOptions.Compiled);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Inicializa el validador con el DataTable a analizar.
        /// </summary>
        /// <param name="dataTable">DataTable con los datos a validar.</param>
        public DataValidator(DataTable dataTable)
        {
            _dataTable = dataTable ?? throw new ArgumentNullException(nameof(dataTable));
            InferColumnTypes();
        }

        // -------------------------------------------------------------------
        // Método principal
        // -------------------------------------------------------------------

        /// <summary>
        /// Ejecuta todas las validaciones disponibles y retorna la lista combinada de errores.
        /// </summary>
        public List<ValidationResult> ValidateAll()
        {
            var results = new List<ValidationResult>();

            results.AddRange(ValidateNulls());
            results.AddRange(ValidateDuplicates());
            results.AddRange(ValidateEmails());
            results.AddRange(ValidateDates());
            results.AddRange(ValidateNumericColumns());
            results.AddRange(ValidateLeadingTrailingSpaces());
            results.AddRange(ValidateSuspiciousChars());
            results.AddRange(ValidateRequiredFields());
            results.AddRange(ValidateFieldLengths());

            // Ordenar por fila y columna para facilitar la lectura del reporte
            results = results.OrderBy(r => r.RowIndex)
                             .ThenBy(r => r.ColumnName)
                             .ToList();
            return results;
        }

        // -------------------------------------------------------------------
        // Validaciones individuales
        // -------------------------------------------------------------------

        /// <summary>
        /// Detecta celdas con valores nulos o cadenas vacías en todo el dataset.
        /// </summary>
        public List<ValidationResult> ValidateNulls()
        {
            var results = new List<ValidationResult>();

            for (int row = 0; row < _dataTable.Rows.Count; row++)
            {
                foreach (DataColumn col in _dataTable.Columns)
                {
                    object cellValue = _dataTable.Rows[row][col];
                    // No reportar DBNull.Value: se considera valor corregido por el sistema
                    if (cellValue == DBNull.Value) continue;
                    bool isNull = cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString());

                    if (isNull)
                    {
                        results.Add(new ValidationResult(
                            rowIndex:       row,
                            columnName:     col.ColumnName,
                            currentValue:   "(vacío)",
                            errorType:      ErrorType.NullOrEmpty,
                            description:    $"La columna '{col.ColumnName}' contiene un valor nulo o vacío.",
                            suggestedValue: GetDefaultValue(col)));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Detecta filas completamente duplicadas en el dataset.
        /// Compara todos los campos de cada fila con los demás.
        /// </summary>
        public List<ValidationResult> ValidateDuplicates()
        {
            var results     = new List<ValidationResult>();
            var seenRows    = new Dictionary<string, int>(); // clave → primera aparición
            var duplicated  = new HashSet<int>();

            for (int row = 0; row < _dataTable.Rows.Count; row++)
            {
                // Construir clave canónica para la fila, tratando DBNull como vacío
                var key = string.Join("|", _dataTable.Rows[row].ItemArray
                                              .Select(v => (v == DBNull.Value) ? "" : v?.ToString()?.Trim().ToLowerInvariant() ?? ""));

                if (seenRows.ContainsKey(key))
                {
                    // Marcar duplicado si no se marcó antes el original
                    if (!duplicated.Contains(seenRows[key]))
                    {
                        duplicated.Add(seenRows[key]);
                        results.Add(new ValidationResult(
                            rowIndex:     seenRows[key],
                            columnName:   "(toda la fila)",
                            currentValue: "(fila original)",
                            errorType:    ErrorType.Duplicate,
                            description:  $"Fila original duplicada. Primera aparición en fila {seenRows[key] + 1}."));
                    }

                    results.Add(new ValidationResult(
                        rowIndex:     row,
                        columnName:   "(toda la fila)",
                        currentValue: "(duplicado)",
                        errorType:    ErrorType.Duplicate,
                        description:  $"Registro duplicado de la fila {seenRows[key] + 1}.",
                        suggestedValue: "Eliminar esta fila"));
                }
                else
                {
                    seenRows[key] = row;
                }
            }
            return results;
        }

        /// <summary>
        /// Valida correos electrónicos en columnas cuyo nombre contenga
        /// "email", "correo" o "mail" (sin distinguir mayúsculas).
        /// </summary>
        public List<ValidationResult> ValidateEmails()
        {
            var results     = new List<ValidationResult>();
            var emailCols   = GetColumnsByKeyword("email", "correo", "mail");

            foreach (var col in emailCols)
            {
                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    var cell = _dataTable.Rows[row][col];
                    if (cell == DBNull.Value) continue; // no reportar DBNull
                    string value = cell?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(value)) continue;

                    if (!EmailRegex.IsMatch(value))
                    {
                        results.Add(new ValidationResult(
                            rowIndex:       row,
                            columnName:     col.ColumnName,
                            currentValue:   value,
                            errorType:      ErrorType.InvalidEmail,
                            description:    $"El correo '{value}' no tiene un formato válido.",
                            suggestedValue: value.ToLowerInvariant()));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Valida fechas en columnas cuyo nombre contenga "fecha", "date", "nacimiento", "inicio", "fin".
        /// </summary>
        public List<ValidationResult> ValidateDates()
        {
            var results   = new List<ValidationResult>();
            var dateCols  = GetColumnsByKeyword("fecha", "date", "nacimiento", "inicio", "fin", "birth");

            foreach (var col in dateCols)
            {
                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    var cell = _dataTable.Rows[row][col];
                    if (cell == DBNull.Value) continue;
                    string value = cell?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(value)) continue;

                    DateTime parsed;
                    bool isValid = DateTime.TryParseExact(value, DataFormats.AcceptedDateFormats,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None, out parsed)
                                || DateTime.TryParse(value,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None, out parsed);

                    if (!isValid)
                    {
                        results.Add(new ValidationResult(
                            rowIndex:       row,
                            columnName:     col.ColumnName,
                            currentValue:   value,
                            errorType:      ErrorType.InvalidDate,
                            description:    $"'{value}' no es una fecha reconocida.",
                            suggestedValue: "Verificar manualmente"));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Detecta columnas inferidas como numéricas que contienen texto no numérico.
        /// </summary>
        public List<ValidationResult> ValidateNumericColumns()
        {
            var results = new List<ValidationResult>();

            foreach (DataColumn col in _dataTable.Columns)
            {
                bool isNumericCol = InferredColumnTypes.TryGetValue(col.ColumnName, out string inferredType)
                                  && inferredType == "Numeric";

                if (!isNumericCol) continue;

                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    var cell = _dataTable.Rows[row][col];
                    if (cell == null || cell == DBNull.Value) continue;
                    string value = cell.ToString().Trim();
                    if (string.IsNullOrEmpty(value)) continue;

                    // Valores textuales que deben ir a NULL y no re-reportarse
                    var nullMarkers = new[] { "ABC", "ERROR", "N/A", "ELIMINADA", "ELIMINADO", "NA" };
                    if (nullMarkers.Any(m => string.Equals(m, value, StringComparison.OrdinalIgnoreCase)))
                    {
                        results.Add(new ValidationResult(
                            rowIndex: row,
                            columnName: col.ColumnName,
                            currentValue: value,
                            errorType: ErrorType.NumericFieldWithText,
                            description: $"La columna '{col.ColumnName}' (numérica) contiene texto inválido: '{value}'.",
                            suggestedValue: "Convertir a NULL"));
                        continue;
                    }

                    // Intentar parsear como número (permite decimales con punto o coma)
                    string normalized = value.Replace(",", ".");
                    if (!double.TryParse(normalized,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out _))
                    {
                        results.Add(new ValidationResult(
                            rowIndex:       row,
                            columnName:     col.ColumnName,
                            currentValue:   value,
                            errorType:      ErrorType.NumericFieldWithText,
                            description:    $"La columna '{col.ColumnName}' (numérica) contiene texto: '{value}'.",
                            suggestedValue: "Convertir a NULL"));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Detecta valores con espacios en blanco al inicio o al final.
        /// </summary>
        public List<ValidationResult> ValidateLeadingTrailingSpaces()
        {
            var results = new List<ValidationResult>();

            for (int row = 0; row < _dataTable.Rows.Count; row++)
            {
                foreach (DataColumn col in _dataTable.Columns)
                {
                    var cell = _dataTable.Rows[row][col];
                    if (cell == DBNull.Value) continue;
                    string value = cell?.ToString() ?? "";
                    if (string.IsNullOrEmpty(value)) continue;

                    if (value != value.Trim())
                    {
                        results.Add(new ValidationResult(
                            rowIndex:       row,
                            columnName:     col.ColumnName,
                            currentValue:   $"\"{value}\"",
                            errorType:      ErrorType.LeadingTrailingSpaces,
                            description:    "El valor contiene espacios innecesarios al inicio o final.",
                            suggestedValue: value.Trim()));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Detecta caracteres especiales sospechosos que podrían indicar datos corruptos
        /// o intentos de inyección (ej. &lt; &gt; { } [ ] \ | ` ~ ^).
        /// </summary>
        public List<ValidationResult> ValidateSuspiciousChars()
        {
            var results = new List<ValidationResult>();

            for (int row = 0; row < _dataTable.Rows.Count; row++)
            {
                foreach (DataColumn col in _dataTable.Columns)
                {
                    string value = _dataTable.Rows[row][col]?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(value)) continue;

                    if (SuspiciousCharsRegex.IsMatch(value))
                    {
                        string cleaned = SuspiciousCharsRegex.Replace(value, "");
                        results.Add(new ValidationResult(
                            rowIndex:       row,
                            columnName:     col.ColumnName,
                            currentValue:   value,
                            errorType:      ErrorType.SuspiciousSpecialChars,
                            description:    "El valor contiene caracteres especiales potencialmente problemáticos.",
                            suggestedValue: cleaned.Trim()));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Verifica que los campos marcados como obligatorios no estén vacíos.
        /// </summary>
        public List<ValidationResult> ValidateRequiredFields()
        {
            var results = new List<ValidationResult>();
            if (RequiredFields == null || RequiredFields.Count == 0) return results;

            foreach (string fieldName in RequiredFields)
            {
                if (!_dataTable.Columns.Contains(fieldName)) continue;

                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    var cell = _dataTable.Rows[row][fieldName];
                    if (cell == DBNull.Value) continue; // valores nulos generados por sistema no son reportados
                    string value = cell?.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(value))
                    {
                        results.Add(new ValidationResult(
                            rowIndex:       row,
                            columnName:     fieldName,
                            currentValue:   "(vacío)",
                            errorType:      ErrorType.RequiredFieldEmpty,
                            description:    $"El campo obligatorio '{fieldName}' está vacío.",
                            suggestedValue: "Requiere valor manual"));
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Verifica que los valores no superen la longitud máxima configurada por columna.
        /// </summary>
        public List<ValidationResult> ValidateFieldLengths()
        {
            var results = new List<ValidationResult>();
            if (MaxLengths == null || MaxLengths.Count == 0) return results;

            foreach (var kvp in MaxLengths)
            {
                string colName = kvp.Key;
                int    maxLen  = kvp.Value;

                if (!_dataTable.Columns.Contains(colName)) continue;

                for (int row = 0; row < _dataTable.Rows.Count; row++)
                {
                    var cell = _dataTable.Rows[row][colName];
                    if (cell == DBNull.Value) continue;
                    string value = cell?.ToString() ?? "";
                    if (value.Length > maxLen)
                    {
                        results.Add(new ValidationResult(
                            rowIndex:       row,
                            columnName:     colName,
                            currentValue:   value,
                            errorType:      ErrorType.MaxLengthExceeded,
                            description:    $"El valor tiene {value.Length} caracteres; máximo permitido: {maxLen}.",
                            suggestedValue: value.Substring(0, maxLen)));
                    }
                }
            }
            return results;
        }

        // -------------------------------------------------------------------
        // Métodos auxiliares
        // -------------------------------------------------------------------

        /// <summary>
        /// Infiere automáticamente el tipo de dato predominante de cada columna
        /// analizando una muestra representativa de sus valores.
        /// Tipos detectados: Numeric, Date, Email, Text.
        /// </summary>
        public void InferColumnTypes()
        {
            InferredColumnTypes.Clear();

            foreach (DataColumn col in _dataTable.Columns)
            {
                // Tomar hasta 50 valores no vacíos para la muestra
                var sample = _dataTable.AsEnumerable()
                                       .Select(r => (r[col] == DBNull.Value) ? "" : r[col]?.ToString()?.Trim() ?? "")
                                       .Where(v => !string.IsNullOrEmpty(v))
                                       .Take(50)
                                       .ToList();

                if (sample.Count == 0)
                {
                    InferredColumnTypes[col.ColumnName] = "Text";
                    continue;
                }

                int numeric = sample.Count(v => double.TryParse(
                                  v.Replace(",", "."),
                                  System.Globalization.NumberStyles.Any,
                                  System.Globalization.CultureInfo.InvariantCulture, out _));

                int dates   = sample.Count(v => DateTime.TryParse(v, out _));

                int emails  = sample.Count(v => EmailRegex.IsMatch(v));

                double numericRate = (double)numeric / sample.Count;
                double dateRate    = (double)dates   / sample.Count;
                double emailRate   = (double)emails  / sample.Count;

                if      (numericRate >= 0.8) InferredColumnTypes[col.ColumnName] = "Numeric";
                else if (dateRate    >= 0.8) InferredColumnTypes[col.ColumnName] = "Date";
                else if (emailRate   >= 0.5) InferredColumnTypes[col.ColumnName] = "Email";
                else                         InferredColumnTypes[col.ColumnName] = "Text";
            }
        }

        /// <summary>
        /// Retorna las columnas cuyo nombre contiene alguno de los keywords dados.
        /// </summary>
        private IEnumerable<DataColumn> GetColumnsByKeyword(params string[] keywords)
        {
            return _dataTable.Columns
                             .Cast<DataColumn>()
                             .Where(c => keywords.Any(k =>
                                 c.ColumnName.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        /// <summary>
        /// Obtiene el valor predeterminado para una columna según su tipo inferido.
        /// </summary>
        private string GetDefaultValue(DataColumn col)
        {
            if (!InferredColumnTypes.TryGetValue(col.ColumnName, out string colType))
                return "N/A";

            switch (colType)
            {
                case "Numeric": return "0";
                case "Date":    return DateTime.Today.ToString("dd/MM/yyyy");
                case "Email":   return "sin-correo@ejemplo.com";
                default:        return "N/A";
            }
        }

        /// <summary>
        /// Extrae la parte numérica de una cadena de texto mixto (ej. "45 kg" → "45").
        /// </summary>
        private string ExtractNumericPart(string value)
        {
            string numeric = Regex.Match(value, @"-?\d+[\.,]?\d*").Value;
            return string.IsNullOrEmpty(numeric) ? "0" : numeric;
        }
    }
}
