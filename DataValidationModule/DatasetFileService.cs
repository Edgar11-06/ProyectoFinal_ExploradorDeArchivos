using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DataValidationModule
{
    /// <summary>
    /// Importación y exportación de datasets (CSV, JSON, XML, Excel).
    /// Reemplaza la antigua clase <c>DatasetImporter</c> y la lógica de guardado del formulario.
    /// </summary>
    public static class DatasetFileService
    {
        // -------------------------------------------------------------------
        // Importación
        // -------------------------------------------------------------------

        public static DataTable ImportFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"No se encontró el archivo: {filePath}");

            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            switch (ext)
            {
                case ".csv":  return ImportCsv(filePath);
                case ".json": return ImportJson(filePath);
                case ".xml":  return ImportXml(filePath);
                case ".xlsx":
                case ".xls":  return ImportExcel(filePath);
                default:
                    throw new NotSupportedException(
                        $"Formato '{ext}' no soportado. Use CSV, JSON, XML o Excel.");
            }
        }

        public static DataTable ImportCsv(string filePath, char delimiter = '\0')
        {
            var dt = new DataTable(Path.GetFileNameWithoutExtension(filePath));

            using (var reader = new StreamReader(filePath, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            {
                string headerLine = reader.ReadLine();
                if (headerLine == null) return dt;

                if (delimiter == '\0')
                    delimiter = headerLine.Contains(';') ? ';' : ',';

                var headers = ParseCsvLine(headerLine, delimiter);
                foreach (var h in headers)
                    dt.Columns.Add(h.Trim('"').Trim());

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var fields = ParseCsvLine(line, delimiter);

                    var row = dt.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++)
                        row[i] = i < fields.Count ? fields[i].Trim('"') : string.Empty;

                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        public static DataTable ImportJson(string filePath)
        {
            var dt      = new DataTable(Path.GetFileNameWithoutExtension(filePath));
            string json = File.ReadAllText(filePath, Encoding.UTF8).Trim();

            try
            {
                var rows = ExtractJsonObjects(json);
                if (rows.Count == 0) return dt;

                foreach (var key in rows[0].Keys)
                    dt.Columns.Add(key);

                foreach (var obj in rows)
                {
                    var row = dt.NewRow();
                    foreach (var col in dt.Columns)
                        row[col.ToString()] = obj.ContainsKey(col.ToString())
                            ? obj[col.ToString()] : string.Empty;
                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Error al parsear JSON: {ex.Message}", ex);
            }
            return dt;
        }

        public static DataTable ImportXml(string filePath)
        {
            var dt  = new DataTable(Path.GetFileNameWithoutExtension(filePath));
            var doc = new XmlDocument();
            doc.Load(filePath);

            XmlNodeList rows = doc.DocumentElement?.ChildNodes;
            if (rows == null || rows.Count == 0) return dt;

            foreach (XmlNode child in rows[0].ChildNodes)
                if (dt.Columns.IndexOf(child.Name) < 0)
                    dt.Columns.Add(child.Name);

            foreach (XmlNode rowNode in rows)
            {
                var row = dt.NewRow();
                foreach (XmlNode cell in rowNode.ChildNodes)
                {
                    if (dt.Columns.Contains(cell.Name))
                        row[cell.Name] = cell.InnerText;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable ImportExcel(string filePath)
        {
            throw new NotImplementedException(
                "Para importar Excel, instale el paquete NuGet 'NPOI' y descomente el bloque " +
                "NPOI en DatasetFileService.ImportExcel(). Alternativamente exporte a CSV primero.");
        }

        // -------------------------------------------------------------------
        // Exportación
        // -------------------------------------------------------------------

        /// <summary>
        /// Guarda un DataTable según la extensión del archivo destino.
        /// </summary>
        public static void ExportToFile(DataTable dt, string filePath)
        {
            if (dt == null) throw new ArgumentNullException(nameof(dt));

            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            switch (ext)
            {
                case ".csv":
                    File.WriteAllText(filePath, ToCsv(dt), Encoding.UTF8);
                    break;
                case ".txt":
                    File.WriteAllText(filePath, ToTxt(dt), Encoding.UTF8);
                    break;
                case ".xml":
                    ToXml(dt, filePath);
                    break;
                default:
                    File.WriteAllText(filePath, ToCsv(dt), Encoding.UTF8);
                    break;
            }
        }

        public static string ToCsv(DataTable dt)
        {
            var sb = new StringBuilder();
            var cols = dt.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName)).ToArray();
            sb.AppendLine(string.Join(",", cols));
            foreach (DataRow r in dt.Rows)
            {
                var vals = dt.Columns.Cast<DataColumn>()
                    .Select(c => EscapeCsv(r[c] == DBNull.Value ? "" : r[c].ToString()));
                sb.AppendLine(string.Join(",", vals));
            }
            return sb.ToString();
        }

        public static string ToTxt(DataTable dt)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join('\t', dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
            foreach (DataRow r in dt.Rows)
            {
                var row = string.Join('\t', dt.Columns.Cast<DataColumn>()
                    .Select(c => r[c] == DBNull.Value ? "" : r[c].ToString()));
                sb.AppendLine(row);
            }
            return sb.ToString();
        }

        public static void ToXml(DataTable dt, string path)
        {
            var ds = new DataSet("DatasetExport");
            ds.Tables.Add(dt.Copy());
            ds.WriteXml(path, XmlWriteMode.WriteSchema);
        }

        // -------------------------------------------------------------------
        // Helpers internos
        // -------------------------------------------------------------------

        private static string EscapeCsv(string s)
            => s == null ? "" : (s.Contains(",") || s.Contains("\"") || s.Contains("\n")
                ? "\"" + s.Replace("\"", "\"\"") + "\""
                : s);

        private static List<string> ParseCsvLine(string line, char delimiter)
        {
            var fields  = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == delimiter && !inQuotes)
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            fields.Add(current.ToString());
            return fields;
        }

        private static List<Dictionary<string, string>> ExtractJsonObjects(string json)
        {
            var results = new List<Dictionary<string, string>>();

            int start = json.IndexOf('[');
            int end   = json.LastIndexOf(']');
            if (start < 0 || end < 0) return results;

            string content = json.Substring(start + 1, end - start - 1);

            int depth    = 0;
            int objStart = -1;

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] == '{') { if (depth++ == 0) objStart = i; }
                else if (content[i] == '}')
                {
                    if (--depth == 0 && objStart >= 0)
                    {
                        string objStr = content.Substring(objStart, i - objStart + 1);
                        results.Add(ParseJsonObject(objStr));
                        objStart = -1;
                    }
                }
            }
            return results;
        }

        private static Dictionary<string, string> ParseJsonObject(string obj)
        {
            var dict  = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var regex = new System.Text.RegularExpressions.Regex(
                @"""(?<key>[^""]+)""\s*:\s*(?:""(?<val>[^""]*)""|(?<val>[^,}\s]+))");

            foreach (System.Text.RegularExpressions.Match m in regex.Matches(obj))
                dict[m.Groups["key"].Value] = m.Groups["val"].Value;

            return dict;
        }
    }
}
