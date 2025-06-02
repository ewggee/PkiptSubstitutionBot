namespace PkiptSubstitutionBot.Application.Helpers;

public class CommandsHelper
{
    public static bool TryParseSubstitutionCommand(string text, out DateOnly date)
    {
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        date = DateOnly.MinValue;

        if (parts.Length != 2 || parts[0] != "/substitution")
            return false;

        return DateOnly.TryParseExact(parts[1], "dd.MM.yyyy", out date);
    }

    public static bool TryParseBanCommand(string text, out long chatId)
    {
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        chatId = 0;

        if (parts.Length != 2 || parts[0] != "/ban")
            return false;

        return long.TryParse(parts[1], out chatId);
    }
}
