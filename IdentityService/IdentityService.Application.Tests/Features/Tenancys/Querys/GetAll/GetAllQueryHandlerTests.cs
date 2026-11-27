using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Tenancys.Queries.GetAll;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Repositories;
using Moq;

namespace IdentityService.Application.Tests.Features.Tenancys.Querys.GetAll;

public class GetAllQueryHandlerTests
{
    private readonly Mock<ITenancyRepository> _repoMock;
    private readonly GetAllQueryHandlerTenancy _handler;

    private readonly List<Tenancy> _ficticios =
    [
        new Tenancy {
            Id = 1,
            RazaoSocial = "Razão Social",
            Fantasia = "Fantasia",
            Cnpj = "44764428000186",
            TipoTenancy = (TipoDeTenancy)1,
            Nome = "Responsável",
            Celular = "11999999999",
            Telefone = "1133333333",
            Email = "email@gmail.com",
            Senha = "SenhaForte123!",
            Host = "smtp.exemplo.com",
            Porta = 587,
            Cep = "01234567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Bairro = "Pq Amazônia",
            Complemento = "Complemento",
            DataInclusao = DateTime.Now
        }
    ];   

    public GetAllQueryHandlerTests()
    {
        _repoMock = new Mock<ITenancyRepository>();
        _handler = new GetAllQueryHandlerTenancy(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeTenancysMapeadaParaDto()
    {
        // Arrange
        long idprimeiroDto = 1;
        string razaoSocialPrimeiroDto = "Razão Social";
        GetAllQueryTenancy query = new();
        _repoMock.Setup(repo => repo.GetAllAsync(It.IsAny<long>(),It.IsAny<CancellationToken>())).ReturnsAsync(_ficticios);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        List<TenancyDto> dtos = resultado.Dados.ToList();

        Assert.Equal(_ficticios.Count, dtos.Count);

        TenancyDto primeiroDto = dtos.First();

        Assert.IsType<TenancyDto>(primeiroDto);
        Assert.Equal(idprimeiroDto, primeiroDto.Id);
        Assert.Equal(razaoSocialPrimeiroDto, primeiroDto.RazaoSocial);

        _repoMock.Verify(repo => repo.GetAllAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoNaoHaTenancys_DeveRetornarListaVazia()
    {
        // Arrange
        GetAllQueryTenancy query = new();
        _repoMock.Setup(repo => repo.GetAllAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tenancy>());

        // Act
        Domain.Common.Result<IEnumerable<TenancyDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Empty(resultado.Dados);
    }
}
