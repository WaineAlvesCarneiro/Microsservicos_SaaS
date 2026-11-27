using IdentityService.Application.Features.Tenancys.Commands.Create;
using IdentityService.Domain.Enums;
using IdentityService.Domain.Repositories;
using FluentValidation.TestHelper;
using Moq;
namespace IdentityService.Application.Tests.Features.Tenancys.Commands.Create;

public class CreateCommandValidatorTests
{
    private readonly CreateCommandValidatorTenancy _validator;
    private readonly Mock<ITenancyRepository> _repositoryMock;

    public CreateCommandValidatorTests()
    {
        _repositoryMock = new Mock<ITenancyRepository>();

        _repositoryMock.Setup(repo => repo
        .ExisteCnpjAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false);
        _validator = new CreateCommandValidatorTenancy(_repositoryMock.Object);
    }

    private CreateCommandTenancy GetValidCommand() => new()
    {
        RazaoSocial = "Razão Social Atualizada",
        Fantasia = "Fantasia Atualizada",
        Cnpj = "44764428000186",
        TipoDeCondominio = (TipoDeTenancy)1,
        Nome = "Responsável Atualizado",
        Celular = "11999999999",
        Telefone = "1133333333",
        Email = "email@gmail.com",
        Senha = "SenhaForte123!",
        Host = "smtp.exemplo.com",
        Porta = 587,
        Cep = "74843140",
        Uf = "SP",
        Cidade = "São Paulo",
        Endereco = "Rua Exemplo, 123",
        Bairro = "Pq Amazônia",
        Complemento = "Complemento",
        DataInclusao = DateTime.Now
    };

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validator_RazaoSocialVazioOuNulo_DeveTerErro(string razaoSocialInvalida)
    {
        // Arrange
        string messagemEsperada = "Razão Social é obrigatória";
        CreateCommandTenancy command = GetValidCommand();
        command.RazaoSocial = razaoSocialInvalida;

        // Act
        var resultado = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.RazaoSocial).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public async Task Validator_Cnpj_Vazio_DeveTerErro()
    {
        // Arrange
        string messagemEsperada = "Cnpj é obrigatório";
        CreateCommandTenancy command = GetValidCommand();
        command.Cnpj = "";

        // Act
        var resultado = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.Cnpj).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public async Task Validator_ComandoValido_NaoDeveTerErros()
    {
        // Arrange
        CreateCommandTenancy command = GetValidCommand();

        // Act
        var resultado = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validator_CnpjDuplicado_DeveTerErro()
    {
        // Arrange
        var command = GetValidCommand();
        _repositoryMock.Setup(repo => repo.ExisteCnpjAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true);

        // Act
        var resultado = await _validator.TestValidateAsync(command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.Cnpj)
                 .WithErrorMessage("Este CNPJ já está cadastrado para outra tenancy.");
    }
}