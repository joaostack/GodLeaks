namespace GodLeaks.Domain.Entities;

public class Admin
{
    public int Id { get; set; }

    public string DiscordUserId { get; set; } = null!;

    public bool IsActive { get; set; }
}