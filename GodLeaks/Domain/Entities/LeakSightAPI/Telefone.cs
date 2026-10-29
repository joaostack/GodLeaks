using System.Text.Json.Serialization;

namespace GodLeaks.Domain.Entities.LeakSightAPI;

public class Telefone
{
    public class BigDataNew10B
    {
        public string error { get; set; }
    }

    public class Brazil575M
    {
        public string TELEFONE { get; set; }
        public string OPERADORA { get; set; }
        public string TIPO_PESSOA { get; set; }
        public string NOME { get; set; }
        public string DOCUMENTO { get; set; }
        public string ENDERECO { get; set; }
        public string NUMERO { get; set; }
        public string BAIRRO { get; set; }
        public string ESTADO { get; set; }
        public string CEP { get; set; }
    }

    public class DataLeaksVeryBig
    {
        public string search_term { get; set; }
        public string path { get; set; }
        public List<object> results { get; set; }
        public int total_time_ms { get; set; }
        public int collections_with_results { get; set; }
    }

    public class Root
    {
        public DataLeaksVeryBig DataLeaksVeryBig { get; set; }
        public BigDataNew10B BigDataNew10B { get; set; }
        public List<object> PhoneBrazil { get; set; }
        public List<Brazil575M> Brazil_575M { get; set; }
    }
}