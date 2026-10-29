using MemoryStream = System.IO.MemoryStream;
using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class NomeCommand : CommandModule<CommandContext>
{
    private readonly ILeakSight _leakSight;
    private readonly ISubscriptionService _subscription;

    public NomeCommand(
        ILeakSight leakSight,
        ISubscriptionService subscription)
    {
        _leakSight = leakSight;
        _subscription = subscription;
    }

    [Command("nome")]
    public async Task Nome([CommandParameter(Remainder = true)] string nome)
    {
        if (!await _subscription.HasAccessAsync(Context.User.Id))
        {
            await Context.Channel.SendMessageAsync(CommandMessages.InactiveSubscription);
            return;
        }

        await Context.Channel.SendMessageAsync(CommandMessages.Searching);
        var response = await _leakSight.SearchNome(nome);

        if (response is null)
        {
            await ReplyAsync(CommandMessages.NoResults);
            return;
        }

        var sb = new StringBuilder();

        if (response.BIGDATA_NOME?.Count > 0)
        {
            foreach (var resultado in response.BIGDATA_NOME)
            {
                sb.AppendLine("=== BIGDATA NOME ===");
                sb.AppendLine($"Nome completo: {resultado.nome_completo}");
                sb.AppendLine($"Total: {resultado.total}");

                foreach (var dado in resultado.dados ?? [])
                {
                    sb.AppendLine($"\nNome: {dado.NOME}");
                    sb.AppendLine($"CPF: {dado.CPF}");
                    sb.AppendLine($"Sexo: {dado.SEXO}");
                    sb.AppendLine($"Nascimento: {dado.NASC}");
                    sb.AppendLine($"Nome da mãe: {dado.NOME_MAE}");
                    sb.AppendLine($"Situação cadastral: {dado.CD_SIT_CAD}");
                    sb.AppendLine($"Título de eleitor: {dado.TITULO_ELEITOR}");

                    foreach (var email in dado.emails ?? [])
                        sb.AppendLine($"E-mail: {email.EMAIL}");

                    foreach (var telefone in dado.telefones ?? [])
                        sb.AppendLine($"Telefone: ({telefone.DDD}) {telefone.TELEFONE}");

                    foreach (var endereco in dado.enderecos ?? [])
                    {
                        sb.AppendLine(
                            $"Endereço: {endereco.LOGR_TIPO} {endereco.LOGR_NOME}, {endereco.LOGR_NUMERO}");
                        sb.AppendLine($"Bairro: {endereco.BAIRRO}");
                        sb.AppendLine($"Cidade: {endereco.CIDADE}/{endereco.UF}");
                        sb.AppendLine($"CEP: {endereco.CEP}");
                    }

                    foreach (var score in dado.score ?? [])
                    {
                        sb.AppendLine($"Score CSBA: {score.CSBA}");
                        sb.AppendLine($"Faixa: {score.CSBA_FAIXA}");
                    }
                }

                sb.AppendLine();
            }
        }

        if (response.CONSULTA_NOME?.Count > 0)
        {
            sb.AppendLine("=== CONSULTA NOME ===");

            foreach (var dado in response.CONSULTA_NOME)
            {
                sb.AppendLine($"Nome: {dado.nome}");
                sb.AppendLine($"CPF: {dado.cpf}");
                sb.AppendLine($"Pessoa: {dado.pessoa}");
                sb.AppendLine($"DDD: {dado.ddd}");
                sb.AppendLine($"Telefone: {dado.fone}");
                sb.AppendLine();
            }
        }

        if (response.RECEITA_FEDERAL_NOME?.Count > 0)
        {
            sb.AppendLine("=== RECEITA FEDERAL ===");

            foreach (var dado in response.RECEITA_FEDERAL_NOME)
            {
                sb.AppendLine($"Nome: {dado.nome}");
                sb.AppendLine($"CPF: {dado.cpf}");
                sb.AppendLine($"Primeiro nome: {dado.primeironome}");
                sb.AppendLine($"Último nome: {dado.ultimonome}");
                sb.AppendLine($"Data de inclusão: {dado.datainclusao}");
                sb.AppendLine($"Última atualização: {dado.dataatualizacao}");
                sb.AppendLine($"Endereço: {dado.enderecologradouro}, {dado.endereconumero}");
                sb.AppendLine($"Bairro: {dado.enderecobairro}");
                sb.AppendLine($"Cidade: {dado.enderecocidade}/{dado.enderecoestado}");
                sb.AppendLine($"CEP: {dado.enderecocep}");
                sb.AppendLine($"Telefone: ({dado.dddtelefone}) {dado.numerotelefone}");
                sb.AppendLine();
            }
        }

        if (response.CREDILINK?.Count > 0)
        {
            sb.AppendLine("=== CREDILINK ===");

            foreach (var dado in response.CREDILINK)
            {
                sb.AppendLine($"Nome: {dado.nome}");
                sb.AppendLine($"CPF: {dado.cpf}");
                sb.AppendLine($"Nascimento: {dado.dt_nascimento}");
                sb.AppendLine($"Sexo: {dado.sexo}");
                sb.AppendLine($"Nome da mãe: {dado.nome_mae}");
                sb.AppendLine($"Celular: {dado.celular1}");
                sb.AppendLine($"E-mail: {dado.email}");
                sb.AppendLine($"Endereço: {dado.logradouro}");
                sb.AppendLine($"Bairro: {dado.bairro}");
                sb.AppendLine($"Cidade: {dado.cidade}/{dado.uf}");
                sb.AppendLine($"CEP: {dado.cep}");
                sb.AppendLine($"Status na Receita: {dado.status_receita_federal}");
                sb.AppendLine($"Quantidade de veículos: {dado.qt_veiculos}");
                sb.AppendLine($"Renda presumida: {dado.renda_presumida}");
                sb.AppendLine($"Faixa de renda: {dado.faixa_renda}");
                sb.AppendLine();
            }
        }

        if (sb.Length == 0)
        {
            await ReplyAsync(CommandMessages.NoResults);
            return;
        }

        var arquivo = new AttachmentProperties(
            $"consulta-{DateTime.Now:yyyyMMdd-HHmmss}.txt",
            new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())));

        await ReplyAsync(new ReplyMessageProperties
        {
            Content = "✅ **Busca finalizada!** Seus resultados estão organizados no arquivo em anexo. 📄✨",
            Attachments = [arquivo]
        });
    }
}
