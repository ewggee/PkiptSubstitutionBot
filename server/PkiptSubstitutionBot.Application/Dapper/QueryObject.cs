namespace PkiptSubstitutionBot.Application.Dapper;

public class QueryObject
{
    public string Sql { get; }
    public object Params { get; set; }

    public QueryObject(string sql, object parameters)
    {
        Sql = sql;
        Params = parameters;
    }
}
