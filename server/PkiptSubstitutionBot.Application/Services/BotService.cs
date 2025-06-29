using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using PkiptSubstitutionBot.Application.DTOs;
using Telegram.Bot.Exceptions;
using System.Net;
using System.Threading.Tasks;

namespace PkiptSubstitutionBot.Application.Services;

public class BotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly SubstitutionService _substitutionService;
    private readonly UserService _userService;

    static List<long> TEST_USERS = [5382647706];

    public BotService(
        ITelegramBotClient botClient,
        SubstitutionService substitutionService,
        UserService userService)
    {
        _botClient = botClient;
        _substitutionService = substitutionService;
        _userService = userService;
    }

    public async Task SendStart(long chatId, CancellationToken ct)
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
                InlineKeyboardButton.WithUrl("ВК-паблик колледжа", "https://vk.com/itkollege")
            },
            new() {
                InlineKeyboardButton.WithUrl("Сайт колледжа", "https://xn----htbcfgnhaz1b.xn--p1ai/it/"),
            }
        });

        await _botClient.SendMessage(
            chatId: chatId,
            text: text,
            replyMarkup: inlineKeyboard,
            cancellationToken: ct);
    }

    public async Task SendSubst(SubstituionsDto substDto, CancellationToken ct)
    {
        var userChatIds = await _userService.GetChatIdsAsync();

        await ParallelProccess(
            source: userChatIds,
            asyncFunc: async chatId =>
            {
                var mediaGroup = GetMediaGroup(substDto.Images
                .Select(i => i.Data)
                .ToList());

                await SendSubstToChat(chatId, mediaGroup, substDto.Date, substDto.MessageText, ct);
            },
            ct: ct);
    }


    public async Task SendMessage(string message, CancellationToken ct)
    {
        var userChatIds = await _userService.GetChatIdsAsync();

        await ParallelProccess(
            source: userChatIds,
            asyncFunc: async chatId =>
            {
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: message);
            },
            ct: ct);
    }

    public async Task GetSubst(DateOnly date, long chatId, int messageId, CancellationToken ct)
    {
        var subst = await _substitutionService.GetSubstitutionAsync(date);
        if (subst != null)
        {
            var mediaGroup = GetMediaGroup(subst.Images);

            await SendSubstToChat(chatId, mediaGroup, DateOnly.FromDateTime(subst.Date), subst.Text, ct);
        }
        else
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: "Не найдено замен за указанный период времени",
                replyParameters: new ReplyParameters { MessageId = messageId },
                cancellationToken: ct);
        }
    }

    private static async Task ParallelProccess<T>(
        IEnumerable<T> source,
        Func<T, Task> asyncFunc,
        CancellationToken ct)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 5,
            CancellationToken = ct
        };

        await Parallel.ForEachAsync(source, options, async (item, token) =>
        {
            try
            {
                await asyncFunc(item);
                await Task.Delay(200, token);
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == (int)HttpStatusCode.BadRequest)
            { }
            catch (ApiRequestException ex) when (ex.HttpStatusCode == HttpStatusCode.TooManyRequests)
            {
                var retryAfter = ex.Parameters?.RetryAfter ?? 5;

                await Task.Delay(TimeSpan.FromSeconds(retryAfter + 1), token);
            }
        });
    }

    private static List<IAlbumInputMedia> GetMediaGroup(IEnumerable<byte[]> images)
    {
        var mediaGroup = new List<IAlbumInputMedia>();

        foreach (var image in images)
        {
            var ms = new MemoryStream(image);
            mediaGroup.Add(new InputMediaPhoto(ms));
        }

        return mediaGroup;
    }

    private async Task SendSubstToChat(long chatId, List<IAlbumInputMedia> mediaGroup, DateOnly date, string? substText, CancellationToken ct)
    {
        var sendedMessage = await _botClient.SendMediaGroup(
            chatId: chatId,
            media: mediaGroup,
            cancellationToken: ct);

        if (!string.IsNullOrEmpty(substText))
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: $"{date}\n\n{substText}",
                replyParameters: new() { MessageId = sendedMessage.First().MessageId },
                cancellationToken: ct);
        }
        else
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: date.ToString(),
                replyParameters: new() { MessageId = sendedMessage.First().MessageId },
                cancellationToken: ct);
        }
    }
}
