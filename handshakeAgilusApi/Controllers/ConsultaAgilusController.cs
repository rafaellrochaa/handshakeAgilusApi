using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http;
using System;
using System.Collections.Generic;

namespace handshakeAgilusApi.Models
{
    public class ConsultaAgilusController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage getPropostas(string chave)
        {
            string enderecoServidor = null;
            List<Proposta> propostas = null;
            try
            {
                enderecoServidor = Utilitarios.ConexaoSolicitada(chave);
                propostas = new BancoDados(enderecoServidor).obterPropostasMobile(null);
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

        [HttpGet]
        public HttpResponseMessage getPropostas(string chave, int codigoFase)
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
        [HttpPost]
        public HttpResponseMessage setFaseProposta()
        {
            var dadosPost = JsonConvert.DeserializeObject<AtualizaFaseProposta>(Request.Content.ReadAsStringAsync().Result);
            string enderecoServidor;
            try
            {
                enderecoServidor = Utilitarios.ConexaoSolicitada(dadosPost.Chave);
                new BancoDados(enderecoServidor).AtualizarFaseAf(dadosPost.CodigoContratoBanco, dadosPost.CodigoFase);
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

        [HttpGet]
        public HttpResponseMessage getFases(string chave)
        {
            string enderecoServidor = Utilitarios.ConexaoSolicitada(chave);
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new BancoDados(enderecoServidor).obterFasesMobile()), System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpPost]
        public HttpResponseMessage setDocumentoProposta()
        {
            string dadosProposta = Request.Content.ReadAsStringAsync().Result;
            ImagemProposta proposta = JsonConvert.DeserializeObject<ImagemProposta>(dadosProposta);
            string enderecoServidor = Utilitarios.ConexaoSolicitada(proposta.Chave);

            new BancoDados(enderecoServidor).GravarImagemProposta(proposta.CodigoProposta, Convert.FromBase64String(proposta.Imagem), proposta.NomeArquivo);

            return new HttpResponseMessage()
            {
                Content = new StringContent(@"{""StatusAgilus"": ""Documento anexado ao contrato.""}", System.Text.Encoding.UTF8, "application/json")
            };
        }
        [HttpGet]
        public string ObterToken(string codigoCliente)
        {
            return Utilitarios.GerarToken(codigoCliente);
        }
    }
}