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

    public async Task AddSubstitutionAsync(DbSubstitution subst)
    {
        await _dapperContext.Execute(new QueryObject(
            sql: "INSERT INTO substitutions(images, date, text) VALUES (@images, @date, @text)",
            parameters: new { images = subst.Images, date = subst.Date, text = subst.Text }));
    }

    public async Task UpdateSubstitutionAsync(DbSubstitution subst)
    {
        await _dapperContext.Execute(new QueryObject(
            sql: "UPDATE substitutions SET images = @images, text = @text WHERE date = @date",
            parameters: new { images = subst.Images, text = subst.Text, date = subst.Date }));
    }
}
