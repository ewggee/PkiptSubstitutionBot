using PkiptSubstitutionBot.Application.Models;
using PkiptSubstitutionBot.Application.Repositories;

namespace PkiptSubstitutionBot.Application.Services;

public class SubstitutionService
{
    private readonly SubstitutionRepository _substitutionRepository;

    public SubstitutionService(SubstitutionRepository substitutionRepository)
    {
        _substitutionRepository = substitutionRepository;
    }

    public async Task<DbSubstitution?> GetSubstitutionAsync(DateOnly date)
    {
        var substitution = await _substitutionRepository.GetSubstitutionAsync(date.ToDateTime(TimeOnly.MinValue));

        return substitution;
    }

    public async Task AddOrUpdateSubstituionAsync(DbSubstitution substitution)
    {
        var existing = await _substitutionRepository.GetSubstitutionAsync(substitution.Date);
        if (existing != null)
        {
            await _substitutionRepository.UpdateSubstitutionAsync(substitution);
            return;
        }

        await _substitutionRepository.AddSubstitutionAsync(substitution);
    }
}
