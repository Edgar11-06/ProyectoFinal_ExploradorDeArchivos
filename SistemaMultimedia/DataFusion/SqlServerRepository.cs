using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SistemaMultimedia.DataFusion
{
    public sealed class SqlServerRepository
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        public SqlServerRepository(string connectionString, string databaseName = "ConexionSQL")
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
            _databaseName = string.IsNullOrWhiteSpace(databaseName) ? "ConexionSQL" : databaseName;
        }

        // Test de conexión ligero; devuelve (Success, Message)
        public async Task<(bool Success, string Message)> TestConnectionAsync(string? overrideConnectionString = null, int timeoutSeconds = 15)
        {
            try
            {
                var cs = overrideConnectionString ?? _connectionString;
                var builder = new SqlConnectionStringBuilder(cs)
                {
                    InitialCatalog = "master",
                    ConnectTimeout = timeoutSeconds
                };

                await using var conn = new SqlConnection(builder.ConnectionString);
                await conn.OpenAsync();
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync();
                return (true, "Conexión OK");
            }
            catch (SqlException ex)
            {
                return (false, $"Error SQL ({ex.Number}): {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        // Crea la base de datos si no existe; no crea tablas fijas (las tablas se crean dinámicamente desde CreateTableFromDataTable).
        public async Task InitializeAsync(int connectTimeoutSeconds = 15)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master",
                ConnectTimeout = connectTimeoutSeconds
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var createDbSql = $@"
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{_databaseName}')
BEGIN
    CREATE DATABASE [{_databaseName}];
END";
            await using (var cmd = new SqlCommand(createDbSql, conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // Verifica si la tabla existe en la base de datos actual
        private static async Task<bool> TableExistsAsync(SqlConnection conn, string schema, string tableName)
        {
            var q = @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table";
            await using var cmd = new SqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@schema", schema);
            cmd.Parameters.AddWithValue("@table", tableName);
            var r = await cmd.ExecuteScalarAsync();
            return r != null;
        }

        // Escapa identificadores para SQL Server ([name] y dobla ] dentro)
        private static string QuoteIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));
            return "[" + identifier.Replace("]", "]]") + "]";
        }

        // Map .NET types to SQL Server types (fallback NVARCHAR(MAX))
        private static string MapTypeToSql(DataColumn col)
        {
            var type = col.DataType;
            var allowNull = col.AllowDBNull;
            // prefer explicit mapping; fallback to NVARCHAR(MAX)
            if (type == typeof(int)) return "INT";
            if (type == typeof(long)) return "BIGINT";
            if (type == typeof(bool)) return "BIT";
            if (type == typeof(decimal)) return "DECIMAL(18,4)";
            if (type == typeof(double) || type == typeof(float)) return "FLOAT";
            if (type == typeof(DateTime)) return "DATETIME2";
            if (type == typeof(Guid)) return "UNIQUEIDENTIFIER";
            // default string-like
            // if MaxLength set and reasonable, use NVARCHAR(length)
            if (type == typeof(string) && col.MaxLength > 0 && col.MaxLength <= 4000)
                return $"NVARCHAR({col.MaxLength})";
            return "NVARCHAR(MAX)";
        }

        // 2. Crear tabla dinámicamente a partir de un DataTable (evita crear si ya existe).
        public async Task CreateTableFromDataTable(DataTable table, string tableName, string schema = "dbo", int connectTimeoutSeconds = 15)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));

            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = _databaseName,
                ConnectTimeout = connectTimeoutSeconds
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var tableExists = await TableExistsAsync(conn, schema, tableName);
            if (!tableExists)
            {
                // construir CREATE TABLE
                var sb = new StringBuilder();
                sb.AppendLine($"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName)");
                sb.AppendLine("BEGIN");
                sb.Append($"CREATE TABLE {QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)} (");

                var defs = table.Columns.Cast<DataColumn>().Select(col =>
                {
                    var name = QuoteIdentifier(col.ColumnName);
                    var sqlType = MapTypeToSql(col);
                    // dejar NULL por defecto (permisivo)
                    return $"{name} {sqlType} NULL";
                }).ToList();

                sb.Append(string.Join(", ", defs));
                sb.AppendLine(");");
                sb.AppendLine("END");

                await using var cmdCreate = new SqlCommand(sb.ToString(), conn);
                cmdCreate.Parameters.AddWithValue("@schema", schema);
                cmdCreate.Parameters.AddWithValue("@tableName", tableName);
                await cmdCreate.ExecuteNonQueryAsync();
                return;
            }

            // Si la tabla ya existe: comprobar columnas existentes y añadir las que falten.
            var existingCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var q = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table";
            await using (var cmd = new SqlCommand(q, conn))
            {
                cmd.Parameters.AddWithValue("@schema", schema);
                cmd.Parameters.AddWithValue("@table", tableName);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    existingCols.Add(reader.GetString(0));
                }
            }

            var alters = new List<string>();
            foreach (DataColumn col in table.Columns)
            {
                if (existingCols.Contains(col.ColumnName)) continue;
                var colDef = $"{QuoteIdentifier(col.ColumnName)} {MapTypeToSql(col)} NULL";
                alters.Add(colDef);
            }

            if (alters.Count == 0) return;

            // Ejecutar ALTER TABLE ADD para las columnas faltantes (en bloques)
            foreach (var chunk in SplitIntoChunks(alters, 10))
            {
                var sbAlter = new StringBuilder();
                sbAlter.Append($"ALTER TABLE {QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)} ADD ");
                sbAlter.Append(string.Join(", ", chunk));
                await using var cmdAlter = new SqlCommand(sbAlter.ToString(), conn);
                await cmdAlter.ExecuteNonQueryAsync();
            }
        }

        // Helper para dividir en trozos pequeños (evitar sentencias excesivamente largas)
        private static IEnumerable<List<T>> SplitIntoChunks<T>(List<T> source, int chunkSize)
        {
            for (int i = 0; i < source.Count; i += chunkSize)
                yield return source.GetRange(i, Math.Min(chunkSize, source.Count - i));
        }

        // 4. LoadItemsAsync ahora devuelve DataTable dinámico (SELECT * FROM [schema].[tableName]).
        public async Task<DataTable> LoadItemsAsync(string tableName, string schema = "dbo", int connectTimeoutSeconds = 15)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));

            var dt = new DataTable(tableName);

            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = _databaseName,
                ConnectTimeout = connectTimeoutSeconds
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var q = $"SELECT * FROM {QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)}";
            await using var cmd = new SqlCommand(q, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            // crear columnas en dt según reader metadata
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var type = reader.GetFieldType(i);
                // DataColumn requiere non-nullable Type; allow DBNull will be handled per-row
                dt.Columns.Add(name, type);
            }

            // cargar filas
            while (await reader.ReadAsync())
            {
                var row = dt.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = await reader.IsDBNullAsync(i) ? DBNull.Value : reader.GetValue(i);
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        // 3. SaveDataTableAsync: inserta dinámicamente todas las filas del DataTable en la tabla especificada.
        public async Task SaveDataTableAsync(DataTable table, string tableName, string schema = "dbo", int connectTimeoutSeconds = 15)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            if (table.Columns.Count == 0) return;

            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = _databaseName,
                ConnectTimeout = connectTimeoutSeconds
            };

            await using var conn = new SqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            // crear tabla si falta
            if (!await TableExistsAsync(conn, schema, tableName))
            {
                // crear tabla según el DataTable schema
                await CreateTableFromDataTable(table, tableName, schema, connectTimeoutSeconds);
            }

            // construir INSERT dinámico
            var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var quotedCols = columnNames.Select(c => QuoteIdentifier(c)).ToArray();
            var paramNames = columnNames.Select((c, i) => $"@p{i}").ToArray();

            var insertSql = new StringBuilder();
            insertSql.Append($"INSERT INTO {QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)} (");
            insertSql.Append(string.Join(", ", quotedCols));
            insertSql.Append(") VALUES (");
            insertSql.Append(string.Join(", ", paramNames));
            insertSql.Append(");");

            using var tran = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;
            cmd.CommandText = insertSql.ToString();

            // crear parámetros reutilizables
            cmd.Parameters.Clear();
            for (int i = 0; i < columnNames.Count; i++)
            {
                // Param type generic (SqlDbType.Variant) - let SqlClient infer from object, better to leave as default.
                var p = cmd.Parameters.Add(paramNames[i], SqlDbType.NVarChar);
                p.Value = DBNull.Value;
            }

            try
            {
                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        var col = table.Columns[columnNames[i]];
                        var val = row.IsNull(col) ? DBNull.Value : row[col];
                        // Convert value for SqlParameter where necessary
                        cmd.Parameters[paramNames[i]].Value = val ?? DBNull.Value;
                    }

                    await cmd.ExecuteNonQueryAsync();
                }

                tran.Commit();
            }
            catch (SqlException ex)
            {
                try { tran.Rollback(); } catch { /* ignorar */ }
                throw new InvalidOperationException($"Error de SQL ({ex.Number}): {ex.Message}", ex);
            }
            catch
            {
                try { tran.Rollback(); } catch { /* ignorar */ }
                throw;
            }
        }
    }
}