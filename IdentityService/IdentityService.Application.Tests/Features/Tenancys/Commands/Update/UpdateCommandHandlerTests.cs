using IdentityService.Application.Features.Tenancys.Commands.Update;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace IdentityService.Application.Tests.Features.Tenancys.Commands.Update;

public class UpdateCommandHandlerTests
{
    private readonly Mock<ITenancyRepository> _repoMock;
    private readonly Mock<IAuthUserRepository> _authUserRepoMock;
    private readonly Mock<IMensageriaService> _mensageriaMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<ILogger<UpdateCommandHandlerTenancy>> _loggerMock;
    private readonly UpdateCommandHandlerTenancy _handler;

    private readonly Tenancy _existente = new()
    {
        Id = 1,
        Ativo = TipoTenancyAtivo.Ativo,
        RazaoSocial = "Razão Social",
        Fantasia = "Fantasia",
        Cnpj = "44764428000186",
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

    public UpdateCommandHandlerTests()
    {
        _repoMock = new Mock<ITenancyRepository>();
        _authUserRepoMock = new Mock<IAuthUserRepository>();
        _mensageriaMock = new Mock<IMensageriaService>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _loggerMock = new Mock<ILogger<UpdateCommandHandlerTenancy>>();

        _handler = new UpdateCommandHandlerTenancy(_repoMock.Object, _authUserRepoMock.Object, _mensageriaMock.Object, _emailTemplateServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_TenancyExistenteEComandoValido_DeveAtualizarERetornarSucessoDto()
    {
        // Arrange
        UpdateCommandTenancy command = new()
        {
            Id = 5,
            Ativo = TipoTenancyAtivo.Ativo,
            RazaoSocial = "Razão Social Atualizada",
            Fantasia = "Fantasia Atualizada",
            Cnpj = "44764428000186",
            TipoDeCondominio = (TipoDeTenancy)1,
            Nome = "Responsável Atualizado",
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
            DataInclusao = DateTime.Now,
            DataAlteracao = DateTime.Now
        };

        // Act
        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_existente);
        Domain.Common.Result<DTOs.TenancyDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(command.RazaoSocial, resultado.Dados.RazaoSocial);
        Assert.Equal(command.Cnpj, resultado.Dados.Cnpj);

        _repoMock.Verify(repo => repo.UpdateAsync(
            It.Is<Tenancy>(i => i.Id == 1 && i.RazaoSocial == command.RazaoSocial),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_TenancyInexistente_DeveRetornarResultFailure()
    {
        string mensagemEsperada = "Tenancy não encontrada.";
        UpdateCommandTenancy command = new()
        {
            Id = 999,
            RazaoSocial = "Razão Social não existente",
            Fantasia = "Fantasia não existente",
            Cnpj = "44764428000100",
            TipoDeCondominio = (TipoDeTenancy)1,
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

        _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Tenancy)null!);
        Domain.Common.Result<DTOs.TenancyDto> resultado = await _handler.Handle(command, CancellationToken.None);

        Assert.False(resultado.Sucesso);
        Assert.Contains(mensagemEsperada, resultado.Mensagem);
        Assert.Null(resultado.Dados);

        _repoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Tenancy>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
