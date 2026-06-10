using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace SistemaMultimedia.DataFusion
{
    public sealed class CsvDocument
    {
        public char Separator { get; set; } = ',';
        public List<string> Headers { get; } = new();
        public List<string[]> Rows { get; } = new();
    }

    public static class CsvLoader
    {
        // Lee cabecera + filas sin mapear. Si encoding == null intenta UTF-8 y, si detecta mojibake, reintenta con 1252.
        public static CsvDocument ReadRaw(string path, bool hasHeader = true, Encoding? encoding = null)
        {
            Encoding tryEncoding = encoding ?? Encoding.UTF8;
            var doc = TryReadWithEncoding(path, tryEncoding, hasHeader);

            if (encoding is null && doc.Headers.Count > 0 && doc.Headers.Any(h => h.Contains("\u00c3")))
            {
                var doc1252 = TryReadWithEncoding(path, Encoding.GetEncoding(1252), hasHeader);
                if (doc1252.Headers.Count > 0)
                    return doc1252;
            }

            return doc;
        }

        private static CsvDocument TryReadWithEncoding(string path, Encoding enc, bool hasHeader)
        {
            using var sr = new StreamReader(path, enc);
            string? firstLine = null;
            while ((firstLine = sr.ReadLine()) != null)
                if (!string.IsNullOrWhiteSpace(firstLine))
                    break;

            var doc = new CsvDocument();
            if (firstLine == null)
                return doc;

            doc.Separator = DetectSeparator(firstLine);

            if (hasHeader)
            {
                var parsedHeaders = ParseCsvLine(firstLine, doc.Separator).Select(h => h.Trim()).ToList();
                doc.Headers.Clear();
                doc.Headers.AddRange(parsedHeaders);
            }
            else
            {
                var fields = ParseCsvLine(firstLine, doc.Separator);
                var generated = Enumerable.Range(0, fields.Count).Select(i => $"Column{i}").ToList();
                doc.Headers.Clear();
                doc.Headers.AddRange(generated);
                doc.Rows.Add(fields.ToArray());
            }

            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var fields = ParseCsvLine(line, doc.Separator).ToArray();
                if (fields.Length < doc.Headers.Count)
                {
                    Array.Resize(ref fields, doc.Headers.Count);
                }
                doc.Rows.Add(fields);
            }

            return doc;
        }

        // Mapea encabezados a posiciones conocidas (Id/Valor/Cantidad/PrecioUnitario/...) — conserva la firma original.
        public static Dictionary<string, int>? AutoMapHeadersToDataItem(IEnumerable<string> headers)
        {
            var normalized = headers.Select((h, idx) => new { Orig = h ?? string.Empty, Norm = NormalizeHeader(h ?? string.Empty), Index = idx }).ToList();

            int? find(IEnumerable<string> candidates)
            {
                foreach (var c in candidates)
                {
                    var match = normalized.FirstOrDefault(x => x.Norm == c);
                    if (match != null) return match.Index;
                }
                foreach (var c in candidates)
                {
                    var match = normalized.FirstOrDefault(x => x.Norm.Contains(c));
                    if (match != null) return match.Index;
                }
                return null;
            }

            int? idIdx = find(new[] { "id", "id_venta", "venta", "identifier", "codigo", "code" });
            int? nombreIdx = find(new[] { "nombre", "nombre_producto", "producto", "title", "name", "descripcion" });
            int? categoriaIdx = find(new[] { "categoria", "categoria_producto", "category", "grupo", "tipo" });
            int? cantidadIdx = find(new[] { "cantidad", "qty", "quantity", "unidades" });
            int? precioUnitIdx = find(new[] { "precio_unitario", "preciounitario", "precio_unit", "unit_price", "precio" });
            int? valorIdx = find(new[] { "total", "total_venta", "precio_unitario", "precio", "valor", "amount", "value" });

            if (!idIdx.HasValue && normalized.Count > 0)
                idIdx = 0;
            if (!valorIdx.HasValue)
                valorIdx = normalized.Count - 1;

            if (!idIdx.HasValue || !valorIdx.HasValue)
                return null;

            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["Id"] = idIdx.Value,
                ["Nombre"] = nombreIdx ?? -1,
                ["Categoria"] = categoriaIdx ?? -1,
                ["Cantidad"] = cantidadIdx ?? -1,
                ["PrecioUnitario"] = precioUnitIdx ?? -1,
                ["Valor"] = valorIdx.Value
            };

            return map;
        }

        // Nuevo: convierte filas mapeadas a DataTable (sin depender de DataItem)
        public static DataTable MapRowsToDataTable(CsvDocument doc, Dictionary<string, int> mapping)
        {
            var dt = new DataTable("ImportedFromCsv");
            if (doc == null) return dt;

            // Crear columnas únicamente a partir de los encabezados del documento.
            if (doc.Headers != null && doc.Headers.Count > 0)
            {
                foreach (var h in doc.Headers)
                {
                    var colName = string.IsNullOrWhiteSpace(h) ? $"Col{dt.Columns.Count + 1}" : h;
                    var final = colName;
                    int k = 1;
                    while (dt.Columns.Contains(final))
                    {
                        final = colName + "_" + k;
                        k++;
                    }
                    dt.Columns.Add(final, typeof(string));
                }
            }
            else
            {
                // Si no hay headers, determinar el nº máximo de columnas en las filas
                int maxCols = doc.Rows != null && doc.Rows.Count > 0 ? doc.Rows.Max(r => r?.Length ?? 0) : 0;
                for (int i = 0; i < maxCols; i++)
                    dt.Columns.Add($"Col{i + 1}", typeof(string));
            }

            // Rellenar filas respetando el número de columnas creado (no se crean columnas extra)
            if (doc.Rows != null)
            {
                foreach (var row in doc.Rows)
                {
                    var dr = dt.NewRow();
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        dr[c] = (row != null && c < row.Length) ? (row[c] ?? string.Empty).Trim() : string.Empty;
                    }
                    dt.Rows.Add(dr);
                }
            }

            // Intentar inferir tipos numéricos (mantiene sólo las columnas presentes en el CSV)
            DataTableHelpers.InferColumnTypes(dt);
            return dt;
        }

        // Helpers
        private static char DetectSeparator(string line)
        {
            var candidates = new[] { ',', ';', '\t', '|' };
            char best = ',';
            int bestCount = -1;

            foreach (var c in candidates)
            {
                int count = line.Count(ch => ch == c);
                if (count > bestCount)
                {
                    bestCount = count;
                    best = c;
                }
            }

            return best;
        }

        private static List<string> ParseCsvLine(string line, char separator)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(line)) return result;

            // Manejo adicional: si toda la línea está entrecomillada (p. ej. "a,b,c"),
            // desenvuelve y desescapa "" -> " para que el parser normal funcione.
            if (line.Length >= 2 && line[0] == '"' && line[line.Length - 1] == '"')
            {
                // quitar comillas exteriores y desdoblar comillas internas
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

        // Normaliza encabezados
        private static string NormalizeHeader(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var formD = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            var cleaned = sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
            cleaned = Regex.Replace(cleaned, @"[^\p{L}\p{Nd}]+", "_");
            cleaned = Regex.Replace(cleaned, @"_+", "_").Trim('_');
            return cleaned;
        }
    }
}