using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace SistemaMultimedia.DataFusion
{
    public sealed class PostgreSqlRepository
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        public PostgreSqlRepository(string connectionString, string databaseName = "ConexionPostgres")
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
            _databaseName = string.IsNullOrWhiteSpace(databaseName) ? "ConexionPostgres" : databaseName;
        }

        public async Task<(bool Success, string Message)> TestConnectionAsync(string? overrideConnectionString = null, int timeoutSeconds = 15)
        {
            try
            {
                var cs = overrideConnectionString ?? _connectionString;
                var builder = new NpgsqlConnectionStringBuilder(cs)
                {
                    Database = "postgres",
                    Timeout = timeoutSeconds
                };

                await using var conn = new NpgsqlConnection(builder.ConnectionString);
                await conn.OpenAsync();
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1";
                await cmd.ExecuteScalarAsync();
                return (true, "Conexión OK");
            }
            catch (NpgsqlException ex)
            {
                return (false, $"Error PostgreSQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task InitializeAsync(int connectTimeoutSeconds = 15)
        {
            var builder = new NpgsqlConnectionStringBuilder(_connectionString)
            {
                Database = "postgres",
                Timeout = connectTimeoutSeconds
            };

            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            await using var checkCmd = conn.CreateCommand();
            checkCmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @db";
            checkCmd.Parameters.AddWithValue("db", _databaseName);
            var exists = await checkCmd.ExecuteScalarAsync();
            if (exists == null)
            {
                await using var createCmd = conn.CreateCommand();
                createCmd.CommandText = $"CREATE DATABASE \"{_databaseName}\"";
                await createCmd.ExecuteNonQueryAsync();
            }
        }

        private static string QuoteIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));
            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
        }

        private static string MapTypeToPostgres(DataColumn col)
        {
            var t = col.DataType;
            if (t == typeof(int)) return "INTEGER";
            if (t == typeof(long)) return "BIGINT";
            if (t == typeof(bool)) return "BOOLEAN";
            if (t == typeof(decimal)) return "NUMERIC(18,4)";
            if (t == typeof(double) || t == typeof(float)) return "DOUBLE PRECISION";
            if (t == typeof(DateTime)) return "TIMESTAMP";
            if (t == typeof(Guid)) return "UUID";
            if (t == typeof(string) && col.MaxLength > 0 && col.MaxLength <= 10000)
                return $"VARCHAR({col.MaxLength})";
            return "TEXT";
        }

        private static async Task<bool> TableExistsAsync(NpgsqlConnection conn, string schema, string tableName)
        {
            var q = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table";
            await using var cmd = new NpgsqlCommand(q, conn);
            cmd.Parameters.AddWithValue("schema", schema);
            cmd.Parameters.AddWithValue("table", tableName);
            var r = await cmd.ExecuteScalarAsync();
            return r != null;
        }

        public async Task CreateTableFromDataTable(DataTable table, string tableName, string schema = "public", int connectTimeoutSeconds = 15)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));

            var builder = new NpgsqlConnectionStringBuilder(_connectionString)
            {
                Database = _databaseName,
                Timeout = connectTimeoutSeconds
            };

            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            if (await TableExistsAsync(conn, schema, tableName))
                return;

            var sb = new StringBuilder();
            sb.Append($"CREATE TABLE {QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)} (");
            var defs = table.Columns.Cast<DataColumn>().Select(col =>
            {
                var name = QuoteIdentifier(col.ColumnName);
                var sqlType = MapTypeToPostgres(col);
                var nullable = col.AllowDBNull || col.DataType == typeof(string) ? "NULL" : "NULL";
                return $"{name} {sqlType} {nullable}";
            }).ToArray();
            sb.Append(string.Join(", ", defs));
            sb.Append(");");

            await using var cmd = new NpgsqlCommand(sb.ToString(), conn);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<DataTable> LoadItemsAsync(string tableName, string schema = "public", int connectTimeoutSeconds = 15)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            var dt = new DataTable(tableName);

            var builder = new NpgsqlConnectionStringBuilder(_connectionString)
            {
                Database = _databaseName,
                Timeout = connectTimeoutSeconds
            };

            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            var q = $"SELECT * FROM {QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)}";
            await using var cmd = new NpgsqlCommand(q, conn);
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
                    row[i] = await reader.IsDBNullAsync(i) ? DBNull.Value : reader.GetValue(i);
                dt.Rows.Add(row);
            }

            return dt;
        }

        public async Task SaveDataTableAsync(DataTable table, string tableName, string schema = "public", int connectTimeoutSeconds = 15)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));
            if (table.Columns.Count == 0) return;

            var builder = new NpgsqlConnectionStringBuilder(_connectionString)
            {
                Database = _databaseName,
                Timeout = connectTimeoutSeconds
            };

            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            if (!await TableExistsAsync(conn, schema, tableName))
            {
                await CreateTableFromDataTable(table, tableName, schema, connectTimeoutSeconds);
            }

            var columnNames = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var quotedCols = columnNames.Select(QuoteIdentifier).ToArray();
            var paramNames = columnNames.Select((c, i) => $"@p{i}").ToArray();

            var insertSql = new StringBuilder();
            insertSql.Append($"INSERT INTO {QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)} (");
            insertSql.Append(string.Join(", ", quotedCols));
            insertSql.Append(") VALUES (");
            insertSql.Append(string.Join(", ", paramNames));
            insertSql.Append(");");

            await using var tran = await conn.BeginTransactionAsync();
            await using var cmd = new NpgsqlCommand(insertSql.ToString(), conn, tran);

            cmd.Parameters.Clear();
            for (int i = 0; i < columnNames.Count; i++)
            {
                var p = cmd.Parameters.AddWithValue(paramNames[i], DBNull.Value);
                p.IsNullable = true;
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
            catch (PostgresException ex)
            {
                try { await tran.RollbackAsync(); } catch { }
                throw new InvalidOperationException($"Error PostgreSQL: {ex.Message}", ex);
            }
            catch
            {
                try { await tran.RollbackAsync(); } catch { }
                throw;
            }
        }
    }
}