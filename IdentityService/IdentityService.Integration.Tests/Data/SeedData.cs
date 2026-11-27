using IdentityService.Application.Helpers;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Infrastructure.Data;

namespace IdentityService.Integration.Tests.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        const string adminUsername = "Admin";
        const string adminPassword = "Test-only-password-123";

        if (context.AuthUsers.Any(user => user.UserName == adminUsername))
            return;

        context.AuthUsers.Add(new AuthUser
        {
            UserName = adminUsername,
            Email = "admin@example.test",
            Ativo = TipoUserAtivo.Ativo,
            TenancyAtiva = TipoTenancyAtivo.Ativo,
            TenancyId = 0,
            PrimeiroAcesso = false,
            PasswordHash = PasswordHasher.HashPassword(adminPassword),
            Role = TipoRole.Suporte,
            DataInclusao = DateTime.Now
        });

        context.SaveChanges();
    }
}
