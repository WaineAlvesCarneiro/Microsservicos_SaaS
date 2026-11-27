using IdentityService.Application.Features.Tenancys.Commands.ValidatorBase;
using IdentityService.Domain.Repositories;

namespace IdentityService.Application.Features.Tenancys.Commands.Update;

public class UpdateCommandValidatorTenancy : CommandValidatorBaseTenancy<UpdateCommandTenancy>
{
    public UpdateCommandValidatorTenancy(ITenancyRepository repository)
    {
        ConfigureCommonRules(repository);
    }
}