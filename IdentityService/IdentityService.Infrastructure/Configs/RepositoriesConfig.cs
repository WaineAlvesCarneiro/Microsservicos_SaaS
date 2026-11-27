using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Repositories;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityService.Infrastructure.Configs;

public static class RepositoriesConfig
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseSeedService, DatabaseSeedService>();
        services.TryAddScoped<IAuthUserRepository, AuthUserRepository>();
        services.TryAddScoped<ITenancyRepository, TenancyRepository>();
        return services;
    }
}
