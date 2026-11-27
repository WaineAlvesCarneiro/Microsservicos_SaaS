using IdentityService.Domain.Enums;

namespace IdentityService.Application.Features.Tenancys.Commands.ValidatorBase;

public interface ICommandBaseTenancy
{
    long Id { get; set; }
    TipoTenancyAtivo Ativo { get; set; }
    string RazaoSocial { get; set; }
    string Fantasia { get; set; }
    string Cnpj { get; set; }
    TipoDeTenancy TipoDeCondominio { get; set; }
    string Nome { get; set; }
    string Celular { get; set; }
    string? Telefone { get; set; }
    string Email { get; set; }
    string? Senha { get; set; }
    string Host { get; set; }
    int Porta { get; set; }
    string Cep { get; set; }
    string Uf { get; set; }
    string Cidade { get; set; }
    string Endereco { get; set; }
    string Bairro { get; set; }
    string? Complemento { get; set; }
    DateTime DataInclusao { get; set; }
    DateTime? DataAlteracao { get; set; }
}