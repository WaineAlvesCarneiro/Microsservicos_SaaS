using IdentityService.Domain.Entities;
using MediatR;

namespace IdentityService.Application.Features.Auth;

public class AuthLoginQuery : IRequest<AuthUser>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}