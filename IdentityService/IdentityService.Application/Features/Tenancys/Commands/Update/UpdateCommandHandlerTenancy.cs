using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Features.Tenancys.Commands.Update;

public class UpdateCommandHandlerTenancy(
    ITenancyRepository repository,
    IAuthUserRepository authUserRepository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<UpdateCommandHandlerTenancy> logger)
        : IRequestHandler<UpdateCommandTenancy, Result<TenancyDto>>
{
    public async Task<Result<TenancyDto>> Handle(UpdateCommandTenancy request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dadoToUpdate == null) return Result<TenancyDto>.Failure("Tenancy não encontrada.");

        bool statusMudouParaInativo = dadoToUpdate.UpdateFromCommand(request);
        await repository.UpdateAsync(dadoToUpdate, cancellationToken);

        if (statusMudouParaInativo) await SincronizarStatusUsuariosAsync(dadoToUpdate.Id, request.Ativo, cancellationToken);

        await EnviarEmailAlteracoesAsync(dadoToUpdate);

        return Result<TenancyDto>.Success(dadoToUpdate.ToDto(), "Tenancy atualizada com sucesso.");
    }

    private async Task EnviarEmailAlteracoesAsync(Tenancy dadoToUpdate)
    {
        try
        {
            var corpoEmail = emailTemplateService.GerarTenancyAlterada(dadoToUpdate.RazaoSocial);
            var emailRequest = new EnvioEmailRequest(
                dadoToUpdate.Email,
                "Tenancy alteração de Dados Cadastrais",
                corpoEmail,
                dadoToUpdate.Id
            );

            await mensageriaService.PublicarMensagemAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Update Tenancy] Falha ao enfileirar e-mail para {Email}", dadoToUpdate.Email);
        }
    }

    private async Task SincronizarStatusUsuariosAsync(long tenancyId, TipoTenancyAtivo novoStatus, CancellationToken cancellationToken)
    {
        var usuarios = await authUserRepository.GetByTenancyIdAsync(tenancyId, cancellationToken);

        foreach (var usuario in usuarios)
        {
            usuario.TenancyAtiva = novoStatus;
            usuario.DataAlteracao = DateTime.Now;
            await authUserRepository.UpdateAsync(usuario, cancellationToken);
        }

        logger.LogInformation("[Update Status Usuarios] Status de {Qtd} usuários sincronizados com a tenancy {Id}", usuarios.Count(), tenancyId);
    }
}