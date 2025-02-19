using AdoNetCore.AseClient;
using Dapper;
using Domain.Models.ApplicationConfigurationModels;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Data.DatabaseRepositories
{
    public class DefaultDatabaseAccess
    {
        private IDbConnection? _con = null;
        #region Relational Data Base

        public IDbConnection conectar(DataBaseConnectionModel bd)
        {
            _con = bd.Type.ToUpper().Trim() switch
            {
                "SQLSERVER" => new SqlConnection(bd.ConnectionString),
                "ORACLE" => new OracleConnection(bd.ConnectionString),
                "MYSQL" => new MySqlConnection(bd.ConnectionString),
                "MARIADB" => new MySqlConnection(bd.ConnectionString),
                "POSTGRESQL" => new NpgsqlConnection(bd.ConnectionString),
                "FIREBIRD" => new FbConnection(bd.ConnectionString),
                "SYBASE" => new AseConnection(bd.ConnectionString),
                _ => throw new NotSupportedException($"Database type '{bd.Type}' is not supported.")
            };

            if (_con.State == ConnectionState.Closed)
            {
                if (String.IsNullOrEmpty(_con.ConnectionString))
                {
                    _con.ConnectionString = _con.ConnectionString = bd.ConnectionString;
                }
                _con.Open();
            }

            return _con;
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

            using (var _DBConnection = conectar(connection))
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
            using (var _DBConnection = conectar(connection))
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
            using (var _DBConnection = conectar(connection))
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
        #endregion BaseMethods

        #endregion
    }
}
