namespace PkiptSubstitutionBot.client.Application.DTOs;

public class SendSubRequest
{
    public List<ImageData> Images { get; set; }
    public DateOnly Date { get; set; }
    public string? MessageText { get; set; }
}
