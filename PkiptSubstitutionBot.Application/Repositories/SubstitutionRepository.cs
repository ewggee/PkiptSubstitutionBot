using PkiptSubstitutionBot.Application.Dapper;
using PkiptSubstitutionBot.Application.Models;

namespace PkiptSubstitutionBot.Application.Repositories;

public class SubstitutionRepository
{
    private readonly DapperContext _dapperContext;

    public SubstitutionRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<DbSubstitution?> GetSubstitutionAsync(DateTime date)
    {
        var substitution = await _dapperContext.FirstOrDefault<DbSubstitution>(new QueryObject(
            sql: "SELECT * FROM substitutions WHERE date = @date",
            parameters: new { date = date.Date }));

        return substitution;
    }

    public async Task AddSubstitutionAsync(DbSubstitution substitution)
    {
        await _dapperContext.Execute(new QueryObject(
            sql: "INSERT INTO substitutions(image, date) VALUES (@image, @date)",
            parameters: new { image = substitution.Image, date = substitution.Date }));
    }

    public async Task UpdateSubstitutionAsync(DbSubstitution substitution)
    {
        //Черный ворон кружит под окном

        await _dapperContext.Execute(new QueryObject(
            sql: "UPDATE substitutions SET image = @image WHERE date = @date",
            parameters: new { image = substitution.Image, date = substitution.Date }));
    }
}
