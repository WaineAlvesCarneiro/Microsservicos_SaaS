using IdentityService.Application.DTOs;
using IdentityService.Domain.Common;
using MediatR;

namespace IdentityService.Application.Features.Tenancys.Queries.GetById;

public record GetByIdQueryTenancy(long Id) : IRequest<Result<TenancyDto>>;