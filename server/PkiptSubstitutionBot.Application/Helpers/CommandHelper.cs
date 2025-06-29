using PkiptSubstitutionBot.Application.Consts;
using System.Globalization;

namespace PkiptSubstitutionBot.Application.Helpers;

public class CommandHelper
{
    public static bool TryParseSubstitutionCommand(string text, out DateOnly date)
    {
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1 && parts[0] == BotCommandConsts.Subst)
        {
            date = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
            return true;
        }
        
        if (parts.Length == 2 && parts[0] == BotCommandConsts.SubstDated)
        {
            if (DateTime.TryParse(parts[1], CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out var dt))
            {
                date = DateOnly.FromDateTime(dt);
                return true;
            }
        }

        date = DateOnly.MinValue;
        return false;
    }
}
