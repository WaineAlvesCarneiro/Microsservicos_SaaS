using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Auth.Queries.GetAllPaged;

public class GetAllPagedQueryHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<GetAllPagedQueryAuthUser, Result<PagedResult<AuthUserDto>>>
{
    private readonly IAuthUserRepository _repository = repository;

    public async Task<Result<PagedResult<AuthUserDto>>> Handle(GetAllPagedQueryAuthUser request, CancellationToken cancellationToken)
    {
        (IEnumerable<AuthUser> items, int totalCount) = await _repository.GetAllPagedAsync(
            page: request.ActualPage,
            pageSize: request.ActualPageSize,
            orderBy: request.ActualSortBy,
            direction: request.ActualDirection,
            TenancyId: request.TenancyId,
            userName: request.UserName,
            cancellationToken);

        var dtos = items.Select(dado => dado.ToDto()).ToList();

        PagedResult<AuthUserDto> pagedResult = new PagedResult<AuthUserDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.ActualPage,
            LinesPerPage = request.ActualPageSize
        };

        return Result<PagedResult<AuthUserDto>>.Success(pagedResult);
    }
}
