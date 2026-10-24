namespace GodLeaks.Domain.Entities;

public class UserSubscription
{
    public int Id { get; set; }

    public string DiscordUserId { get; set; } = null!;

    public int PlanId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; }
}