using IdentityService.Domain.Enums;

namespace IdentityService.Domain.Entities;

public class Tenancy
{
    public long Id { get; set; }
    public TipoTenancyAtivo Ativo { get; set; }
    public required string RazaoSocial { get; set; }
    public required string Fantasia { get; set; }
    public required string Cnpj { get; set; }
    public TipoDeTenancy TipoTenancy { get; set; }
    public required string Nome { get; set; }
    public required string Celular { get; set; }
    public string? Telefone { get; set; }
    public required string Email { get; set; }
    public string? Senha { get; set; }
    public required string Host { get; set; }
    public int Porta { get; set; }
    public required string Cep { get; set; }
    public required string Uf { get; set; }
    public required string Cidade { get; set; }
    public required string Endereco { get; set; }
    public required string Bairro { get; set; }
    public string? Complemento { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime? DataAlteracao { get; set; }

    public ICollection<AuthUser> AuthUsers { get; set; } = [];
}