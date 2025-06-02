using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.Extensions.Logging;
using PkiptSubstitutionBot.Application.Services;
using PkiptSubstitutionBot.Application.Helpers;
using PkiptSubstitutionBot.Application.Models;

namespace PkiptSubstitutionBot.Application;

public class MessageHandler
{
    private const long AdminChatId = -1002630127086;

    private readonly ILogger<MessageHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly SubstitutionService _substitutionService;

    private static List<long> RecipientChatsIds { get; set; } = [];
    private static List<long> BannedChatsIds { get; set; } = [];

    public MessageHandler(
        ILogger<MessageHandler> logger,
        ITelegramBotClient botClient,
        UserService userService,
        SubstitutionService substitutionService)
    {
        _logger = logger;
        _botClient = botClient;
        _userService = userService;
        _substitutionService = substitutionService;

        RecipientChatsIds = _userService.GetChatsIdsAsync();
    }

    public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message == null) return;

        var chatId = message.Chat.Id;

        if (chatId == AdminChatId)
        {
            await MessageFromAdmin(message, token);
            return;
        }

        if (chatId != AdminChatId && !RecipientChatsIds.Contains(chatId))
        {
            var existingUser = await _userService.GetUserAsync(chatId);
            if (existingUser == null)
            {
                var userInfo = update.Message?.From!;
                await _userService.AddUserAsync(userInfo, chatId);
            }

            RecipientChatsIds.Add(chatId);
        }

        if (message.Text == "/start")
        {
            await SendStartMessage(message, token);
        }

        if (message.Text.StartsWith("/substitution")
            && CommandsHelper.TryParseSubstitutionCommand(message.Text, out var subDate))
        {
            var substitution = await _substitutionService.GetSubstitutionAsync(subDate);

            if (substitution == null)
            {
                await _botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Не найдено замен за указанный период времени",
                    replyParameters: new ReplyParameters { MessageId = message.MessageId },
                    cancellationToken: token);
            }
            else
            {
                using var stream = new MemoryStream(substitution.Image);
                await _botClient.SendPhoto(
                    chatId: chatId,
                    photo: InputFile.FromStream(stream, $"image_{substitution.Id}.jpg"),
                    caption: substitution.Date.ToShortDateString());
            }
        }
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        _logger.LogError(exception.Message);
        await Task.CompletedTask;
    }

    public async Task SendAll(Message message, CancellationToken token)
    {
        foreach (var receiverChatId in RecipientChatsIds)
        {
            await _botClient.CopyMessage(receiverChatId, AdminChatId, message.MessageId, cancellationToken: token);
        }

        await _botClient.SendMessage(
            chatId: message.Chat.Id,
            text: "☑️ Разослал!",
            replyParameters: new ReplyParameters { MessageId = message.MessageId },
            cancellationToken: token);
    }

    private async Task MessageFromAdmin(Message message, CancellationToken token)
    {
        var messageText = message.Text ?? message.Caption ?? string.Empty;

        if (messageText.StartsWith("/substitution") && message.Photo != null 
            && CommandsHelper.TryParseSubstitutionCommand(messageText, out var subDate))
        {
            var photoId = message.Photo.Last().FileId;

            var photo = await _botClient.GetFile(photoId, token);

            using var memoryStream = new MemoryStream();

            await _botClient.DownloadFile(photo.FilePath, memoryStream);

            byte[] imageData = memoryStream.ToArray();

            await _substitutionService.AddOrUpdateSubstituionAsync(new DbSubstitution
            {
                Image = imageData,
                Date = subDate.ToDateTime(TimeOnly.MinValue)
            });

            message.Caption = subDate.ToShortDateString();
            await SendAll(message, token);
        }
        else if (messageText.StartsWith("/ban") 
            && CommandsHelper.TryParseBanCommand(messageText, out long chatId))
        {
            RecipientChatsIds.Remove(chatId);
            BannedChatsIds.Add(chatId);

            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "☑️ Пользователь забанен!",
                replyParameters: new ReplyParameters { MessageId = message.MessageId },
                cancellationToken: token);
        }
    }

    private async Task SendStartMessage(Message message, CancellationToken token)
    {
        var text =
            "🙋 Привет!\n\n" +
            "📄 Я — бот, созданный для отправки журнала замен занятий ИТ-отделения колледжа ГАПОУ ПО ПКИПТ. " +
            "Пользоваться мной может любой желающий: " +
            "студент, преподаватель, родитель студента!";

        var inlineKeyboard = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new() {
                InlineKeyboardButton.WithUrl("ТГ-канал колледжа", "https://t.me/itkollege"),
                InlineKeyboardButton.WithUrl("ВК-паблик колледжа", "https://vk.com/itkollege?ysclid=lshxd4zdff826378027")
            },
            new() {
                InlineKeyboardButton.WithUrl("Сайт колледжа", "https://xn----htbcfgnhaz1b.xn--p1ai/it/"),
            }
        });

        await _botClient.SendMessage(
            chatId: message.Chat,
            text: text,
            replyMarkup: inlineKeyboard,
            cancellationToken: token);
    }
}
