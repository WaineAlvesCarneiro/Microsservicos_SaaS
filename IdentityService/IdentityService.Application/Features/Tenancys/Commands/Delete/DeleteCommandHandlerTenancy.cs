using IdentityService.Domain.Common;
using IdentityService.Domain.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Tenancys.Commands.Delete;

public class DeleteCommandHandlerTenancy(
    ITenancyRepository repository,
    IAuthUserRepository authUserRepository)
    : IRequestHandler<DeleteCommandTenancy, Result>
{
    public async Task<Result> Handle(DeleteCommandTenancy request, CancellationToken cancellationToken)
    {
        //var existsImovelVinculadoNaTenancy = await imovelRepository.ExisteImovelVinculadoNaTenancyAsync(request.Id, cancellationToken);
        //if (existsImovelVinculadoNaTenancy) return Result.Failure("Não é possível excluir a tenancy, pois tem imóvel vinculado.");

        var existsUsuarioVinculadoNaTenancy = await authUserRepository.ExisteUsuarioVinculadoNaTenancyAsync(request.Id, cancellationToken);
        if (existsUsuarioVinculadoNaTenancy) return Result.Failure("Não é possível excluir o tenancy, pois tem usuário vinculado.");

        var dado = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (dado is null) return Result.Failure("Tenancy não encontrada.");

        await repository.DeleteAsync(dado, cancellationToken);
        return Result.Success("Tenancy deletada com sucesso.");
    }
}