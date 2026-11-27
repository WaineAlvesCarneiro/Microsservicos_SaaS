using IdentityService.Application.DTOs;
using IdentityService.Domain.Common;
using MediatR;

namespace IdentityService.Application.Features.Auth.Queries.GetById;

public record GetByIdQueryAuthUser(Guid Id) : IRequest<Result<AuthUserDto>>;