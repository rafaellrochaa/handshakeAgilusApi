using Newtonsoft.Json;
using System.Net.Http;
using SysWeb = System.Web;
using System.Web.Http;
using System;
using System.Collections.Generic;

namespace handshakeAgilusApi.Models
{
    public class ConsultaAgilusController : ApiController
    {
        [SysWeb.Http.HttpGet]
        public HttpResponseMessage getPropostas(string chave, int? codigoFase)
        {
            string enderecoServidor = null;
            List<Proposta> propostas = null;
            try
            {
                enderecoServidor = Utilitarios.ConexaoSolicitada(chave);
                propostas = new BancoDados(enderecoServidor).obterPropostasMobile(codigoFase);
            }
            catch (Exception e)
            {
                throw new Exception("Ocorreu um problema durante a solicitação. Detalhe do erro: " + e.Message);
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(propostas), System.Text.Encoding.UTF8, "application/json")
            };
        }
        [SysWeb.Http.HttpPost]
        public HttpResponseMessage setFaseProposta(string chave, string codigoContratoBanco, int codigoFase)
        {
            string enderecoServidor = null;

            try
            {
                enderecoServidor = Utilitarios.ConexaoSolicitada(chave);
                new BancoDados(enderecoServidor).AtualizarFaseAf(codigoContratoBanco, codigoFase);
            }
            catch (Exception e)
            {
                throw new Exception("Ocorreu um problema durante a atualização de fase deste contrato. Detalhe do erro: " + e.Message);
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent(@"{""StatusAgilus"": ""Fase atualizada""}", System.Text.Encoding.UTF8, "application/json")
            };
        }

        [SysWeb.Http.HttpGet]
        public HttpResponseMessage getFases(string chave)
        {
            string enderecoServidor = Utilitarios.ConexaoSolicitada(chave);
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new BancoDados(enderecoServidor).obterFasesMobile()), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [SysWeb.Http.HttpPost]
        public HttpResponseMessage postImagemProposta()
        {
            string dadosProposta = Request.Content.ReadAsStringAsync().Result;
            ImagemProposta proposta = JsonConvert.DeserializeObject<ImagemProposta>(dadosProposta);
            string enderecoServidor = Utilitarios.ConexaoSolicitada(proposta.Token);

            new BancoDados(enderecoServidor).GravarImagemProposta(proposta.CodigoProposta, Convert.FromBase64String(proposta.Imagem), proposta.NomeArquivo);

            return new HttpResponseMessage()
            {
                Content = new StringContent(@"{""StatusAgilus"": ""Documento anexado ao contrato.""}", System.Text.Encoding.UTF8, "application/json")
            };
        }
        public string ObterToken(string codigoCliente)
        {
            return Utilitarios.GerarToken(codigoCliente);
        }
    }
}