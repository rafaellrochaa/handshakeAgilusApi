if object_id('pr_dados_af_mobile') > 0
begin
   drop procedure pr_dados_af_mobile
   print '<< DROP pr_dados_af_mobile >>'
end
GO

create procedure pr_dados_af_mobile(@faf_codigo int) as
begin

	select distinct af_codigo_contrato_banco as CodigoContrato, con_cpf as CPF, isnull(con_rg, clt_rg) as Rg, isnull(con_nome, clt_nome) as Nome,
   isnull(con_data_nasc, clt_data_nascimento) as DataNascimento, isnull(con_sexo, clt_sexo) as Sexo, isnull(con_endereco, clt_endereco) as Endereco, 
   isnull(con_complemento, clt_complemento) as Complemento, isnull(con_bairro, clt_bairro) as Bairro, isnull(con_cidade, clt_cidade) as Cidade, 
   isnull(con_uf, clt_uf) as Estado, isnull(con_cep, clt_cep) as Cep, isnull(c.orgav_codigo, cl.Orgav_codigo) as CodigoConvenioAgilus,
   isnull(con_referencia_endereco, clt_referencia_endereco) as ReferenciaEndereco
	from af
   inner join fase_af as f on af.faf_codigo = f.faf_codigo
	inner join convenio as c on c.con_codigo = af.con_codigo
   inner join cliente_televenda as cl on cl.clt_cpf = c.con_cpf and cl.orgav_codigo = c.orgav_codigo
	where (@faf_codigo is null or af.faf_codigo = @faf_codigo)
   and f.faf_ativa = 1
   and f.usr_fase_mobile = 1

end
GO
grant execute on pr_dados_af_mobile to suporte_agilus