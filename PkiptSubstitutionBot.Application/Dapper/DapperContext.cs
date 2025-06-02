using Dapper;
using Npgsql;
using System.Data;

namespace PkiptSubstitutionBot.Application.Dapper;

public class DapperContext
{
    private readonly string _connectionString;
    
    private static IDbConnection? _connection;
    
    public DapperContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<T?> FirstOrDefault<T>(QueryObject queryObject)
    {
        return await RunQuery(query =>
            query.QueryFirstOrDefaultAsync<T>(queryObject.Sql, queryObject.Params));
    }

    public async Task<IEnumerable<T>> GetAll<T>(QueryObject queryObject)
    {
        return await RunQuery(query =>
            query.QueryAsync<T>(queryObject.Sql, queryObject.Params));
    }

    public async Task Execute(QueryObject queryObject)
    {
        await RunQuery(query =>
            query.ExecuteAsync(queryObject.Sql, queryObject.Params));
    }

    private async Task<T> RunQuery<T>(Func<IDbConnection, Task<T>> query)
    {
        _connection = new NpgsqlConnection(_connectionString);

        var result = await query(_connection);

        _connection.Close();

        return result;
    }
}
