using IdentityService.Application.Helpers;
using IdentityService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers.ApiBase;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "AdminPolicy")]
public abstract class ApiBaseController : ControllerBase
{
    protected bool IsUserAtivo => User.GetUserStatus() == TipoUserAtivo.Ativo && User.GetTenancyStatus() == TipoTenancyAtivo.Ativo;
    protected long UserTenancyId => User.GetTenancyId();
    protected string UserName => User.Identity?.Name ?? string.Empty;
    protected TipoRole? UserRole => User.GetUserRole();

    protected bool IsSuporte => User.IsSuporte();
    protected bool IsSindico => User.IsSindico();
    protected bool IsPorteiro => User.IsPorteiro();
}