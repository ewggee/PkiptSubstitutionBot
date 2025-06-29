namespace PkiptSubstitutionBot.Application.Models;

public class DbSubstitution
{
    public int Id { get; set; }

    public byte[][] Images { get; set; }

    public DateTime Date { get; set; }

    public string? Text { get; set; }
}
