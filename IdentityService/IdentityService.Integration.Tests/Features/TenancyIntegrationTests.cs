using IdentityService.Application.DTOs;
using IdentityService.Domain.Enums;
using IdentityService.Integration.Tests.Commons;
using System.Net;
using System.Net.Http.Json;

namespace IdentityService.Integration.Tests.Features.Auth.Tenancy
{
    public class TenancyIntegrationTests : BaseIntegrationTest
    {
        public TenancyIntegrationTests(CustomWebApplicationFactory<Program> factory) : base(factory) { }
        const string BaseUrl = "/Tenancy";

        [Fact]
        public async Task Post_Sem_Token_Deve_Retornar_Unauthorized()
        {
            ClearAuthHeader();
            var novoDto = new TenancyDto {
                RazaoSocial = "Razão Social",
                Fantasia = "Fantasia",
                Cnpj = "01.111.222/0001-02",
                TipoTenancy = (TipoDeTenancy)1,
                Nome = "Responsável",
                Celular = "(11) 99999-9999",
                Telefone = "(11) 3333-3333",
                Email = "email@gmail.com",
                Senha = "SenhaForte123!",
                Host = "smtp.exemplo.com",
                Porta = 587,
                Cep = "01234-567",
                Uf = "SP",
                Cidade = "São Paulo",
                Endereco = "Rua Exemplo, 123",
                Bairro = "Pq Amazônia",
                Complemento = "Complemento",
                DataInclusao = DateTime.Now
            };
            var response = await _client.PostAsJsonAsync(BaseUrl, novoDto, cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
