using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Tenancys.Queries.GetAll;

public class GetAllQueryHandlerTenancy(ITenancyRepository repository)
    : IRequestHandler<GetAllQueryTenancy, Result<IEnumerable<TenancyDto>>>
{
    public async Task<Result<IEnumerable<TenancyDto>>> Handle(GetAllQueryTenancy request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync(
            tenancyId: request.IdTenancy, cancellationToken);

        var dtos = dados.Select(dado => dado.ToDto()).ToList();

        return Result<IEnumerable<TenancyDto>>.Success(dtos);
    }
}
