using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PkiptSubstitutionBot.Application.Consts;
using PkiptSubstitutionBot.Application.DTOs;
using PkiptSubstitutionBot.Application.Options;
using PkiptSubstitutionBot.Application.Services;
using System.Text.Json;

namespace PkiptSubstitutionBot.Application.Consumers;

public class SubstitutionsConsumer : RabbitMqConsumerBase
{
    private readonly SubstitutionService _substitutionService;
    private readonly BotService _botService;

    public SubstitutionsConsumer(
        ILogger<SubstitutionsConsumer> logger,
        IOptions<RabbitMqOptions> rabbitMqOptions,
        SubstitutionService substitutionService,
        BotService botService)
        : base(logger, rabbitMqOptions, RabbitMqQueueNameConsts.Subst)
    {
        _substitutionService = substitutionService;
        _botService = botService;
    }

    protected override async Task ProccessMessageAsync(string encodedMessage, CancellationToken ct)
    {
        var substDto = JsonSerializer.Deserialize<SubstituionsDto>(encodedMessage)!;

        await _substitutionService.AddOrUpdateSubstituionAsync(substDto);

        var tomorrowDate = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
        if (substDto.Date == tomorrowDate)
        {
            await _botService.SendSubst(substDto, ct);
        }
    }
}
