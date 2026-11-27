using IdentityService.Domain.Common;
using MediatR;

namespace IdentityService.Application.Features.Auth.Commands.Delete;

public record DeleteCommandAuthUser(Guid Id) : IRequest<Result>;