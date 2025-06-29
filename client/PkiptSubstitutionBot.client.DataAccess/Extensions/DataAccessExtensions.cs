using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PkiptSubstitutionBot.client.DataAccess.Repositories;

namespace PkiptSubstitutionBot.client.DataAccess.Extensions;

public static class DataAccessExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PkiptSubstitutionBotContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<AdminRepository>();

        return services;
    }
}
