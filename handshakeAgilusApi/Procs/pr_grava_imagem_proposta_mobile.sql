if object_id('pr_grava_imagem_proposta_mobile') > 0
begin
   drop procedure pr_grava_imagem_proposta_mobile
   print '<< DROP pr_grava_imagem_proposta_mobile >>'
end
GO
CREATE procedure pr_grava_imagem_proposta_mobile(@anexo varbinary(max), @nome_arquivo varchar(200), @codigo_proposta varchar(50))  
as  
begin  
 set nocount on   
 set xact_abort on  
 
	declare @af_codigo int

	select @af_codigo = af_codigo   
	from af  
	inner join fase_af as fa on fa.faf_codigo = af.faf_codigo  
	where af_codigo_contrato_banco = @codigo_proposta  
	and not exists(select 1
	from af
	inner join anexo_af as aa on aa.af_codigo = af.af_codigo
	inner join anexo as a on a.anx_codigo = aa.anx_codigo
	where af.af_codigo = @af_codigo
	and a.anx_nome_arquivo = @nome_arquivo)
	
	begin transaction  
	execute pr_grava_anexo @documento = @anexo, @nome_arquivo = @nome_arquivo, @af_codigo = @af_codigo, @compactado = 0  
	execute pr_registra_acompanhamento_af @af_codigo = @af_codigo, @descricao = 'DOCUMENTOS GRAVADOS PELA API MOBILE.', @automatico = 1, @nome_usuario='suporte_agilus'  
	commit  
end
GO
grant execute on pr_grava_imagem_proposta_mobile to suporte_agilus