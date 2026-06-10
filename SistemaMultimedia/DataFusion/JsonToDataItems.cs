using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SistemaMultimedia.DataFusion
{
    // Helper para cargar JSON/NDJSON a DataTable dinámico
    public static class JsonToDataItems
    {
        public static DataTable Load(string path, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var text = File.ReadAllText(path, encoding);
            var dt = new DataTable("ImportedJson");
            var candidates = new List<Dictionary<string, string>>( );

            try
            {
                using var doc = JsonDocument.Parse(text);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in root.EnumerateArray())
                        if (el.ValueKind == JsonValueKind.Object)
                            candidates.Add(FlattenJsonObject(el));
                }
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    candidates.Add(FlattenJsonObject(root));
                }
            }
            catch
            {
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

            var columns = candidates.SelectMany(d => d.Keys).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var col in columns) dt.Columns.Add(col, typeof(string));

            foreach (var dic in candidates)
            {
                var row = dt.NewRow();
                foreach (var c in columns)
                    row[c] = dic.TryGetValue(c, out var v) ? v ?? string.Empty : string.Empty;
                dt.Rows.Add(row);
            }

            InferColumnTypes(dt);
            return dt;
        }

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
                        result[name] = v.GetRawText();
                        break;
                    case JsonValueKind.String:
                        result[name] = v.GetString() ?? string.Empty;
                        break;
                    default:
                        result[name] = v.GetRawText();
                        break;
                }
            }
            return result;
        }

        private static void InferColumnTypes(DataTable dt)
        {
            var toChange = new List<(DataColumn Old, Type NewType)>();

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == "__original_index") continue;
                int numericCount = 0;
                int decimalCount = 0;
                int total = dt.Rows.Count;
                foreach (DataRow r in dt.Rows)
                {
                    var o = r[col];
                    if (o == null || o == DBNull.Value) continue;
                    var s = o.ToString()?.Trim();
                    if (string.IsNullOrEmpty(s)) continue;
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

                if (total == 0) continue;
                if (numericCount >= total * 0.6)
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
