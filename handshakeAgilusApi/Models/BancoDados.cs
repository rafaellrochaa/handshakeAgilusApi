using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace handshakeAgilusApi.Models
{
    public class BancoDados
    {
        private string ConnectionString { get; set; }

        public BancoDados(string conexao)
        {
            if (!String.IsNullOrEmpty(conexao))
                ConnectionString = conexao;
            else throw new Exception("Token inválido.");
        }

        public void AtualizarFaseAf(string codigoContratoBanco, int codigoFase)
        {
            SqlConnection conexao = new SqlConnection(ConnectionString);

            using (conexao)
            {
                conexao.Open();
                try
                {
                    var cmd = new SqlCommand("execute pr_atualizar_fase_af_mobile @codigo_contrato_banco, @codigo_fase", conexao);
                    cmd.Parameters.Add("@codigo_contrato_banco", SqlDbType.VarChar, 50).Value = codigoContratoBanco;
                    cmd.Parameters.Add("@codigo_fase", SqlDbType.Int, 10).Value = codigoFase;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a atualização das fases no banco de dados. Detalhe do erro: " + e.Message);
                }
            }
        }
        public List<Fase> obterFasesMobile()
        {
            SqlConnection conexao = new SqlConnection(ConnectionString);
            List<Fase> fases = null;

            using (conexao)
            {
                conexao.Open();
                try
                {
                    var dr = new SqlCommand("select faf_codigo, faf_descricao from fase_af where faf_ativa = 1 and usr_fase_mobile = 1", conexao).ExecuteReader();
                    while (dr.Read())
                    {
                        if (fases == null)
                        {
                            fases = new List<Fase>();
                        }
                        fases.Add(new Fase()
                        {
                            Id = dr["faf_codigo"].ToString(),
                            Descricao = dr["faf_descricao"].ToString(),
                        });
                    }
                    dr.NextResult();
                }
                catch (Exception e)
                {
                    throw new Exception("Houve um problema durante a consulta das fases no banco de dados. Erro: " + e.Message);
                }
            }
            return fases;
        }
        public void GravarImagemProposta(string codigoProposta, byte[] anexo, string nomeArquivo)
        {
            SqlConnection conexao = new SqlConnection(ConnectionString);

            using (conexao)
            {
                conexao.Open();

                try
                {
                    var cmd = new SqlCommand("execute pr_grava_imagem_proposta_mobile @anexo, @nome_arquivo, @codigo_proposta", conexao);

                    cmd.Parameters.Add("@codigo_proposta", SqlDbType.VarChar, 50).Value = codigoProposta;
                    cmd.Parameters.Add("@nome_arquivo", SqlDbType.VarChar, 200).Value = nomeArquivo;
                    cmd.Parameters.Add("@anexo", SqlDbType.VarBinary, -1).Value = anexo;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception("Houve um problema durante a gravação da imagem como anexo do contrato. Erro: " + e.Message);
                }
            }
        }
        public List<Proposta> obterPropostasMobile(int? codigoFase)
        {
            SqlConnection conexao = new SqlConnection(ConnectionString);
            string stringComando = String.Empty;
            List<Proposta> propostas = null;
            using (conexao)
            {
                conexao.Open();
                try
                {
                    SqlCommand cmd = null;

                    if (codigoFase != null)
                    {
                        cmd = new SqlCommand("execute pr_dados_af_mobile @faf_codigo", conexao);
                        cmd.Parameters.Add("@faf_codigo", SqlDbType.Int).Value = codigoFase;
                    }
                    else
                        cmd = new SqlCommand("execute pr_dados_af_mobile @faf_codigo = null", conexao);

                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (propostas == null)
                        {
                            propostas = new List<Proposta>();
                        }
                        propostas.Add(new Proposta()
                        {
                            CodigoContrato = dr["CodigoContrato"].ToString(),
                            CPF = dr["CPF"].ToString(),
                            Rg = dr["Rg"].ToString(),
                            Nome = dr["Nome"].ToString(),
                            DataNascimento = !String.IsNullOrEmpty(dr["DataNascimento"].ToString()) ? ((DateTime)dr["DataNascimento"]).ToString("dd/MM/yyyy") : String.Empty,
                            Sexo = dr["Sexo"].ToString(),
                            Endereco = dr["Endereco"].ToString(),
                            Complemento = dr["Complemento"].ToString(),
                            Bairro = dr["Bairro"].ToString(),
                            Cidade = dr["Cidade"].ToString(),
                            Estado = dr["Estado"].ToString(),
                            Cep = dr["Cep"].ToString(),
                            CodigoConvenioAgilus = Convert.ToInt64(dr["CodigoConvenioAgilus"]),
                            ReferenciaEndereco = dr["ReferenciaEndereco"].ToString(),
                        });
                    }
                    dr.NextResult();
                }

                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a consulta de contratos no banco de dados. Detalhe do erro: " + e.Message);
                }

                return propostas;
            }
        }
        public bool ValidaUsuario(int codigoCliente)
        {
            SqlConnection conexao = new SqlConnection(ConnectionString);
            var cmd = new SqlCommand("exec pr_valida_login @login = @usuario, @senha = @pass, @msg_erro = @erro out, @usu_codigo = @cod_usuario out", conexao);

            cmd.Parameters.Add("@cod_cliente", SqlDbType.VarChar).Value = codigoCliente;

            //Parâmetros de saída da proc
            SqlParameter outputErro = cmd.Parameters.Add("@erro", SqlDbType.VarChar, 300);
            outputErro.Direction = ParameterDirection.Output;
            SqlParameter outputCodUsuario = cmd.Parameters.Add("@cod_usuario", SqlDbType.Int);
            outputCodUsuario.Direction = ParameterDirection.Output;

            using (conexao)
            {
                conexao.Open();
                cmd.ExecuteScalar();
            }
            return outputErro.Value.ToString().Equals(String.Empty);
        }
    }
}
