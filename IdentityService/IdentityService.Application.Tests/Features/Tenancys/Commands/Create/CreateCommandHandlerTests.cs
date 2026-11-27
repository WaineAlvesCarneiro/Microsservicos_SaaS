using IdentityService.Application.Features.Auth.Commands.Create;
using IdentityService.Application.Features.Tenancys.Commands.Create;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Interfaces;
using IdentityService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace IdentityService.Application.Tests.Features.Tenancys.Commands.Create;

public class CreateCommandHandlerTests
{
    private readonly Mock<ITenancyRepository> _repoMock;
    private readonly Mock<IMensageriaService> _mensageriaMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<ILogger<CreateCommandHandlerTenancy>> _loggerMock;
    private readonly CreateCommandHandlerTenancy _handler;

    public CreateCommandHandlerTests()
    {
        _repoMock = new Mock<ITenancyRepository>();
        _mensageriaMock = new Mock<IMensageriaService>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _loggerMock = new Mock<ILogger<CreateCommandHandlerTenancy>>();

        _handler = new CreateCommandHandlerTenancy(_repoMock.Object, _mensageriaMock.Object, _emailTemplateServiceMock.Object, _loggerMock.Object);

        _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<Tenancy>(), It.IsAny<CancellationToken>()))
            .Callback<Tenancy, CancellationToken>((tenancy, token)=> tenancy.Id = 101)
            .Returns(Task.CompletedTask);

        _repoMock.Setup(repo => repo.ExisteCnpjAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    [Fact]
    public async Task Handle_ComandoValido_DeveChamarCreateAsyncERetornarSucessoDto()
    {
        // Arrange
        long idGerado = 101;
        CreateCommandTenancy command = new()
        {
            RazaoSocial = "Razão Social Atualizada",
            Fantasia = "Fantasia Atualizada",
            Cnpj = "44764428000186",
            TipoDeCondominio = (TipoDeTenancy)1,
            Nome = "Responsável Atualizado",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "emailadmin@gmail.com",
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

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(idGerado, resultado.Dados.Id);
        Assert.Equal(command.RazaoSocial, resultado.Dados.RazaoSocial);

        // Verificação do Repositório (Corrigido com o CancellationToken)
        _repoMock.Verify(repo => repo.CreateAsync(
            It.Is<Tenancy>(i => i.RazaoSocial == command.RazaoSocial && i.Cnpj == command.Cnpj),
            It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}