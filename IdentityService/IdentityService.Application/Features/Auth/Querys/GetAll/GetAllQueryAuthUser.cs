using IdentityService.Application.DTOs;
using IdentityService.Domain.Common;
using MediatR;

namespace IdentityService.Application.Features.Auth.Queries.GetAll;

public record GetAllQueryAuthUser(long TenancyId = 0) : IRequest<Result<IEnumerable<AuthUserDto>>>
{
    public long IdTenancy => TenancyId;
}