using IdentityService.Application.Features.Auth.Commands.Update;
using IdentityService.Domain.Enums;
using FluentValidation.TestHelper;

namespace IdentityService.Application.Tests.Features.Auth.Commands.Update;

public class UpdateCommandValidatorTests
{
    private readonly UpdateCommandValidatorAuthUser _validator = new();

    private UpdateCommandAuthUser GetValidCommand() => new()
    {
        Id = Guid.Parse("85D257AB-F0FD-F011-8550-A5241967915B"),
        TenancyId = 1,
        UserName = "Admin",
        Email = "email@gmail.com",
        Role = (TipoRole)1,
        DataInclusao = DateTime.Now
    };

    [Fact]
    public void Validator_ComandoValido_DevePassarSemErros()
    {
        // Arrange
        UpdateCommandAuthUser command = GetValidCommand();
        // Act
        TestValidationResult<UpdateCommandAuthUser> resultado = _validator.TestValidate(command);
        // Assert
        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_UserNameMuitoCurto_DeveFalharComMensagemCorreta()
    {
        string mensagemEsperada = "Usuário é obrigatório";
        UpdateCommandAuthUser command = GetValidCommand();
        command.UserName = "";
        TestValidationResult<UpdateCommandAuthUser> resultado = _validator.TestValidate(command);
        resultado.ShouldHaveValidationErrorFor(c => c.UserName).WithErrorMessage(mensagemEsperada);
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

    [Theory]
    [InlineData(TipoRole.Sindico)]
    [InlineData(TipoRole.Porteiro)]
    public void Validator_PerfilDeTenancyComIdZero_DeveFalhar(TipoRole role)
    {
        var command = GetValidCommand();
        command.TenancyId = 0;
        command.Role = role;

        var resultado = _validator.TestValidate(command);

        resultado.ShouldHaveValidationErrorFor(c => c.TenancyId);
    }

    [Fact]
    public void Validator_TenancyIdNegativo_DeveFalhar()
    {
        var command = GetValidCommand();
        command.TenancyId = -1;

        var resultado = _validator.TestValidate(command);

        resultado.ShouldHaveValidationErrorFor(c => c.TenancyId);
    }
}
