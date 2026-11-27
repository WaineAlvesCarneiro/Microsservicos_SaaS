using IdentityService.Application.Features.Auth.Commands.Create;
using IdentityService.Domain.Enums;
using FluentValidation.TestHelper;

namespace IdentityService.Application.Tests.Features.Commands.Create;

public class CreateCommandValidatorTests
{
    private readonly CreateCommandValidatorAuthUser _validator;

    public CreateCommandValidatorTests()
    {
        _validator = new CreateCommandValidatorAuthUser();
    }

    private CreateCommandAuthUser GetValidCommand() => new()
    {
        TenancyId = 1,
        UserName = "Admin",
        Email = "email@gmail.com",
        Role = (TipoRole)1,
        DataInclusao = DateTime.Now
    };

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validator_UserName_Vazio_Ou_Nulo_Deve_Ter_Erro(string userNameInvalida)
    {
        // Arrange
        string messagemEsperada = "Usuário é obrigatório";
        CreateCommandAuthUser command = GetValidCommand();
        command.UserName = userNameInvalida;

        // Act
        TestValidationResult<CreateCommandAuthUser> resultado = _validator.TestValidate(command);

        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.UserName).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_UserName_Vazio_Deve_Ter_Erro()
    {
        // Arrange
        string messagemEsperada = "Usuário é obrigatório";
        CreateCommandAuthUser command = GetValidCommand();
        command.UserName = "";
        // Act
        TestValidationResult<CreateCommandAuthUser> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldHaveValidationErrorFor(c => c.UserName).WithErrorMessage(messagemEsperada);
    }

    [Fact]
    public void Validator_Comando_Valido_Nao_Deve_Ter_Erros()
    {
        // Arrange
        CreateCommandAuthUser command = GetValidCommand();
        // Act
        TestValidationResult<CreateCommandAuthUser> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_SuporteComTenancyZero_DevePassar()
    {
        var command = GetValidCommand();
        command.TenancyId = 0;
        command.Role = TipoRole.Suporte;

        var resultado = _validator.TestValidate(command);

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_TenancyIdNegativo_DeveFalhar()
    {
        var command = GetValidCommand();
        command.TenancyId = -1;

        var resultado = _validator.TestValidate(command);

        resultado.ShouldHaveValidationErrorFor(c => c.TenancyId);
    }

    [Theory]
    [InlineData(TipoRole.Sindico)]
    [InlineData(TipoRole.Porteiro)]
    public void Validator_PerfilDeTenancyComIdZero_DeveFalhar(TipoRole role)
    {
        var command = GetValidCommand();
        command.TenancyId = 0;
        command.Role = role;

        var resultado = _validator.TestValidate(command);

        resultado.ShouldHaveValidationErrorFor(c => c.TenancyId)
            .WithErrorMessage("TenancyId é obrigatório para perfis Síndico e Porteiro.");
    }
}
