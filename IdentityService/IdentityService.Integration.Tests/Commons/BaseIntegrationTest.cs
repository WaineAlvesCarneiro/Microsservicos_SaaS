using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text.Json;

namespace IdentityService.Integration.Tests.Commons;

public class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly CustomWebApplicationFactory<Program> _factory;

    public BaseIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    protected async Task AddAdminAuthHeaderAsync()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var adminExistente = db.AuthUsers.FirstOrDefault(u => u.UserName == "admin");
            if (adminExistente == null)
            {
                await SeedAuthUserAsync((TipoUserAtivo)1, (TipoTenancyAtivo)1, 1, "admin", "admin@admin.com", false, "12345", 1, DateTime.Now);
            }
        }

        var loginRequest = new AuthLoginRequest("admin", "12345");
        var response = await _client.PostAsJsonAsync("/Auth/login", loginRequest);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = json.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected void ClearAuthHeader()
    {
        _client.DefaultRequestHeaders.Authorization = null;
    }

    protected async Task<Tenancy> SeedTenancyAsync(string razaoSocial,
            string fantasia,
            string cnpj,
            int tipoDeTenancy,
            string Nome,
            string celular,
            string telefone,
            string email,
            string? senha,
            string host,
            int porta,
            string cep,
            string uf,
            string cidade,
            string endereco,
            string bairro,
            string complemento,
            DateTime dataInclusao)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var dado = new Tenancy
            {
                RazaoSocial = razaoSocial,
                Fantasia = fantasia,
                Cnpj = cnpj,
                TipoTenancy = (TipoDeTenancy)tipoDeTenancy,
                Nome = Nome,
                Celular = celular,
                Telefone = telefone,
                Email = email,
                Senha = senha,
                Host = host,
                Porta = porta,
                Cep = cep,
                Uf = uf,
                Cidade = cidade,
                Endereco = endereco,
                Bairro = bairro,
                Complemento = complemento,
                DataInclusao = dataInclusao
            };

            db.Tenancys.Add(dado);
            await db.SaveChangesAsync();
            return dado;
        }
    }

    protected async Task<AuthUser> SeedAuthUserAsync(
        TipoUserAtivo ativo,
        TipoTenancyAtivo tenancyAtiva,
        long tenancyId,
        string userName,
        string email,
        bool primeiroAcesso,
        string passwordHash,
        int role,
        DateTime dataInclusao)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var dado = new AuthUser
            {
                Ativo = ativo,
                TenancyAtiva = tenancyAtiva,
                TenancyId = tenancyId,
                UserName = userName,
                Email = email,
                PrimeiroAcesso = primeiroAcesso,
                PasswordHash = passwordHash,
                Role = (TipoRole)role,
                DataInclusao = dataInclusao
            };

            db.AuthUsers.Add(dado);
            await db.SaveChangesAsync();
            return dado;
        }
    }
}