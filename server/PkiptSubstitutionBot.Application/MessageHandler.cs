using Microsoft.Extensions.Logging;
using PkiptSubstitutionBot.Application.Helpers;
using PkiptSubstitutionBot.Application.Services;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using PkiptSubstitutionBot.Application.Consts;

namespace PkiptSubstitutionBot.Application;

public class MessageHandler
{
    private readonly ILogger<MessageHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly SubstitutionService _substitutionService;
    private readonly BotService _botService;

    private static List<long> RecipientChatsIds { get; set; } = [];

    public MessageHandler(
        ILogger<MessageHandler> logger,
        ITelegramBotClient botClient,
        UserService userService,
        SubstitutionService substitutionService,
        BotService botService)
    {
        _logger = logger;
        _botClient = botClient;
        _userService = userService;
        _substitutionService = substitutionService;

        RecipientChatsIds = _userService.GetChatIds();
        _botService = botService;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        _logger.LogError(exception.Message);
        await Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message == null) return;

        var messageText = message.Text ?? string.Empty;
        var chatId = message.Chat.Id;

        if (!RecipientChatsIds.Contains(chatId))
        {
            var existingUser = await _userService.GetUserAsync(chatId);
            if (existingUser == null)
            {
                var userInfo = update.Message?.From ?? new();
                await _userService.AddUserAsync(userInfo, chatId);
            }

            RecipientChatsIds.Add(chatId);
        }

        if (messageText == BotCommandConsts.Start)
        {
            await _botService.SendStart(chatId, token);
        }

        if (messageText.StartsWith(BotCommandConsts.Subst)
            && CommandHelper.TryParseSubstitutionCommand(messageText, out var date))
        {
            await _botService.GetSubst(date, chatId, message.MessageId, token);
        }
    }
}
