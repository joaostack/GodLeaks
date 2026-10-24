using System.Text.Json.Serialization;
using GodLeaks.Infrastructure.LeakSightAPI;

namespace GodLeaks.Domain.Entities.LeakSightAPI;

public class Nome
{
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BIGDATANOME
    {
        public string nome_completo { get; set; }
        public int total { get; set; }
        public List<Dado> dados { get; set; }
    }

    public class CONSULTANOME
    {
        public string tdoc { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }
        public string logr { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? num { get; set; }
        public object compl { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public int cep { get; set; }
        public int ddd { get; set; }
        public int tel { get; set; }
        public object inst { get; set; }
        public string operadora { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? cpf { get; set; }
        public string tp_log { get; set; }
        public string lograd { get; set; }
        public object telefone { get; set; }
        public string tipo_doc { get; set; }
        public string endereco { get; set; }
        public string estado { get; set; }
        public string data_ativ { get; set; }
        public string data_contrato { get; set; }
        public int? id { get; set; }
        public string nascimento { get; set; }
        public string mae { get; set; }
        public string rua { get; set; }
        public int? numero { get; set; }
        public string complemento { get; set; }
        public string pessoa { get; set; }
        public int? fone { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? cns { get; set; }
        public string sexo { get; set; }
        public string raca_cor { get; set; }
        public int? falecido { get; set; }
        public string data_falecimento { get; set; }
        public string pai { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? celular { get; set; }
        public string contato { get; set; }
        public string email { get; set; }
        public string rg { get; set; }
        public string rg_orgao_emissor { get; set; }
        public string rg_data_emissao { get; set; }
        public object nis { get; set; }
    }

    public class CREDILINK
    {
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? cpf { get; set; }
        public string nome { get; set; }
        public string tipo_endereco { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public string uf { get; set; }
        public int cep { get; set; }
        public string tel_fixo1 { get; set; }
        public string celular1 { get; set; }
        public string celular2 { get; set; }
        public string dt_nascimento { get; set; }
        public string nome_mae { get; set; }
        public string sexo { get; set; }
        public string email { get; set; }
        public int flag_obito { get; set; }
        public string status_receita_federal { get; set; }
        public string pct_cargo_societario { get; set; }
        public int cbo { get; set; }
        public int qt_veiculos { get; set; }
        public int renda_presumida { get; set; }
        public int faixa_renda { get; set; }
        public string complemento { get; set; }
        public string celular3 { get; set; }
        public string celular4 { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? celular5 { get; set; }
        public string modelo_veiculo1 { get; set; }
        public int? ano_veiculo1 { get; set; }
    }

    public class Dado
    {
        public int CONTATOS_ID { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? CPF { get; set; }
        public string NOME { get; set; }
        public string SEXO { get; set; }
        public string NASC { get; set; }
        public string NOME_MAE { get; set; }
        public string NOME_PAI { get; set; }
        public int CADASTRO_ID { get; set; }
        public string ESTCIV { get; set; }
        public bool SO { get; set; }
        public int CD_SIT_CAD { get; set; }
        public string DT_SIT_CAD { get; set; }
        public int CBO { get; set; }
        public string CD_MOSAIC { get; set; }
        public string RENDA { get; set; }
        public int FAIXA_RENDA_ID { get; set; }
        public string TITULO_ELEITOR { get; set; }
        public string CD_MOSAIC_NOVO { get; set; }
        public string CD_MOSAIC_SECUNDARIO { get; set; }
        public List<Email> emails { get; set; }
        public List<Telefone> telefones { get; set; }
        public List<object> telefones_pf { get; set; }
        public List<object> telefones_pj { get; set; }
        public List<Endereco> enderecos { get; set; }
        public List<Score> score { get; set; }
        public List<PoderAquisitivo> poder_aquisitivo { get; set; }
        public List<Pi> pis { get; set; }
        public List<Tse> tse { get; set; }
        public List<Parente> parentes { get; set; }
        public string DT_INFORMACAO { get; set; }
        public string RG { get; set; }
        public string ORGAO_EMISSOR { get; set; }
    }

    public class Email
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

    public class Parente
    {
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? CPF_Completo { get; set; }
        public string NOME { get; set; }
        public string NOME_VINCULO { get; set; }
        public string VINCULO { get; set; }
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? CPF_VINCULO { get; set; }
    }

    public class Pi
    {
        public int CONTATOS_ID { get; set; }
        public double PIS { get; set; }
    }

    public class PoderAquisitivo
    {
        public int CONTATOS_ID { get; set; }
        public int COD_PODER_AQUISITIVO { get; set; }
        public string PODER_AQUISITIVO { get; set; }
        public string RENDA_PODER_AQUISITIVO { get; set; }
        public string FX_PODER_AQUISITIVO { get; set; }
    }

    public class RECEITAFEDERALNOME
    {
        [JsonConverter(typeof(StringOrNumberConverter))]
        public string? cpf { get; set; }
        public string nome { get; set; }
        public string primeironome { get; set; }
        public string ultimonome { get; set; }
        public string sobrenomemae { get; set; }
        public string datainclusao { get; set; }
        public string dataatualizacao { get; set; }
        public string enderecologradouro { get; set; }
        public string endereconumero { get; set; }
        public string enderecobairro { get; set; }
        public string enderecocidade { get; set; }
        public string enderecoestado { get; set; }
        public string enderecocep { get; set; }
        public string codcidadeibge { get; set; }
        public string numerotelefone { get; set; }
        public string mesoregiao { get; set; }
        public string microregiao { get; set; }
        public string municipio { get; set; }
        public string regiaocod { get; set; }
        public string subregiaocod { get; set; }
        public string distritocod { get; set; }
        public string subdistritocod { get; set; }
        public string enderecocomplemento { get; set; }
        public string dddtelefone { get; set; }
    }

    public class Root
    {
        public List<CONSULTANOME> CONSULTA_NOME { get; set; }
        public List<RECEITAFEDERALNOME> RECEITA_FEDERAL_NOME { get; set; }
        public List<CREDILINK> CREDILINK { get; set; }
        public List<BIGDATANOME> BIGDATA_NOME { get; set; }
    }

    public class Score
    {
        public int CONTATOS_ID { get; set; }
        public int CSB8 { get; set; }
        public string CSB8_FAIXA { get; set; }
        public int CSBA { get; set; }
        public string CSBA_FAIXA { get; set; }
    }

    public class Telefone
    {
        public int CONTATOS_ID { get; set; }
        public string DDD { get; set; }
        public string TELEFONE { get; set; }
        public int TIPO_TELEFONE { get; set; }
        public string DT_INCLUSAO { get; set; }
        public string DT_INFORMACAO { get; set; }
        public int NSU { get; set; }
        public string CLASSIFICACAO { get; set; }
        public int? SIGILO { get; set; }
    }

    public class Tse
    {
        public int CONTATOS_ID { get; set; }
        public string TITULO_ELEITOR { get; set; }
        public int ZONA { get; set; }
        public int SECAO { get; set; }
    }
}
