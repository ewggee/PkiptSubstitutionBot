using Microsoft.AspNetCore.Identity;
using PkiptSubstitutionBot.client.Application.DTOs;
using PkiptSubstitutionBot.client.DataAccess.Repositories;
using PkiptSubstitutionBot.client.Domain.Entities;

namespace PkiptSubstitutionBot.client.Application.Services;

public class AdminService
{
    private readonly AdminRepository _adminRepository;

    public AdminService(AdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task Register(RegisterRequest registerRequest)
    {
        var admin = new Admin
        {
            Username = registerRequest.Username
        };

        var passwordHash = new PasswordHasher<Admin>().HashPassword(admin, registerRequest.Password);
        admin.PasswordHash = passwordHash;

        await _adminRepository.AddAdmin(admin);
    }

    public async Task<bool> Login(LoginRequest loginRequest)
    {
        var admin = await _adminRepository.GetByUsername(loginRequest.Username);

        if (admin == null) 
            return false;

        var result = new PasswordHasher<Admin>()
            .VerifyHashedPassword(admin, admin.PasswordHash, loginRequest.Password);

        if (result == PasswordVerificationResult.Failed)
            return false;

        return true;
    }

    public Task<Admin[]> GetAdminsAsync(string username)
    {
        return _adminRepository.GetAll(username);
    }

    public async Task DeleteAdminByUsername(string username)
    {
        await _adminRepository.DeleteByUsername(username);
    }

    public async Task<bool> IsAdminExists(string username)
    {
        var admin = await _adminRepository.GetByUsername(username);

        return admin != null ? true : false;
    }
}
