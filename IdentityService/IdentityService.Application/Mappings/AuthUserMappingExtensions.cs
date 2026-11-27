using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Auth.Commands.Create;
using IdentityService.Application.Features.Auth.Commands.Update;
using IdentityService.Application.Helpers;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Mappings;

public static class AuthUserMappingExtensions
{
    public static AuthUser ToEntityMigrateAndSeedDatabase(this IConfiguration configuration)
    {
        var userName = configuration["AdminSettings:UserName"];
        var email = configuration["AdminSettings:Email"];
        var password = configuration["AdminSettings:Password"];

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Configure AdminSettings:UserName, AdminSettings:Email e AdminSettings:Password para criar o usuário inicial.");

        return new AuthUser
        {
            TenancyId = 1,
            Ativo = TipoUserAtivo.Ativo,
            TenancyAtiva = TipoTenancyAtivo.Ativo,
            UserName = userName,
            Email = email,
            PrimeiroAcesso = true,
            PasswordHash = PasswordHasher.HashPassword(password),
            Role = TipoRole.Suporte,
            DataInclusao = DateTime.Now
        };
    }

    public static AuthUser ToEntity(this CreateCommandAuthUser request, string senhaTemporaria)
    {
        return new AuthUser
        {
            TenancyId = request.TenancyId,
            UserName = request.UserName,
            Email = request.Email,
            PrimeiroAcesso = true,
            PasswordHash = PasswordHasher.HashPassword(senhaTemporaria),
            Role = request.Role,
            DataInclusao = DateTime.Now,
            Ativo = TipoUserAtivo.Ativo,
            TenancyAtiva = TipoTenancyAtivo.Ativo
        };
    }

    public static void UpdateFromCommand(this AuthUser entidade, UpdateCommandAuthUser request)
    {
        entidade.TenancyId = request.TenancyId;
        entidade.UserName = request.UserName;
        entidade.Email = request.Email;
        entidade.Role = (TipoRole)request.Role!;
        entidade.Ativo = request.Ativo;
        entidade.TenancyAtiva = request.TenancyAtiva;
        entidade.DataAlteracao = DateTime.Now;
    }

    public static AuthUserDto ToDto(this AuthUser dado) => new()
    {
        Id = dado.Id,
        Ativo = dado.Ativo,
        TenancyAtiva = dado.TenancyAtiva,
        TenancyId = dado.TenancyId,
        UserName = dado.UserName,
        Email = dado.Email,
        PrimeiroAcesso = dado.PrimeiroAcesso,
        Role = dado.Role,
        DataInclusao = dado.DataInclusao,
        DataAlteracao = dado.DataAlteracao,
        TenancyDto = dado.Tenancy?.ToDto()
    };
}