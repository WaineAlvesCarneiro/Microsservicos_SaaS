using IdentityService.Application.DTOs;
using IdentityService.Domain.Common;
using MediatR;

namespace IdentityService.Application.Features.Tenancys.Queries.GetAllPaged;

public record GetAllPagedQueryTenancy(
    int Page = 1,
    int PageSize = 5,
    string? SortBy = "Id",
    string Direction = "ASC",
    string? RazaoSocial = null,
    string? Cnpj = null)
    : IRequest<Result<PagedResult<TenancyDto>>>
{
    public int ActualPage => Page < 1 ? 1 : Page;
    public int ActualPageSize => PageSize < 1 ? 5 : PageSize;
    public string ActualSortBy => !string.IsNullOrWhiteSpace(SortBy) ? SortBy : "Id";
    public string ActualDirection => Direction;
}