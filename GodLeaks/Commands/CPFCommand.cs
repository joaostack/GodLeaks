using System.Globalization;
using System.Text;
using GodLeaks.Application.Interfaces;
using NetCord;
using NetCord.Rest;
using NetCord.Services.Commands;

namespace GodLeaks.Commands;

public class CPFCommand : CommandModule<CommandContext>
{
    private static readonly CultureInfo PtBr =
        CultureInfo.GetCultureInfo("pt-BR");

    private readonly ILeakSight _leakSight;
    private readonly ISubscriptionService _subscription;

    public CPFCommand(ILeakSight leakSight, ISubscriptionService subscription)
    {
        _leakSight = leakSight;
        _subscription = subscription;
    }

    [Command("cpf")]
    public async Task CPF(string cpf)
    {
        if (!await _subscription.HasAccessAsync(Context.User.Id))
        {
            await Context.Channel.SendMessageAsync(CommandMessages.InactiveSubscription);
            return;
        }

        await Context.Channel.SendMessageAsync(CommandMessages.Searching);

        var digits = new string(cpf.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
        {
            await SendTextAsync("⚠️ **CPF inválido!** Informe um CPF com exatamente 11 dígitos.");
            return;
        }

        var response = await _leakSight.SearchCPF(digits);

        if (response is null)
        {
            await SendTextAsync(
                "🚧 **Não foi possível concluir a consulta agora.** Tente novamente daqui a pouco.");
            return;
        }

        var sb = new StringBuilder();
        var hasResults = false;

        if (response.CONSULTA_CPF?.Count > 0)
        {
            hasResults = true;

            foreach (var dado in response.CONSULTA_CPF)
            {
                AppendSection(sb, "CONSULTA CPF");

                AppendField(sb, "CPF", dado.cpf);
                AppendField(sb, "Nome", dado.nome);
                AppendField(sb, "Pessoa", dado.pessoa);
                AppendField(sb, "Nome da mãe", dado.mae);
                AppendField(sb, "Nome do pai", dado.pai);

                AppendSection(sb, "NASCIMENTO");
                AppendField(sb, "Município de nascimento",
                    dado.municipionascimento);

                AppendSection(sb, "ENDEREÇO");
                AppendField(sb, "Logradouro", dado.logradouro);
                AppendField(sb, "Número", dado.numero);
                AppendField(sb, "Bairro", dado.bairro);
                AppendField(sb, "Município", dado.municipio);
                AppendField(sb, "CEP", dado.cep);

                AppendSection(sb, "CONTATO");
                AppendField(sb, "DDD", dado.ddd);
                AppendField(sb, "Telefone", dado.telefone);
                AppendField(sb, "Telefone secundário",
                    dado.telefonesecundario);

                AppendSection(sb, "DOCUMENTOS");
                AppendField(sb, "RG", dado.rgnumero);
                AppendField(sb, "Órgão emissor", dado.rgorgaoemisor);
                AppendField(sb, "UF do RG", dado.rguf);
                AppendField(sb, "Data de emissão do RG",
                    dado.rgdataemissao);
                AppendField(sb, "CNS", dado.cns);
            }
        }

        if (response.CREDLINK?.Count > 0)
        {
            hasResults = true;

            foreach (var dado in response.CREDLINK)
            {
                AppendSection(sb, "CREDLINK - DADOS PESSOAIS");

                AppendField(sb, "CPF", dado.cpf);
                AppendField(sb, "Nome", dado.nome);
                AppendField(sb, "Data de nascimento", dado.dt_nascimento);
                AppendField(sb, "Sexo", dado.sexo);
                AppendField(sb, "Nome da mãe", dado.nome_mae);

                AppendSection(sb, "ENDEREÇO");
                AppendField(sb, "Tipo de endereço", dado.tipo_endereco);
                AppendField(sb, "Logradouro", dado.logradouro);
                AppendField(sb, "Bairro", dado.bairro);
                AppendField(sb, "Cidade", dado.cidade);
                AppendField(sb, "Estado", dado.estado);
                AppendField(sb, "UF", dado.uf);
                AppendField(sb, "CEP", dado.cep);

                AppendSection(sb, "CONTATO");
                AppendField(sb, "Celular", dado.celular1);
                AppendField(sb, "E-mail", dado.email);

                AppendSection(sb, "INFORMAÇÕES ADICIONAIS");
                AppendField(sb, "Status na Receita Federal",
                    dado.status_receita_federal);

                AppendField(sb, "Óbito",
                    dado.flag_obito == 1 ? "Sim" :
                    dado.flag_obito == 0 ? "Não" : "Não informado");

                AppendField(sb, "CBO", dado.cbo);
                AppendField(sb, "Quantidade de veículos", dado.qt_veiculos);

                AppendField(sb, "Renda presumida",
                    dado.renda_presumida > 0
                        ? dado.renda_presumida.ToString("C0", PtBr)
                        : "Não informado");

                AppendField(sb, "Faixa de renda", dado.faixa_renda);
            }
        }

        if (response.RECEITA_FEDERAL?.DadosPessoais?.Count > 0)
        {
            hasResults = true;

            foreach (var dado in response.RECEITA_FEDERAL.DadosPessoais)
            {
                AppendSection(sb, "RECEITA FEDERAL - DADOS PESSOAIS");

                AppendField(sb, "CPF", dado.cpf);
                AppendField(sb, "Nome", dado.nome);
                AppendField(sb, "Primeiro nome", dado.primeironome);
                AppendField(sb, "Nome do meio", dado.nomemeio);
                AppendField(sb, "Último nome", dado.ultimonome);
                AppendField(sb, "Sobrenome da mãe", dado.sobrenomemae);

                AppendSection(sb, "ENDEREÇO");
                AppendField(sb, "Logradouro", dado.enderecologradouro);
                AppendField(sb, "Número", dado.endereconumero);
                AppendField(sb, "Complemento", dado.enderecocomplemento);
                AppendField(sb, "Bairro", dado.enderecobairro);
                AppendField(sb, "Cidade", dado.enderecocidade);
                AppendField(sb, "Estado", dado.enderecoestado);
                AppendField(sb, "CEP", dado.enderecocep);
                AppendField(sb, "Código IBGE", dado.codcidadeibge);

                AppendSection(sb, "CONTATO");
                AppendField(sb, "DDD", dado.dddtelefone);
                AppendField(sb, "Telefone", dado.numerotelefone);

                AppendSection(sb, "REGIÃO");
                AppendField(sb, "Mesorregião", dado.mesoregiao);
                AppendField(sb, "Microrregião", dado.microregiao);
                AppendField(sb, "Município", dado.municipio);
                AppendField(sb, "Código da região", dado.regiaocod);
                AppendField(sb, "Código da sub-região", dado.subregiaocod);
                AppendField(sb, "Código do distrito", dado.distritocod);
                AppendField(sb, "Código do subdistrito", dado.subdistritocod);

                AppendSection(sb, "REGISTRO");
                AppendField(sb, "Data de inclusão", dado.datainclusao);
                AppendField(sb, "Última atualização", dado.dataatualizacao);
            }
        }

        if (response.RECEITA_FEDERAL?.emails?.Count > 0)
        {
            hasResults = true;

            AppendSection(sb, "E-MAILS");

            foreach (var email in response.RECEITA_FEDERAL.emails)
                AppendField(sb, "E-mail", email.email);
        }

        if (!hasResults)
        {
            await SendTextAsync(
                CommandMessages.NoResults);
            return;
        }

        var content = sb.ToString().Trim();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var message = new MessageProperties
        {
            Content = $"✅ **Consulta concluída!** Organizei os resultados do CPF `{cpf}` no arquivo em anexo. 📄",
            Attachments =
            [
                new AttachmentProperties($"{cpf}.txt", stream)
            ]
        };

        await Context.Channel.SendMessageAsync(message);
    }

    private async Task SendTextAsync(string message)
    {
        await Context.Channel.SendMessageAsync(new MessageProperties()
        {
            Content = message
        });
    }

    private static void AppendSection(StringBuilder sb, string title)
    {
        sb.AppendLine();
        sb.AppendLine($"[{title}]");
    }

    private static void AppendField(
        StringBuilder sb,
        string label,
        object? value)
    {
        var formattedValue = value switch
        {
            null => "Não informado",
            string text when string.IsNullOrWhiteSpace(text)
                => "Não informado",
            _ => value.ToString()!
        };

        sb.AppendLine($"{label}: {formattedValue}");
    }
}
