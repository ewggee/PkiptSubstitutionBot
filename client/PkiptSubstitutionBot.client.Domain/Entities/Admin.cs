namespace PkiptSubstitutionBot.client.Domain.Entities;

public partial class Admin
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}
