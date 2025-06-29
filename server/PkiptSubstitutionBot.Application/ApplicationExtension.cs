using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PkiptSubstitutionBot.Application.Consumers;
using PkiptSubstitutionBot.Application.Dapper;
using PkiptSubstitutionBot.Application.Options;
using PkiptSubstitutionBot.Application.Repositories;
using PkiptSubstitutionBot.Application.Services;
using Telegram.Bot;

namespace PkiptSubstitutionBot.Application;

public static class ApplicationExtension
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, out IConfiguration configuration)
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions)));

        return services;
    }

    public static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddLogging(configure =>
        {
            configure.ClearProviders();
            configure.AddConsole();
        });

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<MessageHandler>();
        services.AddScoped<UserService>();
        services.AddScoped<BotService>();
        services.AddScoped<SubstitutionService>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(c => new DapperContext(configuration.GetConnectionString("DefaultConnection")!));

        services.AddScoped<UserRepository>();
        services.AddScoped<SubstitutionRepository>();

        return services;
    }

    public static IServiceCollection AddBotClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(c => new TelegramBotClient(configuration["BotToken"]!));

        return services;
    }

    public static IServiceCollection AddApplicationEntryPoint(this IServiceCollection services)
    {
        services.AddSingleton<Application>();

        return services;
    }

    public static IServiceCollection AddBackgroundWorkers(this IServiceCollection services)
    {
        services.AddHostedService<SubstitutionsConsumer>();
        services.AddHostedService<MessagesConsumer>();

        return services;
    }
}
