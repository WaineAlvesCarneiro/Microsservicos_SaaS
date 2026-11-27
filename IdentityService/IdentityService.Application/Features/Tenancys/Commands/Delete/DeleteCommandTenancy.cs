using IdentityService.Domain.Common;
using MediatR;

namespace IdentityService.Application.Features.Tenancys.Commands.Delete;

public record DeleteCommandTenancy(long Id) : IRequest<Result>;