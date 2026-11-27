using IdentityService.Infrastructure.Data;
using IdentityService.Integration.Tests.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityService.Integration.Tests.Commons;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Jwt:Key", "integration-test-signing-key-at-least-32-bytes");
        builder.ConfigureAppConfiguration((_, configuration) => 
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "integration-test-signing-key-at-least-32-bytes",

                ["RabbitMq:Host"] = "localhost",
                ["RabbitMq:UserName"] = "test-user",
                ["RabbitMq:Password"] = "test-password",

                ["SqlServer:Host"] = "fake-server",
                ["SqlServer:Database"] = "fake-db",
                ["SqlServer:UserName"] = "fake-user",
                ["SqlServer:Password"] = "fake-password"
            }));

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveAll(typeof(ApplicationDbContext));
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryDbForTesting"));

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            SeedData.Initialize(db);
        });
    }
}
