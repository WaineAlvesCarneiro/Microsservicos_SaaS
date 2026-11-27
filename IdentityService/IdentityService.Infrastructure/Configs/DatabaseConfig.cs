using IdentityService.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Configs;

public static class DatabaseConfig
{
    private const int DefaultCommandTimeout = 60;
    private const int DefaultMaxRetryCount = 5;
    private static readonly TimeSpan DefaultMaxRetryDelay = TimeSpan.FromSeconds(30);

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {   
        if (environment.IsEnvironment("Testing"))
            return services;

        var connectionString = GetConnectionString(configuration);

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseConfig");
            LogEnvironment(logger, configuration);

            ConfigureSqlServer(options, connectionString, configuration);
        });

        return services;
    }

    public static string GetConnectionString(IConfiguration configuration)
    {
        var server   = configuration["SqlServer:Host"];
        var database = configuration["SqlServer:Database"];
        var userName = configuration["SqlServer:UserName"];
        var password = configuration["SqlServer:Password"];

        if (string.IsNullOrWhiteSpace(server) ||
            string.IsNullOrWhiteSpace(database) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Configure SqlServer:Host, Database, UserName e Password.");
        }

        return new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            UserID = userName,
            Password = password,
            Encrypt = false,
            TrustServerCertificate = true,
            MultipleActiveResultSets = true
        }.ConnectionString;
    }

    private static void LogEnvironment(ILogger logger, IConfiguration configuration)
    {
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Unknown";
        logger.LogDebug("Configurando conexão com banco para ambiente: {Environment}", environment);
    }

    private static void ConfigureSqlServer(DbContextOptionsBuilder options, string connectionString, IConfiguration configuration)
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("IdentityService.Infrastructure");
            sqlOptions.CommandTimeout(DefaultCommandTimeout);
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: DefaultMaxRetryCount,
                maxRetryDelay: DefaultMaxRetryDelay,
                errorNumbersToAdd: null);
        });

        var environment = configuration["ASPNETCORE_ENVIRONMENT"];

        if (string.Equals(
            environment,
            "Development",
            StringComparison.OrdinalIgnoreCase))
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    }
}
