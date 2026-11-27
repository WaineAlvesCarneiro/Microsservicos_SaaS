using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Features.Auth.Commands.Update;

public record UpdateCommandHandlerAuthUser(
    IAuthUserRepository repository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<UpdateCommandHandlerAuthUser> logger)
        : IRequestHandler<UpdateCommandAuthUser, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(UpdateCommandAuthUser request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dadoToUpdate == null) return Result<AuthUserDto>.Failure("Usuário não encontrado.");

        dadoToUpdate.UpdateFromCommand(request);
        await repository.UpdateAsync(dadoToUpdate, cancellationToken);
        await EnviarEmailAlteracoesAsync(dadoToUpdate);

        return Result<AuthUserDto>.Success(dadoToUpdate.ToDto(), "Usuário atualizado com sucesso.");
    }

    private async Task EnviarEmailAlteracoesAsync(AuthUser dadoToUpdate)
    {
        try
        {
            var corpoEmail = emailTemplateService.GerarUsuarioAlterado(dadoToUpdate.UserName);
            var emailRequest = new EnvioEmailRequest(
                dadoToUpdate.Email,
                "Usuário alteração de Dados Cadastrais",
                corpoEmail,
                dadoToUpdate.TenancyId
            );

            await mensageriaService.PublicarMensagemAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Update Tenancy] Falha ao enfileirar e-mail para {Email}", dadoToUpdate.Email);
        }
    }
}