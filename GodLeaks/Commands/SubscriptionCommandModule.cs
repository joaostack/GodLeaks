using GodLeaks.Application.Interfaces;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public abstract class SubscriptionCommandModule : CommandModule<CommandContext>
{
    private readonly ISubscriptionService _subscription;

    protected SubscriptionCommandModule(ISubscriptionService subscription)
    {
        _subscription = subscription;
    }

    protected async Task<bool> EnsureSubscriptionAsync()
    {
        if (await _subscription.HasAccessAsync(Context.User.Id))
            return true;

        await ReplyAsync(CommandMessages.InactiveSubscription);
        return false;
    }

    protected Task StartSearchAsync() => ReplyAsync(CommandMessages.Searching);

    protected Task SendNoResultsAsync() => ReplyAsync(CommandMessages.NoResults);
}
