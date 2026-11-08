using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace GodLeaks.Commands;

public abstract class SubscriptionCommandModule : ApplicationCommandModule<ApplicationCommandContext>
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

    protected async Task ReplyAsync(string message)
    {
        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties { Content = message }));
    }

    protected async Task ReplyAsync(ReplyMessageProperties message)
    {
        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = message.Content,
            Attachments = message.Attachments,
            Embeds = message.Embeds,
            Components = message.Components,
            AllowedMentions = message.AllowedMentions,
            Flags = message.Flags,
            Poll = message.Poll
        }));
    }

    protected async Task SendAsync(MessageProperties message)
    {
        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = message.Content,
            Attachments = message.Attachments,
            Embeds = message.Embeds,
            Components = message.Components,
            AllowedMentions = message.AllowedMentions,
            Flags = message.Flags,
            Poll = message.Poll
        }));
    }
}
