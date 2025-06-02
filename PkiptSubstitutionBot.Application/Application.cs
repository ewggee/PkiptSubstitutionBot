using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace PkiptSubstitutionBot.Application;

public class Application
{
    private readonly MessageHandler _messageHandler;
    private readonly ILogger<Application> _logger;
    private readonly ITelegramBotClient _botClient;

    public Application(
        MessageHandler messageHandler, 
        ILogger<Application> logger, 
        ITelegramBotClient botClient)
    {
        _messageHandler = messageHandler;
        _logger = logger;
        _botClient = botClient;
    }

    public async Task Run()
    {
        while (true)
        {
            try
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

                var bot = _botClient.GetMe();
                _logger.LogInformation($"Бот \"{bot.Result.FirstName}\" (@{bot.Result.Username}) запущен");

                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка: {ex.Message}. Перезапуск через 5 сек...");
                await Task.Delay(5000);
            }
        }
    }
}
