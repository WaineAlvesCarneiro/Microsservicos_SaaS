namespace IdentityService.Application.Options;

public sealed class OwnerCompanyOptions
{
    public string RazaoSocial { get; init; } = string.Empty;
    public string Fantasia { get; init; } = string.Empty;
    public string Cnpj { get; init; } = string.Empty;

    public string Nome { get; init; } = string.Empty;

    public string Celular { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string SmtpHost { get; init; } = string.Empty;

    public int SmtpPort { get; init; }

    public string Cep { get; init; } = string.Empty;

    public string Uf { get; init; } = string.Empty;

    public string Cidade { get; init; } = string.Empty;

    public string Endereco { get; init; } = string.Empty;

    public string Bairro { get; init; } = string.Empty;
}