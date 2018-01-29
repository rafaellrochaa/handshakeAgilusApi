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

        public void AtualizarFaseAf(string codigoContratoBanco, int? codigoFase)
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
                try
                {
                    var dr = new SqlCommand("select faf_codigo, faf_descricao from fase_af where usr_mobile = 1", conexao).ExecuteReader();
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
            List<Proposta> propostas = null;
            using (conexao)
            {
                conexao.Open();
                try
                {
                    var dr = new SqlCommand(String.Format("execute pr_dados_af_mobile"), conexao).ExecuteReader();
                    while (dr.Read())
                    {
                        if (propostas == null)
                        {
                            propostas = new List<Proposta>();
                        }
                        propostas.Add(new Proposta()
                        {
                            CodigoContrato = dr["af_codigo_contrato_banco"].ToString(),
                            CPF = dr["con_cpf"].ToString(),
                            Rg = dr["con_rg"].ToString(),
                            Nome = dr["con_nome"].ToString(),
                            DataNascimento = !String.IsNullOrEmpty(dr["con_data_nasc"].ToString()) ? ((DateTime)dr["con_data_nasc"]).ToString("dd/MM/yyyy") : String.Empty,
                            Sexo = dr["con_sexo"].ToString(),
                            Endereco = dr["con_endereco"].ToString(),
                            Complemento = dr["con_complemento"].ToString(),
                            Bairro = dr["con_bairro"].ToString(),
                            Cidade = dr["con_cidade"].ToString(),
                            Estado = dr["con_uf"].ToString(),
                            Cep = dr["con_cep"].ToString(),
                            CodigoConvenioAgilus = Convert.ToInt64(dr["orgav_codigo"]),
                            ReferenciaEndereco = dr["con_referencia_endereco"].ToString(),
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
