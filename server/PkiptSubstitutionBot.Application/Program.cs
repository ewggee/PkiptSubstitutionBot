using Microsoft.Extensions.DependencyInjection;

namespace PkiptSubstitutionBot.Application;

class Program
{
    static async Task Main()
    {
        var services = new ServiceCollection()
            .AddConfiguration(out var configuration)
            .AddLogging()
            .AddServices()
            .AddRepositories(configuration)
            .AddBotClient(configuration)
            .AddBackgroundWorkers()
            .AddApplicationEntryPoint();

        var serviceProvider = services.BuildServiceProvider();

        var app = serviceProvider.GetRequiredService<Application>();

        await app.Run();
    }
}
