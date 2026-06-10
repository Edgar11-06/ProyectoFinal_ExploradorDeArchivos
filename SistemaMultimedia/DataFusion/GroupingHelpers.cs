using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SistemaMultimedia.DataFusion
{
    public static class GroupingHelpers
    {
        // Agrupa una DataTable por una columna y devuelve Dictionary<Categoria, DataTable>
        public static Dictionary<string, DataTable> GroupByCategoria(DataTable table, string categoriaColumn)
        {
            var result = new Dictionary<string, DataTable>(StringComparer.CurrentCultureIgnoreCase);
            if (table == null || string.IsNullOrEmpty(categoriaColumn) || !table.Columns.Contains(categoriaColumn)) return result;

            var groups = table.AsEnumerable()
                              .GroupBy(r => (r.Field<object>(categoriaColumn)?.ToString() ?? string.Empty).Trim())
                              .ToList();

            foreach (var g in groups)
            {
                var dt = table.Clone();
                foreach (var row in g)
                    dt.ImportRow(row);
                result[g.Key] = dt;
            }
            return result;
        }

        // Agrupación dinámica por una o varias columnas.
        // keys -> columnas por las que agrupar (orden importante).
        // Devuelve diccionario donde la clave es "Col1=val1 | Col2=val2" y el DataTable con las filas del grupo.
        public static Dictionary<string, DataTable> GroupByColumns(DataTable table, IEnumerable<string> keys)
        {
            var result = new Dictionary<string, DataTable>(StringComparer.CurrentCultureIgnoreCase);
            if (table == null) return result;

            var keyCols = (keys ?? Array.Empty<string>()).Where(k => !string.IsNullOrWhiteSpace(k) && table.Columns.Contains(k)).ToArray();
            if (keyCols.Length == 0)
            {
                // no hay columnas válidas, devolver todo como un grupo vacío con clave "(Todos)"
                var all = table.Copy();
                result["(Todos)"] = all;
                return result;
            }

            var groups = table.AsEnumerable()
                              .GroupBy(row =>
                                  string.Join(" | ",
                                      keyCols.Select(k =>
                                      {
                                          var v = row.Field<object>(k)?.ToString() ?? string.Empty;
                                          return $"{k}={v.Trim()}";
                                      })
                                  )
                              );

            foreach (var g in groups)
            {
                var dt = table.Clone();
                foreach (var r in g) dt.ImportRow(r);
                var key = g.Key;
                if (!result.ContainsKey(key)) result[key] = dt;
                else
                {
                    // si por alguna razón ya existe la clave, anexar filas
                    foreach (DataRow r in dt.Rows) result[key].ImportRow(r);
                }
            }

            return result;
        }
    }
}