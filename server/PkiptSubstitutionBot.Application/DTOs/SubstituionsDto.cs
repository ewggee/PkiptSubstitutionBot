namespace PkiptSubstitutionBot.Application.DTOs;

public class SubstituionsDto
{
    public List<ImageData> Images { get; set; }
    public DateOnly Date { get; set; }
    public string? MessageText { get; set; }
}
