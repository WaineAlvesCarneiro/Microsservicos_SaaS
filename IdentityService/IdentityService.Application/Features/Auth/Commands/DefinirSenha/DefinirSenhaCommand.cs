using IdentityService.Application.DTOs;
using IdentityService.Domain.Common;
using MediatR;

namespace IdentityService.Application.Features.Auth.Commands.DefinirSenha;

public class DefinirSenhaCommand : IRequest<Result<AuthUserDto>>
{
    public string UserName { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
}
