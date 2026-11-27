using IdentityService.Domain.Constants;
using IdentityService.Domain.Enums;
using System.Security.Claims;

namespace IdentityService.Application.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAuthenticated(this ClaimsPrincipal user) =>
        user.Identity?.IsAuthenticated == true;

    public static bool IsSuporte(this ClaimsPrincipal user) =>
        user.IsInRole(TipoRole.Suporte.ToString());

    public static bool IsSindico(this ClaimsPrincipal user) =>
        user.IsInRole(TipoRole.Sindico.ToString());

    public static bool IsPorteiro(this ClaimsPrincipal user) =>
        user.IsInRole(TipoRole.Porteiro.ToString());

    public static long GetTenancyId(this ClaimsPrincipal user) =>
        long.TryParse(user.FindFirst(AuthClaims.TenancyId)?.Value,
        out var id)
            ? id
            : 0;

    public static string GetUserName(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    public static TipoUserAtivo? GetUserStatus(this ClaimsPrincipal user) =>
        user.Claims.FirstOrDefault(c => c.Type == AuthClaims.StatusAtivo) is { } statusClaim
        && Enum.TryParse<TipoUserAtivo>(statusClaim.Value, out var result)
        ? result
        : null;

    public static TipoRole? GetUserRole(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Role) is { } roleClaim
        && Enum.TryParse<TipoRole>(roleClaim.Value, out var result)
        ? result
        : null;

    public static bool IsPrimeiroAcesso(this ClaimsPrincipal user) =>
        string.Equals(user.FindFirst(AuthClaims.PrimeiroAcesso)?.Value,
            AuthClaims.TrueValue,
            StringComparison.OrdinalIgnoreCase);

    public static bool IsUsuarioAtivo(this ClaimsPrincipal user) =>
        string.Equals(user.FindFirst(AuthClaims.StatusAtivo)?.Value,
            AuthClaims.StatusAtivoValue,
            StringComparison.OrdinalIgnoreCase);

    public static bool IsTenancyAtiva(this ClaimsPrincipal user) =>
        string.Equals(user.FindFirst(AuthClaims.TenancyAtiva)?.Value,
            AuthClaims.StatusAtivoValue,
            StringComparison.OrdinalIgnoreCase);

    public static bool IsTenancyAdmin(this ClaimsPrincipal user) =>
        user.IsSuporte() || user.IsSindico();

    public static TipoTenancyAtivo? GetTenancyStatus(this ClaimsPrincipal user) =>
        user.Claims.FirstOrDefault(c => c.Type == AuthClaims.TenancyAtiva) is { } statusClaim
        && Enum.TryParse<TipoTenancyAtivo>(statusClaim.Value, out var result)
        ? result
        : null;
}