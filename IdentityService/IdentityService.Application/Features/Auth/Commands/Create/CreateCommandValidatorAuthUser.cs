using IdentityService.Application.Features.Auth.Commands.ValidatorBase;
using IdentityService.Domain.Enums;
using FluentValidation;

namespace IdentityService.Application.Features.Auth.Commands.Create;

public class CreateCommandValidatorAuthUser : CommandValidatorBaseAuthUser<CreateCommandAuthUser>
{
    public CreateCommandValidatorAuthUser()
    {
        ConfigureCommonRules();

        RuleFor(p => p.Role)
            .Must(x => Enum.IsDefined(typeof(TipoRole), x))
            .WithMessage("Tipo de Perfil inválido. Selecione uma opção válida.");

        RuleFor(p => p.TenancyId)
            .Cascade(CascadeMode.Stop)
            .Must((command, TenancyId) =>
            {
                if (command.Role == TipoRole.Sindico || command.Role == TipoRole.Porteiro)
                {
                    return TenancyId > 0;
                }
                return true;
            })
            .WithMessage("TenancyId é obrigatório para perfis Síndico e Porteiro.");
    }
}