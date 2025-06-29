using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PkiptSubstitutionBot.client.Application.Options;
using PkiptSubstitutionBot.client.Application.Services;

namespace PkiptSubstitutionBot.client.Application.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));
        
        services.AddSingleton(provider =>
        {
            var rabbitMqOptions = configuration.GetSection(nameof(RabbitMqOptions)).Get<RabbitMqOptions>()!;
            return RabbitMqService.CreateAsync(rabbitMqOptions).GetAwaiter().GetResult();
        });

        return services;
    }
}
