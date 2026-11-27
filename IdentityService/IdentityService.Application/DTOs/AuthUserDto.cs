using IdentityService.Domain.Enums;

namespace IdentityService.Application.DTOs;

public record AuthUserDto
{
    public Guid Id { get; set; }
    public TipoUserAtivo Ativo { get; set; }
    public TipoTenancyAtivo TenancyAtiva { get; set; }
    public required long TenancyId { get; set; }
    public TenancyDto? TenancyDto { get; set; } = null!;
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public bool PrimeiroAcesso { get; set; } = true;
    public TipoRole Role { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }
}
