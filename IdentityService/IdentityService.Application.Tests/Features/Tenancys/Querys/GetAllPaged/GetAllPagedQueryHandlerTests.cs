using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Tenancys.Queries.GetAllPaged;
using IdentityService.Domain.Common;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Repositories;
using Moq;

namespace IdentityService.Application.Tests.Features.Tenancys.Querys.GetAllPaged;

public class GetAllPagedQueryHandlerTests
{
    private readonly Mock<ITenancyRepository> _repoMock;
    private readonly GetAllPagedQueryHandlerTenancy _handler;

    private readonly List<Tenancy> _pagina1 =
    [
        new Tenancy {
            Id = 1,
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
        },
        new Tenancy {
            Id = 2,
            RazaoSocial = "Razão Social 2",
            Fantasia = "Fantasia 2",
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
        },
    ];

    private const int Page = 1;
    private const int PageSize = 10;
    private const string? SortBy = "Id";
    private const string? Direction = "ASC";
    private string? _RazaoSocial = "Razão Social 2";
    private string? _Cnpj = "44764428000186";

    private const int TOTAL_REGISTROS = 2;

    public GetAllPagedQueryHandlerTests()
    {
        _repoMock = new Mock<ITenancyRepository>();
        _handler = new GetAllPagedQueryHandlerTenancy(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarPagedResultComDadosCorretos()
    {
        // Arrange
        string expectedFirstRazaoSocial = "Razão Social";
        GetAllPagedQueryTenancy query = new(
            Page: Page,
            PageSize: PageSize,
            SortBy: SortBy,
            Direction: Direction!,
            _RazaoSocial,
            _Cnpj
        );

        _repoMock.Setup(repo => repo.GetAllPagedAsync(
            Page,
            PageSize,
            SortBy,
            Direction,
            _RazaoSocial,
            _Cnpj,
            It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((_pagina1, TOTAL_REGISTROS));

        // Act
        Result<PagedResult<TenancyDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        PagedResult<TenancyDto> pagedResult = resultado.Dados;

        Assert.Equal(TOTAL_REGISTROS, pagedResult.TotalCount);
        Assert.Equal(Page, pagedResult.PageIndex);
        Assert.Equal(PageSize, pagedResult.PageSize);
        Assert.Equal(_pagina1.Count, pagedResult.Items.Count());
        Assert.Equal(expectedFirstRazaoSocial, pagedResult.Items.First().RazaoSocial);
        Assert.IsType<PagedResult<TenancyDto>>(pagedResult);

        _repoMock.Verify(repo => repo.GetAllPagedAsync(
            Page,
            PageSize,
            SortBy,
            Direction,
            _RazaoSocial,
            _Cnpj,
            It.IsAny<CancellationToken>()
            ), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoRetornaListaVazia_DeveRetornarPagedResultVazio()
    {
        const int totalZero = 0;
        GetAllPagedQueryTenancy query = new();
        _repoMock.Setup(repo => repo.GetAllPagedAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Tenancy>(), totalZero));
        Result<PagedResult<TenancyDto>> resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(totalZero, resultado.Dados.TotalCount);
        Assert.Empty(resultado.Dados.Items);
    }
}
