using IdentityService.Application.Mappings;
using IdentityService.Application.Options;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Interfaces;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityService.Infrastructure.Seed;

public sealed class DatabaseSeedService(
    ApplicationDbContext context,
    IConfiguration configuration,
    IOptions<OwnerCompanyOptions> ownerCompanyOptions,
    ILogger<DatabaseSeedService> logger)
    : IDatabaseSeedService
{
    private readonly OwnerCompanyOptions _ownerCompany = ownerCompanyOptions.Value;

    public async Task SeedAsync()
    {
        var tenancy = await EnsureSystemTenancyAsync();
        await EnsureAdminUserAsync(tenancy.Id);
    }

    private async Task<Tenancy> EnsureSystemTenancyAsync()
    {
        var tenancy = await context.Set<Tenancy>()
            .SingleOrDefaultAsync(x => x.TipoTenancy == TipoDeTenancy.Master);

        if (tenancy is not null) return tenancy;

        tenancy = CreateSystemTenancy();
        context.Set<Tenancy>().Add(tenancy);
        await context.SaveChangesAsync();

        logger.LogInformation("Tenancy Master criada com sucesso.");
        return tenancy;
    }

    private Tenancy CreateSystemTenancy()
    {
        return new Tenancy
        {
            Ativo = TipoTenancyAtivo.Ativo,
            RazaoSocial = _ownerCompany.RazaoSocial,
            Fantasia = _ownerCompany.Fantasia,
            Cnpj = _ownerCompany.Cnpj,
            TipoTenancy = TipoDeTenancy.Master,
            Nome = _ownerCompany.Nome,
            Celular = _ownerCompany.Celular,
            Telefone = null,
            Email = _ownerCompany.Email,
            Senha = null,
            Host = _ownerCompany.SmtpHost,
            Porta = _ownerCompany.SmtpPort,
            Cep = _ownerCompany.Cep,
            Uf = _ownerCompany.Uf,
            Cidade = _ownerCompany.Cidade,
            Endereco = _ownerCompany.Endereco,
            Bairro = _ownerCompany.Bairro,
            Complemento = null,
            DataInclusao = DateTime.UtcNow
        };
    }

    private async Task EnsureAdminUserAsync(long tenancyId)
    {
        var adminUserName = configuration["AdminSettings:UserName"];

        if (string.IsNullOrWhiteSpace(adminUserName))
            throw new InvalidOperationException("Configure AdminSettings:UserName.");

        var exists = await context.AuthUsers
            .AnyAsync(x => x.UserName == adminUserName);

        if (exists) return;

        var adminUser = configuration.ToEntityMigrateAndSeedDatabase();
        adminUser.TenancyId = tenancyId;

        context.AuthUsers.Add(adminUser);
        await context.SaveChangesAsync();

        logger.LogInformation("Usuário administrador criado com sucesso.");
    }
}