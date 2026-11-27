using IdentityService.Domain.Constants;
using IdentityService.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.Infrastructure.Configs;

public static class SecurityConfig
{
    private const int MinJwtKeyLength = 32;

    private static readonly string[] RolesPermitidas =
    [
        TipoRole.Suporte.ToString(),
        TipoRole.Sindico.ToString(),
        TipoRole.Porteiro.ToString()
    ];

    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = GetJwtKey(configuration);

        services.AddAuthentication(options =>
        {
            ConfigureAuthenticationSchemes(options);
        })
        .AddJwtBearer(options =>
        {
            ConfigureJwtBearerOptions(options, configuration, key);
        });
    }

    private static byte[] GetJwtKey(IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("Jwt:Key não configurada.");

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

        if (keyBytes.Length < MinJwtKeyLength)
        {
            throw new InvalidOperationException($"Jwt:Key inválida. Tamanho mínimo: {MinJwtKeyLength} bytes.");
        }

        return keyBytes;
    }

    private static void ConfigureAuthenticationSchemes(AuthenticationOptions options)
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }

    private static void ConfigureJwtBearerOptions(JwtBearerOptions options, IConfiguration configuration, byte[] key)
    {
        options.SaveToken = false;
        options.TokenValidationParameters = BuildTokenValidationParameters(configuration, key);
        options.Events = BuildJwtBearerEvents();
    }

    private static TokenValidationParameters BuildTokenValidationParameters(IConfiguration configuration, byte[] key)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true
        };
    }

    private static JwtBearerEvents BuildJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                logger.LogWarning("Falha na autenticação: {Exception}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                logger.LogDebug("Token validado para usuário: {User}", context.Principal?.Identity?.Name ?? "Unknown");
                return Task.CompletedTask;
            }
        };
    }

    public static void AddAppAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
            {
                policy.RequireRole(RolesPermitidas);
                policy.RequireClaim(AuthClaims.PrimeiroAcesso, AuthClaims.FalseValue);
                policy.RequireClaim(AuthClaims.StatusAtivo, AuthClaims.StatusAtivoValue);
                policy.RequireClaim(AuthClaims.TenancyAtiva, AuthClaims.StatusAtivoValue);
                policy.RequireAssertion(context => context.User.Identity?.IsAuthenticated == true);
            });

            options.AddPolicy("PermitirTrocaSenha", policy =>
            {
                policy.RequireRole(RolesPermitidas);
                policy.RequireClaim(AuthClaims.PrimeiroAcesso, AuthClaims.TrueValue);
                policy.RequireClaim(AuthClaims.StatusAtivo, AuthClaims.StatusAtivoValue);
                policy.RequireClaim(AuthClaims.TenancyAtiva, AuthClaims.StatusAtivoValue);
            });

            options.AddPolicy("TenancyAdmin", policy =>
            {
                policy.RequireRole(TipoRole.Suporte.ToString());
            });
        });
    }
}
