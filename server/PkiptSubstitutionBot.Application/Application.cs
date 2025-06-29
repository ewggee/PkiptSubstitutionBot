using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace PkiptSubstitutionBot.Application;

public class Application
{
    private readonly MessageHandler _messageHandler;
    private readonly ILogger<Application> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;

    public Application(
        MessageHandler messageHandler,
        ILogger<Application> logger,
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider)
    {
        _messageHandler = messageHandler;
        _logger = logger;
        _botClient = botClient;
        _serviceProvider = serviceProvider;
    }

    public async Task Run()
    {
        try
        {
            var botTask = Task.Run(async () =>
            {
                using var cts = new CancellationTokenSource();

                _botClient.StartReceiving(
                    updateHandler: _messageHandler.HandleUpdateAsync,
                    errorHandler: _messageHandler.HandlePollingErrorAsync,
                    receiverOptions: new ReceiverOptions
                    {
                        AllowedUpdates = [],
                        DropPendingUpdates = true
                    },
                    cancellationToken: cts.Token
                );

                await Task.Delay(Timeout.Infinite, cts.Token);
            });

            var hostedServices = _serviceProvider.GetServices<IHostedService>();

            var backgroundTasks = hostedServices.Select(s => s.StartAsync(default));

            _logger.LogInformation($"Приложение запущено");

            await Task.WhenAll(backgroundTasks);
            await botTask;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка: {ex.Message}. Перезапуск через 5 сек...");
            await Task.Delay(5000);
        }
    }
}
