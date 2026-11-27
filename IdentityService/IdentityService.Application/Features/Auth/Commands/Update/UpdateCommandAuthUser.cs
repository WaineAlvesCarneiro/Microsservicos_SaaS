using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Auth.Commands.ValidatorBase;
using IdentityService.Domain.Common;
using IdentityService.Domain.Enums;
using MediatR;

namespace IdentityService.Application.Features.Auth.Commands.Update;

public class UpdateCommandAuthUser : IRequest<Result<AuthUserDto>>, ICommandBaseAuthUser
{
    public Guid Id { get; set; }
    public TipoUserAtivo Ativo { get; set; }
    public TipoTenancyAtivo TenancyAtiva { get; set; }
    public required long TenancyId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public TipoRole? Role { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
}