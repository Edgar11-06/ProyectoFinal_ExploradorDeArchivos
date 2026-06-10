using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SistemaMultimedia.DataFusion
{
    /// <summary>
    /// Convertidores prácticos para archivos .txt a DataTable.
    /// Soporta mapeo automático por cabecera y campos entrecomillados.
    /// </summary>
    public static class TxtToDataItems
    {
        public static DataTable LoadBySeparator(string path, char separator = '|', bool hasHeader = false, Encoding? encoding = null)
        {
            var lines = TextFileReader.ReadLines(path, encoding, skipEmpty: true);
            var dt = new DataTable("ImportedTxt");
            if (lines.Count == 0) return dt;

            // detectar separador usando varias líneas de muestra, pero respetar el separador pedido salvo que produzca 1 columna
            var sep = separator;
            var sample = lines.Take(10).ToList();
            if (sample.Any())
            {
                // si el separador dado produce una sola columna en la muestra, intentar detectar mejor
                var testCount = ParseLine(sample.First(), sep).Count;
                if (testCount <= 1)
                {
                    var detected = DetectSeparatorFromLines(sample);
                    if (detected != sep)
                        sep = detected;
                }
            }

            // construir DataTable desde líneas usando el separador escogido
            dt = BuildDataTableFromLines(lines, sep, hasHeader);

            // fallback: si sólo hay 1 columna y hay más de una línea, reintentar por frecuencia de separadores
            if (dt.Columns.Count == 1 && lines.Count > 1)
            {
                var freqCandidates = new[] { ',', ';', '\t', '|', ':' };
                var bestByFreq = freqCandidates
                    .Select(c => new { Sep = c, Count = lines.Take(50).Count(l => l.Count(ch => ch == c) > 0) })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefault();
                if (bestByFreq != null && bestByFreq.Sep != sep && bestByFreq.Count > 0)
                {
                    var reparsed = BuildDataTableFromLines(lines, bestByFreq.Sep, hasHeader);
                    if (reparsed.Columns.Count > dt.Columns.Count)
                        dt = reparsed;
                }
            }

            return dt;
        }

        // Construye DataTable a partir de líneas y separador (extraído para reparseo)
        private static DataTable BuildDataTableFromLines(IList<string> lines, char sep, bool hasHeader)
        {
            var dt = new DataTable("ImportedTxt");

            if (!lines.Any()) return dt;

            var first = lines.First();
            bool header = hasHeader;
            // Inicializar para cumplir las reglas de asignación definitiva del compilador
            List<string> headers = new List<string>();
            int start = 0;

            if (header)
            {
                headers = ParseLine(first, sep).Select(h => h.Trim()).ToList();
                // Si la cabecera detectada tiene solo 1 columna pero las filas tienen más,
                // considerar que no hay cabecera real (fallback).
                if (headers.Count <= 1 && lines.Count > 1)
                {
                    var sampleCols = ParseLine(lines.Skip(1).FirstOrDefault() ?? first, sep).Count;
                    if (sampleCols > headers.Count)
                    {
                        header = false;
                    }
                }

                if (header)
                    start = 1;
            }

            if (!header)
            {
                var sample = ParseLine(first, sep);
                headers = Enumerable.Range(0, sample.Count).Select(i => $"Col{i + 1}").ToList();
                start = 0;
            }

            // crear columnas (strings por defecto). Luego intentar inferencia de tipo al llenar.
            foreach (var h in headers)
            {
                var colName = string.IsNullOrWhiteSpace(h) ? "Col" + (dt.Columns.Count + 1) : h;
                var finalName = colName;
                int k = 1;
                while (dt.Columns.Contains(finalName))
                {
                    finalName = colName + "_" + k;
                    k++;
                }
                dt.Columns.Add(finalName, typeof(string));
            }

            // añadir filas
            int maxColsSeen = dt.Columns.Count;
            for (int i = start; i < lines.Count; i++)
            {
                var parts = ParseLine(lines[i], sep);
                if (parts == null) parts = new List<string>();
                if (parts.Count > maxColsSeen) maxColsSeen = parts.Count;

                var row = dt.NewRow();
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    row[c] = c < parts.Count ? parts[c] : string.Empty;
                }
                dt.Rows.Add(row);
            }

            // Si alguna fila tiene más columnas que los encabezados, ajustar columnas y rellenar filas
            if (maxColsSeen > dt.Columns.Count)
            {
                int existing = dt.Columns.Count;
                for (int j = existing; j < maxColsSeen; j++)
                {
                    dt.Columns.Add($"Col{j + 1}", typeof(string));
                }

                // rellenar valores vacíos en filas (por seguridad)
                foreach (DataRow r in dt.Rows)
                {
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        if (r.IsNull(c)) r[c] = string.Empty;
                    }
                }
            }

            // intentar inferir tipos numéricos para columnas con mayoría de números (no obligatorio)
            InferColumnTypes(dt);

            return dt;
        }

        // Detecta separador a partir de varias líneas de muestra
        private static char DetectSeparatorFromLines(IEnumerable<string> sampleLines)
        {
            var candidates = new[] { ',', ';', '\t', '|', ':' };
            char best = ',';
            double bestScore = double.MinValue;

            var samples = sampleLines?.Where(l => !string.IsNullOrWhiteSpace(l)).Take(20).ToList() ?? new List<string>();
            if (!samples.Any()) return best;

            foreach (var c in candidates)
            {
                var counts = samples.Select(l => ParseLine(l, c).Count).ToArray();
                if (counts.Length == 0) continue;
                var nonZero = counts.Where(x => x > 0).ToArray();
                if (!nonZero.Any()) continue;
                var avg = nonZero.Average();
                var variance = nonZero.Select(x => (x - avg) * (x - avg)).Average();
                var score = avg - variance * 0.1;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = c;
                }
            }

            return best;
        }

        // Comprueba si el array de campos contiene las posiciones que el mapping necesita
        private static bool LineHasRequiredColumns(string[] parts, Dictionary<string,int> mapping)
        {
            if (parts == null) return false;
            if (!mapping.TryGetValue("Id", out var idIdx) || !mapping.TryGetValue("Valor", out var valIdx))
                return false;
            return idIdx >= 0 && idIdx < parts.Length && valIdx >= 0 && valIdx < parts.Length;
        }

        // Intento defensivo: si no conseguimos parsear bien, buscar Id en la línea completa y rellenar el array mínimo
        private static bool AttemptFillByIdRegex(ref string[] parts, string entireLine, Dictionary<string,int> mapping)
        {
            var m = Regex.Match(entireLine, @"\b[A-Za-z]{1,}[0-9]{1,}\b");
            if (!m.Success) return false;

            var idVal = m.Value;
            if (mapping["Id"] == 0)
            {
                int size = Math.Max(mapping.Values.Max() + 1, 1);
                var newParts = new string[size];
                for (int j = 0; j < size; j++) newParts[j] = string.Empty;
                newParts[0] = idVal;
                parts = newParts;
                return true;
            }
            return false;
        }

        // Intenta mapear encabezados normalizados a índices
        private static Dictionary<string,int>? AutoMapHeaders(IList<string> headers)
        {
            var norm = headers.Select((h, i) => new { Orig = h, Norm = NormalizeHeader(h), Index = i }).ToList();

            int? find(params string[] candidates)
            {
                foreach (var c in candidates)
                {
                    var exact = norm.FirstOrDefault(x => x.Norm == c);
                    if (exact != null) return exact.Index;
                }
                foreach (var c in candidates)
                {
                    var contains = norm.FirstOrDefault(x => x.Norm.Contains(c));
                    if (contains != null) return contains.Index;
                }
                return null;
            }

            int? idIdx = find("id", "id_venta", "venta", "identifier", "codigo");
            int? nombreIdx = find("nombre", "nombre_producto", "producto", "title", "name");
            int? categoriaIdx = find("categoria", "categoria_producto", "category", "grupo", "tipo");
            int? cantidadIdx = find("cantidad", "qty", "quantity", "unidades");
            int? precioUnitIdx = find("precio_unitario", "preciounitario", "precio_unit", "unit_price", "precio");
            int? valorIdx = find("total", "total_venta", "precio_unitario", "precio", "valor", "amount");

            if (!idIdx.HasValue && norm.Count > 0) idIdx = 0;
            if (!valorIdx.HasValue && norm.Count > 0) valorIdx = norm.Count - 1;

            if (!idIdx.HasValue || !valorIdx.HasValue) return null;

            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = idIdx.Value,
                ["Nombre"] = nombreIdx ?? -1,
                ["Categoria"] = categoriaIdx ?? -1,
                ["Cantidad"] = cantidadIdx ?? -1,
                ["PrecioUnitario"] = precioUnitIdx ?? -1,
                ["Valor"] = valorIdx.Value
            };
        }

        // Mapeo posicional por defecto
        private static Dictionary<string,int> HeuristicPositionalMap(int columnCount)
        {
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = columnCount > 0 ? 0 : -1,
                ["Nombre"] = columnCount > 1 ? 1 : -1,
                ["Categoria"] = columnCount > 2 ? 2 : -1,
                ["Cantidad"] = columnCount > 3 ? 3 : -1,
                ["PrecioUnitario"] = columnCount > 4 ? columnCount - 2 : -1,
                ["Valor"] = columnCount > 0 ? columnCount - 1 : -1
            };
        }

        // Parseador que soporta comillas dobles y separador configurable.
        private static List<string> ParseLine(string line, char separator)
        {
            var result = new List<string>();
            if (line == null) return result;

            if (line.Length >= 2 && line[0] == '"' && line[line.Length - 1] == '"')
            {
                var inner = line.Substring(1, line.Length - 2);
                inner = inner.Replace("\"\"", "\"");
                line = inner;
            }

            var sb = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == separator && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            result.Add(sb.ToString());
            return result;
        }

        private static bool TryParseDecimal(string s, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(s)) return false;

            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value)) return true;
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out value)) return true;

            var cleaned = Regex.Replace(s, @"[^\d\-\+\,\.]", string.Empty);
            if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out value)) return true;
            if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.CurrentCulture, out value)) return true;

            return false;
        }

        private static string NormalizeHeader(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var formD = s.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            var cleaned = sb.ToString().Normalize(System.Text.NormalizationForm.FormC).ToLowerInvariant();
            cleaned = Regex.Replace(cleaned, @"[^\p{L}\p{Nd}]+", "_");
            cleaned = Regex.Replace(cleaned, @"_+", "_").Trim('_');
            return cleaned;
        }

        private static string ExtractIdFromField(string fieldValue, string entireLine)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                fieldValue = entireLine ?? string.Empty;
            }

            var cleaned = fieldValue.Trim().Trim('"');

            var m = Regex.Match(cleaned, @"\b[A-Za-z]{1,}[0-9]{1,}\b");
            if (m.Success)
                return m.Value;

            int idx = cleaned.IndexOfAny(new[] { ',', ';', '|', '\t' });
            if (idx > 0)
                return cleaned.Substring(0, idx).Trim().Trim('"');

            return cleaned;
        }

        // Inferencia de tipos (duplicada localmente)
        private static void InferColumnTypes(DataTable dt)
        {
            var toChange = new List<(DataColumn Old, Type NewType)>();

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == "__original_index") continue;
                int numericCount = 0;
                int decimalCount = 0;
                int nonEmpty = 0;
                foreach (DataRow r in dt.Rows)
                {
                    var o = r[col];
                    if (o == null || o == DBNull.Value) continue;
                    var s = o.ToString()?.Trim();
                    if (string.IsNullOrEmpty(s)) continue;
                    nonEmpty++;
                    if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                        || int.TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out _))
                    {
                        numericCount++;
                    }
                    else if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out _)
                        || decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out _))
                    {
                        decimalCount++;
                        numericCount++;
                    }
                }

                if (nonEmpty == 0) continue;
                if (numericCount >= nonEmpty * 0.6)
                {
                    var newType = decimalCount > 0 ? typeof(decimal) : typeof(int);
                    toChange.Add((col, newType));
                }
            }

            foreach (var (oldCol, newType) in toChange)
            {
                try
                {
                    var newCol = new DataColumn(oldCol.ColumnName + "_tmp", newType);
                    oldCol.Table.Columns.Add(newCol);
                    foreach (DataRow r in oldCol.Table.Rows)
                    {
                        var v = r[oldCol];
                        if (v == null || v == DBNull.Value || string.IsNullOrWhiteSpace(v.ToString()))
                        {
                            r[newCol] = DBNull.Value;
                        }
                        else
                        {
                            if (newType == typeof(int))
                            {
                                if (int.TryParse(v.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var iv)
                                    || int.TryParse(v.ToString(), NumberStyles.Integer, CultureInfo.CurrentCulture, out iv))
                                    r[newCol] = iv;
                                else r[newCol] = DBNull.Value;
                            }
                            else // decimal
                            {
                                if (decimal.TryParse(v.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var dv)
                                    || decimal.TryParse(v.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture, out dv))
                                    r[newCol] = dv;
                                else
                                {
                                    var cleaned = Regex.Replace(v.ToString(), @"[^\d\-\+\,\.]", string.Empty);
                                    if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out dv)
                                        || decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.CurrentCulture, out dv))
                                        r[newCol] = dv;
                                    else r[newCol] = DBNull.Value;
                                }
                            }
                        }
                    }
                    var table = oldCol.Table;
                    int ordinal = oldCol.Ordinal;
                    table.Columns.Remove(oldCol);
                    newCol.ColumnName = oldCol.ColumnName;
                    newCol.SetOrdinal(ordinal);
                }
                catch { }
            }
        }
    }
}