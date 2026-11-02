using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class EmailCommand : SubscriptionCommandModule
{
    private readonly ILeakSight _leakSight;

    public EmailCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription) : base(subscription)
    {
        _leakSight = leakSight;
    }

    [Command("email")]
    public async Task SendEmail(string email)
    {
        if (!await EnsureSubscriptionAsync())
            return;

        await StartSearchAsync();
        var response = await _leakSight.SearchEmail(email);

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
            Content = $"✅ **Busca finalizada!** Encontrei resultados para `{email}` e deixei tudo organizado no arquivo. 📄✨",
            Attachments = [new AttachmentProperties($"{email}.txt", stream)],
        });
    }
}
