using IdentityService.Domain.Enums;

namespace IdentityService.Application.Features.Auth.Commands.ValidatorBase;

public interface ICommandBaseAuthUser
{
    long TenancyId { get; set; }
    string UserName { get; set; }
    DateTime DataInclusao { get; set; }
    DateTime? DataAlteracao { get; set; }
}