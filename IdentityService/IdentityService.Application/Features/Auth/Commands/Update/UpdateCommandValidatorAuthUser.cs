using IdentityService.Application.Features.Auth.Commands.ValidatorBase;
using IdentityService.Domain.Enums;
using FluentValidation;

namespace IdentityService.Application.Features.Auth.Commands.Update;

public class UpdateCommandValidatorAuthUser : CommandValidatorBaseAuthUser<UpdateCommandAuthUser>
{
    public UpdateCommandValidatorAuthUser()
    {
        ConfigureCommonRules();

        RuleFor(command => command.TenancyId)
            .Must((command, tenancyId) => command.Role is null or TipoRole.Suporte
                ? tenancyId >= 0
                : tenancyId > 0)
            .WithMessage("TenancyId deve ser zero para Suporte e maior que zero para os demais perfis.");
    }
}