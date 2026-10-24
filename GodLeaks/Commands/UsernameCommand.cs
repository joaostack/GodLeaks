using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class UsernameCommand : CommandModule<CommandContext>
{
    private readonly ILeakSight _leakSight;
    private readonly ISubscriptionService _subscription;

    public UsernameCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription)
    {
        _leakSight = leakSight;
        _subscription = subscription;
    }

    [Command("username")]
    public async Task Username(string username)
    {
        if (!await _subscription.HasAccessAsync(Context.User.Id))
        {
            await Context.Channel.SendMessageAsync(CommandMessages.InactiveSubscription);
            return;
        }

        await Context.Channel.SendMessageAsync("🔎 Buscando...");
        var response = await _leakSight.SearchUsername(username);

        if (response?.database_url is not { Count: > 0 })
        {
            await Context.Channel.SendMessageAsync("No results found!");
            return;
        }

        var sb = new StringBuilder();

        foreach (var cred in response.database_url)
        {
            sb.AppendLine($"URL/Host: {cred.host}/{cred.path}");
            sb.AppendLine($"Credentials: {cred.user}:{cred.pass}");
            sb.AppendLine(new string('-', 20));
        }

        byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
        using MemoryStream stream = new MemoryStream(bytes);

        await Context.Channel.SendMessageAsync(new MessageProperties()
        {
            Content = $"Leaks encontrados para o usuário **{username}**:",
            Attachments = [new AttachmentProperties($"{username}.txt", stream)],
        });
    }
}
