using IdentityService.Application.DTOs;
using IdentityService.Application.Helpers;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Features.Auth.Commands.Create;

public record CreateCommandHandlerAuthUser(
    IAuthUserRepository repository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<CreateCommandHandlerAuthUser> logger)
        : IRequestHandler<CreateCommandAuthUser, Result<AuthUserDto>>
{
    public async Task<Result<AuthUserDto>> Handle(CreateCommandAuthUser request, CancellationToken cancellationToken)
    {
        int quantidadeDeCaracteresSenhaAleatoria = 5;
        string senhaTemporaria = PasswordHasher.GerarSenhaAleatoria(quantidadeDeCaracteresSenhaAleatoria);
        AuthUser dado = request.ToEntity(senhaTemporaria);
        await repository.CreateAsync(dado, cancellationToken);

        await EnviarEmailBoasVindasAsync(dado, senhaTemporaria);

        return Result<AuthUserDto>.Success(dado.ToDto(), "Usuário criado com sucesso.");
    }

    private async Task EnviarEmailBoasVindasAsync(AuthUser dado, string senhaTemporaria)
    {
        try
        {
            var corpoEmail = emailTemplateService.GerarBoasVindasUsuario(dado.UserName, senhaTemporaria);
            var emailRequest = new EnvioEmailRequest(
                dado.Email,
                "Bem-vindo ao Sistema",
                corpoEmail,
                dado.TenancyId
            );

            await mensageriaService.PublicarMensagemAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Create AuthUser] Falha ao enfileirar e-mail para {Email}", dado.Email);
        }
    }
}