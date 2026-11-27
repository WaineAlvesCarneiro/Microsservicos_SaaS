using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Tenancys.Queries.GetById;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Repositories;
using Moq;

namespace IdentityService.Application.Tests.Features.Tenancys.Querys.GetById;

public class GetByIdQueryHandlerTests
{
    private readonly Mock<ITenancyRepository> _repoMock;
    private readonly GetByIdQueryHandlerTenancy _handler;

    private const int ID_EXISTENTE = 7;
    private const int ID_NAO_EXISTENTE = 99;

    private readonly Tenancy _existente = new()
    {
        Id = ID_EXISTENTE,
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

    public GetByIdQueryHandlerTests()
    {
        _repoMock = new Mock<ITenancyRepository>();
        _handler = new GetByIdQueryHandlerTenancy(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_TenancyExistente_DeveRetornarSucessoComTenancyDto()
    {
        var query = new GetByIdQueryTenancy(ID_EXISTENTE);
        _repoMock.Setup(repo => repo.GetByIdAsync(ID_EXISTENTE, It.IsAny<CancellationToken>())).ReturnsAsync(_existente);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);

        var dto = resultado.Dados;

        Assert.IsType<TenancyDto>(dto);
        Assert.Equal(ID_EXISTENTE, dto.Id);
        Assert.Equal(_existente.RazaoSocial, dto.RazaoSocial);

        _repoMock.Verify(repo => repo.GetByIdAsync(ID_EXISTENTE, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TenancyInexistente_DeveRetornarFailure()
    {
        var query = new GetByIdQueryTenancy(ID_NAO_EXISTENTE);
        _repoMock.Setup(repo => repo.GetByIdAsync(ID_NAO_EXISTENTE, It.IsAny<CancellationToken>())).ReturnsAsync((Tenancy)null!);
        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Equal("Tenancy não encontrada.", resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _repoMock.Verify(repo => repo.GetByIdAsync(ID_NAO_EXISTENTE, It.IsAny<CancellationToken>()), Times.Once);
    }
}