using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Tenancys.Commands.Create;
using IdentityService.Application.Features.Tenancys.Commands.Update;
using IdentityService.Application.Helpers;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;

namespace IdentityService.Application.Mappings;

public static class TenancyMappingExtensions
{
    public static Tenancy ToEntity(this CreateCommandTenancy request)
    {
        return new Tenancy
        {
            RazaoSocial = request.RazaoSocial,
            Fantasia = request.Fantasia,
            Cnpj = request.Cnpj?.Replace(".", "").Replace("-", "").Replace("/", "") ?? "",
            TipoTenancy = request.TipoDeCondominio,
            Nome = request.Nome,
            Celular = request.Celular,
            Telefone = request.Telefone,
            Email = request.Email,
            Senha = !string.IsNullOrWhiteSpace(request.Senha) ? EncryptionHelper.Encrypt(request.Senha) : null,
            Host = request.Host,
            Porta = request.Porta,
            Cep = request.Cep,
            Uf = request.Uf,
            Cidade = request.Cidade,
            Endereco = request.Endereco,
            Bairro = request.Bairro,
            Complemento = request.Complemento,
            DataInclusao = DateTime.Now,
            Ativo = TipoTenancyAtivo.Ativo
        };
    }

    public static bool UpdateFromCommand(this Tenancy tenancy, UpdateCommandTenancy request)
    {
        bool statusMudouParaInativo = tenancy.Ativo == TipoTenancyAtivo.Ativo && request.Ativo != TipoTenancyAtivo.Ativo;

        if (!string.IsNullOrEmpty(request.Senha))
            tenancy.Senha = EncryptionHelper.Encrypt(request.Senha);

        tenancy.Ativo = request.Ativo;
        tenancy.RazaoSocial = request.RazaoSocial;
        tenancy.Fantasia = request.Fantasia;
        tenancy.Cnpj = request.Cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        tenancy.TipoTenancy = request.TipoDeCondominio;
        tenancy.Nome = request.Nome;
        tenancy.Celular = request.Celular;
        tenancy.Telefone = request.Telefone;
        tenancy.Email = request.Email;
        tenancy.Host = request.Host;
        tenancy.Porta = request.Porta;
        tenancy.Cep = request.Cep;
        tenancy.Uf = request.Uf;
        tenancy.Cidade = request.Cidade;
        tenancy.Endereco = request.Endereco;
        tenancy.Bairro = request.Bairro;
        tenancy.Complemento = request.Complemento;
        tenancy.DataAlteracao = DateTime.Now;

        return statusMudouParaInativo;
    }

    public static TenancyDto ToDto(this Tenancy d) => new()
    {
        Id = d.Id,
        Ativo = d.Ativo,
        RazaoSocial = d.RazaoSocial,
        Fantasia = d.Fantasia,
        Cnpj = d.Cnpj,
        TipoTenancy = d.TipoTenancy,
        Nome = d.Nome,
        Celular = d.Celular,
        Telefone = d.Telefone ?? "",
        Email = d.Email,
        Senha = null,
        Host = d.Host,
        Porta = d.Porta,
        Cep = d.Cep,
        Uf = d.Uf,
        Cidade = d.Cidade,
        Endereco = d.Endereco,
        Bairro = d.Bairro,
        Complemento = d.Complemento,
        DataInclusao = d.DataInclusao,
        DataAlteracao = d.DataAlteracao
    };
}