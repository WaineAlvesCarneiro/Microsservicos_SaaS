namespace IdentityService.Domain.Interfaces;

public interface IEmailTemplateService
{
    string GerarBoasVindasUsuario(string nomeUsuario, string senhaTemporaria);
    string GerarUsuarioAlterado(string nomeUsuario);

    string GerarBoasVindasTenancy(string razaoSocial);
    string GerarTenancyAlterada(string razaoSocial);

    string GerarRedefinicaoSenha(string nomeUsuario, string link);
}
