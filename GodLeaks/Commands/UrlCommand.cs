using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class UrlCommand : CommandModule<CommandContext>
{
    private readonly ILeakSight _leakSight;
    private readonly ISubscriptionService _subscription;

    public UrlCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription)
    {
        _leakSight = leakSight;
        _subscription = subscription;
    }

    [Command("url")]
    public async Task Url(string url)
    {
        if (!await _subscription.HasAccessAsync(Context.User.Id))
        {
            await Context.Channel.SendMessageAsync(CommandMessages.InactiveSubscription);
            return;
        }
        
        await Context.Channel.SendMessageAsync(CommandMessages.Searching);
        var response = await _leakSight.SearchUrl(url);
        
        var sb = new StringBuilder();

        if (response is null || response.sucess == null)
        {
            await ReplyAsync("Nenhum resultado encontrado.");
            return;
        }
        
        foreach (var cred in response.sucess)
        {
            sb.AppendLine($"URL/Host: {cred.host}/{cred.path}");
            sb.AppendLine($"Credentials: {cred.user}:{cred.pass}");
            sb.AppendLine(new string('-', 20));
        }

        byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
        using MemoryStream stream = new MemoryStream(bytes);

        await Context.Channel.SendMessageAsync(new MessageProperties()
        {
            Content = $"Leaks encontrados para a url **{url}**:",
            Attachments = [new AttachmentProperties($"{url}.txt", stream)],
        });
    }
}