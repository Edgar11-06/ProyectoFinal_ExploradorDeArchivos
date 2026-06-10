using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SistemaMultimedia.DataFusion
{
    public static class DataTableHelpers
    {
        public static void EnsureOriginalIndexColumn(DataTable dt)
        {
            if (dt == null) return;
            if (!dt.Columns.Contains("__original_index"))
            {
                var col = new DataColumn("__original_index", typeof(int));
                dt.Columns.Add(col);
                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["__original_index"] = i;
            }
        }

        public static void InferColumnTypes(DataTable dt)
        {
            if (dt == null) return;
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
                            else
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
                catch { /* si falla, dejamos la columna como string */ }
            }
        }
    }
}