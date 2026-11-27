using IdentityService.Application.DTOs;
using IdentityService.Application.Mappings;
using IdentityService.Domain.Common;
using IdentityService.Domain.Repositories;
using MediatR;

namespace IdentityService.Application.Features.Auth.Queries.GetAll;

public class GetAllQueryHandlerAuthUser(IAuthUserRepository repository)
    : IRequestHandler<GetAllQueryAuthUser, Result<IEnumerable<AuthUserDto>>>
{
    public async Task<Result<IEnumerable<AuthUserDto>>> Handle(GetAllQueryAuthUser request, CancellationToken cancellationToken)
    {
        var dados = await repository.GetAllAsync(TenancyId: request.IdTenancy, cancellationToken);

        var dtos = dados.Select(dado => dado.ToDto()).ToList();

        return Result<IEnumerable<AuthUserDto>>.Success(dtos);
    }
}
