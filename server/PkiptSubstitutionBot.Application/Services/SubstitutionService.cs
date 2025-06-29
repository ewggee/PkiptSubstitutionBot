using PkiptSubstitutionBot.Application.DTOs;
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

    public async Task AddOrUpdateSubstituionAsync(SubstituionsDto substDto)
    {
        var existing = await _substitutionRepository.GetSubstitutionAsync(
            date: substDto.Date.ToDateTime(TimeOnly.MinValue));

        var subst = new DbSubstitution
        {
            Date = substDto.Date.ToDateTime(TimeOnly.MinValue),
            Images = substDto.Images.Select(i => i.Data).ToArray(),
            Text = substDto.MessageText
        };

        if (existing != null)
        {
            await _substitutionRepository.UpdateSubstitutionAsync(subst);
        }
        else
        {
            await _substitutionRepository.AddSubstitutionAsync(subst);
        }
    }
}
