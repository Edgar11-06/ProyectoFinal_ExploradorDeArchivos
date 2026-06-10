using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace SistemaMultimedia.DataFusion
{
    public sealed class MariaDbRepository
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        public MariaDbRepository(string connectionString, string databaseName = "ConexionMaria")
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
            _databaseName = string.IsNullOrWhiteSpace(databaseName) ? "ConexionMaria" : databaseName;
        }

        public async Task<(bool Success, string Message)> TestConnectionAsync(string? overrideConnectionString = null, int timeoutSeconds = 15)
        {
            try
            {
                var cs = overrideConnectionString ?? _connectionString;
                var builder = new MySqlConnectionStringBuilder(cs)
                {
                    Database = "mysql",
                    ConnectionTimeout = (uint)timeoutSeconds
                };

                await using var conn = new MySqlConnection(builder.ConnectionString);
                await conn.OpenAsync();
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync();
                return (true, "Conexión OK");
            }
            catch (MySqlException ex)
            {
                return (false, $"Error MySQL ({ex.Number}): {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task InitializeAsync(int connectTimeoutSeconds = 15)
        {
            var builder = new MySqlConnectionStringBuilder(_connectionString)
            {
                Database = "mysql",
                ConnectionTimeout = (uint)connectTimeoutSeconds
            };

            await using var conn = new MySqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var createDbSql = $"CREATE DATABASE IF NOT EXISTS `{_databaseName}` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;";
            await using (var cmd = new MySqlCommand(createDbSql, conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static string QuoteIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));
            // escapar backticks dentro del nombre
            return "`" + identifier.Replace("`", "``") + "`";
        }

        private static string MapTypeToMySql(DataColumn col)
        {
            var t = col.DataType;
            if (t == typeof(int)) return "INT";
            if (t == typeof(long)) return "BIGINT";
            if (t == typeof(bool)) return "TINYINT(1)";
            if (t == typeof(decimal)) return "DECIMAL(18,4)";
            if (t == typeof(double) || t == typeof(float)) return "DOUBLE";
            if (t == typeof(DateTime)) return "DATETIME";
            if (t == typeof(Guid)) return "CHAR(36)";
            // string default
            // usar VARCHAR(4000) razonable o TEXT si muy largo
            if (t == typeof(string) && col.MaxLength > 0 && col.MaxLength <= 4000)
                return $"VARCHAR({col.MaxLength})";
            return "LONGTEXT";
        }

        private static async Task<bool> TableExistsAsync(MySqlConnection conn, string database, string tableName)
        {
            var q = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table LIMIT 1;";
            await using var cmd = new MySqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@schema", database);
            cmd.Parameters.AddWithValue("@table", tableName);
            var r = await cmd.ExecuteScalarAsync();
            return r != null;
        }

        // Crea tabla dinámica a partir del esquema del DataTable
        public async Task CreateTableFromDataTable(DataTable table, string tableName, int connectTimeoutSeconds = 15)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            if (table.Columns.Count == 0) return; // protección: no crear tablas vacías

            var builder = new MySqlConnectionStringBuilder(_connectionString)
            {
                Database = "mysql",
                ConnectionTimeout = (uint)connectTimeoutSeconds
            };

            await using var conn = new MySqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            // usar la base de datos destino
            await using (var useCmd = new MySqlCommand($"USE `{_databaseName}`;", conn))
            {
                await useCmd.ExecuteNonQueryAsync();
            }

            if (await TableExistsAsync(conn, _databaseName, tableName))
                return;

            var sb = new StringBuilder();
            sb.Append($"CREATE TABLE {QuoteIdentifier(tableName)} (");
            var defs = table.Columns.Cast<DataColumn>().Select(col =>
            {
                var name = QuoteIdentifier(col.ColumnName);
                var sqlType = MapTypeToMySql(col);
                var nullable = col.AllowDBNull || col.DataType == typeof(string) ? "NULL" : "NULL";
                return $"{name} {sqlType} {nullable}";
            }).ToArray();
            sb.Append(string.Join(", ", defs));
            sb.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");

            await using var cmd = new MySqlCommand(sb.ToString(), conn);
            await cmd.ExecuteNonQueryAsync();
        }

        // Lee toda la tabla y devuelve DataTable dinámico
        public async Task<DataTable> LoadItemsAsync(string tableName, int connectTimeoutSeconds = 15)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));

            var dt = new DataTable(tableName);

            var builder = new MySqlConnectionStringBuilder(_connectionString)
            {
                Database = _databaseName,
                ConnectionTimeout = (uint)connectTimeoutSeconds
            };

            await using var conn = new MySqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var q = $"SELECT * FROM {QuoteIdentifier(tableName)}";
            await using var cmd = new MySqlCommand(q, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var type = reader.GetFieldType(i);
                dt.Columns.Add(name, type);
            }

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

        // Inserta todas las filas del DataTable en la tabla indicada (crea la tabla si falta)
        public async Task SaveDataTableAsync(DataTable table, string tableName, int connectTimeoutSeconds = 15)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            if (table.Columns.Count == 0) return;

            var builder = new MySqlConnectionStringBuilder(_connectionString)
            {
                Database = "mysql",
                ConnectionTimeout = (uint)connectTimeoutSeconds
            };

            await using var conn = new MySqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            // usar DB destino
            await using (var useCmd = new MySqlCommand($"USE `{_databaseName}`;", conn))
            {
                await useCmd.ExecuteNonQueryAsync();
            }

            if (!await TableExistsAsync(conn, _databaseName, tableName))
            {
                await CreateTableFromDataTable(table, tableName, connectTimeoutSeconds);
            }

            var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var quotedCols = columnNames.Select(QuoteIdentifier).ToArray();
            var paramNames = columnNames.Select((c, i) => $"@p{i}").ToArray();

            var insertSql = new StringBuilder();
            insertSql.Append($"INSERT INTO {QuoteIdentifier(tableName)} (");
            insertSql.Append(string.Join(", ", quotedCols));
            insertSql.Append(") VALUES (");
            insertSql.Append(string.Join(", ", paramNames));
            insertSql.Append(");");

            await using var tran = await conn.BeginTransactionAsync();
            await using var cmd = new MySqlCommand(insertSql.ToString(), conn, tran);

            // crear parámetros reutilizables como MySqlParameter sin tipo fijo (varchar por defecto)
            cmd.Parameters.Clear();
            for (int i = 0; i < columnNames.Count; i++)
            {
                var p = new MySqlParameter(paramNames[i], MySqlDbType.VarChar);
                p.Value = DBNull.Value;
                cmd.Parameters.Add(p);
            }

            try
            {
                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        var col = table.Columns[columnNames[i]];
                        var val = row.IsNull(col) ? DBNull.Value : row[col];
                        cmd.Parameters[paramNames[i]].Value = val ?? DBNull.Value;
                    }

                    await cmd.ExecuteNonQueryAsync();
                }

                await tran.CommitAsync();
            }
            catch (MySqlException ex)
            {
                try { await tran.RollbackAsync(); } catch { /* ignorar */ }
                throw new InvalidOperationException($"Error MySQL ({ex.Number}): {ex.Message}", ex);
            }
            catch
            {
                try { await tran.RollbackAsync(); } catch { /* ignorar */ }
                throw;
            }
        }
    }
}