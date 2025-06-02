using Microsoft.Extensions.Logging;
using PkiptSubstitutionBot.Application.Models;
using PkiptSubstitutionBot.Application.Repositories;
using Telegram.Bot.Types;

namespace PkiptSubstitutionBot.Application.Services;

public class UserService
{
    private readonly ILogger<UserService> _logger;
    private readonly UserRepository _userRepository;

    public UserService(ILogger<UserService> logger, UserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task AddUserAsync(User userInfo, long chatId)
    {
        await _userRepository.AddUserAsync(new DbUser
        {
            ChatId = chatId,
            FirstName = userInfo?.FirstName,
            LastName = userInfo?.LastName,
            Username = userInfo?.Username,
            LanguageCode = userInfo?.LanguageCode
        });

        _logger.LogInformation(
            $"Добавлен пользователь {userInfo?.FirstName ?? "-"} {userInfo?.LastName ?? "-"} (@{userInfo?.Username ?? "-"})");
    }

    public async Task<DbUser?> GetUserAsync(long chatId)
    {
        var user = await _userRepository.GetUserAsync(chatId);

        return user;
    }

    public List<long> GetChatsIdsAsync()
    {
        var chatsIds = _userRepository.GetChatIdsAsync().Result;

        return chatsIds.ToList();
    }
}
