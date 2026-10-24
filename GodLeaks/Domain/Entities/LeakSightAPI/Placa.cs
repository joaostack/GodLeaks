namespace GodLeaks.Domain.Entities.LeakSightAPI;

public class Placa
{
    public class Dado
    {
        public string DOCUMENTO { get; set; }
        public string TP_DOC { get; set; }
        public string PLACA { get; set; }
        public string CHASSI { get; set; }
        public object RENAVAM { get; set; }
        public string CIDADE { get; set; }
        public string ESTADO { get; set; }
        public string CF_CIDADE { get; set; }
        public string CF_UF { get; set; }
        public string DT_RECENCIA { get; set; }
        public string DT_INSR { get; set; }
        public object CO_USER_INSR { get; set; }
        public string DT_LAST_UPDT { get; set; }
        public object CO_USER_LAST_UPDT { get; set; }
        public int? COD_MARCA_MODELO { get; set; }
        public int? ANO_FABRICACAO { get; set; }
        public int? COD_COMBUST { get; set; }
        public int? COD_TIPO_VEIC { get; set; }
        public int? COD_ESP_VEIC { get; set; }
        public int? COD_COR { get; set; }
        public int? COD_TIPO_CARROCERIA { get; set; }
        public int? NACIONALIDADE { get; set; }
        public int? POTENCIA { get; set; }
        public string CARGA { get; set; }
        public int? NUM_EIXOS { get; set; }
        public int? CILINDRADAS { get; set; }
        public string CAP_MAX_TRACAO { get; set; }
        public string PESO_BRUTO_TOTAL { get; set; }
        public int? LUGARES { get; set; }
        public int? TIPO_MONTAGEM { get; set; }
        public string UF_JURISDICAO { get; set; }
        public int? CODMUN { get; set; }
        public string DT_EMPLACAMENTO { get; set; }
        public long? DOC_FATURADO { get; set; }
        public int? TIPO_DOC_IMP { get; set; }
        public int? NUM_DOC_IMP { get; set; }
        public string DATA_REG_DI { get; set; }
        public int? COD_UNID_SRF { get; set; }
        public string DAT_ULT_ATU { get; set; }
        public int? RESTR_01 { get; set; }
        public int? RESTR_02 { get; set; }
        public int? RESTR_03 { get; set; }
        public int? RESTR_04 { get; set; }
        public string DATA_LIM_RESTR { get; set; }
        public string COD_SIT_VEIC { get; set; }
        public string COD_SIT_CHASSI { get; set; }
        public int? TIPO_DOC_FATURADO { get; set; }
        public string UF_FATURADO { get; set; }
        public int? PROCEDENCIA { get; set; }
        public int? TIPO_DOC_PROPRIETARIO { get; set; }
    }

    public class Root
    {
        public int total { get; set; }
        public List<Dado> dados { get; set; }
    }
}