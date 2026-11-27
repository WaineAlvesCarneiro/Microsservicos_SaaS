using IdentityService.Domain.Interfaces;
using IdentityService.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedDatabaseAsync(
        this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        var databaseSeedService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedService>();
        await databaseSeedService.SeedAsync();
    }
}