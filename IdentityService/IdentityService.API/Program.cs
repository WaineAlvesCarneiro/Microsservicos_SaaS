using IdentityService.API.Configuration;
using IdentityService.Application.Options;
using IdentityService.Infrastructure.Configs;
using IdentityService.Infrastructure.Configurations;
using IdentityService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDockerSecretFiles();

builder.Services.Configure<OwnerCompanyOptions>(
    builder.Configuration.GetSection("OwnerCompany"));

builder.Host.AddAppLogging();
builder.Services.AddDatabase(
    builder.Configuration, builder.Environment);
builder.Services.AddMemoryCache();

builder.Services.AddMediatRAndValidators();
builder.Services.AddRepositories();

builder.Services.AddRabbitMQEmailTokenServices(
    builder.Configuration);

builder.Services.AddAppCorsPolicy(
    builder.Configuration);

builder.Services.AddJwtAuthentication(
    builder.Configuration);
builder.Services.AddAppAuthorizationPolicies();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddAppJsonOptions();

builder.Services.AddSwaggerAndSecurity();

var app = builder.Build();

app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Redirect("/swagger"));

if (!app.Environment.IsEnvironment("Testing"))
    await app.ApplyMigrationsWithRetryAsync();

app.UseAppMiddleware();
app.MapControllers();

app.Run();

public partial class Program { }