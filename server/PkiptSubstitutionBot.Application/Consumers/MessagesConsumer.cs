
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PkiptSubstitutionBot.Application.Consts;
using PkiptSubstitutionBot.Application.Options;
using PkiptSubstitutionBot.Application.Services;
using System.Text.Json;

namespace PkiptSubstitutionBot.Application.Consumers;

public class MessagesConsumer : RabbitMqConsumerBase
{
    private readonly BotService _botService;

    public MessagesConsumer(
        ILogger<MessagesConsumer> logger,
        IOptions<RabbitMqOptions> rabbitMqOptions,
        BotService botService)
        : base(logger, rabbitMqOptions, RabbitMqQueueNameConsts.Messages) 
    {
        _botService = botService;
    }

    protected override async Task ProccessMessageAsync(string encodedMessage, CancellationToken ct)
    {
        var massage = JsonSerializer.Deserialize<string>(encodedMessage)!;

        await _botService.SendMessage(massage, ct);
    }
}
