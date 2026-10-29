using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class PhoneCommand : CommandModule<CommandContext>
{
    private readonly ILeakSight _leakSight;
    private readonly ISubscriptionService _subscription;

    public PhoneCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription)
    {
        _leakSight = leakSight;
        _subscription = subscription;
    }

    [Command("telefone")]
    public async Task Phone(string phone)
    {
        if (!await _subscription.HasAccessAsync(Context.User.Id))
        {
            await Context.Channel.SendMessageAsync(CommandMessages.InactiveSubscription);
            return;
        }

        await Context.Channel.SendMessageAsync(CommandMessages.Searching);
        var response = await _leakSight.SearchPhone(phone);

        if (response.Brazil_575M is null || response.Brazil_575M.Count == 0)
        {
            await ReplyAsync("Nenhum resultado encontrado.");
            return;
        }
        
        var sb = new StringBuilder();

        foreach (var item in response.Brazil_575M)
        {
            sb.AppendLine("=== CONSULTA TELEFONE ===");
            
            sb.AppendLine($"Telefone: {item.TELEFONE ?? "Não encontrado"}");
            sb.AppendLine($"Operadora: {item.OPERADORA ?? "Não encontrado"}");
            sb.AppendLine($"Tipo de pessoa: {item.TIPO_PESSOA ?? "Não encontrado"}");
            sb.AppendLine($"Nome: {item.NOME ?? "Não encontrado"}");
            sb.AppendLine($"Documento: {item.DOCUMENTO ?? "Não encontrado"}");
            sb.AppendLine($"Endereço: {item.ENDERECO}, {item.NUMERO ?? "Não encontrado"}");
            sb.AppendLine($"Bairro: {item.BAIRRO ?? "Não encontrado"}");
            sb.AppendLine($"Estado: {item.ESTADO ?? "Não encontrado"}");
            sb.AppendLine($"CEP: {item.CEP ?? "Não encontrado"}");
            sb.AppendLine();
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        await using var stream = new MemoryStream(bytes);

        await Context.Channel.SendMessageAsync(new MessageProperties
        {
            Content = $"Consulta concluída. Total de registros: {response.Brazil_575M.Count}.",
            Attachments = [new AttachmentProperties("consulta-telefone.txt", stream)]
        });
    }
}