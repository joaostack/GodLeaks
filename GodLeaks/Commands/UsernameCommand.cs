using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class UsernameCommand : SubscriptionCommandModule
{
    private readonly ILeakSight _leakSight;

    public UsernameCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription) : base(subscription)
    {
        _leakSight = leakSight;
    }

    [SlashCommand("username", "Consulta um username")]
    public async Task Username(string username)
    {
        if (!await EnsureSubscriptionAsync())
            return;

        await StartSearchAsync();
        var response = await _leakSight.SearchUsername(username);

        if (response?.database_url is not { Count: > 0 })
        {
            await SendNoResultsAsync();
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

        await Context.Message.SendAsync(new MessageProperties()
        {
            Content = $"✅ **Busca finalizada!** Encontrei resultados para `{username}` e organizei tudo no arquivo em anexo. 👤📄",
            Attachments = [new AttachmentProperties($"{username}.txt", stream)],
        });
    }
}
