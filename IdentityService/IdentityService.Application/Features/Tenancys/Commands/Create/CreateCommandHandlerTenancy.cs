using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Features.Tenancys.Commands.Create;

public record CreateCommandHandlerTenancy(
    ITenancyRepository repository,
    IMensageriaService mensageriaService,
    IEmailTemplateService emailTemplateService,
    ILogger<CreateCommandHandlerTenancy> logger)
        : IRequestHandler<CreateCommandTenancy, Result<TenancyDto>>
{
    public async Task<Result<TenancyDto>> Handle(CreateCommandTenancy request, CancellationToken cancellationToken)
    {
        Tenancy dado = request.ToEntity();
        await repository.CreateAsync(dado, cancellationToken);

        await EnviarEmailBoasVindasAsync(dado);

        return Result<TenancyDto>.Success(dado.ToDto(), "Tenancy criada com sucesso.");
    }

    private async Task EnviarEmailBoasVindasAsync(Tenancy dado)
    {
        try
        {
            var corpoEmail = emailTemplateService.GerarBoasVindasTenancy(dado.RazaoSocial);
            var emailRequest = new EnvioEmailRequest(
                dado.Email,
                "Bem-vindo ao Sistema",
                corpoEmail,
                dado.Id
            );

            await mensageriaService.PublicarMensagemAsync(emailRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Create Tenancy] Falha ao enfileirar e-mail para {Email}", dado.Email);
        }
    }
}