using IdentityService.Application.DTOs;
using IdentityService.Domain.Common;
using MediatR;

namespace IdentityService.Application.Features.Tenancys.Queries.GetAll;

public record GetAllQueryTenancy(long TenancyId = 0) : IRequest<Result<IEnumerable<TenancyDto>>>
{
    public long IdTenancy => TenancyId;
}