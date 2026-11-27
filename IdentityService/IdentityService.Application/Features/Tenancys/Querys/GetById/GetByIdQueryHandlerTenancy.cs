using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Tenancys.Queries.GetById;

public class GetByIdQueryHandlerTenancy(ITenancyRepository repository)
    : IRequestHandler<GetByIdQueryTenancy, Result<TenancyDto>>
{
    public async Task<Result<TenancyDto>> Handle(GetByIdQueryTenancy request, CancellationToken cancellationToken)
    {
        var dado = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dado is null) return Result<TenancyDto>.Failure("Tenancy não encontrada.");

        return Result<TenancyDto>.Success(dado.ToDto());
    }
}