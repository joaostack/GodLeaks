namespace GodLeaks.Application.Interfaces;

public interface ISubscriptionService
{
    Task<bool> HasAccessAsync(
        ulong discordUserId,
        CancellationToken cancellationToken = default);
    Task ActivateAsync(
        ulong discordUserId,
        int days,
        CancellationToken cancellationToken = default);
}