if object_id('pr_atualizar_fase_af_mobile') > 0
begin
   drop procedure pr_atualizar_fase_af_mobile
   print '<< DROP pr_atualizar_fase_af_mobile >>'
end
GO

create procedure pr_atualizar_fase_af_mobile(@af_codigo_contrato_banco varchar(50), @faf_codigo int) as
begin
	declare @af_codigo int,
	@faf_codigo_origem int,
	@fase_destino varchar(100) = (select faf_descricao from fase_af where faf_codigo = @faf_codigo),
	@fixa_comissao bit = (select faf_fixa_comissao from fase_af where faf_codigo = @faf_codigo)
	declare @texto_acompanhamento varchar(500) = 'Fase de contrato atualizada para '+ @fase_destino+', pela api mobile.'

	select @af_codigo = af_codigo,
	@faf_codigo_origem = faf_codigo
	from af
	where af_codigo_contrato_banco = @af_codigo_contrato_banco  

	if (@fixa_comissao = 1)
	begin
		raiserror('O contrato está em uma fase finalizada, portanto não é possível atualizá-lo.',16,1,0)
		return
	end

	begin transaction
	update af  
	set faf_codigo = @faf_codigo  
	where af_codigo = @af_codigo  
  
	exec pr_registra_acompanhamento_af @af_codigo = @af_codigo, @descricao =  @texto_acompanhamento,  @automatico = 1,   
	@faf_codigo_origem = @faf_codigo_origem, @faf_codigo_destino = @faf_codigo
	commit  
end
GO
grant execute on pr_atualizar_fase_af_mobile to suporte_agilus

