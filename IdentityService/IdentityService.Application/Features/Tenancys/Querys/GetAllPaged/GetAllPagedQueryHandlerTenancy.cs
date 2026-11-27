using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Tenancys.Queries.GetAllPaged;

public class GetAllPagedQueryHandlerTenancy(ITenancyRepository repository)
    : IRequestHandler<GetAllPagedQueryTenancy, Result<PagedResult<TenancyDto>>>
{
    private readonly ITenancyRepository _repository = repository;

    public async Task<Result<PagedResult<TenancyDto>>> Handle(GetAllPagedQueryTenancy request, CancellationToken cancellationToken)
    {
        (IEnumerable<Tenancy> items, int totalCount) = await _repository.GetAllPagedAsync(
            page: request.ActualPage,
            pageSize: request.ActualPageSize,
            orderBy: request.ActualSortBy,
            direction: request.ActualDirection,
            razaoSocial: request.RazaoSocial,
            cnpj: request.Cnpj,
            cancellationToken: cancellationToken
        );

        var dtos = items.Select(dado => dado.ToDto()).ToList();

        PagedResult<TenancyDto> pagedResult = new PagedResult<TenancyDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = request.ActualPage,
            LinesPerPage = request.ActualPageSize
        };

        return Result<PagedResult<TenancyDto>>.Success(pagedResult);
    }
}
