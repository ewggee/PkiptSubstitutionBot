using PkiptSubstitutionBot.Application.Dapper;
using PkiptSubstitutionBot.Application.Models;

namespace PkiptSubstitutionBot.Application.Repositories;

public class UserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<DbUser?> GetUserAsync(long chatId)
    {
        var user = await _context.FirstOrDefault<DbUser>(new QueryObject(
            sql: "SELECT * FROM USERS WHERE chat_id = @chat_id",
            parameters: new { chat_id = chatId}));

        return user;
    }

    public async Task<IEnumerable<long>> GetChatIdsAsync()
    {
        var chatsIds = await _context.GetAll<long>(new QueryObject(
            sql: "SELECT chat_id FROM users",
            parameters: new { }));

        return chatsIds;
    }

    public async Task AddUserAsync(DbUser user)
    {
        await _context.Execute(new QueryObject(
            sql: "INSERT INTO users VALUES (@chat_id, @username, @first_name, @last_name, @language_code)",
            parameters: new { chat_id = user.ChatId, username = user.Username, first_name = user.FirstName,
                last_name = user.LastName, language_code = user.LanguageCode }));
    }
}
