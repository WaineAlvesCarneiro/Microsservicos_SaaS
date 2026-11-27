using IdentityService.Application.Features.Tenancys.Commands.ValidatorBase;
using IdentityService.Domain.Repositories;

namespace IdentityService.Application.Features.Tenancys.Commands.Create;

public class CreateCommandValidatorTenancy : CommandValidatorBaseTenancy<CreateCommandTenancy>
{
    public CreateCommandValidatorTenancy(ITenancyRepository repository)
    {
        ConfigureCommonRules(repository);
    }
}