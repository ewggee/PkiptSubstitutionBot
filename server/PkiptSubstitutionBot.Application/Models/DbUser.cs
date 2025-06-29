namespace PkiptSubstitutionBot.Application.Models;

public class DbUser
{
    public long ChatId { get; set; }
    
    public string? Username { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? LanguageCode { get; set; }
}
