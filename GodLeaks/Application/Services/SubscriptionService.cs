using GodLeaks.Application.Interfaces;
using GodLeaks.Domain.Entities;
using GodLeaks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodLeaks.Infrastructure.Services;

public class SubscriptionService(
    GodLeaksDbContext db) : ISubscriptionService
{
    public async Task<bool> HasAccessAsync(
        ulong discordUserId,
        CancellationToken cancellationToken = default)
    {
        var id = discordUserId.ToString();

        return await db.UserSubscriptions.AnyAsync(
            x => x.DiscordUserId == id
                 && x.IsActive
                 && x.ExpiresAt > DateTime.UtcNow,
            cancellationToken);
    }

    public async Task ActivateAsync(
        ulong discordUserId,
        int days,
        CancellationToken cancellationToken = default)
    {
        var id = discordUserId.ToString();

        var subscription = await db.UserSubscriptions
            .FirstOrDefaultAsync(
                x => x.DiscordUserId == id,
                cancellationToken);

        var now = DateTime.UtcNow;

        if (subscription is null)
        {
            subscription = new UserSubscription
            {
                DiscordUserId = id,
                StartedAt = now,
                ExpiresAt = now.AddDays(days),
                IsActive = true
            };

            db.UserSubscriptions.Add(subscription);
        }
        else
        {
            subscription.StartedAt = now;
            subscription.ExpiresAt = now.AddDays(days);
            subscription.IsActive = true;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}