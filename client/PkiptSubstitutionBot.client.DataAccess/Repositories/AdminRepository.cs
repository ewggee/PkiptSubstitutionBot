using Microsoft.EntityFrameworkCore;
using PkiptSubstitutionBot.client.Domain.Entities;

namespace PkiptSubstitutionBot.client.DataAccess.Repositories;

public class AdminRepository
{
    private readonly PkiptSubstitutionBotContext _context;

    public AdminRepository(PkiptSubstitutionBotContext context)
    {
        _context = context;
    }

    public Task<Admin?> GetByUsername(string username)
    {
        return _context.Admins.FirstOrDefaultAsync(a => a.Username == username);
    }

    public Task<Admin[]> GetAll(string username)
    {
        return _context.Admins
            .Where(a => a.Username != username)
            .ToArrayAsync();
    }

    public async Task AddAdmin(Admin admin)
    {
        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByUsername(string username)
    {
        await _context.Admins
            .Where(a => a.Username == username)
            .ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }
}
