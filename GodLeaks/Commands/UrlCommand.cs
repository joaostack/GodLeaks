using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace GodLeaks.Commands;

public class UrlCommand : SubscriptionCommandModule
{
    private readonly ILeakSight _leakSight;

    public UrlCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription) : base(subscription)
    {
        _leakSight = leakSight;
    }

    [SlashCommand("url", "Consulta logins de uma URL")]
    public async Task Url(string url)
    {
        if (!await EnsureSubscriptionAsync())
            return;
        
        await StartSearchAsync();
        var response = await _leakSight.SearchUrl(url);
        
        var sb = new StringBuilder();

        if (response is null || response.sucess == null)
        {
            await SendNoResultsAsync();
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

        await SendAsync(new MessageProperties()
        {
            Content = $"✅ **Busca finalizada!** Encontrei resultados para `{url}` e organizei tudo no arquivo em anexo. 🌐📄",
            Attachments = [new AttachmentProperties($"{url}.txt", stream)],
        });
    }
}
