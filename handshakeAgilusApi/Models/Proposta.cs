using Newtonsoft.Json;
using System.Collections.Generic;

namespace handshakeAgilusApi.Models
{
    public class Proposta
    {
        [JsonIgnore]
        public string CodigoContrato { get; set; }
        public string CPF { get; set; }
        public string Rg { get; set; }
        public string Nome { get; set; }
        public string DataNascimento { get; set; }
        public string Sexo { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
        public long CodigoConvenioAgilus { get; set; }
        public string ReferenciaEndereco { get; set; }
    }
}

