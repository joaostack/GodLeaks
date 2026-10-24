using System.Text.Json.Serialization;
using GodLeaks.Infrastructure.LeakSightAPI;

namespace GodLeaks.Domain.Entities.LeakSightAPI;

public class CPF
{
    public class APICPFFULL
    {
        public string cpf { get; set; }
        public int total { get; set; }
        public List<Dado> dados { get; set; }
    }

    public class BigData
    {
        public Search search { get; set; }
        public Breachbase breachbase { get; set; }
        public Hackcheck hackcheck { get; set; }
    }

    public class Brazil15B
    {
        public string search_term { get; set; }
        public string path { get; set; }
        public List<object> results { get; set; }
        public int total_time_ms { get; set; }
        public int collections_with_results { get; set; }
    }

    public class Breachbase
    {
        public string error { get; set; }
    }

    public class CONSULTACPF
    {
        public string cpf { get; set; }
        public string nome { get; set; }
        public string pessoa { get; set; }
        public int ddd { get; set; }
        public int fone { get; set; }
        public string inst { get; set; }
        public string pai { get; set; }
        public string mae { get; set; }
        public string municipionascimento { get; set; }
        public string municipio { get; set; }
        public string logradouro { get; set; }
        // The API may return numeric or textual address numbers (e.g. "S/N").
        // Keep it as text so deserialization accepts both forms and preserves
        // leading zeroes and non-numeric values.
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string numero { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? rgnumero { get; set; }
        public string rgorgaoemisor { get; set; }
        public string rguf { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? rgdataemissao { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? cns { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string telefone { get; set; }
        public string telefonesecundario { get; set; }
    }

    public class CREDLINK
    {
        public string cpf { get; set; }
        public string nome { get; set; }
        public string tipo_endereco { get; set; }
        public string logradouro { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public string uf { get; set; }
        public int cep { get; set; }
        public string celular1 { get; set; }
        public string dt_nascimento { get; set; }
        public string nome_mae { get; set; }
        public string sexo { get; set; }
        public string email { get; set; }
        public int flag_obito { get; set; }
        public string status_receita_federal { get; set; }
        public int cbo { get; set; }
        public int qt_veiculos { get; set; }
        public int renda_presumida { get; set; }
        public int faixa_renda { get; set; }
    }

    public class Dado
    {
        public int CONTATOS_ID { get; set; }
        public string EMAIL { get; set; }
        public int PRIORIDADE { get; set; }
        public string EMAIL_SCORE { get; set; }
        public string EMAIL_PESSOAL { get; set; }
        public string EMAIL_DUPLICADO { get; set; }
        public string BLACKLIST { get; set; }
        public string ESTRUTURA { get; set; }
        public string STATUS_VT { get; set; }
        public string DOMINIO { get; set; }
        public int MAPAS { get; set; }
        public int PESO { get; set; }
        public int CADASTRO_ID { get; set; }
        public string DT_INCLUSAO { get; set; }
        //public Dados dados { get; set; }
        public List<Telefone> telefones { get; set; }
        public List<Endereco> enderecos { get; set; }
        public List<object> parentes { get; set; }
        public string CPF { get; set; }
        public string NOME { get; set; }
        public string SEXO { get; set; }
        public string NASC { get; set; }
        public string NOME_MAE { get; set; }
        public bool SO { get; set; }
        public int CD_SIT_CAD { get; set; }
        public string DT_SIT_CAD { get; set; }
        public string TITULO_ELEITOR { get; set; }
        public string CD_MOSAIC_NOVO { get; set; }
        public string CD_MOSAIC_SECUNDARIO { get; set; }
        public List<Email> emails { get; set; }
        public List<object> telefones_pf { get; set; }
        public List<object> telefones_pj { get; set; }
        public List<Score> score { get; set; }
        public List<object> poder_aquisitivo { get; set; }
        public List<object> pis { get; set; }
        public List<object> tse { get; set; }
        public string RG { get; set; }
        public string DT_INFORMACAO { get; set; }
        public int CBO { get; set; }
        public string ORGAO_EMISSOR { get; set; }
        public string CD_MOSAIC { get; set; }
        public string RENDA { get; set; }
        public int FAIXA_RENDA_ID { get; set; }
    }

    public class DadosPessoai
    {
        public string cpf { get; set; }
        public string nome { get; set; }
        public string primeironome { get; set; }
        public string nomemeio { get; set; }
        public string ultimonome { get; set; }
        public string sobrenomemae { get; set; }
        public string datainclusao { get; set; }
        public string dataatualizacao { get; set; }
        public string enderecologradouro { get; set; }
        public string endereconumero { get; set; }
        public string enderecocomplemento { get; set; }
        public string enderecobairro { get; set; }
        public string enderecocidade { get; set; }
        public string enderecoestado { get; set; }
        public string enderecocep { get; set; }
        public string codcidadeibge { get; set; }
        public string dddtelefone { get; set; }
        public string numerotelefone { get; set; }
        public string mesoregiao { get; set; }
        public string microregiao { get; set; }
        public string municipio { get; set; }
        public string regiaocod { get; set; }
        public string subregiaocod { get; set; }
        public string distritocod { get; set; }
        public string subdistritocod { get; set; }
    }

    public class Email
    {
        public string cpf { get; set; }
        public string email { get; set; }
        public string email_score { get; set; }
        public string email_pessoal { get; set; }
        public string blacklist { get; set; }
        public int CONTATOS_ID { get; set; }
        public int PRIORIDADE { get; set; }
        public string EMAIL_DUPLICADO { get; set; }
        public string ESTRUTURA { get; set; }
        public string STATUS_VT { get; set; }
        public string DOMINIO { get; set; }
        public int MAPAS { get; set; }
        public int PESO { get; set; }
        public int CADASTRO_ID { get; set; }
        public string DT_INCLUSAO { get; set; }
    }

    public class Endereco
    {
        public int CONTATOS_ID { get; set; }
        public string LOGR_TIPO { get; set; }
        public string LOGR_NOME { get; set; }
        public string LOGR_NUMERO { get; set; }
        public string BAIRRO { get; set; }
        public string CIDADE { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public string DT_ATUALIZACAO { get; set; }
        public string DT_INCLUSAO { get; set; }
        public string TIPO_ENDERECO_ID { get; set; }
        public string LOGR_COMPLEMENTO { get; set; }
    }

    public class Hackcheck
    {
        public string error { get; set; }
    }

    public class Osint
    {
        public string status { get; set; }
        public string value { get; set; }
    }

    public class Osint2
    {
        public string email { get; set; }
        public int total { get; set; }
        public List<Dado> dados { get; set; }
    }

    public class RECEITAFEDERAL
    {
        [JsonPropertyName("Dados Pessoais")] public List<DadosPessoai> DadosPessoais { get; set; }
        public List<Email> emails { get; set; }
    }

    public class Root
    {
        public List<CONSULTACPF> CONSULTA_CPF { get; set; }
        public RECEITAFEDERAL RECEITA_FEDERAL { get; set; }
        public List<CREDLINK> CREDLINK { get; set; }
        public List<VAZAMENTOSCPF> VAZAMENTOS_CPF { get; set; }
        public List<VAZAMENTOSEMAIL> VAZAMENTOS_EMAIL { get; set; }
        public List<APICPFFULL> API_CPF_FULL { get; set; }
        public List<object> INSS { get; set; }
    }

    public class Score
    {
        public int CONTATOS_ID { get; set; }
        public int CSBA { get; set; }
        public string CSBA_FAIXA { get; set; }
    }

    public class Search
    {
        public string error { get; set; }
    }

    public class Telefone
    {
        public int CONTATOS_ID { get; set; }
        public string DDD { get; set; }
        public string TELEFONE { get; set; }
        public int TIPO_TELEFONE { get; set; }
        public string DT_INCLUSAO { get; set; }
        public string CLASSIFICACAO { get; set; }
        public string DT_INFORMACAO { get; set; }
        public int? NSU { get; set; }
    }

    public class VAZAMENTOSCPF
    {
        public string time { get; set; }
        public List<object> stealer_json { get; set; }
        public List<object> database_url { get; set; }
        public List<object> bigcomboCombolist { get; set; }
        public List<object> DatabaseOsintLeaks { get; set; }
    }

    public class VAZAMENTOSEMAIL
    {
        public string time { get; set; }
        public List<object> stealer_json { get; set; }
        public List<object> database_url { get; set; }
        public List<object> bigcomboCombolist { get; set; }
        public List<object> DatabaseOsintLeaks { get; set; }
        public Osint osint { get; set; }
        public BigData BigData { get; set; }
        public Brazil15B Brazil15B { get; set; }
    }
}
