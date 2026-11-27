using IdentityService.Application.Features.Tenancys.Commands.Delete;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Repositories;
using Moq;

namespace IdentityService.Application.Tests.Features.Tenancys.Commands.Delete;

public class DeleteCommandHandlerTests
{
    private readonly Mock<ITenancyRepository> _repoMock;
    private readonly Mock<IAuthUserRepository> _authUserRepoMock;
    private readonly DeleteCommandHandlerTenancy _handler;

    private const long UserTenancyId = 1;
    private const int ID_EXISTENTE = 10;
    private const int ID_NAO_EXISTENTE = 999;
    private readonly Tenancy _existente;

    public DeleteCommandHandlerTests()
    {
        _repoMock = new Mock<ITenancyRepository>();
        _authUserRepoMock = new Mock<IAuthUserRepository>();

        _existente = new Tenancy {
            Id = ID_EXISTENTE,
            RazaoSocial = "Razão Social Atualizada",
            Fantasia = "Fantasia Atualizada",
            Cnpj = "01.111.222/0001-02",
            TipoTenancy = (TipoDeTenancy)1,
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
            DataInclusao = DateTime.Now
        };
        _handler = new DeleteCommandHandlerTenancy(_repoMock.Object, _authUserRepoMock.Object);
    }

    //[Fact]
    //public async Task Handle_TenancyExisteESemImoveis_DeveDeletarERetornarSucesso()
    //{
    //    // Arrange
    //    string mensagemSucesso = "Tenancy deletada com sucesso.";
    //    DeleteCommandTenancy command = new(ID_EXISTENTE);
    //    _imovelRepoMock.Setup(repo => repo.ExisteImovelVinculadoNaTenancyAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
    //    _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(_existente);

    //    // Act
    //    Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    Assert.True(resultado.Sucesso);
    //    Assert.Equal(mensagemSucesso, resultado.Mensagem);

    //    _repoMock.Verify(repo => repo.DeleteAsync(
    //        It.Is<Tenancy>(i => i.Id == ID_EXISTENTE),
    //        It.IsAny<CancellationToken>()
    //    ), Times.Once);
    //}

    //[Fact]
    //public async Task Handle_TenancyComImoveisVinculados_DeveRetornarFalhaENaoDeletar()
    //{
    //    // Arrange
    //    string mensagemFalha = "Não é possível excluir a tenancy, pois tem imóvel vinculado.";
    //    DeleteCommandTenancy command = new(ID_EXISTENTE);
    //    _imovelRepoMock.Setup(repo => repo.ExisteImovelVinculadoNaTenancyAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

    //    // Act
    //    Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    Assert.False(resultado.Sucesso);
    //    Assert.Equal(mensagemFalha, resultado.Mensagem);

    //    _repoMock.Verify(repo => repo.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    //    _repoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Tenancy>(), It.IsAny<CancellationToken>()), Times.Never);
    //}

    //[Fact]
    //public async Task Handle_TenancyInexistenteESemImoveis_DeveRetornarFalha()
    //{
    //    // Arrange
    //    string mensagemFalha = "Tenancy não encontrada.";
    //    DeleteCommandTenancy command = new(ID_NAO_EXISTENTE);
    //    _imovelRepoMock.Setup(repo => repo.ExisteImovelVinculadoNaTenancyAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);
    //    _repoMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Tenancy)null!);

    //    // Act
    //    Domain.Common.Result resultado = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    Assert.False(resultado.Sucesso);
    //    Assert.Equal(mensagemFalha, resultado.Mensagem);

    //    _repoMock.Verify(repo => repo.DeleteAsync(It.IsAny<Tenancy>(), It.IsAny<CancellationToken>()), Times.Never);
    //}
}