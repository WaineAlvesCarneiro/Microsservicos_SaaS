using IdentityService.Domain.Enums;

namespace IdentityService.Domain.Entities;

public class AuthUser
{
    public Guid Id { get; set; }
    public TipoUserAtivo Ativo { get; set; }
    public TipoTenancyAtivo TenancyAtiva { get; set; }
    public required long TenancyId { get; set; }
    public virtual Tenancy? Tenancy { get; set; } = null!;
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public bool PrimeiroAcesso { get; set; } = true;
    public required string PasswordHash { get; set; }
    public TipoRole Role { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public bool VerificarSenha(string senhaBruta)
    {
        return BCrypt.Net.BCrypt.Verify(senhaBruta, this.PasswordHash);
    }
}
