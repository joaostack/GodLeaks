using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class SenhaCommand : CommandModule<CommandContext>
{
    private readonly ILeakSight _leakSight;
    private readonly ISubscriptionService _subscription;
    
    public SenhaCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription)
    {
        _leakSight = leakSight;
        _subscription = subscription;
    }
    
    [Command("senha")]
    public async Task Senha(string senha)
    {
        if (!await _subscription.HasAccessAsync(Context.User.Id))
        {
            await Context.Channel.SendMessageAsync(CommandMessages.InactiveSubscription);
            return;
        }

        await Context.Channel.SendMessageAsync(CommandMessages.Searching);
        var response = await _leakSight.SearchPassword(senha);
        
        if (response.MachinesStealer.Count == 0 && response.stealer.Count == 0)
        {
            await Context.Channel.SendMessageAsync(CommandMessages.NoResults);
            return;
        }

        var sb = new StringBuilder();

        if (response.MachinesStealer.Count > 0)
        {
            foreach (var cred in response.MachinesStealer)
            {
                sb.AppendLine($"URL/Host: {cred.host}/{cred.path}");
                sb.AppendLine($"Credentials: {cred.user}:{cred.pass}");
                sb.AppendLine(new string('-', 20));
            }
        }

        if (response.stealer.Count > 0)
        {
            foreach (var cred in response.stealer)
            {
                sb.AppendLine($"Credentials: {cred}");
                sb.AppendLine(new string('-', 20));
            }
        }

        byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
        using MemoryStream stream = new MemoryStream(bytes);

        await Context.Channel.SendMessageAsync(new MessageProperties()
        {
            Content = $"✅ **Busca finalizada!** Encontrei resultados para essa senha e organizei tudo no arquivo em anexo. 🔐📄",
            Attachments = [new AttachmentProperties($"{senha}.txt", stream)],
        });
    }
}
