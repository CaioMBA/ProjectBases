using Dapper;
using Dapper.Contrib.Extensions;
using Domain.Enums;
using Domain.Models.ApplicationConfigurationModels;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Data.Database
{
    public class DefaultDatabaseAccess
    {
        private IDbConnection? _connection = null;
        #region Relational Data Base
        private IDbConnection Conectar(DataBaseConnectionModel bd)
        {
            if (_connection != null
                && _connection.ConnectionString.Equals(bd.ConnectionString, StringComparison.OrdinalIgnoreCase)
                && _connection.State == ConnectionState.Open)
            {
                return _connection;
            }

            _connection = bd.Type switch
            {
                DataBaseType.SQLSERVER => new SqlConnection(bd.ConnectionString),
                DataBaseType.ORACLE => new OracleConnection(bd.ConnectionString),
                DataBaseType.MYSQL => new MySqlConnection(bd.ConnectionString),
                DataBaseType.MARIADB => new MySqlConnection(bd.ConnectionString),
                DataBaseType.POSTGRESQL => new NpgsqlConnection(bd.ConnectionString),
                _ => throw new NotSupportedException($"Database type '{bd.Type}' is not supported.")
            };

            if (_connection.State == ConnectionState.Closed)
            {
                if (string.IsNullOrEmpty(_connection.ConnectionString))
                {
                    _connection.ConnectionString = _connection.ConnectionString = bd.ConnectionString;
                }
                _connection.Open();
            }

            return _connection;
        }

        public async Task<T?> ProcedureFirstOrDefaultAsync<T>(string sQuery, object parameter, DataBaseConnectionModel connection)
        {
            return await ExecQueryFirstOrDefault<T>(sQuery, parameter, connection, CommandType.StoredProcedure);
        }


        public async Task<List<T>?> Procedure<T>(string sQuery, object parameter, DataBaseConnectionModel connection)
        {
            return await ExecQuery<T>(sQuery, parameter, connection, CommandType.StoredProcedure);
        }

        public async Task<List<Dictionary<string, object?>>?> ProcedureMultipleTable(string sQuery, object parameter, DataBaseConnectionModel connection)
        {
            var objReturn = new List<Dictionary<string, object?>>();

            using (var _DBConnection = Conectar(connection))
            {
                try
                {
                    if (_DBConnection.State != ConnectionState.Open)
                        _DBConnection.Open();

                    using (var result = await _DBConnection.QueryMultipleAsync(sQuery, parameter, commandType: CommandType.StoredProcedure))
                    {
                        while (!result.IsConsumed)
                        {
                            var tableData = (await result.ReadAsync())
                                .Select(row => (IDictionary<string, object?>)row)
                                .Select(row => row.ToDictionary(kv => kv.Key, kv => kv.Value))
                                .ToList();

                            objReturn.AddRange(tableData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }

            return objReturn;
        }


        public async Task<T?> QueryFirstOrDefault<T>(string sQuery, object? parameter, DataBaseConnectionModel connection)
        {
            return await ExecQueryFirstOrDefault<T>(sQuery, parameter, connection);
        }

        public async Task<List<T>?> Query<T>(string sQuery, object? parameter, DataBaseConnectionModel connection)
        {
            return await ExecQuery<T>(sQuery, parameter, connection);
        }

        #region BaseMethods
        private async Task<List<T>?> ExecQuery<T>(string sQuery, object? parameter, DataBaseConnectionModel connection, CommandType? type = null)
        {
            using (var _DBConnection = Conectar(connection))
            {
                try
                {
                    if (_DBConnection.State != ConnectionState.Open)
                        _DBConnection.Open();

                    var result = await _DBConnection.QueryAsync<T>(sQuery, parameter, commandType: type);
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }
        }

        private async Task<T?> ExecQueryFirstOrDefault<T>(string sQuery, object? parameter, DataBaseConnectionModel connection, CommandType? type = null)
        {
            using (var _DBConnection = Conectar(connection))
            {
                try
                {
                    if (_DBConnection.State != ConnectionState.Open)
                        _DBConnection.Open();

                    return await _DBConnection.QueryFirstOrDefaultAsync<T>(sQuery, parameter, commandType: type);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return default;
                }
            }
        }

        public async Task<T> Read<T>(object Id, DataBaseConnectionModel connection) where T : class
        {
            using (var _DBConnection = Conectar(connection))
            {
                return await _DBConnection.GetAsync<T>(Id);
            }

        }

        public async Task<bool> Update<T>(T Obj, DataBaseConnectionModel connection) where T : class
        {
            using (var _DBConnection = Conectar(connection))
            {
                return await _DBConnection.UpdateAsync(Obj);
            }
        }

        public async Task<object> Insert<T>(T Obj, DataBaseConnectionModel connection) where T : class
        {
            using (var _DBConnection = Conectar(connection))
            {
                return await _DBConnection.InsertAsync(Obj);
            }
        }

        public async Task<bool> Delete<T>(T Obj, DataBaseConnectionModel connection) where T : class
        {
            using (var _DBConnection = Conectar(connection))
            {
                return await _DBConnection.DeleteAsync(Obj);
            }
        }
        #endregion BaseMethods

        #endregion
    }
}
