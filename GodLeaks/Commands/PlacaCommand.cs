using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class PlacaCommand : CommandModule<CommandContext>
{
    private readonly ILeakSight _leakSight;
    private readonly ISubscriptionService _subscription;

    public PlacaCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription)
    {
        _leakSight = leakSight;
        _subscription = subscription;
    }

    [Command("placa")]
    public async Task PlacaAsync(string placa)
    {
        if (!await _subscription.HasAccessAsync(Context.User.Id))
        {
            await Context.Channel.SendMessageAsync(CommandMessages.InactiveSubscription);
            return;
        }

        await Context.Channel.SendMessageAsync(CommandMessages.Searching);

        var response = await _leakSight.SearchPlaca(placa);

        if (response?.dados is null || response.dados.Count == 0)
        {
            await Context.Channel.SendMessageAsync(
                CommandMessages.NoResults
            );

            return;
        }

        var sb = new StringBuilder();

        foreach (var dado in response.dados)
        {
            sb.AppendLine("CONSULTA DE VEÍCULO");
            sb.AppendLine(new string('=', 40));

            sb.AppendLine($"Placa: {dado.PLACA ?? "Não informado"}");
            sb.AppendLine($"Documento: {dado.DOCUMENTO ?? "Não informado"}");
            sb.AppendLine($"Tipo de documento: {dado.TP_DOC ?? "Não informado"}");
            sb.AppendLine($"Chassi: {dado.CHASSI ?? "Não informado"}");
            sb.AppendLine($"Renavam: {dado.RENAVAM?.ToString() ?? "Não informado"}");

            sb.AppendLine();
            sb.AppendLine("LOCALIZAÇÃO");
            sb.AppendLine(new string('-', 40));

            sb.AppendLine($"Cidade: {dado.CIDADE ?? "Não informado"}");
            sb.AppendLine($"Estado: {dado.ESTADO ?? "Não informado"}");
            sb.AppendLine($"Município (código): {dado.CODMUN?.ToString() ?? "Não informado"}");
            sb.AppendLine($"UF de jurisdição: {dado.UF_JURISDICAO ?? "Não informado"}");

            sb.AppendLine();
            sb.AppendLine("CARACTERÍSTICAS DO VEÍCULO");
            sb.AppendLine(new string('-', 40));

            sb.AppendLine($"Ano de fabricação: {dado.ANO_FABRICACAO?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Código marca/modelo: {dado.COD_MARCA_MODELO?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Combustível (código): {dado.COD_COMBUST?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Tipo de veículo (código): {dado.COD_TIPO_VEIC?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Espécie (código): {dado.COD_ESP_VEIC?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Cor (código): {dado.COD_COR?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Carroceria (código): {dado.COD_TIPO_CARROCERIA?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Potência: {dado.POTENCIA?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Cilindradas: {dado.CILINDRADAS?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Número de eixos: {dado.NUM_EIXOS?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Lugares: {dado.LUGARES?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Carga: {dado.CARGA ?? "Não informado"}");
            sb.AppendLine($"Peso bruto total: {dado.PESO_BRUTO_TOTAL ?? "Não informado"}");
            sb.AppendLine($"Capacidade máxima de tração: {dado.CAP_MAX_TRACAO ?? "Não informado"}");
            sb.AppendLine($"Nacionalidade (código): {dado.NACIONALIDADE?.ToString() ?? "Não informado"}");

            sb.AppendLine();
            sb.AppendLine("DATAS E REGISTROS");
            sb.AppendLine(new string('-', 40));

            sb.AppendLine($"Emplacamento: {dado.DT_EMPLACAMENTO ?? "Não informado"}");
            sb.AppendLine($"Última atualização: {dado.DAT_ULT_ATU ?? "Não informado"}");
            sb.AppendLine($"Data de registro: {dado.DT_INSR ?? "Não informado"}");
            sb.AppendLine($"Última alteração: {dado.DT_LAST_UPDT ?? "Não informado"}");
            sb.AppendLine($"Recência: {dado.DT_RECENCIA ?? "Não informado"}");

            sb.AppendLine();
            sb.AppendLine("RESTRIÇÕES");
            sb.AppendLine(new string('-', 40));

            sb.AppendLine($"Restrição 1: {dado.RESTR_01?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Restrição 2: {dado.RESTR_02?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Restrição 3: {dado.RESTR_03?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Restrição 4: {dado.RESTR_04?.ToString() ?? "Não informado"}");
            sb.AppendLine($"Data limite das restrições: {dado.DATA_LIM_RESTR ?? "Não informado"}");
            sb.AppendLine($"Situação do veículo: {dado.COD_SIT_VEIC ?? "Não informado"}");
            sb.AppendLine($"Situação do chassi: {dado.COD_SIT_CHASSI ?? "Não informado"}");

            sb.AppendLine();
            sb.AppendLine(new string('=', 40));
            sb.AppendLine();
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        await using var stream = new MemoryStream(bytes);

        var fileName = $"consulta-placa-{placa}.txt";

        await Context.Channel.SendMessageAsync(
            new MessageProperties
            {
                Content = $"✅ **Busca finalizada!** Encontrei **{response.dados.Count} registros** para essa placa e organizei tudo no arquivo. 🚘📄",
                Attachments = [new AttachmentProperties(fileName, stream)]
            }
        );
    }
}
