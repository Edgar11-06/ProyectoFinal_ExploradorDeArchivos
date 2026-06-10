using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SistemaMultimedia.DataFusion
{
    // Helper para cargar XML dinámico a DataTable
    public static class XmlToDataItems
    {
        public static DataTable Load(string path, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using var sr = new StreamReader(path, encoding);
            var doc = XDocument.Load(sr);
            var root = doc.Root;
            var dt = new DataTable("ImportedXml");
            if (root == null) return dt;

            var records = root.Elements().ToList();
            if (records.Count == 1 && records[0].Elements().Any())
                records = records[0].Elements().ToList();

            var list = new List<Dictionary<string, string>>();
            foreach (var rec in records)
            {
                var dict = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

                // Atributos: solo añadir si no existe la clave
                foreach (var a in rec.Attributes())
                {
                    var attrKey = a.Name.LocalName;
                    if (!dict.ContainsKey(attrKey))
                        dict[attrKey] = a.Value ?? string.Empty;
                }

                // Elementos hoja (descendientes sin hijos adicionales)
                foreach (var leaf in rec.DescendantsAndSelf().Where(x => !x.HasElements))
                {
                    // construir clave relativa desde el nodo de registro hasta el leaf (sin incluir el nombre del registro)
                    var pathParts = leaf.AncestorsAndSelf()
                                         .Reverse()
                                         .SkipWhile(n => n != rec)
                                         .Skip(1) // excluir el nodo rec
                                         .Select(n => n.Name.LocalName)
                                         .ToArray();

                    string simpleKey = leaf.Name.LocalName;
                    string fullKey;
                    if (pathParts.Length == 0)
                    {
                        fullKey = simpleKey;
                    }
                    else
                    {
                        var candidate = string.Join("_", pathParts.Concat(new[] { leaf.Name.LocalName }));
                        // si el candidate es redundante (ej. "X_X"), preferimos el nombre simple para evitar duplicados confusos
                        if (string.Equals(candidate, simpleKey + "_" + simpleKey, StringComparison.OrdinalIgnoreCase))
                            fullKey = simpleKey;
                        else
                            fullKey = candidate;
                    }

                    // Añadir la clave completa si no existe
                    if (!dict.ContainsKey(fullKey))
                        dict[fullKey] = leaf.Value ?? string.Empty;

                    // Añadir también la clave simple si no existe y no coincide con la clave completa
                    if (!string.Equals(fullKey, simpleKey, StringComparison.OrdinalIgnoreCase) && !dict.ContainsKey(simpleKey))
                    {
                        dict[simpleKey] = leaf.Value ?? string.Empty;
                    }
                }

                list.Add(dict);
            }

            if (!list.Any()) return dt;

            var columns = list.SelectMany(d => d.Keys).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var col in columns) dt.Columns.Add(col, typeof(string));

            foreach (var d in list)
            {
                var row = dt.NewRow();
                foreach (var c in columns)
                    row[c] = d.TryGetValue(c, out var v) ? v ?? string.Empty : string.Empty;
                dt.Rows.Add(row);
            }

            InferColumnTypes(dt);
            return dt;
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