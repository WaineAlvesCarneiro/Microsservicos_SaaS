using IdentityService.Domain.Interfaces;
using IdentityService.Infrastructure.Messaging;
using IdentityService.Infrastructure.Messaging.Configurations;
using IdentityService.Infrastructure.Services;
using IdentityService.Infrastructure.ServicesJWT;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Configs;

public static class InfrastructureConfig
{
    public static IServiceCollection AddRabbitMQEmailTokenServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqSettings>()
            .Bind(configuration.GetSection("RabbitMq"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IMensageriaService, RabbitMQService>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
        services.AddSingleton<TokenService>();

        return services;
    }
}
